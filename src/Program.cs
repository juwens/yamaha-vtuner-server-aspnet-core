using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

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
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting.Internal;

namespace VtnrNetRadioServer
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var logFactory = new LoggerFactory()
            .AddConsole(LogLevel.Debug)
            .AddDebug();

            var logger = logFactory.CreateLogger<Type>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            System.Console.WriteLine("Environment: " + environment);

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"hosting.{environment.ToLower()}.json", true)
                .Build();

            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
                .ConfigureServices((_, services) =>
                {
                    services.AddMvc();
                    services.AddResponseCompression();
                    services.AddVtunerServices();
                })
                .Configure((app) =>
                {
                    app.UseMvcWithDefaultRoute();
                    app.UseResponseBuffering();
                    app.UseResponseCompression();
                    app.UseVtunerServices(app.ApplicationServices);

                    var env = app.ApplicationServices.GetService<IHostingEnvironment>();
                    if (env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                })
                .Build()
                .Run();
        }
    }
}
