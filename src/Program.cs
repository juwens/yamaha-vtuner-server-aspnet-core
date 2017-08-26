using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

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
