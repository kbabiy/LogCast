using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogCast.Loggers.Elapsed;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace LogCast.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LoggingMiddleware
    {
        public ILogger Logger { get; }
        public Func<IOwinContext, ILogger, IOwinContextLogMap> OwinContextLogMapFactory { get; }

        public LoggingMiddleware()
            : this(LogManager.GetLogger(), (owinContext, logger) => new OwinContextLogMap(owinContext))
        {
            
        }

        public LoggingMiddleware(
            ILogger logger,
            Func<IOwinContext, ILogger, IOwinContextLogMap> owinContextLogMapFactory)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            OwinContextLogMapFactory = owinContextLogMapFactory ?? throw new ArgumentNullException(nameof(owinContextLogMapFactory));
        }

        public Func<AppFunc, AppFunc> GetApplicationFunction()
        {
            return next => environment => Invoke(next, environment);
        }

        public async Task Invoke(AppFunc next, IDictionary<string, object> environment)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            if (environment == null) throw new ArgumentNullException(nameof(environment));

            using (var logContext = new LogCastContext())
            {
                var context = new OwinContext(environment);

                var owinContextLogMap = OwinContextLogMapFactory(context, Logger);

                using (new ElapsedLogger(Logger, context.Request.Method))
                {
                    try
                    {
                        await next(environment);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception);
                        context.Response.StatusCode = 500;
                    }

                    owinContextLogMap.AfterNextHandler(logContext);
                }
            }
        }
    }
}
