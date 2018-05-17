namespace LogCast.Context
{
    internal static class Context
    {
        public static T GetCurrent<T>()
        {
            var container = LogConfig.Current.ContextStrategy.GetContainer<T>();
            return container == null ? default(T) : container.Context;
        }

        public static T Register<T>(T context)
        {
            var currentContainer = LogConfig.Current.ContextStrategy.GetContainer<T>();
            ContextContainer<T> parentContainer;
            if (currentContainer != null && ReferenceEquals(currentContainer.Context, context))
            {
                // Context is already registered, just return its parent (if any)
                parentContainer = currentContainer.Parent;
            }
            else
            {
                // Context is not yet register so current context (if any) becomes its parent
                parentContainer = currentContainer;
                LogConfig.Current.ContextStrategy.StoreContainer(new ContextContainer<T>(context, parentContainer));
            }

            return parentContainer == null ? default(T) : parentContainer.Context;
        }

        public static void Unregister<T>()
        {
            var container = LogConfig.Current.ContextStrategy.GetContainer<T>();
            LogConfig.Current.ContextStrategy.RemoveContainer<T>();

            var parent = container?.Parent;
            if (parent != null)
                LogConfig.Current.ContextStrategy.StoreContainer(parent);
        }
    }
}