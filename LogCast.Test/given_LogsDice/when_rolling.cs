using System;
using LogCast.Utilities;
using FluentAssertions;
using NUnit.Framework;
using BddStyle.NUnit;

namespace LogCast.Test.given_LogsDice
{
    public class when_rolling : ContextBase
    {
        private WinLoseDice _sut;

        [Test]
        public void then_0_fail_rate_always_wins()
        {
            _sut = new WinLoseDice(0);
            for (int i = 0; i < 100; i++)
            {
                _sut.Roll().Should().BeTrue();
            }
        }

        [Test]
        public void then_100_fail_rate_always_lose()
        {
            _sut = new WinLoseDice(100);
            for (int i = 0; i < 100; i++)
            {
                _sut.Roll().Should().BeFalse();
            }
        }

        [Test]
        public void then_deviation_from_fail_rate_is_within_10_percent_for_1000_rolls()
        {
            RunDeviationTest(1000, 10);
        }

        [Test]
        public void then_deviation_from_fail_rate_is_within_5_percent_for_10000_rolls()
        {
            // These values correlate with each other: the more is numberOfRolls the less is deviation 
            // For 10000 rools 5% deviation is satisfied
            RunDeviationTest(10000, 5);
        }

        private void RunDeviationTest(int numberOfRolls, int maxDeviation)
        {
            int failRate = 30;
            int failsCount = 0;
            _sut = new WinLoseDice(failRate);
            for (int i = 0; i < numberOfRolls; i++)
            {
                if (!_sut.Roll())
                {
                    failsCount++;
                }
            }

            var deviation = Math.Abs(failRate - failsCount * 100 / (double)numberOfRolls);
            deviation.Should().BeLessOrEqualTo(maxDeviation);
        }
    }
}