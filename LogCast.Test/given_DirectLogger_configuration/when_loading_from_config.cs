using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
{
    public class when_loading_from_config : Context
    {
        [Test]
        public void then_endpoint_is_initialized()
        {
            ClientMock.Options.Endpoint.Should().Be("http://kibana.test.247ms.com:9200");
        }

        [Test]
        public void then_throttling_is_initialized()
        {
            ClientMock.Options.Throttling.Should().Be(1000);
        }

        [Test]
        public void then_type_is_initialized()
        {
            Target.Options.SystemType.Should().Be("Examples");
        }
        
        [Test]
        public void then_sending_thread_count_is_initialized()
        {
            ClientMock.Options.SendingThreadCount.Should().Be(1);
        }

        [Test]
        public void then_fallback_file_name_is_initialized()
        {
            Target.Options.FallbackLogDirectory.Should().Be("logs");
        }

        [Test]
        public void then_days_to_keep_logs_is_initialized()
        {
            Target.Options.DaysToKeepFallbackLogs.Should().Be(42);
        }

        [Test]
        public void then_enable_self_diagnostics_initialized()
        {
            ClientMock.Options.EnableSelfDiagnostics.Should().Be(true);
        }
    }
}