using System;
using System.Web.Http.ExceptionHandling;
using JetBrains.Annotations;

namespace LogCast.WebApi
{
    [UsedImplicitly]
    public class LogCastExceptionLogger : ExceptionLogger
    {
        private readonly UnhandledExceptionLogger _logger;

        public LogCastExceptionLogger(params Type[] excludeExceptions)
        {
            _logger = new UnhandledExceptionLogger(excludeExceptions);
        }

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error(context.Exception);
            base.Log(context);
        }
    }
}