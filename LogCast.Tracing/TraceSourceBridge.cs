using System;
using System.Diagnostics;
using LogCast.Data;
using LogCast.Loggers;

namespace LogCast.Tracing
{
    internal class TraceSourceBridge : ILoggerBridge
    {
        private readonly TraceSource _source;
        private readonly string _name;

        public TraceSourceBridge(string name)
        {
            _name = name;
        }

        public TraceSourceBridge(string name, TraceSource source)
            : this(name)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            if (_source == null)
                return true;

            bool result;
            switch (level)
            {
                case LogLevel.Info:
                    result = _source.Switch.Level >= SourceLevels.Information;
                    break;
                case LogLevel.Debug:
                    result = _source.Switch.Level >= SourceLevels.Verbose;
                    break;
                case LogLevel.Warn:
                    result = _source.Switch.Level >= SourceLevels.Warning;
                    break;
                case LogLevel.Error:
                    result = _source.Switch.Level >= SourceLevels.Error;
                    break;
                case LogLevel.Trace:
                    result = _source.Switch.Level >= SourceLevels.Verbose;
                    break;
                case LogLevel.Fatal:
                    result = _source.Switch.Level >= SourceLevels.Critical;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        public void Log(LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
            if (_source == null)
            {
                // When trace listener is configured without source we lose some information about the message
                if (level <= LogLevel.Info)
                {
                    Trace.TraceInformation(message);
                }
                else if (level <= LogLevel.Warn)
                {
                    Trace.TraceWarning(message);
                }
                else
                {
                    Trace.TraceError(message);
                }
            }
            else
            {
                _source.TraceData(TraceHelper.TranslateLogLevel(level), 0, message, new ExtendedTraceData
                    {
                        TimeStamp = LogCastContext.PreciseNow,
                        LoggerName = _name,
                        Error = exception,
                        Properties = properties
                    });
            }  
        }
    }
}