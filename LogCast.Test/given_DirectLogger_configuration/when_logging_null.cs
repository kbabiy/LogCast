using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
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
            LastLog.Should().NotBeNull();
        }

        [Test]
        public void then_message_field_exists()
        {
            LastLog.PropertyExists(Property.Message).Should().BeTrue();
        }

        [Test]
        public void then_message_is_null()
        {
            LastLog.GetProperty<object>(Property.Message).Should().Be(null);
        }
    }
}