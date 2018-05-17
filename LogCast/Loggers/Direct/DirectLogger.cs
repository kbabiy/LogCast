using System;
using LogCast.Data;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Rendering;

namespace LogCast.Loggers.Direct
{
    internal class DirectLogger
    {
        private readonly LogLevel _minLogLevel;
        private readonly LogMessageRouter _messageRouter;
        private readonly IFallbackLogger _fallbackLogger;
        private readonly MessageLayout _layout;

        private DirectLogger(LogLevel minLogLevel, LogMessageRouter messageRouter)
        {
            _minLogLevel = minLogLevel;
            _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
        }

        public DirectLogger(LogLevel minLogLevel, LogMessageRouter messageRouter, IFallbackLogger fallbackLogger, 
            MessageLayout layout)
            : this(minLogLevel, messageRouter)
        {
            _fallbackLogger = fallbackLogger;
            _layout = layout;
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return level >= _minLogLevel;
        }

        public void Log(string loggerName, LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
            if (!IsLevelEnabled(level))
                return;

            try
            {
                var logMessage = CreateMessage(loggerName, level, message, exception, properties);
                _messageRouter.DispatchMessage(logMessage);
            }
            catch (Exception ex)
            {
                _fallbackLogger?.Write(ex,
                    $"Failed to dispatch a message. Original message: [{loggerName}] {message}");
            }
        }

        private LogMessage CreateMessage(string loggerName, LogLevel level, string message, Exception exception, 
            LogProperty[] properties)
        {
            var logMessage = new LogMessage
            {
                Level = level,
                OriginalMessage = message,
                LoggerName = loggerName,
                Exception = exception,
                CreatedAt = LogCastContext.PreciseNow,
                Properties = properties
            };

            if (_layout != null)
            {
                logMessage.RenderedMessage = _layout.Render(message, loggerName, level, logMessage.CreatedAt);
            }

            return logMessage;
        }
    }
}
