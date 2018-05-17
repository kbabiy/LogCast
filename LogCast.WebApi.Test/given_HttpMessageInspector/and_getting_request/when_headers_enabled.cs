using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public class when_headers_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogRequestHeaders = true
        };

        public override void Arrange()
        {
            base.Arrange();
            RequestMessage.Headers.Add(HttpInspector.AuthHeader, "AuthValue");
        }

        [Test]
        public void then_headers_logged()
        {
            Request.Headers.Should().NotBeNull();
        }

        [Test]
        public void then_auth_header_is_removed()
        {
            Request.Headers[HttpInspector.AuthHeader]
                .Should().BeEquivalentTo(Property.Values.Removed);
        }
    }
}