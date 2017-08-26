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
            this._logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("ConfigureServices()");

            // Add framework services.
            services.AddMvc();

            // https://weblog.west-wind.com/posts/2016/may/23/strongly-typed-configuration-settings-in-aspnet-core
            var cfgSection = Configuration.GetSection(nameof(VtunerConfig));
            services.Configure<VtunerConfig>(cfgSection);

            // FirebaseConfig
            services.Configure<FirebaseConfig>(Configuration.GetSection(nameof(FirebaseConfig)));
            services.AddSingleton<Flurl.Http.IFlurlClient, Flurl.Http.FlurlClient>();
            
            //services.AddTransient<IStationsRepository, StationsRepository_Firebase>();
            
            services.AddSingleton<IStationsRepository2, StationsRepository_InMemory>();
            services.AddSingleton<IStationsRepository>(x => x.GetService<IStationsRepository2>());

            services.AddSingleton<SationsRepository_FirebaseSync>();
            services.AddTransient<ForwardingDnsServer>();
            services.AddTransient<NetworkInterfaceHelper>();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
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
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error");
            // }

            app.UseResponseBuffering();
            app.UseResponseCompression();


            //app.UseStaticFiles();

            //app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //    app.Run(async (context) =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
        }
    }
}
