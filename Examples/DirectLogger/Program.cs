using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogCast;
using LogCast.Loggers.Direct;
using JetBrains.Annotations;

namespace Examples.DirectLogger
{
    static class Program
    {
        private static readonly ILogger Log = LogManager.GetLogger();

        static void Main()
        {
            //Configuring from App.config
            LogConfig.Configure(new DirectLogManager());

            Operation();

            //Let the logs sending complete
            LogManager.Flush(TimeSpan.FromMinutes(1));
        }

        private static void Operation()
        {
            using (new LogCastContext("direct log example call"))
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

        [UsedImplicitly]
        private static void OperationWithParallelThreads()
        {
            using (new LogCastContext())
            {
                Log.Info("main started");

                var tasks = new List<Task>();
                for (int i = 0; i < 3; i++)
                {
                    tasks.Add(Task.Factory.StartNew(id =>
                    {
                        using (new LogCastContextBranch())
                        {
                            Log.Info($"task {id} started");
                            
                            //Some work done here
                            Thread.Sleep(10);

                            Log.Info($"task {id} progresses");

                            //Some more work done here
                            Thread.Sleep(15);

                            Log.Info($"task {id} finished");
                        }
                    }, i));
                }

                Task.WaitAll(tasks.ToArray());
                Log.Info("main finished");
            }
        }

        //public static string Operation(string input, string correlationId)
        //{
        //    using (new LogCastContext(correlationId))
        //    {
        //        Log.Info($"started: {input}");
        //        // logic here
        //        Log.Debug("progress");
        //        // and some more logic here	
        //        string result = "result value";
        //        Log.Info($"completed: {result}");
        //        return result;
        //    }
        //}
    }
}