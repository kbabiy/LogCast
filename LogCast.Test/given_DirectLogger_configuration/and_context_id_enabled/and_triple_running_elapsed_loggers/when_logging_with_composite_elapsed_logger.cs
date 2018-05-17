using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using LogCast.Loggers.Elapsed;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled.and_triple_running_elapsed_loggers
{
    public class when_logging_with_composite_elapsed_logger : Context
    {
        protected override void RunElapsedLoggers(TimeSpan delay)
        {
            using (new AllDurationsElapsedLogger(
                new DurationsAverageElapsedLogger(
                    new DurationsMaxElapsedLogger(
                        new DurationsSumElapsedLogger(
                            new ElapsedLogger(Logger, OperationName))))))
            {
                Thread.Sleep(delay);
            }
        }

        [TestCase("average")]
        [TestCase("max")]
        [TestCase("total")]
        public void then_expected_duration_aggregation_exists(string attributeSuffix)
        {
            var aggregation = (int) GetDurations()[$"{OperationName}.{attributeSuffix}"];
            aggregation.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void then_all_durations_aggregation_as_expected()
        {
            var all = (int[]) GetDurations()[$"{OperationName}"];
            all.Should()
                .NotBeNull()
                .And.HaveCount(3)
                .And.Match(_ => _.All(i => i > 0));
        }

        [Test]
        public void then_elapsed_logger_occurs_in_details_as_expected()
        {
            var details = LastLog.GetProperty<string>(Property.Details);
            Regex.Matches(details, $"'{OperationName}' operation took").Count.Should().Be(3);
        }
    }
}