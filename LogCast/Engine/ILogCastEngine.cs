using System;
using LogCast.Delivery;
using LogCast.Fallback;
using LogCast.Inspectors;
using JetBrains.Annotations;

namespace LogCast.Engine
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public interface ILogCastEngine
    {
        bool IsInitialized { get; }
        void Initialize(LogCastOptions options, IFallbackLogger fallbackLogger);
        Action<ILogCastEngine> LazyInitializer { get; set; }

        void RegisterInspector(ILogDispatchInspector inspector);
        T GetInspector<T>() where T : ILogDispatchInspector;

        void SendContextLessMessage(LogMessage message);
        void OnContextClose(LogCastContext context);
        void Flush(TimeSpan timeout);
    }
}