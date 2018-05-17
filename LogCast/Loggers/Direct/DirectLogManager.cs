using System;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Inspectors;
using LogCast.Rendering;
using JetBrains.Annotations;

namespace LogCast.Loggers.Direct
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class DirectLogManager : ILogManager
    {
        public const string FrameworkName = "Direct";

        private DirectLogger _logger;
        private LogCastOptions _logCastOptions;
        private IFallbackLogger _falbackLogger;

        public DirectLoggerOptions Options { get; private set; }

        public DirectLogManager()
        {
            var configuration = ConfigurationSection.Read();

            var logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), configuration.LogLevel);
            var loggerOptions = new DirectLoggerOptions(logLevel, configuration.Type)
            {
                Layout = configuration.Layout,
                FallbackLogDirectory = configuration.FallbackLogDirectory,
                DaysToKeepFallbackLogs = configuration.DaysToKeepFallbackLogs
            };

            var logCastOptions = LogCastOptions.Parse(configuration.Endpoint, configuration.Throttling,
                null, configuration.SendingThreadCount, null, configuration.EnableSelfDiagnostics);

            Init(loggerOptions, logCastOptions);
        }

        public DirectLogManager(DirectLoggerOptions loggerOptions, LogCastOptions logCastOptions)
        {
            if (loggerOptions == null)
                throw new ArgumentNullException(nameof(loggerOptions));
            if (logCastOptions == null)
                throw new ArgumentNullException(nameof(logCastOptions));

            Init(loggerOptions, logCastOptions);
        }

        private void Init(DirectLoggerOptions loggerOptions, LogCastOptions logCastOptions)
        {
            Options = loggerOptions;
            _logCastOptions = logCastOptions;
            _falbackLogger = new FileFallbackLogger(loggerOptions.FallbackLogDirectory,
                loggerOptions.DaysToKeepFallbackLogs);

            _logger = new DirectLogger(loggerOptions.MinLogLevel,
                new LogMessageRouter(loggerOptions.SkipPercentage),
                _falbackLogger,
                string.IsNullOrEmpty(loggerOptions.Layout) ? null : new MessageLayout(loggerOptions.Layout));

        }

        public ILoggerBridge GetLoggerBridge(string name)
        {
            return new DirectLoggerBridge(name, _logger);
        }

        //Nothing to be done in this implementation here
        public void Flush(TimeSpan timeout)
        { }

        public void InitializeEngine(ILogCastEngine engine)
        {
            engine.Initialize(_logCastOptions, _falbackLogger);
            engine.RegisterInspector(new ConfigurationInspector(Options.SystemType, FrameworkName));
        }
    }
}