using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_adding_informational_messages : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Debug, "debug"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Info, "info 1"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Info, "info 2"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.None, "none"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Info, "info 3"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Trace, "trace"));            
        }

        [Test]
        public void then_log_level_is_max()
        {
            Summary.Level.Should().Be(LogLevel.Info);
        }

        [Test]
        public void then_message_is_the_last_message()
        {
            Summary.Message.Should().Be("trace");
        }
    }
}