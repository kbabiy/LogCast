using System;
using System.Collections.Generic;
using LogCast.Inspectors;
using LogCast.Rendering;
using JetBrains.Annotations;

namespace LogCast.Engine
{
    internal class LogCastDocumentFactory
    {
        private readonly IDetailsFormatter _detailsFormatter;

        public LogCastDocumentFactory(IDetailsFormatter detailsFormatter)
        {
            _detailsFormatter = detailsFormatter ?? throw new ArgumentNullException(nameof(detailsFormatter));
        }

        public LogCastDocument Create(LogMessage message, IEnumerable<ILogDispatchInspector> dispatchInspectors)
        {
            var document = new LogCastDocument();
            ApplyMessage(document, message);

            if (message.Properties?.Length > 0)
            {
                var accumulator = new PropertyAccumulator();
                accumulator.AddProperties(message.Properties);
                accumulator.Apply(document);
            }

            foreach (var inspector in dispatchInspectors)
            {
                inspector.BeforeSend(document, message);
            }

            return document;
        }

        public LogCastDocument Create(LogCastContext context, IEnumerable<ILogDispatchInspector> dispatchInspectors)
        {
            // Many messages will have no properties so we create aggregator only when necessary
            PropertyAccumulator accumulator = null;
            if (!context.Properties.IsEmpty)
            {
                accumulator = new PropertyAccumulator();
                accumulator.AddProperties(context.Properties);
            }

            var summaryBuilder = new ContextSummaryBuilder(context, _detailsFormatter, context.BranchHistory);
            foreach (var message in context.PendingMessages)
            {
                if (message.Properties?.Length > 0)
                {
                    if (accumulator == null)
                    {
                        accumulator = new PropertyAccumulator();
                    }
                    accumulator.AddProperties(message.Properties);
                }

                summaryBuilder.AddMessage(message);
            }

            var summary = summaryBuilder.Build();
            var document = new LogCastDocument();
            ApplyContext(document, context, summary);

            // Apply properties after context so that client could override standard properties
            accumulator?.Apply(document);

            foreach (var inspector in dispatchInspectors)
            {
                inspector.BeforeSend(document, context);
            }

            document.AddProperty(Property.Durations.Name, Property.Durations.Total,
                (int) context.Elapsed.TotalMilliseconds);

            return document;
        }

        private void ApplyMessage(LogCastDocument document, LogMessage message)
        {
            document.AddProperty(Property.Timestamp, message.CreatedAt);
            document.AddProperty(Property.LogLevel, message.Level.ToString());
            document.AddProperty(Property.LogLevelCode, (int)message.Level);
            document.AddProperty(Property.LoggerName, message.LoggerName);
            document.AddProperty(Property.Message, message.OriginalMessage);
            document.AddProperty(Property.Details, _detailsFormatter.FormatStandaloneMessage(message));

            ApplyError(document, message.Exception);
        }

        private static void ApplyContext(LogCastDocument document, LogCastContext context, ContextSummary summary)
        {
            document.AddProperty(Property.Timestamp, context.StartedAt);
            document.AddProperty(Property.CorrelationId, context.CorrelationId, true);
            document.AddProperty(Property.OperationName, context.OperationName, true);

            if (summary != null)
            {
                document.AddProperty(Property.LogLevel, summary.Level.ToString());
                document.AddProperty(Property.LogLevelCode, (int)summary.Level);
                document.AddProperty(Property.Message, summary.Message);
                document.AddProperty(Property.Details, summary.Details);
                document.AddProperty(Property.Durations.Name, Property.DefaultChildName, summary.Durations);
                document.AddProperty(Property.LoggerName, summary.Loggers);

                ApplyError(document, summary.Exceptions);
            }
        }

        private static void ApplyError(LogCastDocument document, Exception exception)
        {
            if (exception == null)
                ApplyError(document, (List<Exception>) null);
            else
                ApplyError(document, new List<Exception> {exception});
        }

        private static void ApplyError(LogCastDocument document, [CanBeNull] List<Exception> exceptions)
        {
            ExceptionSummary summary = null;

            int exceptionCount = 0;
            if (exceptions != null)
                exceptionCount = exceptions.Count;

            if (exceptionCount > 0)
            {
                var types = new HashSet<string>(StringComparer.Ordinal);
                var values = new string[exceptionCount];
                for (int i = 0; i < exceptionCount; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var exception = exceptions[i];
                    types.Add(exception.GetType().Name);
                    values[i] = exception.ToString();
                }

                summary = new ExceptionSummary
                {
                    Types = types,
                    Values = values
                };
            }

            document.AddProperty(Property.Exceptions, summary);
        }
    }
}