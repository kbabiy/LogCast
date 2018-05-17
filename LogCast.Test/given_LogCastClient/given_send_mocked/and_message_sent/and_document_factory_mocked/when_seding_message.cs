using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_send_mocked.and_message_sent.and_document_factory_mocked
{
    public class when_seding_message : Context
    {
        private int _sendCount;

        protected override void SendTestMessage()
        {
            Sut.Send(() =>
            {
                _sendCount++;
                return new LogCastDocument();
            });
        }
        
        [Test]
        public void then_factory_called_once()
        {
            _sendCount.Should().Be(1);
        }
    }
}