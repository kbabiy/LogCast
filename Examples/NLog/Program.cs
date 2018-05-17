using System;
using System.Threading;
using LogCast;
using LogCast.NLog;

namespace Examples.NLog
{
    public static class Program
    {
        private static readonly ILogger Log = LogManager.GetLogger();

        static void Main()
        {
            LogConfig.Configure(new NLogManager());

            Operation();

            //Let the logs sending complete
            LogManager.Flush(TimeSpan.FromMinutes(1));
        }

        private static void Operation()
        {
            using (new LogCastContext("nlog example call"))
            {
                Thread.Sleep(5);
                Log.Debug("debug");
                Thread.Sleep(10);
                Log.Info("info");
                Thread.Sleep(15);
                Log.Warn("warn");
                Thread.Sleep(20);
                Log.Error("error");
                Thread.Sleep(25);

                //adding application custom properties
                Log.AddContextProperty("my_prop", 3);
                LogCastContext.Current.Properties.Add("MY_PROP", 5);
            }
        }
    }
}