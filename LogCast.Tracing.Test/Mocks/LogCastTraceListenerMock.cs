using LogCast.Fallback;
using Moq;

namespace LogCast.Tracing.Test.Mocks
{
    public class LogCastTraceListenerMock : Tracing.LogCastTraceListener
    {
        protected override LogCastTraceListenerWorker CreateWorker()
        {
            return new LogCastTraceListenerWorker(this, Attributes, Mock.Of<IFallbackLogger>());
        }
    }
}