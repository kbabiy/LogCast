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
    /// Elapsed time is written to "durations.{operation}_average" property of context
    /// If few measurements occur within one Context, timings are aggregated as an average
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <remarks>Logging is skipped on <see cref="LogCastContext.Current"/> missing" </remarks>
    public class DurationsAverageElapsedLogger : DurationsElapsedLogger<int>
    {
        /// <exception cref="ArgumentNullException"><paramref name="operation"/> is null</exception>
        public DurationsAverageElapsedLogger([NotNull] string operation)
            : base(operation, Average)
        {
        }

        public DurationsAverageElapsedLogger([NotNull] ElapsedLoggerBase inner)
            : base(inner, Average)
        {
        }

        protected override string GetAttribute(string operation)
        {
            return operation + ".average";
        }

        private static int Average(IEnumerable<int> input)
        {
            return (int) input.Average();
        }
    }
}