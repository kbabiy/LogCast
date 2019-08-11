using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_adding_and_decreasing_in_parallel : Context
    {
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public override void Act()
        {
            StartIncreases();
            Thread.Sleep(100);
            StartDecreases();
        }

        [Test]
        public void then_wait_is_successful()
        {
            Sut.WaitUntil(0, TimeSpan.FromSeconds(10)).Should().BeTrue();
        }
    }
}