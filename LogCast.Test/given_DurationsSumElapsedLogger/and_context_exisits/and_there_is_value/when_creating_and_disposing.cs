using System.Threading;
using LogCast.Loggers.Elapsed;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DurationsSumElapsedLogger.and_context_exisits.and_there_is_value
{
    public class when_creating_and_disposing : Context
    {
        public override void Arrange()
        {
            base.Arrange();

            using (new DurationsSumElapsedLogger(OperationName))
            {
                Thread.Sleep(50);
            }
        }

        public override void Act()
        {
            using (new DurationsSumElapsedLogger(OperationName))
            {
                Thread.Sleep(OperationDurationMs);
            }
        }

        [Test]
        public void then_another_property_with_same_name_added()
        {
            GetOperationDurationProperties(CurrentContext, OperationName)
                .Should().HaveCount(2);
        }
    }
}
