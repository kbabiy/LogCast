using System.IO;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCoreService.Tracing
{
    [UsedImplicitly]
    public class Program
    {
        public static void Main()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
