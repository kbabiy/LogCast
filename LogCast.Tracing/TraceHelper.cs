using System.Diagnostics;

namespace LogCast.Tracing
{
    public static class TraceHelper
    {
        public static TraceEventType TranslateLogLevel(LogLevel level)
        {
            // The levels are in the order of usage
            TraceEventType result;
            switch (level)
            {
                case LogLevel.Info:
                    result = TraceEventType.Information;
                    break;
                case LogLevel.Debug:
                    result = TraceEventType.Verbose;
                    break;
                case LogLevel.Warn:
                    result = TraceEventType.Warning;
                    break;
                case LogLevel.Error:
                    result = TraceEventType.Error;
                    break;
                case LogLevel.Fatal:
                    result = TraceEventType.Critical;
                    break;
                default:
                    result = TraceEventType.Verbose;
                    break;
            }

            return result;
        }

        public static LogLevel TranslateEventType(TraceEventType eventType)
        {
            // The levels are in the order of usage
            LogLevel result;
            switch (eventType)
            {
                case TraceEventType.Information:
                    result = LogLevel.Info;
                    break;
                case TraceEventType.Verbose:
                    result = LogLevel.Debug;
                    break;
                case TraceEventType.Warning:
                    result = LogLevel.Warn;
                    break;
                case TraceEventType.Error:
                    result = LogLevel.Error;
                    break;
                case TraceEventType.Critical:
                    result = LogLevel.Fatal;
                    break;
                default:
                    result = LogLevel.Trace;
                    break;
            }

            return result;
        }
    }
}
