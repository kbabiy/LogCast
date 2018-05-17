using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LogCast.Data;
using BddStyle.NUnit;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.Test.given_ComplexLogProperty
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class when_creating_with_default_name : ContextBase
    {
        [Test]
        public void then_exception_thrown_for_aggregation_version()
        {
            this.Invoking(_ => new ComplexLogProperty<int, int>("parent", Property.DefaultChildName, 32, ints => ints.Sum()))
                .Should().Throw<ArgumentException>();
        }

        [Test]
        public void then_exception_thrown_for_simple_version()
        {
            this.Invoking(_ => new ComplexLogProperty<int>("parent", Property.DefaultChildName, 21))
                .Should().Throw<ArgumentException>();
        }
    }
}