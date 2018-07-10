
using System;
using ARSoft.Tools.Net.Dns;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.DnsServer2;
using VtnrNetRadioServer.Helper;
using VtnrNetRadioServer.Repositories;
using VtnrNetRadioServer.Services;

namespace VtnrNetRadioServer
{
    static class DependencyInjection
    {
        private static FirebaseSyncService _fbSync;
        private static DnsServer _dnsServer;

        public static void AddVtunerServices(this IServiceCollection services)
        {
            services.Configure<VtunerConfig>(Program.Configuration.GetSection("vtuner"));
            services.Configure<FirebaseConfig>(Program.Configuration.GetSection("firebase"));

            services.AddSingleton<Flurl.Http.IFlurlClient, Flurl.Http.FlurlClient>();

            services.AddSingleton<IStationsRepository, StationsRepository_InMemory>();

            services.AddSingleton<FirebaseSyncService>();
            services.AddTransient<ForwardingDnsServer>();
            services.AddTransient<NetworkInterfaceHelper>();
        }

        public static void UseVtunerServices(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("DependencyInjection");

            _fbSync = serviceProvider.GetService<FirebaseSyncService>();

            try
            {
                _dnsServer = serviceProvider.GetService<ForwardingDnsServer>().Run();
            }
            catch (System.Exception ex)
            {
                logger.LogError("could not start dns-server.");
                logger.LogTrace(ex.ToString());
            }
        }
    }
}