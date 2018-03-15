using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Diagnostics;

namespace VtnrNetRadioServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("starting");
            var port = 80;

            new WebHostBuilder()
                .UseKestrel()
                //.UseLibuv(x => x.ThreadCount = 1)
                .UseUrls("http://*:" + port)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) => 
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
