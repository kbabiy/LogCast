using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_adding_long_message : Context
    {
        private string _longMessage;

        public override void Arrange()
        {
            base.Arrange();
            _longMessage = new string('#', 500);
            AddInfo(_longMessage);
        }

        [Test]
        public void then_message_is_trimmed_to_300_characters()
        {
            var expected = GetTrimmedMessge('#', 300, 200);
            Summary.Message.Should().Be(expected);
        }

        [Test]
        public void then_details_contains_full_message()
        {
            Summary.Details.Should().Contain(_longMessage);
        }
    }
}