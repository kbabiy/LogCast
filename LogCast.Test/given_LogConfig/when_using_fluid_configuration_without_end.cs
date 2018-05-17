using LogCast.Loggers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_using_fluid_configuration_without_end : Context
    {
        private Mock<ILogManager> _managerMock;

        public override void Arrange()
        {
            _managerMock = new Mock<ILogManager>();
            base.Arrange();
        }

        public override void Act()
        {
            LogConfig.BeginConfiguration(_managerMock.Object).WithMaxMessagesPerContext(23);
            base.Act();
        }

        [Test]
        public void then_log_is_not_configured()
        {
            LogConfig.IsConfigured.Should().BeFalse();
        }
    }
}