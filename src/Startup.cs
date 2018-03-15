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
using System.Diagnostics;

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
            services.AddMvc();
            services.AddResponseCompression();

            services.Configure<VtunerConfig>(Configuration.GetSection("vtuner"));
            services.Configure<FirebaseConfig>(Configuration.GetSection("firebase"));

            services.AddSingleton<Flurl.Http.IFlurlClient, Flurl.Http.FlurlClient>();

            services.AddSingleton<IStationsRepository2, StationsRepository_InMemory>();
            services.AddSingleton<IStationsRepository>(x => x.GetService<IStationsRepository2>());

            services.AddSingleton<SationsRepository_FirebaseSync>();
            services.AddTransient<ForwardingDnsServer>();
            services.AddTransient<NetworkInterfaceHelper>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
            app.UseResponseBuffering();
            app.UseResponseCompression();

            _fbSync = serviceProvider.GetService<SationsRepository_FirebaseSync>();

            try
            {
                _dnsServer = serviceProvider.GetService<ForwardingDnsServer>().Run();
            }
            catch (System.Exception ex)
            {
                _logger.LogError("could not start dns-server.");
                _logger.LogTrace(ex.ToString());
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
