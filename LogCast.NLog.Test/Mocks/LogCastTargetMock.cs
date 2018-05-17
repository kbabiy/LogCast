using LogCast.Fallback;
using Moq;
using NLog.Targets;

namespace LogCast.NLog.Test.Mocks
{
    [Target("LogCast")]
    public class LogCastTargetMock : NLog.LogCastTarget
    {
        protected override IFallbackLogger CreateFallbackLogger()
        {
            return Mock.Of<IFallbackLogger>();
        }
    }
}