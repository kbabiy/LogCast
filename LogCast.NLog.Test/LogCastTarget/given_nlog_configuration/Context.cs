using System;
using System.Diagnostics.CodeAnalysis;
using BddStyle.NUnit;
using BddStyle.NUnit.Utilities;
using LogCast.Test;

namespace LogCast.NLog.Test.LogCastTarget.given_nlog_configuration
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
        protected NLog.LogCastTarget Target;

        public override void Arrange()
        {
            _config = AppConfig.ChangeByResource(this, "App.config");
            global::NLog.LogManager.ReconfigExistingLoggers();

            LogConfig.DisableAutoConfig = true;
            if (LogConfig.IsConfigured)
            {
                LogConfig.Reset();
            }

            ClientMock = new LogCastClientMock();
            LogConfig.BeginConfiguration(new NLogManager())
                .WithLogCastClientFactory(new LogCastClientFactoryMock(ClientMock))
                .End();
            Logger = LogManager.GetLogger();

            if (Target == null)
            {
                Target = NLogManager.GetTarget();
            }

            ClientMock.Options = Target.Options;
            ClientMock.LastLog = null;
            ClientMock.LastWaitAll = null;
            ClientMock.WaitAllTimeout = TimeSpan.FromSeconds(1);
        }

        public override void Cleanup()
        {
            _config.Dispose();
        }
    }
}