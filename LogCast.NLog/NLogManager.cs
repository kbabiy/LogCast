using System;
using System.Linq;
using LogCast.Engine;
using LogCast.Inspectors;
using LogCast.Loggers;
using JetBrains.Annotations;
using NLogNamespace = NLog;

namespace LogCast.NLog
{
    /// <summary>
    /// Pass an instance of this class to <see cref="LogConfig.Configure(ILogManager)"/> method to bind the logging subsystem with NLog
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class NLogManager : ILogManager
    {
        public const string FrameworkName = "NLog";
        private LogCastTarget _target;

        /// <summary>
        /// Initializes a new instance of <see cref="NLogManager"/> class that binds the logging subsystem with 
        /// the NLog configuration
        /// </summary>
        public NLogManager()
        { }

        internal NLogManager(LogCastTarget target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public ILoggerBridge GetLoggerBridge(string name)
        {
            return new LoggerBridge(NLogNamespace.LogManager.GetLogger(name));
        }

        public void Flush(TimeSpan period)
        {
            NLogNamespace.LogManager.Flush(period);
        }

        void ILogManager.InitializeEngine(ILogCastEngine engine)
        {
            if (_target == null)
            {
                _target = GetTarget();
            }
            engine.Initialize(_target.Options, _target.FallbackLogger);
            engine.RegisterInspector(new ConfigurationInspector(_target.SystemType, FrameworkName));
        }

        public static LogCastTarget GetTarget()
        {
            var targets = NLogNamespace.LogManager.Configuration?.ConfiguredNamedTargets?.OfType<LogCastTarget>().ToArray();

            if (targets == null)
                throw new InvalidOperationException("Can not find any configured LogCast targets");

            if (targets.Length == 1)
                return targets[0];

            if (targets.Length > 1)
                throw new InvalidOperationException($"Only one target of '{typeof(LogCastTarget).Name}' type can present in the config file!");

            throw new InvalidOperationException($"Can not find any target of '{typeof(LogCastTarget).Name}' type in the config file!");
        }
    }
}