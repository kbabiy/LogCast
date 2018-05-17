using System;
using FluentAssertions;
using LogCast.Delivery;
using LogCast.Tracing.Test.Mocks;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_loading_from_config : Context
    {
        [Test]
        public void then_listener_type_is_mock()
        {
            Listener.Should().NotBeNull().And.BeOfType<LogCastTraceListenerMock>();
        }

        [Test]
        public void then_random_attribute_is_missing()
        {
            Listener.Attributes["random123"].Should().BeNull();
        }

        [Test]
        public void then_options_are_initialized()
        {
            var worker = Listener.Worker;
            worker.SystemType.Should().Be("TEST");
            worker.Layout.Should().Be("{date} | {message}");
            worker.FallbackLogDirectory.Should().Be("fallbackFolder");
            worker.DaysToKeepFallbackLogs.Should().Be(333);
            worker.SkipPercentage.Should().Be("0");

            var options = worker.Options;
            options.Should().BeEquivalentTo(new LogCastOptions("http://10.4.147.105")
            {
                Throttling = 100,
                RetryTimeout = TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(2)),
                SendingThreadCount = 1,
                SendTimeout = TimeSpan.FromSeconds(30),
                EnableSelfDiagnostics = true
            });
        }
    }
}