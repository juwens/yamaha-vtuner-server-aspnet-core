using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Models;

namespace VtnrNetRadioServer.Controllers
{
    public class VtunerController : Controller
    {
        public VtunerController (IOptions<VtunerConfig> cfg, StationsRepository repo)  {
            this._cfg = cfg.Value;
            this._stationsRepo = repo;
        }

        private StationsRepository _stationsRepo;
        private VtunerConfig _cfg;

        [HttpGet("/setupapp/yamaha/asp/browsexml/FavXML.asp")]
        public ContentResult FavXML_asp()
        {
            return Content(ItemsXml);
        }

        [HttpGet("/setupapp/Yamaha/asp/BrowseXML/loginXML.asp")]
        public ContentResult loginXML_asp(int? token, string mac, string fver, string dlang, int? startitems, int? enditems)
        {
            if (token == 0) {
                return Content($"<EncryptedToken>{this._cfg.EncryptedToken}</EncryptedToken>");
            }

            return Content(ItemsXml);
        }

        [Obsolete]
        private string XmlPath => Path.Combine(System.AppContext.BaseDirectory, "data", "ListOfItems.xml");
        [Obsolete("use StationsRepository instead")]
        private string ItemsXml => System.IO.File.ReadAllText(XmlPath);
    }
}
