using System.Net.Http;
using System.Security.Claims;
using LogCast.Http;
using FluentAssertions;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public class when_claims_enabled : Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogRequestClaims = true
        };

        public override void Arrange()
        {
            base.Arrange();
            RequestMessage.GetRequestContext().Principal = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("claim1", "claim1.value"),
                    new Claim("userId", "userId"),
                    new Claim("swt", "swt value"),
                    new Claim("hmac", "hmac value"),
                    new Claim("RSA-SHA128", "value")
                }));
        }
        //hmac rsa sha
        [Test]
        public void then_claims_are_logged()
        {
            Request.Claims.Should().NotBeNull().And.NotBeEmpty();
        }

        [Test]
        public void then_regular_claims_are_logged()
        {
            Request.Claims["claim1"].Should().BeEquivalentTo("claim1.value");
        }

        [Test]
        public void then_sensitive_claims_are_missing()
        {
            Request.Claims.Should()
                .NotContainKey("userId")
                .And.NotContainKey("userid")
                .And.NotContainKey("swt")
                .And.NotContainKey("hmac")
                .And.NotContainKey("RSA-SHA128");
        }

        [Test]
        public void then_claims_empty_claims_principal_leads_to_empty_claims()
        {
            base.Arrange();
            Act();
            Request.Claims.Should().BeNull();
        }
    }
}