using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Utilities;
using JetBrains.Annotations;

namespace LogCast.Delivery
{
    /// <summary>
    /// Client to send log messages
    /// Handles asynchrony with tweaks
    /// </summary>
    public class LogCastClient : ILogCastClient
    {
        private readonly LogCastOptions _options;
        private readonly IFallbackLogger _fallbackLogger;
        private readonly BlockingCollection<LogCastMessageFactory> _queue;
        protected readonly Thread[] Threads;
        private readonly CountEvent _countEvent;

        public LogCastClient([NotNull] LogCastOptions options, [NotNull] IFallbackLogger fallbackLogger)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (fallbackLogger == null)
                throw new ArgumentNullException(nameof(fallbackLogger));

            if (options.Endpoint == null)
            {
                fallbackLogger.Write("Endpoint cannot be null reference");
                throw new ArgumentException("Endpoint can not be null reference", nameof(options));
            }
            if (options.Throttling < 10)
            {
                fallbackLogger.Write("Throttling cannot be less than 10");
                throw new ArgumentException("Throttling can not be less than 10", nameof(options));
            }

            _options = options;
            _fallbackLogger = fallbackLogger;
            _queue = new BlockingCollection<LogCastMessageFactory>();

            var sendingThreadCount = _options.SendingThreadCount;
            var threadCount = sendingThreadCount > 0 ? sendingThreadCount : 4;

            _countEvent = new CountEvent();
            Threads = Enumerable
                .Range(0, threadCount)
                .Select(i => new Thread(Sender)
                {
                    IsBackground = true
                })
                .ToArray();

            foreach (var thread in Threads)
                thread.Start();
        }


        public void Send(Func<LogCastDocument> documentFactory)
        {
            var job = new LogCastMessageFactory(documentFactory,
                _options.EnableSelfDiagnostics ? DateTime.Now : (DateTime?) null);

            _countEvent.Increase();
            _queue.Add(job);
        }

        public bool WaitAll(TimeSpan timeout)
        {
            return timeout.TotalMilliseconds > 0 
                && _countEvent.WaitUntil(0, timeout);
        }

        private Uri GetEndpoint()
        {
            const string version = "01";
            const string indexPrefix = "elastic-" + version;

            return new Uri($"{_options.Endpoint}/{indexPrefix}-{DateTime.UtcNow:yyyy.MM.dd}/logs/");
        }

        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private void Sender()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            var timeoutMsec = (int) _options.SendTimeout.TotalMilliseconds;
            int dropCount = 0;
            int retryCount = 0;

            foreach (var messageFactory in _queue.GetConsumingEnumerable())
            {
                try
                {
                    if (ConsumeDocument(threadId, messageFactory, timeoutMsec, dropCount, retryCount, out var newRetries))
                    {
                        dropCount = 0;
                        retryCount = 0;
                    }
                    else
                    {
                        ++dropCount;
                    }
                    retryCount += newRetries;
                }
                catch (Exception ex)
                {
                    try
                    {
                        var queueLength = _queue.Count;
                        _fallbackLogger.Write(ex,
                            "UNHANDLED ERROR",
                            Stats(threadId, ++dropCount, retryCount, queueLength));
                    }
                    catch
                    {
                    }
                }
                finally
                {
                    _countEvent.Decrease();
                }
            }
        }

        protected virtual bool ConsumeDocument(int threadId, LogCastMessageFactory logCastMessageFactory, int timeoutMsec, int dropCount, int retryCount, out int newRetries)
        {
            string message = null;
            Stopwatch sw = null;
            newRetries = 0;

            //retry cycle
            while (_queue.Count <= _options.Throttling)
            {
                //if this is not retry, then create a message to send
                if (message == null)
                {
                    sw = Stopwatch.StartNew();
                    message = logCastMessageFactory.Create(dropCount, retryCount);
                }

                var endpoint = GetEndpoint();

                var exception = SendHandled(endpoint, message, timeoutMsec);
                if (exception == null)
                    return true;

                if (SkipRetry(exception))
                {
                    _fallbackLogger.Write("DROPPING by SKIPPING retry",
                        Format(exception, message),
                        Stats(threadId, dropCount + 1, retryCount + newRetries, _queue.Count));
                    return false;
                }

                if (sw.Elapsed >= _options.RetryTimeout)
                {
                    _fallbackLogger.Write($"DROPPING by RetryTimeout ({_options.RetryTimeout})",
                        Format(exception, message),
                        Stats(threadId, dropCount + 1, retryCount + newRetries, _queue.Count));
                    return false;
                }

                ++newRetries;

                //To skip over-intensive retry
                Thread.Sleep(100);
            }

            // We expect a lot of messages with Throttling failure so log only some of them
            if (dropCount % 1000 == 0)
                _fallbackLogger.Write($"DROPPING by Throttling ({_options.Throttling})",
                    Stats(threadId, dropCount + 1, retryCount + newRetries, _queue.Count));
            return false;
        }

        private Exception SendHandled(Uri endpoint, string message, int timeout)
        {
            try
            {
                Send(endpoint, message, timeout);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        protected virtual void Send(Uri endpoint, string message, int timeout)
        {
            using (var client = new TimedWebClient(timeout))
                client.UploadData(endpoint, Encoding.UTF8.GetBytes(message));
        }

        private static bool SkipRetry(Exception e)
        {
            if (!(e is WebException webException))
                return false;

            // Logging system can not understand the message
            if ((webException.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.BadRequest)
                return true;

            // This can mean bad URI etc.
            if (webException.InnerException is NotSupportedException)
                return true;

            return false;
        }

        private static string Stats(int threadId, int dropCount, int retryCount, int queueLength)
        {
            string queueLengthMessage = queueLength < 0 ? Property.Values.None : queueLength.ToString();
            return $"STATS({threadId}): [Drops={dropCount}; Retries={retryCount}; Queue={queueLengthMessage}]";
        }

        private static string Format(Exception e, string message)
        {
            var error = e.ToString();

            var webException = e as WebException;
            var responseStream = webException?.Response?.GetResponseStream();
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    string errorWebResponse = reader.ReadToEnd();
                    error += $"{Environment.NewLine}Response stream:{Environment.NewLine}{errorWebResponse}";
                }
            }

            error = $"SEND failure: {error}{Environment.NewLine}MESSAGE:{Environment.NewLine}{message}";

            return error;
        }
    }
}