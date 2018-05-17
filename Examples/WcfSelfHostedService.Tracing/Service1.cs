using System.ServiceModel;
using System.Threading;
using LogCast;

namespace WcfSelfHostedService.Tracing
{
    [ServiceContract]
    public interface IHelloWorldService
    {
        [OperationContract]
        string SayHello(string name);

        [OperationContract(IsOneWay = true)]
        void SayGoodbye(string name);
    }


    public class Service1 : IHelloWorldService
    {
        private readonly ILogger _logger = LogManager.GetLogger();

        public string SayHello(string name)
        {
            _logger.Info("Started");
            Thread.Sleep(100);
            _logger.Info("Done");

            return $"Hello, {name}";
        }

        public void SayGoodbye(string name)
        {
            _logger.Info("Started goodbye");
            Thread.Sleep(100);
            _logger.Info("Done goodbye");
        }
    }
}