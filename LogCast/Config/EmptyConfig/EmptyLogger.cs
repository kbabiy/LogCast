using System;
using LogCast.Data;
using LogCast.Loggers;

namespace LogCast.Config.EmptyConfig
{
    internal class EmptyLogger : ILoggerBridge
    {
        public bool IsLevelEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogLevel level, string message, Exception exception, LogProperty[] properties)
        {
        }
    }
}