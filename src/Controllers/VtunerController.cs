using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Contract;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Text;

namespace VtnrNetRadioServer.Controllers
{
    public class VtunerController : Controller
    {
        private readonly IStationsRepository _stationsRepo;
        private readonly ILogger<VtunerController> _logger;
        private readonly VtunerConfig _cfg;

        public VtunerController (
            IOptions<VtunerConfig> cfg,
            IStationsRepository repo,
            ILogger<VtunerController> logger)  {
            this._cfg = cfg.Value;
            this._stationsRepo = repo;
            this._logger = logger;
        }

        [HttpGet("/setupapp/yamaha/asp/browsexml/FavXML.asp")]
        public ContentResult FavXML_asp()
        {
            return ListOfItemsXml();
        }

        [HttpGet("/setupapp/Yamaha/asp/BrowseXML/loginXML.asp")]
        public ActionResult loginXML_asp(int? token, string mac, string fver, string dlang, int? startitems, int? enditems)
        {
            if (token == 0) {
                _logger.LogInformation("responde with encrypted token: " + this._cfg.EncryptedToken);
                // Receiver tries to login
                return Content($"<EncryptedToken>{this._cfg.EncryptedToken}</EncryptedToken>");
            }
            else
            {
                // receiver wants station list
                return ListOfItemsXml();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        private ContentResult ListOfItemsXml()
        {
            _logger.LogInformation("responde with ListOfItemsXml");
            var stations = _stationsRepo.GetAllAsync().Result.Take(8);
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("ListOfItems", 
                    new XElement("ItemCount", stations.Count()),
                    stations.Select(x => new XElement("Item", 
                        new XElement("ItemType", x.Item.ItemType),
                        new XElement("StationName", x.Item.StationName),
                        new XElement("StationUrl", x.Item.StationUrl)
                    )
                ))
            );
            var wr = new Utf8StringWriter();
            doc.Save(wr, SaveOptions.None);
            var xml = wr.ToString();
            //Response.ContentLength = xml.Length;
            Response.ContentType = "text/html";
            return Content(xml);
        }
    }
}
