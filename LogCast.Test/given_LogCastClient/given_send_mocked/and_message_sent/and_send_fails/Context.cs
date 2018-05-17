using System;
using System.Threading;
using LogCast.Delivery;
using LogCast.Fallback;
using Moq;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent.and_send_fails
{
    public abstract class Context : and_message_sent.Context
    {
        protected Mock<IFallbackLogger> LoggerMock;

        public override void Arrange()
        {
            LoggerMock = new Mock<IFallbackLogger>(MockBehavior.Loose);
            Sut = new FailingLogCastClientMock(Options, LoggerMock.Object, FailCount);
        }

        protected virtual int FailCount => 1;

        class FailingLogCastClientMock : LogCastClientMock
        {
            private readonly int _failedCount;
            private int _cnt;
            public FailingLogCastClientMock(LogCastOptions options, IFallbackLogger fallbackLogger, int failedCount)
                : base(options, fallbackLogger)
            {
                _failedCount = failedCount;
            }

            protected override void Send(Uri endpoint, string message, int timeout)
            {
                Thread.Sleep(10);
                if (++_cnt <= _failedCount)
                    throw new Exception();

                StoreMessage(endpoint, message, timeout);
            }
        }
    }
}