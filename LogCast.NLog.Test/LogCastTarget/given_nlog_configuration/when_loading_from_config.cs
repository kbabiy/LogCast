using FluentAssertions;
using LogCast.NLog.Test.Mocks;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
{
    public class when_loading_from_config : Context
    {
        [Test]
        public void then_target_type_is_mock()
        {
            Target.Should().BeOfType<LogCastTargetMock>();
        }

        [Test]
        public void then_endpoint_is_initialized()
        {
            Target.Endpoint.Should().Be("http://10.4.147.105");
        }

        [Test]
        public void then_throttling_is_initialized()
        {
            Target.Throttling.Should().Be("100");
        }

        [Test]
        public void then_retry_timeout_is_initialized()
        {
            Target.RetryTimeout.Should().Be("0:1:2");
        }

        [Test]
        public void then_type_is_initialized()
        {
            Target.SystemType.Should().Be("TEST");
        }
        
        [Test]
        public void then_sending_thread_count_is_initialized()
        {
            Target.SendingThreadCount.Should().Be("1");
        }

        [Test]
        public void then_send_timeout_is_initialized()
        {
            Target.SendTimeout.Should().Be("0:0:10");
        }

        [Test]
        public void then_fallback_file_name_is_initialized()
        {
            Target.FallbackLogDirectory.Should().Be("fallbackFolder");
        }

        [Test]
        public void then_days_to_keep_fallback_logs_is_initialized()
        {
            Target.DaysToKeepFallbackLogs.Should().Be(24);
        }

        [Test]
        public void then_enable_self_diagnostics_initialized()
        {
            Target.EnableSelfDiagnostics.Should().Be("true");
        }
    }
}