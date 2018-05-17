using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent.and_send_fails
{
    public class when_short_timeout_set_in_options : Context
    {
        protected override string RetryTimeout => "0:0:0";

        [Test]
        public void then_no_messages_sent()
        {
            Sut.Messages.Count.Should().Be(0);
        }
    }
}