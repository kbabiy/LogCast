using LogCast.Engine;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request.and_route_enabled
{
    public class when_ran : Context
    {
        [Test]
        public void then_contract_serialized_is_as_expected()
        {
            var actualSerialized = LogCastDocument.ToJson(Request.Route);
            var expected = LogCastDocument.ToJson(new
            {
                template = "",
                controller = (string)null,
                action = (string)null,
                values = new {key = "value"}
            });

            actualSerialized.Should().Be(expected);
        }
    }
}