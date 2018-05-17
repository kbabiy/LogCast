using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_response
{
    public class when_headers_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogResponseHeaders = true
        };

        public override void Arrange()
        {
            base.Arrange();
            ResponseMessage.Headers.Add(HttpInspector.AuthHeader, "value");
        }

        [Test]
        public void then_headers_logged()
        {
            Response.Headers.Should().NotBeNull();
        }

        [Test]
        public void then_status_is_zero()
        {
            Response.Status.Should().Be(0);
        }

        [Test]
        public void then_auth_header_is_removed()
        {
            Response.Headers[HttpInspector.AuthHeader]
                .Should().BeEquivalentTo(Property.Values.Removed);
        }
    }
}