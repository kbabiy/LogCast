using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_without_completion : Context
    {
        public override void Act()
        {
            Logger.Warn(TestMessage);
        }

        [Test]
        public void then_log_is_not_written()
        {
            LastLog.Should().BeNull();
        }
    }
}