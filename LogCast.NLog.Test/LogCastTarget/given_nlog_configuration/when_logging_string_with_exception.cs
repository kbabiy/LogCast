using System;
using System.Collections.Generic;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
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
            ClientMock.LastLog.GetProperty<ExceptionSummary>(Property.Exceptions).Should().BeEquivalentTo(
                new ExceptionSummary
                {
                    Types = new HashSet<string>(new[] {"Exception"}),
                    Values = new[] {"System.Exception: Test"}
                });
        }
    }
}