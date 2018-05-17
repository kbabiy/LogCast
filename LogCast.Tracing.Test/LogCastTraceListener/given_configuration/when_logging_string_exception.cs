using System;
using System.Collections.Generic;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public class when_logging_string_exception : Context
    {
        public override void Act()
        {
            Logger.Error("spome teaxt", new Exception("Test"));
        }

        [Test]
        public void then_exception_is_logged()
        {
            ClientMock.LastLog.GetProperty<ExceptionSummary>(Property.Exceptions).Should().BeEquivalentTo(
                new ExceptionSummary
                {
                    Types = new HashSet<string>(new[] { "Exception" }),
                    Values = new[] {"System.Exception: Test"}
                });
        }
    }
}