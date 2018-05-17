using System;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Inspectors;

namespace LogCast.Config.EmptyConfig
{
    internal class EmptyLogCastEngine : ILogCastEngine
    {
        public bool IsInitialized => true;
        public Action<ILogCastEngine> LazyInitializer { get; set; }

        public void Initialize(LogCastOptions options, IFallbackLogger fallbackLogger)
        {
        }

        public void RegisterInspector(ILogDispatchInspector inspector)
        {
        }

        public T GetInspector<T>() where T : ILogDispatchInspector
        {
            return default(T);
        }

        public void SendContextLessMessage(LogMessage message)
        {
        }

        public void OnContextOpen(LogCastContext context)
        {
        }

        public void OnContextClose(LogCastContext context)
        {
        }

        public void Flush(TimeSpan timeout)
        {
        }
    }
}
