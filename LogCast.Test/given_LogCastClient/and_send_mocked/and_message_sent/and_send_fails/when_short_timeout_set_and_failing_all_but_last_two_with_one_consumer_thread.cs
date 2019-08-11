using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent.and_send_fails
{
    public class when_short_timeout_set_and_failing_all_but_last_two_with_one_consumer_thread : Context
    {
        protected override string SendingThreadCount => "1";
        protected override string RetryTimeout => "0:0:0";
        protected override string Throttling => "100";
        protected override int FailCount => 10;

        public override void Act()
        {
            for (int i = 0; i < 12; i++)
                SendTestMessage();
            Sut.WaitAll(TimeSpan.FromMinutes(1));
        }

        [Test]
        public void then_two_messages_sent_successfully()
        {
            Sut.Messages.Count.Should().Be(2);
        }

        [Test]
        public void then_first_message_has_drop_count_equal_to_fail_count()
        {
            Sut.Messages[0].Should().Contain($"\"{Property.Logging.DropCount}\":10");
        }

        [Test]
        public void then_first_message_has_no_retry_count()
        {
            Sut.Messages[0].Should().Contain($"\"{Property.Logging.RetryCount}\":0");
        }

        [Test]
        public void then_second_message_has_no_drop_count()
        {
            Sut.Messages[1].Should().Contain($"\"{Property.Logging.DropCount}\":0");
        }

        [Test]
        public void then_second_message_has_no_retry_count()
        {
            Sut.Messages[1].Should().Contain($"\"{Property.Logging.RetryCount}\":0");
        }

        [Test]
        public void then_drop_logged_once_per_drop()
        {
            LoggerMock.Verify(l => l.Write(It.Is<string[]>(s => s.Any(_ => _.StartsWith("DROPPING by RetryTimeout")))), Times.Exactly(10));
        }

        [Test]
        public void then_stats_logged_once_per_drop()
        {
            LoggerMock.Verify(l => l.Write(It.Is<string[]>(s => s.Any(_ => _.StartsWith("STATS")))), Times.Exactly(10));
        }
    }
}