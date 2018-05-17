using System;
using System.Collections.Generic;
using LogCast.Engine;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_DirectLogger_configuration
{
    public class when_logging_string_with_exception : Context
    {
        public override void Act()
        {
            Logger.Error("string based log", new Exception("Test"));
        }

        [Test]
        public void then_exception_is_logged()
        {
            LastLog.GetProperty<ExceptionSummary>(Property.Exceptions).Should().BeEquivalentTo(
                new ExceptionSummary
                {
                    Types = new HashSet<string>(new[] { "Exception" }),
                    Values = new[] {"System.Exception: Test"}
                });
        }
    }
}