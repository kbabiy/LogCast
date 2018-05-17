using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_context_returned_from_long_running_thread
{
    public class when_returned : Context
    {
        [Test]
        public void then_it_is_null()
        {
            InnerContext.Should().BeNull();
        }
    }
}