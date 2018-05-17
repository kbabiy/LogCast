using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration.and_context_id_enabled
{
    public class when_logging_with_correlation_id_null : Context
    {
        private string _generatedCorrelationId;
        private const string TypePrefix = "Examples-";
        protected override string CorrelationId => null;

        public override void Act()
        {
            Logger.Warn(TestMessage);
            Complete();
            _generatedCorrelationId = LastLog.GetProperty<string>(Property.CorrelationId);
        }

        [Test]
        public void then_correlation_id_is_generated()
        {
            _generatedCorrelationId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void then_generated_correlation_id_starts_from_type()
        {
            _generatedCorrelationId.Should().StartWith(TypePrefix);
        }

        [Test]
        public void then_rest_of_generated_correlation_id_is_guid()
        {
            var noPrefix = _generatedCorrelationId.Substring(TypePrefix.Length, _generatedCorrelationId.Length - TypePrefix.Length);
            Guid.TryParse(noPrefix, out _).Should().BeTrue($"'{_generatedCorrelationId}' is expected to be prefix+guid");
        }
    }
}