using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration.and_context_id_enabled
{
    public class when_logging_two_times : Context
    {
        private LogCastDocument _lastLog;
        private const string SecondTestMessage = "Another data";

        public override void Act()
        {
            Logger.Warn(TestMessage);
            Logger.Error(SecondTestMessage);
            Complete();

            _lastLog = ClientMock.LastLog;
        }

        [Test]
        public void then_higher_log_severity_is_picked_out_of_merge()
        {
            _lastLog.GetProperty<string>(Property.LogLevel)
                .Should().Be("Error",
                    "Merge should have replaced accumulated value of the second log call with higher severity");
        }

        [Test]
        public void then_details_are_set()
        {
            _lastLog.GetProperty<string>(Property.Details).Should().NotBeNullOrEmpty();
        }

        [Test]
        public void then_durations_total_is_populated()
        {
            _lastLog.GetProperty<int>(Property.Durations.Name, Property.Durations.Total)
                .Should().BeGreaterOrEqualTo(0);
        }

        [Test]
        public void then_durations_is_populated()
        {
            _lastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName)
                .Count().Should().Be(3);
        }

        [Test]
        public void then_total_is_greater_or_equal_to_durations()
        {
            var durations = _lastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName);
            var total = _lastLog.GetProperty<int>(Property.Durations.Name, Property.Durations.Total);
            total.Should().BeGreaterOrEqualTo(durations.Sum());
        }
    }
}