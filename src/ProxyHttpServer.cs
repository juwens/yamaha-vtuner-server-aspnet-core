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
    public static class ProxyHttpServer
    {
        static void Log(params string[] strings)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(obj));
            Console.WriteLine(string.Join(" ", strings));
        }

        public static void Start(uint port)
        {

            var host = new WebHostBuilder()
                .UseUrls("http://*:" + port)
                .UseKestrel(options =>
                {
                    //options.UseConnectionLogging();
                })
                //.UseContentRoot
                .Configure(app =>
                {
                    app.Run(RequestAsync);
                })
                .Build();

            host.Run();
        }

        private static async Task RequestAsync(HttpContext context)
        {
            var cfg = (VtunerConfig)context.RequestServices.GetService(typeof(VtunerConfig));

            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = new YamahaProxy(),
            //    UseProxy = true
            //};

            var req = context.Request;
            var res = context.Response;

            if (req.Path == "/favicon.ico")
            {
                context.Abort();
                return;
            }

            // var rootNav = "/setupapp/Yamaha/asp/BrowseXML/loginXML.asp?mac=...&fver=W&dlang=eng&startitems=1&enditems=10";
            // var bookmarksNav = "/setupapp/yamaha/asp/browsexml/FavXML.asp?empty=&mac=...&fver=W&dlang=eng&startitems=1&enditems=10";

            Log($"http request:");
            Log($"\tMethod: {req.Method}");
            Log($"\tPath: '{req.Path}'");
            Log($"\tQueryString: '{req.QueryString}'");

            var passThrough = false;
            if (
                passThrough
                //|| req.Path == "/setupapp/Yamaha/asp/BrowseXML/loginXML.asp?token=0"
                || (
                    !req.Path.Value.StartsWith("/setupapp/yamaha/asp/browsexml/FavXML.asp")
                    && !req.Path.Value.StartsWith("/setupapp/Yamaha/asp/BrowseXML/loginXML.asp")
                )
            )
            {
                Log("\t" + "forward request to real server");
                var url = new Uri(req.Scheme + "://" + req.Host.Value + req.Path + req.QueryString);

                using (var handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = false
                })
                {
                    var client = new HttpClient(handler);
                    var xRes = await client.GetAsync(url);
                    Log("\t" + $"httpClientRes:  {(int)xRes.StatusCode}; location: {xRes.Headers.Location}");
                    CopyTo(xRes.Headers, res.Headers);
                    res.StatusCode = (int)xRes.StatusCode;
                    await xRes.Content.CopyToAsync(res.Body);
                }

                return;
            }

            // Login
            if (req.Path == "/setupapp/Yamaha/asp/BrowseXML/loginXML.asp"
                && req.QueryString.Value == "?token=0")
            {
                var body = $"<EncryptedToken>{cfg.EncryptedToken}</EncryptedToken>";
                Log($"\tanswer with: EncryptedToken");
                await res.WriteAsync(body);
                return;
            }

            Log("\tanswer with: " + "ListOfItems.xml");

            //System.AppContext.BaseDirectory
            //System.AppDomain.CurrentDomain.BaseDirectory

            var basePath = System.AppContext.BaseDirectory;
            var filePath = Path.Combine(basePath, "data", "ListOfItems.xml");

            using (var fh = File.OpenRead(filePath))
            {
                await fh.CopyToAsync(res.Body);
            }
        }

        private static void CopyTo(HttpResponseHeaders from, IHeaderDictionary to)
        {
            foreach (var header in from)
            {
                to[header.Key] = string.Join("; ", header.Value);
            }
        }
    }
}
