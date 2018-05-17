using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContext
{
    public class when_creating_with_operation_name : Context
    {
        private const string OperationName = "TestOperationName";
        private LogCastContext _current;

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public override void Act()
        {
            using (new LogCastContext(CorrelationId, OperationName))
            {
                _current = CurrentContext;
            }
        }

        [Test]
        public void then_correlation_id_is_set()
        {
            _current.CorrelationId.Should().Be(CorrelationId);
        }

        [Test]
        public void then_operation_name_is_set()
        {
            _current.OperationName.Should().Be(OperationName);
        }
    }
}