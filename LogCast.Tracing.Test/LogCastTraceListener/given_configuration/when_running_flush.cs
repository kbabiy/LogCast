using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_running_flush : Context
    {
        [Test]
        public void then_flush_is_called_on_target()
        {
            var timeout = TimeSpan.FromMinutes(1);
            LogManager.Flush(timeout);
            ClientMock.LastWaitAll.Should().Be(timeout);
        }
    }
}