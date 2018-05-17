using LogCast.Rendering;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.given_Layout_rendered
{
    public class when_rendering_with_thread_id : Context
    {
        public override void Act()
        {
            Sut = new MessageLayout("{threadId}");
            base.Act();
        }

        [Test]
        public void then_result_exists()
        {
            int.TryParse(ParseResult, out _).Should().BeTrue();
        }
    }
}