using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.Threading;

namespace LogCast.Wcf
{
    public class OperationContextSynchronizationContext : SynchronizationContext
    {
        private readonly OperationContext _context;

        public OperationContextSynchronizationContext(OperationContext context)
        {
            _context = context;
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            CallCallback(d, state);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
           CallCallback(d, state);
        }

        private void CallCallback(SendOrPostCallback d, object state)
        {
            OperationContext opCtx = _context;
            InternalState internalState = new InternalState()
            {
                OpCtx = opCtx,
                Callback = d,
                State = state,
                SyncCtx = this
            };
            ThreadPool.QueueUserWorkItem(InternalInvoker, internalState);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static void InternalInvoker(object internalState)
        {
            InternalState internalSt = internalState as InternalState;
            
            SetSynchronizationContext(internalSt.SyncCtx);
            using (new OperationContextScope(internalSt.OpCtx))
            {
                internalSt.Callback.Invoke(internalSt.State);
            }
        }
        private class InternalState
        {
            public SynchronizationContext SyncCtx { get; set; }
            public OperationContext OpCtx { get; set; }
            public SendOrPostCallback Callback { get; set; }
            public object State { get; set; }
        }
    }
}