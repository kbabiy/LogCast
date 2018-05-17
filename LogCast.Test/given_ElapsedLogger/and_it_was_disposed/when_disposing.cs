using LogCast.Loggers.Elapsed;
using BddStyle.NUnit;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_ElapsedLogger.and_it_was_disposed
{
    public class when_disposing : ContextBase
    {
        private Mock<ILogger> _loggerMock;
        private ElapsedLogger _sut;

        public override void Arrange()
        {
            _loggerMock = new Mock<ILogger>();
            
            _sut = new ElapsedLogger(_loggerMock.Object, "any");
            _sut.Dispose();

            base.Arrange();
        }

        public override void Act()
        {
            _sut.Dispose();
        }

        [Test]
        public void then_operation_duration_is_logged()
        {
            _loggerMock.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}