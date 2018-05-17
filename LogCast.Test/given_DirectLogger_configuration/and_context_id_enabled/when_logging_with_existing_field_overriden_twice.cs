using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_existing_field_overriden_twice : Context
    {
        public override void Act()
        {
            Logger.Info("original message");
            Logger.AddContextProperty(Property.Message, "1");
            Logger.AddContextProperty(Property.Message, "2");
            Complete();

        }

        [Test]
        public void then_both_overrides_are_applied()
        {
            LastLog.GetProperty<object>(Property.Message).Should().BeEquivalentTo(new[] { "1", "2" });
        }
    }
}