using System;
using System.Diagnostics.CodeAnalysis;
using LogCast.Rendering;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Tracing.Test.given_Layout_rendered
{
    public class when_instantiated : Context
    {
        [Test]
        public void then_4_placeholders_created()
        {
            Sut.ParsedPattern.Should().Be("{3:ddd, hh-mm-ss} | {1} | {2} | {0}");
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void then_non_existing_placehoder_errors()
        {
            this.Invoking(_ => new MessageLayout(TestPattern + " + {nonexisting}"))
                .Should().Throw<ArgumentException>().Where(e => e.Message.Contains("{nonexisting}"));
        }
    }
}