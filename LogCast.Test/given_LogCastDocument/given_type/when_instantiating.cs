using System.Diagnostics.CodeAnalysis;
using BddStyle.NUnit;
using FluentAssertions;
using LogCast.Engine;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastDocument.given_type
{
    public class when_instantiating : ContextBase
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void then_instantiated_correctly()
        {
            this.Invoking(_ => new LogCastDocument()).Should().NotThrow();
        }
    }
}