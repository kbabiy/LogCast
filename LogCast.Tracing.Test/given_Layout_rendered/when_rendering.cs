using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.given_Layout_rendered
{
    public class when_rendering : Context
    {        
        [Test]
        public void then_result_is_as_expected()
        {
            ParseResult.Should().Be("Sat, 04-03-02 | LoggerName | Info | Test message");
        }
    }
}