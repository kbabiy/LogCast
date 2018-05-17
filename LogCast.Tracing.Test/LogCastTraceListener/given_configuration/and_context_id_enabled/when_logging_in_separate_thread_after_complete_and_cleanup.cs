using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration.and_context_id_enabled
{
    public class when_logging_in_separate_thread_after_complete_and_cleanup : Context
    {
        [Test]
        public void then_message_should_be_sent_standalone()
        {
            var t = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                Logger.Warn(TestMessage);
            });

            Complete();
            Cleanup();
            t.Wait();

            ClientMock.LastLog.Should().NotBeNull();
            ClientMock.LastLog.GetProperty<string>(Property.OperationName).Should().BeNull();
        }
    }
}