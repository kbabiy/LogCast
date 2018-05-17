using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_existing_field_overridden : Context
    {
        protected override bool SuppressAct => true;

        private void Act(string propertyName, object newValue)
        {
            Logger.Info("original message");
            Logger.AddContextProperty(propertyName, newValue);
            Complete();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(Property.Message, "overridden");
                yield return new TestCaseData(Property.Timestamp, new DateTime());
                yield return new TestCaseData(Property.CorrelationId, "123");
                yield return new TestCaseData(Property.Details, "MY_DETAILS");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void then_override_is_applied(string propertyName, object newValue)
        {
            Act(propertyName, newValue);
            LastLog.GetProperty<object>(propertyName).Should().Be(newValue);
        }
    }
}