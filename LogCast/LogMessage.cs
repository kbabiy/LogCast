using System;
using LogCast.Data;

namespace LogCast
{
    public class LogMessage
    {
        public string OriginalMessage { get; set; }
        public string LoggerName { get; set; }
        public LogLevel Level { get; set; }
        public Exception Exception { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? BranchId { get; set; }
        public LogProperty[] Properties { get; set; }

        private string _renderedMessage;

        public string RenderedMessage
        {
            get => string.IsNullOrEmpty(_renderedMessage) ? OriginalMessage : _renderedMessage;
            set => _renderedMessage = value;
        }
    }
}