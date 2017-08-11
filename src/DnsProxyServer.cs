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

namespace VtnrNetRadioServer
{

    public class DnsProxyServer
    {
        private VtunerConfig _cfg;

        public DnsProxyServer(VtunerConfig cfg)
        {
            this._cfg = cfg;
        }

        public Task Run()
        {
            var server = new DNS.Server.DnsServer(this._cfg.DnsServer);

            var ownIp = NetworkInterfaceHelper.GetLocalIPv4Address();
            System.Console.WriteLine("host IP: " + ownIp);

            server.MasterFile.AddIPAddressResourceRecord(this._cfg.VtunerServerOne, ownIp.ToString());
            server.MasterFile.AddIPAddressResourceRecord(this._cfg.VtunerServerTwo, ownIp.ToString());

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

            server.Errored += e => Console.WriteLine("err: " + e.Message + Environment.NewLine);

            return server.Listen(this._cfg.DnsPort);
        }
    }

}