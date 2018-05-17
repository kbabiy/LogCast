using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public class when_uri_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogRequestUri = true
        };

        [Test]
        public void then_uri_is_logged()
        {
            Request.Uri
                .Should()
                .NotBeNullOrEmpty()
                .And.Be(RequestMessage.RequestUri.AbsoluteUri);
        }
    }
}