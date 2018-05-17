using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_logging_string : Context
    {
        private const string LogMessage = "string based log";
        public override void Act()
        {
            Logger.Error(LogMessage);
        }

        [Test]
        public void then_something_is_logged()
        {
            ClientMock.LastLog.Should().NotBeNull();
        }

        [Test]
        public void then_message_is_set()
        {
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(LogMessage);
        }

        [Test]
        public void then_log_level_is_error()
        {
            ClientMock.LastLog.GetProperty<string>(Property.LogLevel).Should().Be(LogLevel.Error.ToString());
        }
    }
}