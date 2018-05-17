using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_decreasing_withouth_add : Context
    {
        [Test]
        public void then_no_error()
        {
            Sut.Invoking(t => t.Decrease()).Should().NotThrow();            
        }
    }
}