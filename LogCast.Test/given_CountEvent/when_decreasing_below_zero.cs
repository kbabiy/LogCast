using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_decreasing_below_zero : Context
    {
        public override void Act()
        {
            Sut.Decrease();
        }

        [Test]
        public void then_wait_returns_success()
        {
            Sut.WaitUntil(0, TimeSpan.FromSeconds(1)).Should().BeTrue();
        }
    }
}