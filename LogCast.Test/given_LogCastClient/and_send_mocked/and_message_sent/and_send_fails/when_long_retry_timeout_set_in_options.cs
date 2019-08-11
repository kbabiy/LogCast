using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent.and_send_fails
{
    public class when_long_retry_timeout_set_in_options : Context
    {
        protected override string SendingThreadCount => "1";

        [Test]
        public void then_one_message_is_sent()
        {
            Sut.Messages.Count.Should().Be(1);
        }

        [Test]
        public void then_second_message_contains_reties()
        {
            SendTestMessage();
            Sut.WaitAll(TimeSpan.FromMinutes(1));
            Sut.Messages[1].Should().Contain($"\"{Property.Logging.RetryCount}\":1");
        }
    }
}