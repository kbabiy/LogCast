using System;
using LogCast.Data;

namespace LogCast.Loggers.Direct
{
    internal class DirectLoggerBridge : ILoggerBridge
    {
        private readonly string _loggerName;
        private readonly DirectLogger _logger;

        public DirectLoggerBridge(string loggerName, DirectLogger logger)
        {
            _loggerName = loggerName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return _logger.IsLevelEnabled(level);
        }

        public void Log(LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
            _logger.Log(_loggerName, level, message, exception, properties);
        }
    }
}
