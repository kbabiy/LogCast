using System;

namespace LogCast.Test.given_LogCastClient.and_send_mocked.and_message_sent
{
    public abstract class Context : and_send_mocked.Context
    {
        public override void Act()
        {
            SendTestMessage();
            Sut.WaitAll(TimeSpan.FromMinutes(1));
        }
    }
}