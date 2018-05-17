using System;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_getting_wrong_trace_source : Context
    {
        private string _defaultSource;
        private Exception _configException;

        public override void Arrange()
        {
            _defaultSource = TestTraceSource;
            TestTraceSource = "wrongTrace";
            try
            {
                base.Arrange();
            }
            catch (Exception ex)
            {
                _configException = ex;
            }
        }

        public override void Cleanup()
        {
            TestTraceSource = _defaultSource;
            base.Cleanup();
        }

        [Test]
        public void then_configuration_fails()
        {
            _configException.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }
    }
}