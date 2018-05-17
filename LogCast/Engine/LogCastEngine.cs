using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LogCast.Delivery;
using LogCast.Fallback;
using LogCast.Inspectors;
using LogCast.Rendering;

namespace LogCast.Engine
{
    internal class LogCastEngine : ILogCastEngine
    {
        private readonly ILogCastClientFactory _clientFactory;
        private readonly LogCastDocumentFactory _documentFactory;
        private readonly List<ILogDispatchInspector> _dispatchInspectors;
        private readonly object _initLock = new object();
        private ILogCastClient _client;
        private bool _enableStats;
        private IFallbackLogger _fallbackLogger;

        public bool IsInitialized { get; private set; }
        public Action<ILogCastEngine> LazyInitializer { get; set; }

        public LogCastEngine(ILogCastClientFactory clientFactory, IDetailsFormatter detailsFormatter)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _documentFactory = new LogCastDocumentFactory(detailsFormatter);
            _dispatchInspectors = new List<ILogDispatchInspector>();
        }

        public void Initialize(LogCastOptions options, IFallbackLogger fallbackLogger)
        {
            if (IsInitialized)
                throw new InvalidOperationException("LogCast engine is already initialized!");

            _fallbackLogger = fallbackLogger;
            _client = _clientFactory.Create(options, fallbackLogger);
            _enableStats = options.EnableSelfDiagnostics;

            IsInitialized = true;
        }

        public void RegisterInspector(ILogDispatchInspector inspector)
        {
            if (inspector == null)
                throw new ArgumentNullException(nameof(inspector));

            var inspectorType = inspector.GetType();
            _dispatchInspectors.RemoveAll(x => x.GetType() == inspectorType);
            _dispatchInspectors.Add(inspector);
        }

        public T GetInspector<T>() where T : ILogDispatchInspector
        {
            var inspectorType = typeof(T);

            // Don't use "OfType" because it also matches for derived classes
            var result = _dispatchInspectors.FirstOrDefault(x => x.GetType() == inspectorType);


            return (T) result;
        }

        public void Flush(TimeSpan timeout)
        {
            if (!IsInitialized)
                return;

            var sw = Stopwatch.StartNew();
            if (_client.WaitAll(timeout))
                _fallbackLogger.Flush(timeout - sw.Elapsed);
        }

        public void SendContextLessMessage(LogMessage message)
        {
            CheckInitialized();
            _client.Send(() => _documentFactory.Create(message, _dispatchInspectors));
        }

        public void OnContextClose(LogCastContext context)
        {
            CheckInitialized();
            _client.Send(() => _documentFactory.Create(context, _dispatchInspectors));
        }

        private void CheckInitialized()
        {
            if (IsInitialized)
                return;

            if (LazyInitializer == null)
                throw new InvalidOperationException("LogCast engine is not initialized");

            lock (_initLock)
            {
                if (IsInitialized)
                    return;

                LazyInitializer.Invoke(this);
                if (!IsInitialized)
                    throw new InvalidOperationException("Failed to initialize LogCast engine");
            }
        }
    }
}