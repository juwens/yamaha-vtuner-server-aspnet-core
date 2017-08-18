using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;

namespace VtnrNetRadioServer
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("starting");

            var server = HttpAppServer.Setup(80);
            server.Run();
        }
    }
}
