using LogCast.Data;
using NLogNamespace = NLog;
using NLogLevel = NLog.LogLevel;

namespace LogCast.NLog
{
    public static class LogHelper
    {
        private const string LogPropertiesKey = "LogProperties";

        public static LogProperty[] GetLogProperties(NLogNamespace.LogEventInfo logEvent)
        {
            logEvent.Properties.TryGetValue(LogPropertiesKey, out var result);

            return result as LogProperty[];
        }

        public static void StoreLogProperties(NLogNamespace.LogEventInfo logEvent, LogProperty[] properties)
        {
            logEvent.Properties[LogPropertiesKey] = properties;
        }

        public static NLogLevel TranslateLogLevel(LogLevel level)
        {
            // The levels are in the order of usage
            NLogLevel result;
            switch (level)
            {
                case LogLevel.Info:
                    result = NLogLevel.Info;
                    break;
                case LogLevel.Debug:
                    result = NLogLevel.Debug;
                    break;
                case LogLevel.Warn:
                    result = NLogLevel.Warn;
                    break;
                case LogLevel.Error:
                    result = NLogLevel.Error;
                    break;
                case LogLevel.Trace:
                    result = NLogLevel.Trace;
                    break;
                case LogLevel.Fatal:
                    result = NLogLevel.Fatal;
                    break;
                default:
                    result = NLogLevel.Off;
                    break;
            }

            return result;
        }

        public static LogLevel TranslateLogLevel(NLogLevel level)
        {
            // The levels are in the order of usage
            var name = level?.Name;
            switch (name)
            {
                case "Info":
                    return LogLevel.Info;
                case "Debug":
                    return LogLevel.Debug;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Trace":
                    return LogLevel.Trace;
                case "Fatal":
                    return LogLevel.Fatal;
                default:
                    return LogLevel.None;
            }
        }
    }
}
