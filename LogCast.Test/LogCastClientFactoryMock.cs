using LogCast.Delivery;
using LogCast.Fallback;

namespace LogCast.Test
{
    public class LogCastClientFactoryMock : ILogCastClientFactory
    {
        public LogCastClientMock ConfiguredClient { get; private set; }


        public LogCastClientFactoryMock(LogCastClientMock configuredClient = null)
        {
            ConfiguredClient = configuredClient;
        }

        public ILogCastClient Create(LogCastOptions options, IFallbackLogger logger)
        {
            ConfiguredClient = ConfiguredClient ?? new LogCastClientMock(options);

            return ConfiguredClient;
        }
    }
}