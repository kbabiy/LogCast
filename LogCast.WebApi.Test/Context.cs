using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using BddStyle.NUnit;
using Ploeh.AutoFixture;

namespace LogCast.WebApi.Test
{
    public abstract class Context : ContextBase
    {
        protected Fixture Fixture;

        public override void Arrange()
        {
            Fixture = new Fixture();

            Fixture.Customize<HttpRequestMessage>(
                c => c.With(m => m.Content, new StringContent(string.Empty))
                    .Do(m =>
                    {
                        m.Properties[HttpPropertyKeys.RequestContextKey] =
                            new HttpRequestContext();
                    }));

            Fixture.Customize<HttpResponseMessage>(
                c => c.With(m => m.Content, new StringContent(string.Empty)));

            Fixture.Customize<HttpRequestContext>(
                c => c.Without(ctx => ctx.ClientCertificate));
        }
    }
}