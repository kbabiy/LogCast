using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public class when_default : Context
    {
        [Test]
        public void then_null_is_returned()
        {
            Request.Should().BeNull();
        }
    }
}