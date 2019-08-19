using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent.and_increased
{
    public class when_ran : Context
    {
        [Test]
        public void then_returns_success_immediately()
        {
            Sut.WaitUntil(0, TimeSpan.FromMilliseconds(100)).Should().BeFalse();
        }
    }
}