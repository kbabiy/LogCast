using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LogCast.WebApi
{
    internal class UnhandledExceptionLogger
    {
        private readonly HashSet<Type> _exludedExceptions;

        public UnhandledExceptionLogger(params Type[] excludeExceptions)
        {
            var exceptionTypes = excludeExceptions?
                .Where(t => typeof(Exception).IsAssignableFrom(t));

            _exludedExceptions = new HashSet<Type>(exceptionTypes ?? new Type[0]);
        }

        private readonly ILogger _logger = LogManager.GetLogger();

        public void Error([CanBeNull] Exception exception)
        {
            if (_exludedExceptions.Contains(exception?.GetType()))
                return;

            _logger.Error($"Unhandled exception: {exception?.Message}", exception);
        }
    }
}