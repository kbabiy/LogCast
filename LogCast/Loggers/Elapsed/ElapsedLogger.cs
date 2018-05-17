using System;
using JetBrains.Annotations;

namespace LogCast.Loggers.Elapsed
{
    /// <summary>
    /// Logs time elapsed between consturction and disposal as info text message
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class ElapsedLogger : ElapsedLoggerBase
    {
        private ILogger _logger;

        /// <exception cref="ArgumentNullException">logger arg is null</exception>
        /// <exception cref="ArgumentException">operation arg is null</exception>
        public ElapsedLogger([NotNull] ILogger logger, [NotNull] string operation)
            : base(operation)
        {
            Init(logger);
        }

        public ElapsedLogger([NotNull] ILogger logger, [NotNull] ElapsedLoggerBase inner)
            : base(inner)
        {
            Init(logger);
        }

        private void Init(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Log(string attribute, int elapsed)
        {
            _logger.Info($"'{attribute}' operation took {elapsed}ms");
        }
    }
}