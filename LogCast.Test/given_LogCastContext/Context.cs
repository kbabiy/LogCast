using BddStyle.NUnit;

namespace LogCast.Test.given_LogCastContext
{
    public abstract class Context : ContextBase
    {
        protected const string CorrelationId = "TestCorrelationId1";
        protected LogCastContext CurrentContext => LogCastContext.Current;

        public override void Arrange()
        {
            CleanupContext();
        }

        public override void Cleanup()
        {
            CleanupContext();
        }

        private static void CleanupContext()
        {
            LogCast.Context.Context.Unregister<LogCastContext>();
        }
    }
}