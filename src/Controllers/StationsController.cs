using Microsoft.AspNetCore.Mvc;
using VtnrNetRadioServer.Contract;

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