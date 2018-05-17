using LogCast.Engine;
using LogCast.Loggers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_using_minimal_configuration : Context
    {
        private Mock<ILogManager> _managerMock;

        public override void Arrange()
        {
            _managerMock = new Mock<ILogManager>();
            base.Arrange();
        }

        public override void Act()
        {
            LogConfig.Configure(_managerMock.Object);
            base.Act();
        }

        [Test]
        public void then_log_is_configured()
        {
            LogConfig.IsConfigured.Should().BeTrue();
        }

        [Test]
        public void then_initialization_is_forced()
        {
            _managerMock.Verify(x => x.InitializeEngine(It.IsAny<ILogCastEngine>()), Times.Once);
        }
    }
}