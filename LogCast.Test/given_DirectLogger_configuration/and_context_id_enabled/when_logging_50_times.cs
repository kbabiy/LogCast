using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_50_times : Context
    {
        public override void Act()
        {
            for (int i = 0; i < 50; i++)
                Logger.Warn(TestMessage + i);

            Complete();
        }

        [Test]
        public void then_log_is_written()
        {
            LastLog.Should().NotBeNull();
        }
    }
}