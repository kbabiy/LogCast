using LogCast.Loggers.Elapsed;
using BddStyle.NUnit;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_ElapsedLogger
{
    public class when_creating_and_disposing : ContextBase
    {
        private Mock<ILogger> _loggerMock;

        private string _operationName;

        private string _loggedMessage;

        public override void Arrange()
        {
            _loggerMock = new Mock<ILogger>();
            _loggerMock
                .Setup(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((msg, args) => _loggedMessage = msg);
            
            _operationName = "unit-test";
            
            base.Arrange();
        }

        public override void Act()
        {
            using (new ElapsedLogger(_loggerMock.Object, _operationName))
            {
            }
        }

        [Test]
        public void then_operation_duration_should_be_logged()
        {
            _loggedMessage.Should()
                .NotBeNull()
                .And
                .Contain(_operationName);
        }
    }
}
