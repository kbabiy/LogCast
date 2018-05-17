using LogCast.Loggers;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LogCast.Test.given_LogConfig
{
    public class when_using_fluid_configuration_with_lazy_init : Context
    {
        public override void Act()
        {
            LogConfig.BeginConfiguration(Mock.Of<ILogManager>())
                .WithLazyEngineInitialization()
                .WithMaxMessagesPerContext(234)
                .End();
            base.Act();
        }

        [Test]
        public void then_log_is_configured()
        {
            LogConfig.IsConfigured.Should().BeTrue();
            LogConfig.Current.MaxMessagesPerContext.Should().Be(234);
        }

        [Test]
        public void then_engine_is_not_initialized()
        {
            LogConfig.Current.Engine.IsInitialized.Should().BeFalse();
        }
    }
}