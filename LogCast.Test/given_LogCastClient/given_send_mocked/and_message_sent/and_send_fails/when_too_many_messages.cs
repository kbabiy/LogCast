using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent.and_send_fails
{
    public class when_too_many_messages : Context
    {
        protected override int FailCount => 0;
        protected override string SendingThreadCount => "1";
        protected override string RetryTimeout => "1:0:0";

        public override void Act()
        {
            for (int i = 0; i < 30; i++)
                SendTestMessage();
            Sut.WaitAll(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void then_not_all_messages_sent()
        {
            Sut.Messages.Count.Should().BeLessThan(30);
        }
    }
}