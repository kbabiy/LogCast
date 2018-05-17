using System;
using LogCast.Config;
using LogCast.Config.EmptyConfig;
using LogCast.Context;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Fallback;
using LogCast.Loggers;
using LogCast.Rendering;
using LogCast.Utilities;
using JetBrains.Annotations;

namespace LogCast
{
    /// <summary>
    /// A class used to configure the logging subsystem. As a minimum, during the configuration you must pass
    /// an object implementing <see cref="ILogManager"/> interface to wireup with the logging framework (NLog etc.)
    /// All public methods of this class are not thread-safe and supposed to be called once per process lifetime
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogConfig
    {
        private const int DefaultMaxMessagesPerContext = 10000;

        private static LogConfig _current;
        private bool _isEmpty;

        public ILogCastEngine Engine { get; private set; }
        public IDetailsFormatter DetailsFormatter { get; private set; }
        public int MaxMessagesPerContext { get; private set; }

        internal ContextStrategy ContextStrategy { get; private set; }
        internal ILogManager LogManager { get; }

        // Used in unit tests only
        internal static bool DisableAutoConfig { get; set; }

        private LogConfig(ILogManager logManager)
        {
            LogManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
        }

        /// <summary>
        /// Returns 'true' if the logging subsystem is already configured
        /// </summary>
        public static bool IsConfigured => _current != null && !_current._isEmpty;

        /// <summary>
        /// Returns 'true' if the logging subsystem is in the process of configuration
        /// For use by plugin developers
        /// </summary>
        public static bool IsInProgress { get; private set; }

        /// <summary>
        /// Returns active log configuration. Throws exception is logging subsystem is not configured
        /// </summary>
        public static LogConfig Current
        {
            get
            {
                if (_current == null)
                {
                    if (DisableAutoConfig || !EnvironmentContext.IsUnitTestContext())
                        throw new InvalidOperationException("Logging framework is not configured. Try adding at the start of your program something like: LogConfig.Configure(new NLogManager())");

                    // For unit-test context we automatically initialize empty configuration
                    _current = InitEmptyConfig();
                }
                return _current;
            }
        }

        /// <summary>
        /// Use this method to disable the logging subsystem
        /// Can be called only once per process lifetime.
        /// </summary>
        public static void ConfigureAsDisabled()
        {
            CheckConfigured();

            _current = InitEmptyConfig();
        }

        /// <summary>
        /// Resets existing configuration of the logging subsystem (in case you need reconfigure it)
        /// </summary>
        public static void Reset()
        {
            _current = null;
        }

        /// <summary>
        /// Use this method to configure the logging subsystem with a default configuration
        /// Can be called only once per process lifetime
        /// </summary>
        /// <param name="logManager">An object implementing <see cref="ILogManager"/> interface that binds 
        /// the logging subsystem with a specific logging framework</param>       
        public static void Configure(ILogManager logManager)
        {
            Configure(logManager, false);
        }

        /// <summary>
        /// Use this method to configure the logging subsystem with a default configuration
        /// Can be called only once per process lifetime
        /// </summary>
        /// <param name="logManager">An object implementing <see cref="ILogManager"/> interface that binds 
        /// the logging subsystem with a specific logging framework</param>
        /// <param name="disableLogCastEngine">Specify 'true' if you don't want to send log messages 
        /// but still want to log them using the specified logging framework
        /// In this case <see cref="LogCastContext"/> will be ignored
        /// </param>
        public static void Configure(ILogManager logManager, bool disableLogCastEngine)
        {
            CheckConfigured();

            if (disableLogCastEngine)
            {
                _current = new LogConfig(logManager)
                {
                    DetailsFormatter = new DetailsFormatter(),
                    Engine = new EmptyLogCastEngine(),
                    MaxMessagesPerContext = 0,
                    ContextStrategy = new CallContextStrategy()
                };
                return;
            }

            var config = new LogConfig(logManager);
            var setup = new LogConfigSetup(config.End);
            setup.End();
        }

        /// <summary>
        /// Use this method if you want to customize the logging configuration using fluid syntax
        /// To finalize the configuration <see cref="LogConfigSetup.End"/> method must be called
        /// Can be called only once per process lifetime
        /// </summary>
        /// <param name="logManager">An object implementing <see cref="ILogManager"/> interface that binds 
        /// the logging subsystem with a specific logging framework</param>        
        /// <returns>An instance <see cref="LogConfigSetup"/> class used for fluid-syntax configuration</returns>
        public static LogConfigSetup BeginConfiguration(ILogManager logManager)
        {
            CheckConfigured();

            var config = new LogConfig(logManager);
            return new LogConfigSetup(config.End);
        }

        private void End(LogConfigSetup setup)
        {
            CheckConfigured();
            IsInProgress = true;
            try
            {
                DetailsFormatter = setup.DetailsFormatter ?? new DetailsFormatter();
                MaxMessagesPerContext = setup.MaxMessagesPerContext ?? DefaultMaxMessagesPerContext;
                ContextStrategy = setup.ContextStrategy ?? new CallContextStrategy();

                var clientFactory = setup.ClientFactory ?? new LogCastClientFactory();
                var engineFactory = setup.EngineFactory ?? new LogCastEngineFactory();

                Engine = engineFactory.Create(clientFactory, DetailsFormatter);

                if (setup.DispatchInspectors != null)
                {
                    foreach (var inspector in setup.DispatchInspectors)
                    {
                        Engine.RegisterInspector(inspector);
                    }
                }

                if (setup.IsLazyInitialization)
                {
                    Engine.LazyInitializer = x => LogManager.InitializeEngine(Engine);
                }
                else
                {
                    LogManager.InitializeEngine(Engine);
                }
                _current = this;
            }
            catch (Exception ex)
            {
                new FileFallbackLogger(null, 0).Write(ex, "Failed to configure the logging subsystem.");
                throw;
            }
            finally
            {
                IsInProgress = false;
            }
        }

        private static LogConfig InitEmptyConfig()
        {
            return new LogConfig(new EmptyLogManager())
            {
                _isEmpty = true,
                Engine = new EmptyLogCastEngine(),
                DetailsFormatter = new DetailsFormatter(),
                MaxMessagesPerContext = DefaultMaxMessagesPerContext,
                ContextStrategy = new CallContextStrategy()
            };
        }

        private static void CheckConfigured()
        {
            if (_current != null && !_current._isEmpty)
                throw new InvalidOperationException("Logging framework is already configured!");

            if (IsInProgress)
                throw new InvalidOperationException("The configuration is in progress!");
        }
    }
}
