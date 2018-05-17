using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration.and_context_id_enabled
{
    public class when_logging_with_external_correlation_id : Context
    {
        private const string TestCorrelationId = "TestCorrelationId";
        public override void Arrange()
        {
            InitialCorrelationId = TestCorrelationId;
            base.Arrange();
        }

        public override void Act()
        {
            Logger.Warn(TestMessage);
            Complete();
        }

        [Test]
        public void then_logging_works()
        {
            ClientMock.LastLog.GetProperty<string>(Property.Message).Should().Be(TestMessage);
        }

        [Test]
        public void then_correlation_id_populated()
        {
            ClientMock.LastLog.GetProperty<string>(Property.CorrelationId).Should().Be(TestCorrelationId);
        }
    }
}