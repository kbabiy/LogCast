using BddStyle.NUnit;

namespace LogCast.Test.given_LogConfig
{
    public abstract class Context : ContextBase
    {
        public override void Arrange()
        {
            LogConfig.Reset();
            LogConfig.DisableAutoConfig = true;
        }

        public override void Cleanup()
        {
            LogConfig.Reset();
            LogConfig.DisableAutoConfig = false;
        }
    }
}