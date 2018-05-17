using System;
using System.Threading;
using LogCast.Delivery;
using LogCast.Engine;
using JetBrains.Annotations;

namespace LogCast.Test
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastClientMock : ILogCastClient
    {
        public LogCastClientMock()
        { }

        public LogCastClientMock(LogCastOptions options)
        {
            Options = options;
        }

        public LogCastOptions Options { get; set; }

        public LogCastDocument LastLog { get; set; }
        public TimeSpan? LastWaitAll { get; set; }
        public TimeSpan WaitAllTimeout { get; set; }
        public bool ThrowExceptionOnSend { get; set; }

        public void Send(Func<LogCastDocument> documentFactory)
        {
            if (ThrowExceptionOnSend)
                throw new Exception("Network error occured.");
            LastLog = documentFactory();
        }

        public bool WaitAll(TimeSpan timeout)
        {
            Thread.Sleep(WaitAllTimeout);
            LastWaitAll = timeout;
            return true;
        }
    }
}