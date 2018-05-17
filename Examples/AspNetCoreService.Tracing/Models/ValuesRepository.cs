using System.Collections.Generic;
using System.Diagnostics;
using LogCast;

namespace AspNetCoreService.Tracing.Models
{
    public class ValuesRepository
    {
        private readonly ILogger _logger;

        public ValuesRepository()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public IEnumerable<string> Get()
        {
            _logger.Info("Returning values");
            Debug.Assert(LogCastContext.Current != null);
            var values = new[] {"value1", "value2"};
            _logger.Info("Values returned");
            Debug.Assert(LogCastContext.Current != null);
            return values;
        }
    }
}