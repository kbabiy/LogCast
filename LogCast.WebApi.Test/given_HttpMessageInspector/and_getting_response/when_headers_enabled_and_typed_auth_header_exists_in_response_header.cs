using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_response
{
    public class when_headers_enabled_and_typed_auth_header_exists_in_response_header : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogResponseHeaders = true
        };

        public override void Arrange()
        {
            base.Arrange();
            ResponseMessage.Headers.Add(HttpInspector.AuthHeader, "Bearer tokentokentoken");
        }


        [Test]
        public void then_auth_type_preserved_and_value_removed()
        {
            Response.Headers[HttpInspector.AuthHeader]
                .Should().BeEquivalentTo($"Bearer {Property.Values.Removed}");
        }
    }
}