using System;
using System.Collections.Generic;
using System.Threading;
using BddStyle.NUnit;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;
using Moq;

namespace LogCast.Test.given_LogCastClient.and_send_mocked
{
    public abstract class Context : ContextBase
    {
        protected LogCastClientMock Sut;

        public override void Arrange()
        {
            Sut = new LogCastClientMock(Options, Mock.Of<IFallbackLogger>());
        }

        protected LogCastOptions Options => LogCastOptions.Parse(Host, Throttling, RetryTimeout, SendingThreadCount, SendTimeout, EnableSelfDiagnostics);
        protected virtual string Host => "http://localhost";

        protected virtual string Throttling => "10";
        protected virtual string RetryTimeout => "0:0:10";
        protected virtual string SendingThreadCount => "0";
        protected virtual string SendTimeout => null;
        protected virtual string EnableSelfDiagnostics => null;

        protected class LogCastClientMock : LogCastClient
        {
            public List<string> Messages { get; }
            public Uri LastUri { get; private set; }
            public int LastTimeout { get; private set; }

            public LogCastClientMock(LogCastOptions options, IFallbackLogger fallbackLogger)
                : base(options, fallbackLogger)
            {
                Messages = new List<string>();
            }

            public int ConsumerCount => Threads.Length;

            protected override void Send(Uri endpoint, string message, int timeout)
            {
                Thread.Sleep(100);
                StoreMessage(endpoint, message, timeout);
            }

            protected void StoreMessage(Uri endpoint, string message, int timeout)
            {
                Messages.Add(message);
                LastUri = endpoint;
                LastTimeout = timeout;
            }
        }

        protected virtual void SendTestMessage()
        {
            Sut.Send(() =>
            {
                var doc = new LogCastDocument();
                doc.AddProperty("testKey", "testValue");
                return doc;
            });
        }
    }
}