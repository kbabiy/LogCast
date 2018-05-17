using System;
using System.Diagnostics.CodeAnalysis;
using BddStyle.NUnit;
using LogCast.Delivery;
using LogCast.Fallback;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogCastClient.given_type
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class when_creating : ContextBase
    {
        [Test]
        public void then_options_required()
        {
            Assert.Throws<ArgumentNullException>(() => new LogCastClient(null, Mock.Of<IFallbackLogger>()));
        }

        [Test]
        public void then_logger_required()
        {
            Assert.Throws<ArgumentNullException>(() => new LogCastClient(new LogCastOptions("http://host"), null));
        }

        [Test]
        public void then_endpoint_required()
        {
            Assert.Throws<ArgumentNullException>(() => new LogCastClient(new LogCastOptions(null), Mock.Of<IFallbackLogger>()));
        }

        [Test]
        public void then_valid_endpoint_required()
        {
            Assert.Throws<UriFormatException>(() => new LogCastClient(new LogCastOptions("host"), Mock.Of<IFallbackLogger>()));
        }

        [Test]
        public void then_throttling_less_than_10_throws()
        {
            Assert.Throws<ArgumentException>(() =>
            new LogCastClient(LogCastOptions.Parse("http://host", "9", null, "10", null, null), Mock.Of<IFallbackLogger>()));
        }

        [Test]
        public void then_throttling_10_does_not_throws()
        {
            new LogCastClient(LogCastOptions.Parse("http://host", "10", null, "10", null, null), Mock.Of<IFallbackLogger>());
        }
    }
}