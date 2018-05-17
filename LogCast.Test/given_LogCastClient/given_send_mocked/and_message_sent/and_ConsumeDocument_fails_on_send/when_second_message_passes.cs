using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent.and_ConsumeDocument_fails_on_send
{
    public class when_second_message_passes : Context
    {
        public override void Act()
        {
            base.Act();
            GloballyFailingMock.SkipExceptions();
            base.Act();
        }

        [Test]
        public void then_second_message_is_sent()
        {
            Sut.Messages.Count.Should().Be(2);
        }
        
        [Test]
        public void then_one_drop_happenned()
        {
            GloballyFailingMock.Drops.Should().BeEquivalentTo(new[] { 1 });
        }
    }
}