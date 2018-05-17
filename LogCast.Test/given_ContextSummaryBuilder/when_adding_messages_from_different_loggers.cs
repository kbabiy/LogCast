using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_adding_messages_from_different_loggers : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Builder.AddMessage(Factory.CreateMessage("logger1", LogLevel.Debug, "debug"));
            Builder.AddMessage(Factory.CreateMessage("logger2", LogLevel.Info, "info"));
            Builder.AddMessage(Factory.CreateMessage("logger3", LogLevel.Trace, "trace"));
            Builder.AddMessage(Factory.CreateMessage("logger2", LogLevel.Info, "info 2"));
        }

        [Test]
        public void then_log_level_is_max()
        {
            Summary.Level.Should().Be(LogLevel.Info);
        }

        [Test]
        public void then_message_is_the_last_message()
        {
            Summary.Message.Should().Be("info 2");
        }

        [Test]
        public void then_all_loggers_present_in_summary()
        {
            Summary.Loggers.Should().BeEquivalentTo("logger1", "logger2", "logger3");
        }
    }
}