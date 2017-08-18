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

namespace VtnrNetRadioServer
{
    public static class HttpAppServer
    {
        public static IWebHost Setup(uint port)
        {
            return new WebHostBuilder()
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
                .ConfigureLogging(x => x
                    .AddConsole()
                    .AddDebug())
                .UseStartup<Startup>()
                .Build();
        }
    }
}
