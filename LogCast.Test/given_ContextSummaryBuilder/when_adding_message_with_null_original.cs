using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_adding_message_with_null_original : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            AddInfo(null);
        }

        [Test]
        public void then_message_is_null()
        {
            Summary.Message.Should().BeNull();
        }
    }
}