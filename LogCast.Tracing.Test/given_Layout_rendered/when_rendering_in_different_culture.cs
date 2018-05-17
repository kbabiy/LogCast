using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.given_Layout_rendered
{
    public class when_rendering_in_different_culture : Context
    {
        public override void Arrange()
        {
            base.Arrange();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("da-DK");
        }

        [Test]
        public void then_result_is_as_expected()
        {
            ParseResult.Should().Be("Sat, 04-03-02 | LoggerName | Info | Test message");
        }
    }
}