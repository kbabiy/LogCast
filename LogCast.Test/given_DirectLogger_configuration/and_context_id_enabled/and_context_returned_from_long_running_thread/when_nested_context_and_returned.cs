using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_context_returned_from_long_running_thread
{
    public class when_nested_context_and_returned : Context
    {
        public override void Arrange()
        {
            using (new LogCastContext("42"))
            {
                base.Arrange();
            }
        }

        [Test]
        public void then_it_is_null()
        {
            InnerContext.Should().BeNull();
        }
    }
}