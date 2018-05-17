using System.Linq;
using System.Threading;
using LogCast.Loggers.Elapsed;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DurationsSumElapsedLogger.and_context_exisits
{
    public class when_creating_and_disposing : Context
    {
        public override void Act()
        {
            using (new DurationsSumElapsedLogger(OperationName))
            {
                Thread.Sleep(OperationDurationMs);
            }
        }
        
        [Test]
        public void then_duration_is_added_to_context_property()
        { 
            GetOperationDurationProperties(CurrentContext, OperationName).Single()
                .Should().BeInRange(OperationDurationMs - 50, OperationDurationMs + 50);
        }
    }
}