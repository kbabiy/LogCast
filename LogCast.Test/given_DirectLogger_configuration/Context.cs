using System;
using System.Diagnostics.CodeAnalysis;
using LogCast.Engine;
using LogCast.Loggers.Direct;
using BddStyle.NUnit;
using BddStyle.NUnit.Utilities;

namespace LogCast.Test.given_DirectLogger_configuration
{
    public abstract class Context : ContextBase
    {
        protected const string TestMessage = "some_data";

        // This is also a test - GetLogger shouldn't need any configuration
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static readonly ILogger StaticLogger = LogManager.GetLogger();

        private AppConfig _config;

        protected ILogger Logger;
        protected LogCastClientMock ClientMock;
        protected LogCastDocument LastLog => ClientMock.LastLog;
        
        protected DirectLogManager Target;

        public override void Arrange()
        {
            _config = AppConfig.ChangeByResource(this, "App.config");

            LogConfig.DisableAutoConfig = true;
            if (LogConfig.IsConfigured)
                LogConfig.Reset();
            
            Target = new DirectLogManager();
            var clientFactory = new LogCastClientFactoryMock();

            LogConfig.BeginConfiguration(Target)
                .WithLogCastClientFactory(clientFactory)
                .End();
       
            ClientMock = clientFactory.ConfiguredClient;
            ClientMock.LastLog = null;
            ClientMock.LastWaitAll = null;
            ClientMock.WaitAllTimeout = TimeSpan.FromSeconds(1);

            Logger = LogManager.GetLogger();
        }

        public override void Cleanup()
        {
            _config.Dispose();
        }
    }
}