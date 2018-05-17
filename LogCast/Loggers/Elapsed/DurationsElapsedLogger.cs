using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace LogCast.Loggers.Elapsed
{
    /// <summary>
    /// Logs and aggregates time elapsed between consturction and disposal to the property of durations object into LogCastContext
    /// </summary>
    /// <remarks>
    /// Elapsed time is written to "durations.{operation}" property of context
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <remarks>Logging is skipped on <see cref="LogCastContext.Current"/> missing" </remarks>
    public class DurationsElapsedLogger<TMeasurementResult> : ElapsedLoggerBase
    {
        private readonly Func<IEnumerable<int>, TMeasurementResult> _aggregator;

        public DurationsElapsedLogger([NotNull] string operation,
            [CanBeNull] Func<IEnumerable<int>, TMeasurementResult> aggregator)
            : base(operation)
        {
            _aggregator = aggregator;
        }

        public DurationsElapsedLogger([NotNull] ElapsedLoggerBase inner,
            [CanBeNull] Func<IEnumerable<int>, TMeasurementResult> aggregator)
            : base(inner)
        {
            _aggregator = aggregator;
        }

        protected override void Log(string attribute, int elapsed)
        {
            var props = LogCastContext.Current?.Properties;
            if(props == null)
                return;

            if (_aggregator != null)
                props.Add(Property.Durations.Name, attribute, elapsed, _aggregator);
            else
                props.Add(Property.Durations.Name, attribute, elapsed);
        }
    }
}