using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_response
{
    public class when_response_status_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogResponseStatus = true
        };

        [Test]
        public void then_response_status_logged()
        {
            Response.Status.Should()
                .Be((int) ResponseMessage.StatusCode)
                .And.Should()
                .NotBe(0);
        }
    }
}