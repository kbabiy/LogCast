using System;
using System.Diagnostics;
using LogCast.Utilities;
using JetBrains.Annotations;

namespace LogCast.Tracing
{
    [UsedImplicitly]
    public class LogCastTraceListener : TraceListener
    {
        // We create the worker lazily because Atributes are available only after GetSupportedAttributes has been called
        private readonly Lazy<LogCastTraceListenerWorker> _worker;

        public LogCastTraceListener()
        {
            _worker = new Lazy<LogCastTraceListenerWorker>(CreateWorker);
        }

        internal LogCastTraceListenerWorker Worker => _worker.Value;

        protected override string[] GetSupportedAttributes()
        {
            return LogCastTraceListenerWorker.GetSupportedAttributes();
        }

        #region Trace-specific methods
        public override bool IsThreadSafe => true;

        public override void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        public override void Write(string text)
        {
            TraceInternal(TraceEventType.Information, text, null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format,
            params object[] args)
        {
            TraceInternal(eventType, FormatHelper.FormatMessage(format, args), null);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceInternal(eventType, message, null);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceInternal(eventType, data as string, null);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (data != null && data.Length > 0)
            {
                TraceInternal(eventType, data[0] as string, data.Length > 1 ? data[1] as ExtendedTraceData : null);
            }
        }
        #endregion

        private void TraceInternal(TraceEventType eventType, string message, ExtendedTraceData data)
        {
            Worker.Write(eventType, message, data);
        }

        protected virtual LogCastTraceListenerWorker CreateWorker()
        {
            return new LogCastTraceListenerWorker(this, Attributes);
        }
    }
}
