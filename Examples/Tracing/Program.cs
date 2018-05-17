using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LogCast;
using LogCast.Tracing;
using JetBrains.Annotations;

namespace Examples.Tracing
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    static class Program
    {
        private const string SourceName = "TestSource";
        static void Main()
        {
            // Scenario 1: when the listener added to specific trace source and explicit configuration is done (the best!)
            // Both loggers and "TraceSource" methods are supported
            LogConfig.Configure(new TraceLogManager(SourceName));

            WrappedExample();
            //BareLoggerExample();
            //BareLoggerUsingObjectModel();
            //WrappedExampleWithMultithreadedLogging();
            //NestedContextsExample();

            //Let the logs sending complete
            LogManager.Flush(TimeSpan.FromMinutes(1));
        }

        private static void WrappedExample()
        {
            var logger = LogManager.GetLogger();
            using (new LogCastContext("tracing example call"))
            {
                logger.Debug("debug");
                logger.Info("info");
                logger.Error("error");
            }
        }

        private static void BareLoggerExample()
        {
            BareLoggerRun(l =>
            {
                l.TraceEvent(TraceEventType.Verbose, 0, "debug");
                l.TraceEvent(TraceEventType.Information, 0, "info");
                l.TraceEvent(TraceEventType.Error, 0, "error");
            });
        }

        private static void BareLoggerRun(Action<TraceSource> log)
        {
            var logger = new TraceSource(SourceName);
            using (new LogCastContext())
            {
                log(logger);
            }
        }

        private static void NestedContextsExample()
        {
            var logger = LogManager.GetLogger();
            using (new LogCastContext("c1"))
            {
                logger.Error("m1");
                using (new LogCastContext("c2"))
                {
                    logger.Info("m2");
                }
                logger.Info("m3 dd");
            }
        }


        /* 
         |
         |\
         * |
         * *
         |/
         *
         |\
         * *
         * |
         * |
         |/
         */

        private static void WrappedExampleWithMultithreadedLogging()
        {
            var logger = LogManager.GetLogger();
            using (new LogCastContext())
            {
                logger.Info("M1");
                Task t1, t2;

                using (new LogCastContextBranch())
                    t1 = LogAsync("M2", 2, TimeSpan.FromMilliseconds(50));

                using (new LogCastContextBranch())
                    t2 = LogAsync("M3", 1, TimeSpan.FromMilliseconds(80));

                Task.WaitAll(t1, t2);
                Thread.Sleep(100);
                logger.Info("M4");

                using (new LogCastContextBranch())
                    t1 = LogAsync("M5", 3, TimeSpan.FromMilliseconds(300));
                using (new LogCastContextBranch())
                    t2 = LogAsync("M6", 1, TimeSpan.FromMilliseconds(150));

                Task.WaitAll(t1, t2);

                Thread.Sleep(35);
                logger.Info("M7");

            }
        }

        private static async Task LogAsync(string message, int times, TimeSpan toWait)
        {
            await Task.Factory.StartNew(() =>
            {
                var logger = LogManager.GetLogger();
                for (int i = 0; i < times; i++)
                {
                    Thread.Sleep(toWait);
                    logger.Info(message);
                }
            });
        }

        private static void BareLoggerUsingObjectModel()
        {
            BareLoggerRun(l =>
            {
                l.TraceData(TraceEventType.Verbose, 0, "debug");
                l.TraceData(TraceEventType.Information, 0, "info");
                l.TraceData(TraceEventType.Error, 0, "error");
            });
        }
    }
}