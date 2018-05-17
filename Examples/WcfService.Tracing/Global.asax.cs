using System;
using System.Web;
using LogCast;
using LogCast.Wcf;
using LogCast.Wcf.Configuration;
using LogCast.Tracing;

namespace WcfWebService.Tracing
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            LogConfig.BeginConfiguration(new TraceLogManager("logger"))
                .WithGlobalInspector(new OperationContextInspector(LoggingOptions.All()))
                .End();
        }

        protected void Application_End(object sender, EventArgs e)
        { }
    }
}