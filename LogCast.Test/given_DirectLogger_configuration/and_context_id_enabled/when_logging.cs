using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging : Context
    {
        public override void Act()
        {
            Logger.Warn(TestMessage);
            Complete();
        }

        [Test]
        public void then_logging_happenned_after_completion()
        {
            LastLog.GetProperty<string>(Property.Message).Should().Be(TestMessage);
        }

        [Test]
        public void then_operation_is_set()
        {
            LastLog.GetProperty<string>(Property.OperationName).Should().Be(OperationName);
        }

        [Test]
        public void then_correlation_id_is_set()
        {
            LastLog.GetProperty<string>(Property.CorrelationId).Should().Be(CorrelationId);
        }

        [Test]
        public void then_log_level_is_set()
        {
            LastLog.GetProperty<string>(Property.LogLevel).Should().Be(LogLevel.Warn.ToString());
        }
        
        [Test]
        public void then_log_level_code_is_set()
        {
            LastLog.GetProperty<int>(Property.LogLevelCode).Should().Be((int) LogLevel.Warn);
        }
    }
}