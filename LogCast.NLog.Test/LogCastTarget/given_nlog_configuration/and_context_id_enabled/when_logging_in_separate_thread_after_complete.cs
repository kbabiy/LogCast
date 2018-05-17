using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration.and_context_id_enabled
{
    public class when_logging_in_separate_thread_after_complete_and_cleanup : Context
    {
        public override void Act()
        {
            var t = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                Logger.Warn(TestMessage);
            });

            Complete();
            Cleanup();
            t.Wait();
        }

        [Test]
        public void then_message_is_sent()
        {
            ClientMock.LastLog.Should().NotBeNull();
        }

        [Test]
        public void then_message_is_standalone()
        {
            ClientMock.LastLog.GetProperty<string>(Property.OperationName).Should().BeNull();
        }
    }
}