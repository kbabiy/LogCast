using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
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
            LastLog.GetProperty<object>(Property.Timestamp).Should().NotBeNull();
        }

        [Test]
        public void then_details_are_populated()
        {
            var details = LastLog.GetProperty<string>(Property.Details);
            details.Should().EndWith("| " + LogMessage);
        }

        [Test]
        public void then_message_from_default_application_log_is_logged()
        {
            LastLog.GetProperty<string>(Property.Message).Should().Be(LogMessage);
        }

        [Test]
        public void then_exception_text_is_not_logged_but_placeholder_is_present()
        {
            LastLog.ToJson().Should().Contain($"\"{Property.Exceptions}\":null");
        }

        [Test]
        public void then_wait_all_is_not_called()
        {
            ClientMock.LastWaitAll.Should().Be(null);
        }
    }
}