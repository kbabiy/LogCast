using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using LogCast;
using LogCast.Wcf.Configuration;
using LogCast.Tracing;

namespace WcfSelfHostedService.Tracing
{
    static class Program
    {
        static void Main()
        {
            LogConfig.Configure(new TraceLogManager("logger"));

            Uri baseAddress = new Uri("http://localhost:8080/service1");
            using (ServiceHost host = new ServiceHost(typeof(Service1), baseAddress))
            {
                var smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
                };

                host.Description.Behaviors.Add(smb);
                host.Description.Behaviors.Add(new LogCastContextBehavior
                {
                    LogCallerAddress = true,
                    LogRequestHttpData = true
                });
                //LogConfig.Current.Engine.RegisterInspector(new OperationContextInspector());

                host.Open();

                Console.WriteLine("The service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}