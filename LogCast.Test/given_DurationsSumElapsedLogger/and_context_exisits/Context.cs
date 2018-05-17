using System.Collections.Generic;
using System.Linq;
using LogCast.Data;
using BddStyle.NUnit;

namespace LogCast.Test.given_DurationsSumElapsedLogger.and_context_exisits
{
    public class Context : ContextBase
    {
        protected LogCastContext CurrentContext;

        protected string OperationName;
        protected int OperationDurationMs;

        public override void Arrange()
        {
            CurrentContext = new LogCastContext();

            OperationName = "test";
            OperationDurationMs = 100;

            base.Arrange();
        }

        public override void Cleanup()
        {
            CurrentContext?.Dispose();
        }

        protected IEnumerable<int> GetOperationDurationProperties(LogCastContext context, string operationName)
        {
            string properyName = $"durations.{operationName}.total";
            return context.Properties
                .Where(property => property.Key == properyName)
                .OfType<LogPropertyWithValue<int>>()
                .Select(property => property.Value)
                .ToArray();
        }
    }
}
