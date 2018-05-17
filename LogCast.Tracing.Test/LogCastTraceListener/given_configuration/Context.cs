using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BddStyle.NUnit;
using BddStyle.NUnit.Utilities;
using LogCast.Test;

namespace LogCast.Tracing.Test.LogCastTraceListener.given_configuration
{
    public abstract class Context : ContextBase
    {
        // This is also a test - GetLogger shouldn't need any configuration
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static readonly ILogger StaticLogger = LogManager.GetLogger();

        private AppConfig _config;
        protected Tracing.LogCastTraceListener Listener;
        protected ILogger Logger;
        protected LogCastClientMock ClientMock;

        protected string TestTraceSource = "TestSource";

        public override void Arrange()
        {
            _config = AppConfig.ChangeByResource(this, "App.config");
            Trace.Refresh();

            LogConfig.DisableAutoConfig = true;
            if (LogConfig.IsConfigured)
            {
                LogConfig.Reset();
            }

            ClientMock = new LogCastClientMock();
            LogConfig.BeginConfiguration(new TraceLogManager(TestTraceSource))
                .WithLogCastClientFactory(new LogCastClientFactoryMock(ClientMock))
                .End();
            Logger = LogManager.GetLogger();

            if (Listener == null)
            {
                Listener = GetListener(TestTraceSource);
            }

            ClientMock.Options = Listener.Worker.Options;
            ClientMock.LastLog = null;
            ClientMock.LastWaitAll = null;
            ClientMock.WaitAllTimeout = TimeSpan.FromMilliseconds(10);
        }

        public override void Cleanup()
        {
            _config.Dispose();
        }

        private static Tracing.LogCastTraceListener GetListener(string traceSource)
        {
            var source = new TraceSource(traceSource);
            TraceListener listener = source.Listeners[0];
            return (Tracing.LogCastTraceListener)listener;
        }
    }
}