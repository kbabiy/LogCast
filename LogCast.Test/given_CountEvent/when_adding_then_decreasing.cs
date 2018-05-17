using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class when_adding_then_decreasing : Context
    {
        private int _countAfterAdding;

        public override void Act()
        {
            var tasks = Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Increase)).ToArray();
            Task.WaitAll(tasks, TimeSpan.FromSeconds(10));
            _countAfterAdding = Sut.Count;

            Enumerable.Range(0, ThreadCount).Select(
                i => Task.Factory.StartNew(Sut.Decrease))
                .ToArray();

            Sut.WaitUntil(0, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void then_after_adding_count_is_correct()
        {
            _countAfterAdding.Should().Be(ThreadCount);
        }

        [Test]
        public void then_after_wait_for_decrease_count_is_zero()
        {
            Sut.Count.Should().Be(0);
        }
    }
}