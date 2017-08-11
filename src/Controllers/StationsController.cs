using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VtnrNetRadioServer.Models;

namespace VtnrNetRadioServer.Controllers
{
    /*
     * Used for UserInterface to CRUD Stations
     */
    public class StationsController : Controller
    {
        public StationsController(StationsRepository sationsRepo)
        {
            this._stationsRepo = sationsRepo;
        }

        private StationsRepository _stationsRepo;

        public ActionResult Index()
        {
            var stations = _stationsRepo.GetAll();
            return View(stations);
        }
    }
}