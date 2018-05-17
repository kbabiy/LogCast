using System;
using System.Collections.Generic;

using LogCast.Context;
using LogCast.Delivery;
using LogCast.Engine;
using LogCast.Inspectors;
using LogCast.Rendering;
using JetBrains.Annotations;

namespace LogCast.Config
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class LogConfigSetup
    {
        internal bool IsLazyInitialization { get; private set; }
        internal IDetailsFormatter DetailsFormatter { get; private set; }
        internal ILogCastClientFactory ClientFactory { get; private set; }
        internal ILogCastEngineFactory EngineFactory { get; private set; }
        internal int? MaxMessagesPerContext { get; private set; }
        internal List<ILogDispatchInspector> DispatchInspectors { get; private set; }
        internal ContextStrategy ContextStrategy { get; private set; }

        private readonly Action<LogConfigSetup> _finalAction;

        internal LogConfigSetup(Action<LogConfigSetup> finalAction)
        {
            _finalAction = finalAction ?? throw new ArgumentNullException(nameof(finalAction));
        }

        public LogConfigSetup WithLazyEngineInitialization()
        {
            IsLazyInitialization = true;
            return this;
        }

        public LogConfigSetup WithDetailsFormatter(IDetailsFormatter detailsFormatter)
        {
            DetailsFormatter = detailsFormatter ?? throw new ArgumentNullException(nameof(detailsFormatter));
            return this;
        }

        public LogConfigSetup WithLogCastEngineFactory(ILogCastEngineFactory engineFactory)
        {
            EngineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
            return this;
        }

        public LogConfigSetup WithLogCastClientFactory(ILogCastClientFactory clientFactory)
        {
            ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            return this;
        }

        /// <summary>
        /// Can be called multiple times
        /// </summary>
        public LogConfigSetup WithGlobalInspector(ILogDispatchInspector inspector)
        {
            if (inspector == null)
                throw new ArgumentNullException(nameof(inspector));

            if (DispatchInspectors == null)
            {
                DispatchInspectors = new List<ILogDispatchInspector>();
            }
            DispatchInspectors.Add(inspector);
            return this;
        }

        public LogConfigSetup WithMaxMessagesPerContext(int maxMessagesPerContext)
        {
            if (maxMessagesPerContext >= 0)
            {
                MaxMessagesPerContext = maxMessagesPerContext;
            }
            return this;
        }

        public LogConfigSetup WithContextStrategy(ContextStrategy strategy)
        {
            ContextStrategy = strategy;
            return this;
        }

        public void End()
        {
            _finalAction(this);
        }
    }
}