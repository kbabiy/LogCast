using System;
using JetBrains.Annotations;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class MessageFactory
    {
        private readonly string _defaultLogger;
        private DateTime _baseTime;
        private int _messageNumber;

        public int MessageIntervalMs { get; set; }

        public MessageFactory(string defaultLogger, DateTime baseTime)
        {
            _defaultLogger = defaultLogger;
            _baseTime = baseTime;
            MessageIntervalMs = 10;
        }

        public OrdinalLogMessage CreateMessage(LogLevel level, string message)
        {
            return CreateMessage(_defaultLogger, level, message, null);
        }

        public OrdinalLogMessage CreateMessage(LogLevel level, string message, Exception error)
        {
            return CreateMessage(_defaultLogger, level, message, error);
        }

        public OrdinalLogMessage CreateMessage(string logger, LogLevel level, string message)
        {
            return CreateMessage(logger, level, message, null);
        }

        public OrdinalLogMessage CreateMessage(string logger, LogLevel level, string message, Exception error)
        {
            _messageNumber++;
            var dateCreated = _baseTime.AddMilliseconds(_messageNumber * MessageIntervalMs);
            return CreateMessage(logger, dateCreated, level, message, error);
        }

        public OrdinalLogMessage CreateMessage(string logger, DateTime dateCreated, LogLevel level, string message, Exception error)
        {
            return new OrdinalLogMessage
            {
                CreatedAt = dateCreated,
                Level = level,
                OriginalMessage = message,
                RenderedMessage = $"{dateCreated:HH:mm:ss.ffff} Rendered '{message}' [number {_messageNumber}]",
                Exception = error,
                LoggerName = logger,
                Ordinal = _messageNumber
            };
        }
    }
}
