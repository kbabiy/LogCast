using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_logging_below_log_level_set_up : Context
    {
        public override void Act()
        {
            Logger.Trace("SomeMessage");
        }

        [Test]
        public void then_nothing_is_logged()
        {
            ClientMock.LastLog.Should().BeNull();
        }
    }
}