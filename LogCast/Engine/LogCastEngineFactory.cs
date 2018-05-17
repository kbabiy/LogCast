using LogCast.Delivery;
using LogCast.Inspectors;
using LogCast.Rendering;
using LogCast.Utilities;

namespace LogCast.Engine
{
    internal class LogCastEngineFactory : ILogCastEngineFactory
    {
        public ILogCastEngine Create(ILogCastClientFactory clientFactory, IDetailsFormatter detailsFormatter)
        {
            var engine = new LogCastEngine(clientFactory, detailsFormatter);
            engine.RegisterInspector(new EnvironmentInspector(new EnvironmentContext()));

            return engine;
        }
    }
}
