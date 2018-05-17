using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_two_times : Context
    {
        protected const string SecondTestMessage = "Another data";

        public override void Act()
        {
            Logger.Warn(TestMessage);
            Logger.Error(SecondTestMessage);
            Complete();
        }

        [Test]
        public void then_higher_log_severity_is_picked_out_of_merge()
        {
            LastLog.GetProperty<string>(Property.LogLevel)
                .Should().Be("Error",
                    "Merge should have replaced accumulated value of the second log call with higher severity");
        }

        [Test]
        public void then_durations_total_is_populated()
        {
            LastLog
                .GetProperty<int>(Property.Durations.Name, Property.Durations.Total)
                .Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void then_durations_is_populated()
        {
            LastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName)
                .Count().Should().Be(3);
        }

        [Test]
        public void then_total_is_greater_or_equal_to_durations()
        {
            var durations = LastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName);
            var total = LastLog.GetProperty<int>(Property.Durations.Name, Property.Durations.Total);
            total.Should().BeGreaterOrEqualTo(durations.Sum());
        }

        [Test]
        public void then_details_are_concatenated_from_2_messages()
        {
            LastLog.GetProperty<string>(Property.Details).Should().NotBeNullOrEmpty();
        }

        [Test]
        public void then_durations_are_populated()
        {
            LastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName)
                .Count().Should().Be(3);
        }
    }
}