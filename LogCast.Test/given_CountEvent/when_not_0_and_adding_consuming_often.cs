using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_CountEvent
{
    public class when_not_0_and_adding_consuming_often : Context
    {
        private bool _waitAllResult;
        private bool _waitedForWaitFinish;
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(1);

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public override void Act()
        {
            _waitAllResult = true;
            Sut.Increase();
            int toSleep = (int)(TestTimeout.TotalMilliseconds / 10);
            Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            Sut.Increase();
                            Thread.Sleep(toSleep);
                            Sut.Decrease();
                        }
                    }
                );

            _waitedForWaitFinish = Task.Factory.StartNew(
                () => _waitAllResult = Sut.WaitUntil(0, TestTimeout))
                .Wait((int)(TestTimeout.TotalMilliseconds * 3));
        }

        [Test]
        public void then_wait_with_high_timeout_finishes_after_timeout()
        {
            _waitedForWaitFinish.Should().BeTrue();
            _waitAllResult.Should().BeFalse();
        }
    }
}