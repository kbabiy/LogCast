using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_logging_empty_string : Context
    {
        public override void Act()
        {
            Logger.Info(string.Empty);
        }

        [TestCase]
        public void then_empty_string_is_logged()
        {
            ClientMock.LastLog.Should().NotBeNull();
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(string.Empty);
        }
    }
}