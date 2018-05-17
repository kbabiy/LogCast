using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_adding_messages_with_errors : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Warn, "warn 1 ", new InvalidCastException("cast")));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Error, "error 1", new ArgumentNullException()));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Info, "info"));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Error, "error 2", new InvalidOperationException("op")));
            Builder.AddMessage(Factory.CreateMessage(LogLevel.Warn, "warn 2", new AggregateException("agg")));
        }

        [Test]
        public void then_log_level_is_max()
        {
            Summary.Level.Should().Be(LogLevel.Error);
        }

        [Test]
        public void then_message_is_the_last_message()
        {
            Summary.Message.Should().Be("warn 2");
        }

        [Test]
        public void then_all_exceptions_are_present()
        {
            Summary.Exceptions.Select(e => e.GetType().Name)
                .Should().BeEquivalentTo(
                    "InvalidCastException",
                    "ArgumentNullException",
                    "InvalidOperationException",
                    "AggregateException");
        }
    }
}