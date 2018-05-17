using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_correlation_id_update : Context
    {
        private const string NewCorrelationId = "12345";

        public override void Act()
        {
            CurrentContext.CorrelationId = NewCorrelationId;
            Logger.Warn(TestMessage);
            Complete();
        }

        [Test]
        public void then_correlation_id_is_updated()
        {
            LastLog.GetProperty<string>(Property.CorrelationId).Should().Be(NewCorrelationId);
        }
    }
}