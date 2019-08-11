using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_adding_then_decreasing : Context
    {
        private int _countAfterAdding;
        private bool _decreaseWaitSuccess;

        public override void Act()
        {
            var increases = StartIncreases();
            Task.WaitAll(increases, TimeSpan.FromSeconds(10));
            _countAfterAdding = Sut.Count;

            StartDecreases();

            _decreaseWaitSuccess = Sut.WaitUntil(0, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void then_after_adding_count_is_correct()
        {
            _countAfterAdding.Should().Be(ThreadCount);
        }

        [Test]
        public void then_decrease_wait_is_successful()
        {
            _decreaseWaitSuccess.Should().BeTrue();
        }

        [Test]
        public void then_after_decrease_count_is_zero()
        {
            Sut.Count.Should().Be(0);
        }
    }
}