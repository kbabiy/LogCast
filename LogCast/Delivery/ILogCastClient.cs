using System;
using LogCast.Engine;

namespace LogCast.Delivery
{
    public interface ILogCastClient
    {
        void Send(Func<LogCastDocument> documentFactory);
        bool WaitAll(TimeSpan timeout);
    }
}