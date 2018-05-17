using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContext
{
    public class when_creating_nested : Context
    {
        [TestCase("2")]
        [TestCase(CorrelationId)]
        public void then_any_nested_correlation_id_is_applied(string nestedCorrelationId)
        {
            using (new LogCastContext(CorrelationId))
            {
                CurrentContext.CorrelationId.Should().Be(CorrelationId);
                using (new LogCastContext(nestedCorrelationId))
                {
                    CurrentContext.CorrelationId.Should().Be(nestedCorrelationId);
                }
                CurrentContext.CorrelationId.Should().Be(CorrelationId);
            }
        }
    }
}