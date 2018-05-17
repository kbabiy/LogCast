using System;
using System.Collections.Generic;
using LogCast.Engine;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_exception : Context
    {
        private Exception _lastException;

        public override void Arrange()
        {
            base.Arrange();
            _lastException = new Exception("last", new Exception("inner"));
        }

        public override void Act()
        {
            Logger.Info("start");
            Logger.Error("first cause", new Exception("first"));
            Logger.Error(_lastException);
            Complete();
        }

        [Test]
        public void then_message_is_formed_by_last_exception_type_and_message()
        {
            LastLog.GetProperty<string>(Property.Message).Should().Be("System.Exception: last");
        }

        [Test]
        public void then_details_contain_first_exception_once()
        {
            AssertContains(LastLog.GetProperty<string>(Property.Details),
                "System.Exception: first", 1);
        }

        [Test]
        public void then_details_contain_last_exception_twice()
        {
            AssertContains(LastLog.GetProperty<string>(Property.Details),
                "System.Exception: last", 2);
        }

        [Test]
        public void then_details_contain_last_exception_full_once()
        {
            AssertContains(LastLog.GetProperty<string>(Property.Details),
                _lastException.ToString(), 1);
        }

        [Test]
        public void then_exception_attribute_is_filled_correctly()
        {
            LastLog.GetProperty<ExceptionSummary>(Property.Exceptions).Should().BeEquivalentTo(
                new ExceptionSummary
                {
                    Types = new HashSet<string>(new[] {"Exception"}),
                    Values = new[]
                    {
                        "System.Exception: first",
                        $"System.Exception: last ---> System.Exception: inner{Environment.NewLine}   --- End of inner exception stack trace ---"
                    }
                });
        }

        private static void AssertContains(
            string actual, string expectedSubstring, int expectedOccurences)
        {
            int actualOccurences = 0;
            int index = -1;
            while (true)
            {
                index = actual.IndexOf(expectedSubstring, index + 1, StringComparison.InvariantCulture);
                if (index < 0)
                    break;

                ++actualOccurences;
            }

            actualOccurences.Should().Be(expectedOccurences,
                $@"substring should occur {expectedOccurences} times, but has {actualOccurences}
Expected substring: '{expectedSubstring}'
Actual: '{actual}'");
        }
    }
}