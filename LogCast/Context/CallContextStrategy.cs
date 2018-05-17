using System.Runtime.Remoting.Messaging;

namespace LogCast.Context
{
    public class CallContextStrategy : ContextStrategy
    {
        public override ContextContainer<T> GetContainer<T>()
        {
            return (ContextContainer<T>)CallContext.LogicalGetData(Key<T>());
        }

        public override void StoreContainer<T>(ContextContainer<T> container)
        {
            CallContext.LogicalSetData(Key<T>(), container);
        }

        public override void RemoveContainer<T>()
        {
            CallContext.FreeNamedDataSlot(Key<T>());
        }
    }
}