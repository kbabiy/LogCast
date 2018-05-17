using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
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
            LastLog.Should().NotBeNull();
        }

        [TestCase]
        public void then_empty_string_is_logged()
        {
            LastLog.GetProperty<string>(Property.Message).Should().Be(string.Empty);
        }
    }
}