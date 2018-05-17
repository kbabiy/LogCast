using System;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent
{
    public abstract class Context : given_send_mocked.Context
    {
        public override void Act()
        {
            SendTestMessage();
            Sut.WaitAll(TimeSpan.FromMinutes(1));
        }
    }
}