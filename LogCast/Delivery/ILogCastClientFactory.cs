using LogCast.Fallback;
using JetBrains.Annotations;

namespace LogCast.Delivery
{
    public interface ILogCastClientFactory
    {
        ILogCastClient Create([NotNull]LogCastOptions options, [NotNull] IFallbackLogger logger);
    }
}