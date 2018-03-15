using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Repositories;
using VtnrNetRadioServer.DnsServer2;
using ARSoft.Tools.Net.Dns;
using VtnrNetRadioServer.Helper;
using Microsoft.AspNetCore.ResponseCompression;

namespace VtnrNetRadioServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private ILogger<Startup> _logger;
        private SationsRepository_FirebaseSync _fbSync;
        private DnsServer _dnsServer;

        public Startup(IConfiguration conf, ILogger<Startup> logger)
        {
            Configuration = conf;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("ConfigureServices()");

            services.AddMvc();

            services.Configure<VtunerConfig>(Configuration.GetSection("vtuner"));
            services.Configure<FirebaseConfig>(Configuration.GetSection("firebase"));

            services.AddSingleton<Flurl.Http.IFlurlClient, Flurl.Http.FlurlClient>();

            services.AddSingleton<IStationsRepository2, StationsRepository_InMemory>();
            services.AddSingleton<IStationsRepository>(x => x.GetService<IStationsRepository2>());

            services.AddSingleton<SationsRepository_FirebaseSync>();
            services.AddTransient<ForwardingDnsServer>();
            services.AddTransient<NetworkInterfaceHelper>();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddFile("/tmp/vtuner-log.txt", LogLevel.Information);
            _logger.LogDebug("Configure()");
            _logger.LogDebug("IsDev: " + env.IsDevelopment());

            _fbSync = serviceProvider.GetService<SationsRepository_FirebaseSync>();
            var dnsProxy = serviceProvider.GetService<ForwardingDnsServer>();
            _dnsServer = dnsProxy.Run();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseBuffering();
            app.UseResponseCompression();

            app.UseMvcWithDefaultRoute();
        }
    }
}
