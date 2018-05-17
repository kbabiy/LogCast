using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace LogCast.Loggers.Elapsed
{
    /// <summary>
    /// Logs time elapsed between consturction and disposal to the property of durations object into LogCastContext
    ///  </summary>
    /// <remarks>
    /// Elapsed time is written to "durations.{operation}" property of context
    /// If few measurements occur within one Context, timings are aggregated in an array
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <remarks>Logging is skipped on <see cref="LogCastContext.Current"/> missing" </remarks>
    public class AllDurationsElapsedLogger : DurationsElapsedLogger<IEnumerable<int>>
    {
        public AllDurationsElapsedLogger([NotNull] string operation)
            : base(operation, null)
        {
        }

        public AllDurationsElapsedLogger([NotNull] ElapsedLoggerBase inner)
            : base(inner, null)
        {
        }
    }
}