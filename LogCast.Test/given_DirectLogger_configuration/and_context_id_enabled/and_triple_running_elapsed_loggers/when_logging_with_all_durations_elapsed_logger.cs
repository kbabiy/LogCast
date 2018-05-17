using System;
using System.Linq;
using System.Threading;
using LogCast.Loggers.Elapsed;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_triple_running_elapsed_loggers
{
    public class when_logging_with_all_durations_elapsed_logger : Context
    {
        protected override void RunElapsedLoggers(TimeSpan delay)
        {
            using (new AllDurationsElapsedLogger(OperationName))
            {
                Thread.Sleep(delay);
            }
        }
        
        [Test]
        public void then_attribute_is_added()
        {
            GetDurations().ContainsKey(OperationName).Should().BeTrue();
        }

        [Test]
        public void then_attribute_has_expected_value()
        {
            var durationsArray = (int[])GetDurations()[OperationName];
            durationsArray.Should()
                .NotBeNull()
                .And.HaveCount(3)
                .And.Match(_ => _.All(i => i > 0));
        }
    }
}