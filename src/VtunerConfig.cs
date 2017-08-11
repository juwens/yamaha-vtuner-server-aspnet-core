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
    public class VtunerConfig
    {
        public string EncryptedToken { get; set; } = "";
        public int HttpPort { get;  set; } = 8080;
        public string ReceiverMacAddress { get;  set; } = "";
        public IPAddress DnsServer { get;  set; } = IPAddress.Parse("8.8.8.8");
        public string VtunerServerOne { get;  set; } = "";
        public string VtunerServerTwo { get;  set; } = "";
        public int DnsPort {get;set;} = 53;
    }
}