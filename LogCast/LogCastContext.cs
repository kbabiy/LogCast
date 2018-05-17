using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using LogCast.Context;
using LogCast.Inspectors;

namespace LogCast
{
    /// <summary>
    /// Marks the LogCast context boundaries used to build Unit of Work (UoW)
    /// </summary>
    ///<remarks>Nested usage is supported. All nested contexts are treated as separate UoWs</remarks>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogCastContext : IDisposable, IContextTime
    {
        public static LogCastContext Current
        {
            get
            {
                var context = Context.Context.GetCurrent<LogCastContext>();
                return context?._isDisposed == false ? context : null;
            }
        }

        public static DateTime PreciseNow => Current?.Now ?? DateTime.Now;

        private readonly Stopwatch _startTimer;
        public DateTime StartedAt { get; }
        public DateTime Now => StartedAt.Add(Elapsed);
        public TimeSpan Elapsed => _startTimer.Elapsed;

        public string CorrelationId { get; set; }
        public string OperationName { get; set; }
        public bool? SuppressMessages { get; set; }
        public bool SuppressEmtpyContextMessages { get; set; }

        /// <summary>
        /// Use this collection to add custom properties to the context
        /// On context disposal these properties are going to be added to resulting log message along with the accumulated messages
        /// </summary>
        public ContextPropertiesCollection Properties { get; }

        internal ContextDataCollection<LogMessage> PendingMessages { get; }
        internal ContextDataCollection<LogCastBranchData> BranchHistory { get; }

        private bool _isDisposed;
        private int _lastBranchId;

        public LogCastContext(
            [CanBeNull] string correlationId = null,
            [CanBeNull] [CallerMemberName] string operationName = null)
        {
            StartedAt = DateTime.Now;
            _startTimer = Stopwatch.StartNew();

            var logConfig = LogConfig.Current;
            CorrelationId = correlationId ?? GenerateCorrelationId(logConfig);
            OperationName = operationName;

            Properties = new ContextPropertiesCollection();
            PendingMessages = new ContextDataCollection<LogMessage>(logConfig.MaxMessagesPerContext);
            BranchHistory = new ContextDataCollection<LogCastBranchData>();

            Context.Context.Register(this);
        }

        private static string GenerateCorrelationId(LogConfig logConfig)
        {
            var result = Guid.NewGuid().ToString();

            var type = logConfig.Engine.GetInspector<ConfigurationInspector>()?.AppName;
            if (type != null)
                result = $"{type}-{result}";

            return result;
        }

        public int GetNextBranchId()
        {
            return Interlocked.Increment(ref _lastBranchId);
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            try
            {
                Properties.SetReadOnly();
                PendingMessages.SetReadOnly();
                BranchHistory.SetReadOnly();

                if (SuppressEmtpyContextMessages && PendingMessages.IsEmpty)
                    SuppressMessages = true;

                _startTimer.Stop();
                if (SuppressMessages != true)
                    LogConfig.Current.Engine.OnContextClose(this);
            }
            finally
            {
                Context.Context.Unregister<LogCastContext>();
            }
        }
    }
}