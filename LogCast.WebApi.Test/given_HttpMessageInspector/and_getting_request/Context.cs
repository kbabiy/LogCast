using System.Net.Http;
using LogCast.Http.Contract;
using Ploeh.AutoFixture;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request
{
    public abstract class Context : given_HttpMessageInspector.Context
    {
        protected HttpRequestMessage RequestMessage;
        protected Request Request;
        
        public override void Arrange()
        {
            base.Arrange();
            RequestMessage = Fixture.Create<HttpRequestMessage>();
        }

        public override void Act()
        {
            Request = Sut.GetRequest(LogCastContext, RequestMessage);
        }
    }
}