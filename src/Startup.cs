using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Repositories;
using System.Threading.Tasks;
using VtnrNetRadioServer.Middleware;

namespace VtnrNetRadioServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private ILogger<Startup> _logger;
        private SationsRepository_FirebaseSync _fbSync;

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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            _logger.LogDebug("Configure()");
            _logger.LogDebug("IsDev: " + env.IsDevelopment());

            _fbSync = serviceProvider.GetService<SationsRepository_FirebaseSync>();
            var vtunerCfg = serviceProvider.GetService<IOptions<VtunerConfig>>().Value;
            

            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var dnsProxy = new DnsProxyServer(vtunerCfg);
                        await dnsProxy.Run();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "dns-server crashed");
                    }
                }
            });

            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error");
            // }

            //app.UseStaticFiles();

            app.UseMiddleware<RequestLoggingMiddleware>();

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
