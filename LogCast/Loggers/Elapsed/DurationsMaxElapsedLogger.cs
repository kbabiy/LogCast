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
    /// Elapsed time is written to "durations.{operation}_max" property of context
    /// If few measurements occur within one Context maximal value is taken
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <remarks>Logging is skipped on <see cref="LogCastContext.Current"/> missing" </remarks>
    public class DurationsMaxElapsedLogger : DurationsElapsedLogger<int>
    {
        /// <exception cref="ArgumentNullException"><paramref name="operation"/> is null</exception>
        public DurationsMaxElapsedLogger([NotNull] string operation)
            : base(operation, Max)
        {
        }

        public DurationsMaxElapsedLogger([NotNull] ElapsedLoggerBase inner)
            : base(inner, Max)
        {
        }

        protected override string GetAttribute(string operation)
        {
            return operation + ".max";
        }

        private static int Max(IEnumerable<int> input)
        {
            return input.Max();
        }
    }
}