using System.Net.Http;
using LogCast.Http.Contract;
using Ploeh.AutoFixture;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_response
{
    public abstract class Context : given_HttpMessageInspector.Context
    {
        protected HttpResponseMessage ResponseMessage;
        protected Response Response;

        public override void Arrange()
        {
            base.Arrange();
            ResponseMessage = Fixture.Create<HttpResponseMessage>();
        }

        public override void Act()
        {
            Response = Sut.GetResponse(LogCastContext, ResponseMessage);
        }
    }
}