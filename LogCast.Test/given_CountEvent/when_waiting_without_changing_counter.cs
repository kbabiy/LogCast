using System;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_waiting_without_changing_counter : Context
    {
        private double _elapsedSeconds;
        public override void Act()
        {
            var timer = Stopwatch.StartNew();
            Sut.WaitUntil(0, TimeSpan.FromSeconds(1));
            timer.Stop();
            _elapsedSeconds = timer.Elapsed.TotalSeconds;
        }

        [Test]
        public void then_returns_immediately()
        {
            _elapsedSeconds.Should().BeLessThan(1);
        }
    }
}