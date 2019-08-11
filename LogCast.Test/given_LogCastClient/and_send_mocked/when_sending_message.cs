using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_send_mocked
{
    public class when_sending_message : Context
    {
        public override void Act()
        {
            SendTestMessage();
        }

        [Test]
        public void then_after_wait_message_is_delivered()
        {
            Sut.WaitAll(TimeSpan.FromMinutes(1));
            Sut.Messages.Count.Should().Be(1);
        }

        [Test]
        public void then_instantly_message_is_not_delivered()
        {
            Sut.Messages.Count.Should().Be(0);
        }

        [Test]
        public void then_after_0wait_message_is_not_delievered()
        {
            Sut.WaitAll(TimeSpan.Zero);
            Sut.Messages.Count.Should().Be(0);
        }

        [Test]
        public void then_after_small_wait_message_is_not_delievered()
        {
            Sut.WaitAll(TimeSpan.FromMilliseconds(3));
            Sut.Messages.Count.Should().Be(0);
        }

        [Test]
        public void then_after_wait_can_send_message()
        {
            Sut.WaitAll(TimeSpan.FromMinutes(1));
            SendTestMessage();
        }
    }
}