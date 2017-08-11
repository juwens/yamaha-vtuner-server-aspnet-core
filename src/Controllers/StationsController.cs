using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Models;

namespace VtnrNetRadioServer.Controllers
{
    /*
     * Used for UserInterface to CRUD Stations
     */
    public class StationsController : Controller
    {
        private IStationsRepository _stationsRepo;
        public StationsController(IStationsRepository sationsRepo)
        {
            this._stationsRepo = sationsRepo;
        }

        

        public ActionResult Index()
        {
            var stations = _stationsRepo.GetAll();
            return View(stations);
        }
    }
}