namespace LogCast.Context
{
    public abstract class ContextStrategy
    {
        public abstract ContextContainer<T> GetContainer<T>();
        public abstract void StoreContainer<T>(ContextContainer<T> container);
        public abstract void RemoveContainer<T>();

        protected static string Key<T>()
        {
            return typeof(T).FullName;
        }
    }
}