using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LogCast.Loggers.Elapsed
{
    /// <summary>
    /// Logs time elapsed between consturction and disposal to the property of durations object into LogCastContext
    /// </summary>
    /// <remarks>
    /// Elapsed time is written to "durations.{operation}_total" property of context
    /// If few measurements occur within one Context, timings are aggregated as a sum
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <remarks>Logging is skipped on <see cref="LogCastContext.Current"/> missing" </remarks>
    public class DurationsSumElapsedLogger : DurationsElapsedLogger<int>
    {
        /// <exception cref="ArgumentNullException"><paramref name="operation"/> is null</exception>
        public DurationsSumElapsedLogger([NotNull] string operation)
            : base(operation, Sum)
        {
        }

        public DurationsSumElapsedLogger([NotNull] ElapsedLoggerBase inner)
            : base(inner, Sum)
        {
        }

        protected override string GetAttribute(string operation)
        {
            return operation + ".total";
        }

        private static int Sum(IEnumerable<int> input)
        {
            return input.Sum();
        }
    }
}