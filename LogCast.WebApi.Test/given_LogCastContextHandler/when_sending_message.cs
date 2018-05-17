using System.Net;
using System.Net.Http;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.WebApi.Test.given_LogCastContextHandler
{
    public class when_sending_message : Context
    {
        public override void Act()
        {
            var invoker = new HttpMessageInvoker(Sut);
            Sut.InnerHandler = new ConstantHandler<int>(ModelValue);
            invoker.SendAsync(Request, new CancellationToken()).Wait();
        }

        [Test]
        public void then_response_is_init()
        {
            Response.Should().NotBeNull();
        }

        [Test]
        public void then_context_is_init()
        {
            LogCastContext.Should().NotBeNull();
        }

        [Test]
        public void then_response_status_is_200()
        {
            Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void then_correlation_id_is_set_from_provider_to_context()
        {
            LogCastContext.CorrelationId.Should().Be(CorrelationId);
        }

        [Test]
        public void then_operation_is_set_from_provider_to_context()
        {
            LogCastContext.OperationName.Should().Be(OperationName);
        }

        [Test]
        public void then_message_inspector_get_request_called()
        {
            MessageInspectorMock
                .Verify(_ => _.GetRequest(It.IsAny<LogCastContext>(), Request), Times.Once);
        }

        [Test]
        public void then_message_inspector_get_response_called()
        {
            MessageInspectorMock
                .Verify(_ => _.GetResponse(It.IsAny<LogCastContext>(), Response), Times.Once);
        }
    }
}