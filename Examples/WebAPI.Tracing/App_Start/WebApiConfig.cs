using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using LogCast;
using LogCast.WebApi;
using LogCast.Tracing;

namespace WebApiService.Tracing
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "Default", "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            LogConfig.Configure(new TraceLogManager("logger"));
            config.MessageHandlers.Add(new LogCastContextHandler());
            config.Services.Add(typeof(IExceptionLogger), new LogCastExceptionLogger());

            //LogConfig.BeginConfiguration(new TraceLogManager("logger"))
            //    .WithGlobalInspector(new HttpContextInspector(LoggingOptions.All()))
            //    .End();
        }
    }
}