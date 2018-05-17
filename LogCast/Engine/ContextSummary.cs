using System;
using System.Collections.Generic;

namespace LogCast.Engine
{
    internal class ContextSummary
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public List<Exception> Exceptions { get; set; }
        public List<int> Durations { get; set; }
        public HashSet<string> Loggers { get; set; }
    }
}
