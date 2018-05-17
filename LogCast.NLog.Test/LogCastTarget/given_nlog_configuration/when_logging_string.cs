using FluentAssertions;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
{
    public class when_logging_string : Context
    {
        private const string LogMessage = "string based log";

        public override void Act()
        {
            Logger.Warn(LogMessage);
        }

        [Test]
        public void then_call_context_is_logged()
        {
            ClientMock.LastLog.GetProperty<object>(Property.Timestamp).Should().NotBeNull();
        }

        [Test]
        public void then_details_are_populated()
        {
            var details = ClientMock.LastLog.GetProperty<string>(Property.Details);
            details.Should().EndWith("|" + LogMessage);
        }

        [Test]
        public void then_message_from_default_application_log_is_logged()
        {
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(LogMessage);
        }

        [Test]
        public void then_exception_is_not_logged()
        {
            ClientMock.LastLog.GetProperty<object>(Property.Exceptions).Should().BeNull();
        }

        [Test]
        public void then_wait_all_is_not_called()
        {
            ClientMock.LastWaitAll.Should().Be(null);
        }
    }
}