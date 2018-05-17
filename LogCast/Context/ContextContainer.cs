namespace LogCast.Context
{
    public class ContextContainer<T>
    {
        public ContextContainer<T> Parent { get; }
        public T Context { get; }

        internal ContextContainer(T context, ContextContainer<T> parent)
        {
            Context = context;
            Parent = parent;
        }
    }
}