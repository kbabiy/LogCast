using System;
using LogCast.Data;

namespace LogCast.Loggers
{
    public interface ILoggerBridge
    {
        bool IsLevelEnabled(LogLevel level);
        void Log(LogLevel level, string message, Exception exception, LogProperty[] properties);
    }
}