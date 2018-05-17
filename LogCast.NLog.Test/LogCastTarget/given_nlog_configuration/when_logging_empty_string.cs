using FluentAssertions;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
{
    public class when_logging_empty_string : Context
    {
        public override void Act()
        {
            Logger.Info(string.Empty);
        }

        [Test]
        public void then_message_exists()
        {
            ClientMock.LastLog.Should().NotBeNull();
        }

        [TestCase]
        public void then_empty_string_is_logged()
        {
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(string.Empty);
        }
    }
}