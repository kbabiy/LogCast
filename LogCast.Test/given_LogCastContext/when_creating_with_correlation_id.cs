using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContext
{
    public class when_creating_with_correlation_id : Context
    {
        private LogCastContext _current;

        public override void Act()
        {
            using (new LogCastContext(CorrelationId))
            {
                _current = CurrentContext;
            }
        }

        [Test]
        public void then_correlation_id_is_set()
        {
            _current.CorrelationId.Should().Be(CorrelationId);
        }
    }
}