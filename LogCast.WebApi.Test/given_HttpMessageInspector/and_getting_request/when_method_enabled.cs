using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public class when_method_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogRequestMethod = true
        };

        [Test]
        public void then_method_logged()
        {
            Request.Method
                .Should()
                .NotBeNull()
                .And
                .Be(RequestMessage.Method.Method);
        }
    }
}