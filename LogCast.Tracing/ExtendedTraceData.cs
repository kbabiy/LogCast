using System;
using LogCast.Data;

namespace LogCast.Tracing
{
    internal class ExtendedTraceData
    {
        public string LoggerName { get; set; }
        public DateTime TimeStamp { get; set; }
        public Exception Error { get; set; }
        public LogProperty[] Properties { get; set; }

        public override string ToString()
        {
            return $"Logger name: {LoggerName}, Timestamp: {TimeStamp:O}";
        }
    }
}
