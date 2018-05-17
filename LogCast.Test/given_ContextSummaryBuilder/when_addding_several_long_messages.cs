using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ContextSummaryBuilder
{
    public class when_addding_several_long_messages : Context
    {
        private static readonly string _message0 = "message0";
        private string _longMessage1;
        private string _longMessage1Rendered;
        private static readonly string _message2 = "message2";
        private string _longMessage3;

        public override void Arrange()
        {
            base.Arrange();
            const int halfMegabyte = 1024 * 1024 / 2;
            _longMessage1 = new string('1', halfMegabyte);
            _longMessage3 = new string('3', 500);

            AddInfo(_message0);
            _longMessage1Rendered = AddInfo(_longMessage1).RenderedMessage;
            AddInfo(_message2);
            AddInfo(_longMessage3);
        }

        [Test]
        public void then_message_is_trimmed_independently()
        {
            var expected = GetTrimmedMessge('3', 300, 200);
            Summary.Message.Should().Be(expected);
        }

        [Test]
        public void then_details_contains_short_messages_fully()
        {
            Summary.Details.Should()
                .Contain(_message0)
                .And.Contain(_message2);
        }

        [Test]
        public void then_result_contains_first_long_message_trimmed()
        {
            var cutSymbols = _longMessage1Rendered.Length - 300;
            var expected = $"{_longMessage1Rendered.Substring(0, 300)} ...(cut {cutSymbols})";
            Summary.Details
                .Should()
                .Contain(expected);
        }

        [Test]
        public void then_result_contains_second_long_message_full()
        {
            Summary.Details.Should().Contain(_longMessage3);
        }
    }
}