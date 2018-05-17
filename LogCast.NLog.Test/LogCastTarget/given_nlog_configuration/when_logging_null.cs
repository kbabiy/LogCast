using FluentAssertions;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
{
    public class when_logging_null : Context
    {
        public override void Act()
        {
            Logger.Info(null);
        }

        [TestCase]
        public void then_message_is_logged()
        {
            ClientMock.LastLog.Should().NotBeNull();
        }

        [Test]
        public void then_message_field_exists()
        {
            ClientMock.LastLog.PropertyExists(Property.Message).Should().BeTrue();
        }

        [Test]
        public void then_message_is_null()
        {
            ClientMock.LastLog.GetProperty<object>(Property.Message).Should().Be(null);
        }
    }
}