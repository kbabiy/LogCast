using LogCast.Context;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Loggers;
using LogCast.Rendering;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_using_fluid_configuration : Context
    {
        private Mock<ILogManager> _managerMock;
        private ConfigMocks _config;

        public override void Arrange()
        {
            _managerMock = new Mock<ILogManager>();
            _config = new ConfigMocks
            {
                EngineFactory = new Mock<ILogCastEngineFactory>(),
                ClientFactory = Mock.Of<ILogCastClientFactory>(),
                Details = Mock.Of<IDetailsFormatter>()
            };
            base.Arrange();
        }

        public override void Act()
        {
            LogConfig.BeginConfiguration(_managerMock.Object)
                .WithLogCastEngineFactory(_config.EngineFactory.Object)
                .WithLogCastClientFactory(_config.ClientFactory)
                .WithDetailsFormatter(_config.Details)
                .WithMaxMessagesPerContext(ConfigMocks.MaxMessagesPerContext)
                .WithContextStrategy(new StubContextStrategy())
                .End();
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

        [Test]
        public void then_specified_settings_are_used()
        {
            var config = LogConfig.Current;
            config.DetailsFormatter.Should().BeSameAs(_config.Details);
            config.MaxMessagesPerContext.Should().Be(ConfigMocks.MaxMessagesPerContext);
            config.ContextStrategy.Should().BeOfType<StubContextStrategy>();

            _config.EngineFactory.Verify(x => x.Create(_config.ClientFactory, _config.Details), Times.Once);

        }

        private class ConfigMocks
        {
            public const int MaxMessagesPerContext = 143;

            public IDetailsFormatter Details;
            public ILogCastClientFactory ClientFactory;
            public Mock<ILogCastEngineFactory> EngineFactory;
        }

        private class StubContextStrategy : ContextStrategy
        {
            public override ContextContainer<T> GetContainer<T>()
            {
                throw new System.NotImplementedException();
            }

            public override void StoreContainer<T>(ContextContainer<T> container)
            {
                throw new System.NotImplementedException();
            }

            public override void RemoveContainer<T>()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}