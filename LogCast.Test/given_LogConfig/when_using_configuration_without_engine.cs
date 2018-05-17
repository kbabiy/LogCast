using LogCast.Config.EmptyConfig;
using LogCast.Data;
using LogCast.Loggers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_using_configuration_without_engine : Context
    {
        public override void Act()
        {
            var mock = new Mock<ILogManager>(MockBehavior.Loose);
            mock.Setup(x => x.GetLoggerBridge(It.IsAny<string>())).Returns(Mock.Of<ILoggerBridge>);

            LogConfig.Configure(mock.Object, true);
            base.Act();
        }

        [Test]
        public void then_log_is_configured()
        {
            LogConfig.IsConfigured.Should().BeTrue();
        }

        [Test]
        public void then_engine_is_initialized_and_empty()
        {
            LogConfig.Current.Engine.IsInitialized.Should().BeTrue();
            LogConfig.Current.Engine.Should().BeOfType<EmptyLogCastEngine>();
        }

        [Test]
        public void then_context_throws_no_exception()
        {
            var logger = LogManager.GetLogger();
            using (new LogCastContext())
            {
                logger.Info("Test");
                logger.Error(new LogProperty[] {new LogProperty<int>("key1", 23)}, "My error");
                logger.AddContextProperties(new LogProperty<string>("key2", "test"));
            }
        }
    }
}