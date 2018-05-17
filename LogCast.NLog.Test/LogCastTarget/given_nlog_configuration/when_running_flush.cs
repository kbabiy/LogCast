using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
{
    public class when_running_flush : Context
    {
        public override void Act()
        {
            LogManager.Flush(TimeSpan.FromMinutes(1));
        }

        [Test]
        public void then_flush_is_called_on_target()
        {
            ClientMock.LastWaitAll.Should().Be(TimeSpan.FromMinutes(1));
        }
    }
}