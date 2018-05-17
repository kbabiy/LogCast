using System;
using System.Net.Http;
using Moq;
using Ploeh.AutoFixture;

namespace LogCast.WebApi.Test.given_LogCastContextHandler
{
    public abstract class Context : Test.Context
    {
        protected HttpRequestMessage Request;
        protected int ModelValue;

        protected string CorrelationId;
        protected string OperationName;

        protected LogCastContextHandler Sut;
        protected HttpResponseMessage Response;
        protected LogCastContext LogCastContext;
        
        protected Mock<IHttpMessageInspector> MessageInspectorMock;

        public override void Arrange()
        {
            base.Arrange();

            Request = Fixture.Create<HttpRequestMessage>();
            ModelValue = Fixture.Create<int>();
            CorrelationId = Fixture.Create<string>();
            OperationName = Fixture.Create<string>();

            MessageInspectorMock = new Mock<IHttpMessageInspector>(MockBehavior.Loose);
            MessageInspectorMock
                .Setup(_ => _.GetResponse(It.IsAny<LogCastContext>(), It.IsAny<HttpResponseMessage>()))
                .Callback((LogCastContext c, HttpResponseMessage r) =>
                {
                    Response = r;
                    LogCastContext = c;
                });

            Sut = new LogCastContextHandler(
                GetCorrelationId,
                GetOperationName,
                MessageInspectorMock.Object);
        }

        private string GetOperationName(HttpRequestMessage arg)
        {
            if (arg != Request)
                throw new ArgumentException("Unexpected argument");

            return OperationName;
        }

        private string GetCorrelationId(HttpRequestMessage arg)
        {
            if (arg != Request)
                throw new ArgumentException("Unexpected argument");

            return CorrelationId;
        }
    }
}