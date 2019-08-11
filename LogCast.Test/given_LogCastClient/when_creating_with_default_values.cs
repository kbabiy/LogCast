using System;
using BddStyle.NUnit;
using FluentAssertions;
using LogCast.Delivery;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient
{
    public class when_creating_with_default_values : ContextBase
    {
        private LogCastOptions _cut;
        private const string Url = "http://www.google.com/";

        public override void Act()
        {
            _cut = new LogCastOptions(Url);
        }

        [Test]
        public void then_endpoint_is_set()
        {
            _cut.Endpoint.ToString().Should().Be(Url);
        }

        [Test]
        public void then_sending_thread_count_is_defaulted()
        {
            _cut.SendingThreadCount.Should().Be(4);
        }

        [Test]
        public void then_send_timeout_is_defaulted()
        {
            _cut.SendTimeout.Should().Be(TimeSpan.FromSeconds(20));
        }

        [Test]
        public void then_throttling_is_defaulted()
        {
            _cut.Throttling.Should().Be(10000);
        }

        [Test]
        public void then_retry_timeout_is_defaulted()
        {
            _cut.RetryTimeout.Should().Be(TimeSpan.FromMinutes(5));
        }
    }
}