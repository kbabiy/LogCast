using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
{
    public class when_logging_from_different_threads : Context
    {
        const string ThirdMessage = "Very different third message";

        public override void Act()
        {
            Task.Factory.StartNew(
                () => { Logger.Error(TestMessage); }).Wait();
            Task.Factory.StartNew(() => Logger.Info(ThirdMessage)).Wait();
        }

        [Test]
        public void then_separate_log_message_is_created()
        {
            LastLog.GetProperty<string>(Property.Message).Should().Be(ThirdMessage);
        }

        [Test]
        public void then_separate_log_message_level_is_info()
        {
            LastLog.GetProperty<string>(Property.LogLevel).Should().Be(LogLevel.Info.ToString());
        }
        
        [Test]
        public void then_separate_log_message_has_no_correlation_id()
        {
            LastLog.GetProperty<string>(Property.CorrelationId).Should().BeNull();
        }
    }
}