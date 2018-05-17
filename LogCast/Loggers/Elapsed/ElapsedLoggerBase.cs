using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace LogCast.Loggers.Elapsed
{
    [UsedImplicitly]
    public abstract class ElapsedLoggerBase : IDisposable
    {
        private readonly ElapsedLoggerBase _inner;

        private readonly string _operation;
        private readonly Stopwatch _stopwatch;
        private bool _isDisposed;

        protected ElapsedLoggerBase([NotNull] ElapsedLoggerBase inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        protected ElapsedLoggerBase([NotNull] string operation)
        {
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
            _stopwatch = Stopwatch.StartNew();
        }


        public void Dispose()
        {
            if (_isDisposed)
                return;

            DisposeAndLog();
        }

        private LogData DisposeAndLog()
        {
            LogData data;
            if (_inner != null)
            {
                data = _inner.DisposeAndLog();
            }
            else
            {
                _stopwatch.Stop();
                data = new LogData((int) _stopwatch.Elapsed.TotalMilliseconds, 
                    _operation);
            }

            var attribute = GetAttribute(data.Operation);
            Log(attribute, data.Elapsed);

            _isDisposed = true;
            return data;
        }

        protected abstract void Log(string attribute, int elapsed);

        protected virtual string GetAttribute(string operation)
        {
            return operation;
        }

        private class LogData
        {
            public LogData(int elapsed, string operation)
            {
                Elapsed = elapsed;
                Operation = operation;
            }

            public string Operation { get; }
            public int Elapsed { get; }
        }
    }
}