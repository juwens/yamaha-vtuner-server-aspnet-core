using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Models;

namespace VtnrNetRadioServer.Controllers
{
    public class VtunerController : Controller
    {
        private IStationsRepository _stationsRepo;
        private VtunerConfig _cfg;

        public VtunerController (IOptions<VtunerConfig> cfg, IStationsRepository repo)  {
            this._cfg = cfg.Value;
            this._stationsRepo = repo;
        }

        [HttpGet("/setupapp/yamaha/asp/browsexml/FavXML.asp")]
        public ActionResult FavXML_asp()
        {
            return View("ListOfItemsXml", _stationsRepo.GetAll());
        }

        [HttpGet("/setupapp/Yamaha/asp/BrowseXML/loginXML.asp")]
        public ActionResult loginXML_asp(int? token, string mac, string fver, string dlang, int? startitems, int? enditems)
        {
            if (token == 0) {
                return Content($"<EncryptedToken>{this._cfg.EncryptedToken}</EncryptedToken>");
            }

            return View("ListOfItemsXml", _stationsRepo.GetAll());
        }
    }
}
