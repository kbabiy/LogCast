using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent
{
    public class when_options_are_default : Context
    {
        [Test]
        public void then_thread_count_is_defaulted_to_4()
        {
            Sut.ConsumerCount.Should().Be(4);
        }

        [Test]
        public void then_endpoint_is_initialized()
        {
            Sut.LastUri.ToString().Should().StartWith("http://localhost");
        }

        [Test]
        public void then_timeout_is_defaulted_to_20sec()
        {
            Sut.LastTimeout.Should().Be(20000);
        }

        [Test]
        public void then_message_has_drop_count_field()
        {
            Sut.Messages[0].Should().Contain($"\"{Property.Logging.DropCount}\":0");
        }

        [Test]
        public void then_message_has_retry_count_field()
        {
            Sut.Messages[0].Should().Contain($"\"{Property.Logging.RetryCount}\":0");
        }
    }
}