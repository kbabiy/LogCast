using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastContextBranch
{
    public class when_creating_out_of_context : Context
    {
        private Exception _error;
        public override void Act()
        {
            try
            {
                using (new LogCastContextBranch())
                {
                }
            }
            catch (Exception ex)
            {
                _error = ex;
            }
        }

        [Test]
        public void then_should_throw_invalid_operation_exception()
        {
            _error.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        }
    }
}