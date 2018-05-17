using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration.and_context_id_enabled
{
    public class when_logging : Context
    {
        public override void Act()
        {
            Logger.Warn(TestMessage);
        }

        [Test]
        public void then_log_wasnt_written_without_completion()
        {
            ClientMock.LastLog.Should().BeNull();
        }

        [Test]
        public void then_logging_happenned_after_completion()
        {
            Complete();
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(TestMessage);
        }

        [Test]
        public void then_operation_is_set_on_the_log()
        {
            Complete();
            ClientMock.LastLog.GetProperty<string>(Property.OperationName).Should().Be(OperationName);
        }
    }
}