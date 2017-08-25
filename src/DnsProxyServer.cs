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
using VtnrNetRadioServer.Helper;
using Microsoft.Extensions.Options;
using DNS.Protocol;

namespace VtnrNetRadioServer
{

    public class DnsProxyServer
    {
        private readonly VtunerConfig _cfg;
        private readonly ILogger<DnsProxyServer> _logger;
        private readonly NetworkInterfaceHelper _networkInterfaceHelper;

        public DnsProxyServer(
            IOptions<VtunerConfig> cfg, 
            ILogger<DnsProxyServer> logger,
            NetworkInterfaceHelper networkInterfaceHelper)
        {
            this._cfg = cfg.Value;
            this._logger = logger;
            this._networkInterfaceHelper = networkInterfaceHelper;
        }

        public Task Run()
        {
            var server = new DNS.Server.DnsServer(this._cfg.DnsServer);

            var ownIp = _networkInterfaceHelper.GetMyIPv4Address();
            System.Console.WriteLine("host IP: " + ownIp);

// server.MasterFile.AddIPAddressResourceRecord(this._cfg.VtunerServerOne, ownIp.ToString());
//            server.MasterFile.AddIPAddressResourceRecord(this._cfg.VtunerServerTwo, ownIp.ToString());
            //server.MasterFile.AddIPAddressResourceRecord("radioyamaha.vtuner.com", "192.168.178.28");
            //server.MasterFile.AddIPAddressResourceRecord("radioyamaha2.vtuner.com", "192.168.178.28");
        // server.MasterFile.AddIPAddressResourceRecord(this._cfg.VtunerServerTwo, ownIp.ToString());
            //server.MasterFile.AddIPAddressResourceRecord("foo.bar", "1.2.3.4");
            //server.MasterFile.AddIPAddressResourceRecord("bar.foo", "4.3.2.1");

            server.Requested += req =>
            {
                System.Console.Write("dns request:");
                if (req.Questions.Count > 1)
                {
                    Console.WriteLine();
                }

                foreach (var q in req.Questions)
                {
                    System.Console.WriteLine($"\t{q.Class} {q.Type} {q.Name}");
                }
            };

            server.Responded += (req, res) =>
            {
                Console.WriteLine(res);
            };

            server.Errored += e =>
            {
                Console.WriteLine("err: " + e.Message + Environment.NewLine);
            };

            return server.Listen();
        }
    }

}