using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_two_times_advanced : when_logging_two_times
    {
        private IEnumerable<int> _durations;
        private string _details;
        private string[] _detailLines;

        public override void Arrange()
        {
            base.Arrange();
            _durations = null;
            _details = null;
            _detailLines = null;
        }

        public override void Act()
        {
            base.Act();
            _durations = LastLog.GetProperty<IEnumerable<int>>(Property.Durations.Name, Property.DefaultChildName);
            _details = LastLog.GetProperty<string>(Property.Details);
            _detailLines = _details.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        [Test]
        public void then_details_exist()
        {
            _details.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void then_durations_exist()
        {
            _durations.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void then_duration_has_3_entries()
        {
            _durations.Count().Should().Be(3);
        }

        [Test]
        public void then_detail_lines_has_5_entries()
        {
            _detailLines.Length.Should().Be(5);
        }

        [Test]
        public void then_all_odd_detail_entries_are_delimiters()
        {
            for (int i = 0; i < _detailLines.Length; i += 2)
            {
                var line = _detailLines[i];
                line.Should().StartWith("(");
                line.Should().Contain("--");
            }
        }

        [Test]
        public void then_first_message_is_in_details()
        {
            _detailLines[1].Should().EndWith(TestMessage);
        }

        [Test]
        public void then_second_message_is_in_details()
        {
            _detailLines[3].Should().EndWith(SecondTestMessage);
        }
    }
}