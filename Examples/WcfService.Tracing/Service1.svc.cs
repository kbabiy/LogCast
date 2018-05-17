using System;
using System.Threading;
using System.Threading.Tasks;
using LogCast;

namespace WcfWebService.Tracing
{
    //[LogCastContextBehavior(LogResponseBody = true, LogRequestBody = true)]
    public class Service1 : IService1
    {
        private readonly ILogger _logger = LogManager.GetLogger();

        public string GetData(int value)
        {
            _logger.Info("Started");
            Thread.Sleep(100);
            _logger.Info("Done");

            return "OK";
        }

        public async Task<string> GetData2(int value)
        {
            _logger.Info("Started async");
            Thread.Sleep(100);

            Task task1, task2;
            using (new LogCastContextBranch())
            {
                _logger.Info("Branch root");

                task1 = Task.Factory.StartNew(() =>
                {
                    using (new LogCastContextBranch())
                    {
                        _logger.Info("Branch child A1");
                        Thread.Sleep(200);
                        _logger.Info("Branch child A2");
                    }
                });
                task2 = Task.Factory.StartNew(() =>
                {
                    using (new LogCastContextBranch())
                    {
                        _logger.Info("Branch child B1");
                        Thread.Sleep(50);
                        _logger.Info("Branch child B2");
                    }
                });

                _logger.Info("Branch root end");
            }

            await Task.WhenAll(task1, task2);
            _logger.Info("Done async");

            return "ok async";
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException(nameof(composite));
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public void DoWork(string taskName)
        {
            _logger.Info("Started " + taskName);
            //throw new ArgumentException("My error");
            Thread.Sleep(50);
            _logger.Info("Finished work");
        }
    }
}
