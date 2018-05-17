using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_adding_and_decreasing_in_parallel : Context
    {
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public override void Act()
        {
            Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Increase))
                .ToArray();
            Thread.Sleep(100);

            Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Decrease))
                .ToArray();
        }

        [Test]
        public void then_wait_is_successfull()
        {
            Sut.WaitUntil(0, TimeSpan.FromSeconds(10));
        }
    }
}