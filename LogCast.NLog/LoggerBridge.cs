using System;
using LogCast.Data;
using LogCast.Loggers;
using NLogNamespace = NLog;

namespace LogCast.NLog
{
    internal class LoggerBridge : ILoggerBridge
    {
        private readonly NLogNamespace.ILogger _logger;

        public LoggerBridge(NLogNamespace.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            bool result;
            switch (level)
            {
                case LogLevel.Info:
                    result = _logger.IsInfoEnabled;
                    break;
                case LogLevel.Debug:
                    result = _logger.IsDebugEnabled;
                    break;
                case LogLevel.Warn:
                    result = _logger.IsWarnEnabled;
                    break;
                case LogLevel.Error:
                    result = _logger.IsErrorEnabled;
                    break;
                case LogLevel.Trace:
                    result = _logger.IsTraceEnabled;
                    break;
                case LogLevel.Fatal:
                    result = _logger.IsFatalEnabled;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        public void Log(LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
            var logEvent = new NLogNamespace.LogEventInfo(
                LogHelper.TranslateLogLevel(level), _logger.Name, null, message, null, exception)
            {
                TimeStamp = LogCastContext.PreciseNow
            };

            if (properties != null && properties.Length > 0)
            {
                LogHelper.StoreLogProperties(logEvent, properties);
            }

            _logger.Log(logEvent);
        }
    }
}