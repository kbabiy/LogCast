using LogCast.Fallback;

namespace LogCast.Delivery
{
    internal class LogCastClientFactory : ILogCastClientFactory
    {
        public ILogCastClient Create(LogCastOptions options, IFallbackLogger logger)
        {
            return new LogCastClient(options, logger);
        }
    }
}