using System;
using System.Web.Http.Filters;
using JetBrains.Annotations;

namespace LogCast.WebApi
{
    [UsedImplicitly]
    public class LogCastExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly UnhandledExceptionLogger _logger;

        public LogCastExceptionFilterAttribute(params Type[] excludeExceptions)
        {
            _logger = new UnhandledExceptionLogger(excludeExceptions);
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            _logger.Error(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}