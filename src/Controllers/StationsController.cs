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

        [HttpPost]
        public ActionResult Add(string name, string url)
        {
            _stationsRepo.Add(name, url);
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            _stationsRepo.Delete(id);
            return Redirect(nameof(Index));
        }
    }
}