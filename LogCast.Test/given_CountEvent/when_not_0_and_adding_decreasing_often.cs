using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_not_0_and_adding_decreasing_often : Context
    {
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public override void Act()
        {
            Sut.Increase();
            Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Sut.Increase();
                            Thread.Sleep(100);
                            Sut.Decrease();
                        }
                    }
                );
        }

        [Test]
        public void then_wait_returns_false_by_timeout()
        {
            Timed(TimeSpan.FromSeconds(3),
                () => Sut.WaitUntil(0, TimeSpan.FromSeconds(1)))
                .Should().BeFalse();
        }
    }
}