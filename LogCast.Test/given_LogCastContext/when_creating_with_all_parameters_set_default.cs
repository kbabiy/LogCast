using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContext
{
    public class when_creating_with_all_parameters_set_default : Context
    {
        private LogCastContext _before;
        private LogCastContext _current;
        private LogCastContext _after;

        public override void Act()
        {
            _before = CurrentContext;
            using (new LogCastContext())
            {
                _current = CurrentContext;
            }
            _after = CurrentContext;
        }

        [Test]
        public void then_no_context_out_of_using_scope()
        {
            _before.Should().BeNull();
            _after.Should().BeNull();
        }

        [Test]
        public void then_operation_is_defaulted()
        {
            _current.OperationName.Should().Be("Act");
        }

        [Test]
        public void then_correlationId_is_set()
        {
            _current.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }
    }
}