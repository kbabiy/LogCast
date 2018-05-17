using System;
using LogCast.Engine;

namespace LogCast.Loggers
{
    public interface ILogManager
    {
        ILoggerBridge GetLoggerBridge(string name);
        void Flush(TimeSpan timeout);
        void InitializeEngine(ILogCastEngine engine);
    }
}
