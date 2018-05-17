using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_completing_without_logs : Context
    {
        public override void Act()
        {
            Complete();
        }

        [Test]
        public void then_we_should_log_anyway()
        {
            LastLog.Should().NotBeNull();
        }

        [Test]
        public void then_operation_name_should_be_logged()
        {
            LastLog.GetProperty<string>(Property.OperationName).Should().Be(OperationName);
        }

        [Test]
        public void then_correlation_id_should_be_logged()
        {
            LastLog.GetProperty<string>(Property.CorrelationId).Should().Be(CorrelationId);
        }

        [Test]
        public void then_total_duration_should_be_logged()
        {
            LastLog
                .GetProperty<int>(Property.Durations.Name, Property.Durations.Total)
                .Should().BeGreaterOrEqualTo(0);
        }
    }
}