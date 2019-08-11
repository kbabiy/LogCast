using System;
using System.Collections.Generic;
using LogCast.Delivery;
using LogCast.Fallback;
using Moq;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent.and_ConsumeDocument_fails_on_send
{
    public abstract class Context : and_send_mocked.Context
    {
        protected const string TestMessage = "test_error_call";
        protected Mock<IFallbackLogger> LoggerMock;
        protected GloballyFailingLogCastClientMock GloballyFailingMock;
        protected override string SendingThreadCount => "1";

        public override void Arrange()
        {
            LoggerMock = new Mock<IFallbackLogger>(MockBehavior.Loose);
            Sut = new GloballyFailingLogCastClientMock(Options, LoggerMock.Object);
            GloballyFailingMock = (GloballyFailingLogCastClientMock) Sut;
        }

        protected class GloballyFailingLogCastClientMock : LogCastClientMock
        {
            private bool _skipExceptions;
            public readonly List<int> Drops = new List<int>();

            public GloballyFailingLogCastClientMock(LogCastOptions options, IFallbackLogger fallbackLogger)
                : base(options, fallbackLogger)
            {}

            public void SkipExceptions()
            {
                _skipExceptions = true;
            }

            protected override bool ConsumeDocument(int threadId, LogCastMessageFactory logCastMessageFactory, 
                int timeoutMsec, int dropCount, int previousRetryCount, out int newRetries)
            {
                StoreMessage(null, TestMessage, 10);
                if (dropCount > 0)
                    Drops.Add(dropCount);

                if (!_skipExceptions)
                    throw new Exception("Global error during consuming document");

                newRetries = 0;
                return true;
            }
        }

        public override void Act()
        {
            SendTestMessage();
            Sut.WaitAll(TimeSpan.FromMinutes(1));
        }
    }
}