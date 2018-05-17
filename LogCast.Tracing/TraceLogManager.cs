using System;
using System.Diagnostics;
using System.Linq;
using LogCast.Engine;
using LogCast.Inspectors;
using LogCast.Loggers;
using JetBrains.Annotations;

namespace LogCast.Tracing
{
    /// <summary>
    /// Pass an instance of this class to <see cref="LogConfig.Configure(ILogManager)"/> method to bind the logging subsystem with NLog
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class TraceLogManager : ILogManager
    {
        public const string FrameworkName = "Trace";

        private readonly TraceSource _source;
        private LogCastTraceListenerWorker _worker;
        private bool _isTraceSourceWorker;

        /// <summary>
        /// Initializes a new instance of <see cref="TraceLogManager"/> class that binds the logging subsystem with 
        /// the trace listeners configured in the specified trace source
        /// </summary>
        public TraceLogManager(string traceSourceName)
        {
            if (string.IsNullOrEmpty(traceSourceName))
                throw new ArgumentNullException(nameof(traceSourceName));

            _source = new TraceSource(traceSourceName);
        }

        internal TraceLogManager(LogCastTraceListener listener, LogCastTraceListenerWorker worker)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));
            _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _isTraceSourceWorker = !Trace.Listeners.Contains(listener);
        }

        public ILoggerBridge GetLoggerBridge(string name)
        {
            if (_source != null)
                return new TraceSourceBridge(name, _source);

            if (!_isTraceSourceWorker)
                return new TraceSourceBridge(name);

            // The listener is from trace source but we don't know the source name - thus it makes to sense create loggers and write to Trace.
            throw new InvalidOperationException("Using of loggers is not supported - you must explicitly configure the logging subsystem!");
        }

        public void Flush(TimeSpan timeout)
        {
            if (_source == null)
            {
                Trace.Flush();
            }
            else
            {
                _source.Flush();
            }  
        }

        void ILogManager.InitializeEngine(ILogCastEngine engine)
        {
            if (_worker == null)
            {
                // This call insures trace listener initialization
                var listeners = _source?.Listeners ?? Trace.Listeners;
                if (listeners.Count == 0 || (listeners.Count == 1 && listeners[0] is DefaultTraceListener))
                {
                    throw new ArgumentException(
                        $"Can not find any configured trace listeners{(_source == null ? null : " in trace source " + _source.Name)}!");
                }

                var logCastListener = listeners.OfType<LogCastTraceListener>().SingleOrDefault();
                if (logCastListener == null)
                {
                    throw new InvalidOperationException(
                        $"'{typeof (LogCastTraceListener).Name}' is not configured{(_source == null ? null : " in trace source " + _source.Name)}!");
                }

                _worker = logCastListener.Worker;
                _isTraceSourceWorker = _source != null;
            }

            engine.Initialize(_worker.Options, _worker.FallbackLogger);
            engine.RegisterInspector(new ConfigurationInspector(_worker.SystemType, FrameworkName));
        }
    }
}