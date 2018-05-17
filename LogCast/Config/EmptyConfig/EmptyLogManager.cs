using System;
using LogCast.Engine;
using LogCast.Loggers;

namespace LogCast.Config.EmptyConfig
{
    internal class EmptyLogManager : ILogManager
    {
        private static readonly ILoggerBridge Logger = new EmptyLogger();

        public ILoggerBridge GetLoggerBridge(string name)
        {
            return Logger;
        }

        public void Flush(TimeSpan period)
        {
        }

        public void InitializeEngine(ILogCastEngine engine)
        {
        }
    }
}
