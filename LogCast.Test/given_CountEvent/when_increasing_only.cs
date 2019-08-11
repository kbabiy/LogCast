using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_increasing_only : Context
    {
        public override void Act()
        {
            Sut.Increase();
        }

        [Test]
        public void then_returns_success_immediately()
        {
            Sut.WaitUntil(0, TimeSpan.FromMilliseconds(100)).Should().BeFalse();
        }

    }
}