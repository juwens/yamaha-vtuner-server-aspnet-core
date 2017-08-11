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

namespace VtnrNetRadioServer
{
    public static class HttpAppServer
    {
        public static Task Start(uint port)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:" + port)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging(x => x
                    .AddConsole()
                    .AddDebug())
                .UseStartup<Startup>()
                .Build();

            System.Console.WriteLine("host.Run in started");
            return host.RunAsync();
        }
    }
}
