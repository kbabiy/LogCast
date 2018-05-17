using NUnit.Framework;
using FluentAssertions;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_custom_attribute : Context
    {
        public override void Act()
        {
            Logger.AddContextProperty("test property", "test value");
            Complete();
        }

        [Test]
        public void then_property_is_added_to_result()
        {
            LastLog.GetProperty<string>("test property").Should().Be("test value");
        }
    }
}