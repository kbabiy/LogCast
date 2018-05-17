using System;
using System.Collections;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using LogCast.Context;
using JetBrains.Annotations;

namespace LogCast.Wcf
{
    [UsedImplicitly]
    internal class OperationContextStrategy : ContextStrategy
    {
        private const int FixedOperationContextAssemblyVersion = 1586;

        private static readonly Lazy<int> OperationContextAssemblyVersion
            = new Lazy<int>(InitOperationContextAssemblyVersion, LazyThreadSafetyMode.ExecutionAndPublication);

        private static int InitOperationContextAssemblyVersion()
        {
            var assemblyLocation = typeof(OperationContext).Assembly.Location;
            if (string.IsNullOrWhiteSpace(assemblyLocation))
                throw new InvalidOperationException("Could not get OperationContext assembly location.");
            var version = FileVersionInfo.GetVersionInfo(assemblyLocation);
            return version.FileBuildPart;
        }

        public override ContextContainer<T> GetContainer<T>()
        {
            return OperationContext.Current?.Extensions.Find<Container>()?.Instances[Key<T>()] as ContextContainer<T>;
        }

        public override void StoreContainer<T>(ContextContainer<T> contextContainer)
        {
            // we should not allow to use half working context.
            if (OperationContext.Current == null)
                throw new InvalidOperationException("OperationContext is not available at this scope.");

            if (OperationContextAssemblyVersion.Value < FixedOperationContextAssemblyVersion)
                SynchronizationContext.SetSynchronizationContext(new OperationContextSynchronizationContext(OperationContext.Current));

            var container = OperationContext.Current.Extensions.Find<Container>();
            if (container == null)
                OperationContext.Current.Extensions.Add(container = new Container());
            container.Instances[Key<T>()] = contextContainer;
        }

        public override void RemoveContainer<T>()
        {
            // we should not allow to use half working context.
            if (OperationContext.Current == null)
                throw new InvalidOperationException("OperationContext is not available at this scope.");

            OperationContext.Current.Extensions.Find<Container>()?.Instances.Remove(Key<T>());
        }

        private class Container : IExtension<OperationContext>
        {
            public Hashtable Instances { get; } = new Hashtable();

            void IExtension<OperationContext>.Attach(OperationContext owner)
            {
            }

            void IExtension<OperationContext>.Detach(OperationContext owner)
            {
            }
        }
    }
}