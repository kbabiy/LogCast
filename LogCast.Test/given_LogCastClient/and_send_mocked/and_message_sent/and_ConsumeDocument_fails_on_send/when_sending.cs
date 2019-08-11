using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent.and_ConsumeDocument_fails_on_send
{
    public class when_sending : Context
    {
        [Test]
        public void then_one_message_is_sent()
        {
            Sut.Messages.Should().ContainSingle(m => m == TestMessage);
        }

        [Test]
        public void then_unhandled_message_is_logged()
        {
            LoggerMock.Verify(l => l.Write(It.IsAny<Exception>(), It.Is<string[]>(s => s.Any(_ => _.StartsWith("UNHANDLED")))), Times.Once);
        }

        [Test]
        public void then_stats_are_logged_as_expected()
        {
            LoggerMock.Verify(l => l.Write(It.IsAny<Exception>(), It.Is<string[]>(s => s.Any(_ => _.StartsWith("STATS")))), Times.Once);
        }

        [Test]
        public void then_nothing_is_previously_dropped()
        {
            GloballyFailingMock.Drops.Should().BeEmpty();
        }
    }
}