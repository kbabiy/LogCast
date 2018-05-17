using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Rendering;

namespace LogCast.Tracing
{
    public class LogCastTraceListenerWorker
    {
        #region Options

        public string SystemType { get; private set; }
        public string Layout { get; private set; }
        public string SkipPercentage { get; private set; }
        public string FallbackLogDirectory { get; private set; }
        public int DaysToKeepFallbackLogs { get; private set; }

        public LogCastOptions Options { get; private set; }

        #endregion

        public IFallbackLogger FallbackLogger { get; }

        private readonly LogMessageRouter _messageRouter;
        private MessageLayout _layout;

        [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
        public LogCastTraceListenerWorker(LogCastTraceListener listener, StringDictionary attributes)

            : this(listener, attributes, null)
        {
        }

        public LogCastTraceListenerWorker(LogCastTraceListener listener, StringDictionary attributes, IFallbackLogger fallbackLogger)
        {
            ParseListenerOptions(attributes);
            ParseLogCastOptions(attributes);
            FallbackLogger = fallbackLogger ??
                new FileFallbackLogger(FallbackLogDirectory, DaysToKeepFallbackLogs);

            _messageRouter = new LogMessageRouter(ParseSkipPercentage(SkipPercentage));

            // This case is mostly for scenario when a client uses trace methods directly
            // and doesn't use our ILogger/LogManager/LogConfig
            if (!LogConfig.IsConfigured && !LogConfig.IsInProgress)
            {
                LogConfig.Configure(new TraceLogManager(listener, this));
            }
        }

        internal void Write(TraceEventType eventType, string message, ExtendedTraceData traceData)
        {
            try
            {
                var logMessage = CreateMesage(TraceHelper.TranslateEventType(eventType), message, traceData);
                _messageRouter.DispatchMessage(logMessage);
            }
            catch (Exception ex)
            {
                FallbackLogger.Write(ex, $"Failed to dispatch a message. Original message: {message}");
            }
        }

        private LogMessage CreateMesage(LogLevel level, string message, ExtendedTraceData traceData)
        {
            var logMessage = new LogMessage
            {
                Level = level,
                OriginalMessage = message
            };

            if (traceData != null)
            {
                logMessage.LoggerName = traceData.LoggerName;
                logMessage.Exception = traceData.Error;
                logMessage.Properties = traceData.Properties;
                logMessage.CreatedAt = traceData.TimeStamp;
            }
            else
            {
                logMessage.CreatedAt = LogCastContext.PreciseNow;
            }

            if (_layout != null)
            {
                logMessage.RenderedMessage = _layout.Render(message, logMessage.LoggerName, logMessage.Level, logMessage.CreatedAt);
            }

            return logMessage;
        }

        public static string[] GetSupportedAttributes()
        {
            return new[]
            {
                "endpoint",
                "throttling",
                "retryTimeout",
                "sendingThreadCount",
                "sendTimeout",
                "enableSelfDiagnostics",

                "systemType",
                "skipPercentage",
                "fallbackLogDirectory",
                "daysToKeepFallbackLogs",
                "layout"
            };
        }

        private void ParseListenerOptions(StringDictionary attributes)
        {
            SystemType = attributes["systemType"];
            SkipPercentage = attributes["skipPercentage"];
            FallbackLogDirectory = attributes["fallbackLogDirectory"];
            if (int.TryParse(attributes["daysToKeepFallbackLogs"], out var days))
                DaysToKeepFallbackLogs = days;
            Layout = attributes["layout"];
            if (!string.IsNullOrWhiteSpace(Layout))
            {
                _layout = new MessageLayout(Layout);
            }
        }

        private void ParseLogCastOptions(StringDictionary attributes)
        {
            Options = LogCastOptions.Parse(
                attributes["endpoint"],
                attributes["throttling"],
                attributes["retryTimeout"],
                attributes["sendingThreadCount"],
                attributes["sendTimeout"],
                attributes["enableSelfDiagnostics"]);
        }

        private static int ParseSkipPercentage(string skipPercentage)
        {
            int.TryParse(skipPercentage, out var result);
            return result;
        }
    }
}