using LogCast.Delivery;
using LogCast.Rendering;
using JetBrains.Annotations;

namespace LogCast.Engine
{
    public interface ILogCastEngineFactory
    {
        ILogCastEngine Create(
            [NotNull] ILogCastClientFactory clientFactory, 
            [NotNull] IDetailsFormatter detailsFormatter);
    }
}
