using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Controllers
{
    /*
     * Used for UserInterface to CRUD Stations
     */
    public class StationsController : Controller
    {
        private readonly IStationsRepository _stationsRepo;
        private readonly ILogger<StationsController> _logger;

        public StationsController(IStationsRepository sationsRepo, ILogger<StationsController> logger)
        {
            this._stationsRepo = sationsRepo;
            this._logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            var sw = Stopwatch.StartNew();
            var stations = await _stationsRepo.GetAllAsync();
            sw.Stop();
            _logger.LogCritical("GetAll ms: " + sw.ElapsedMilliseconds);
            return View(stations);
        }

        [HttpPost]
        public async Task<ActionResult> Add(string name, string url)
        {
            await _stationsRepo.AddAsync(name, url);
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            await _stationsRepo.DeleteAsync(id);
            return Redirect(nameof(Index));
        }
        [HttpPost]
        public async Task<ActionResult> Up(string id)
        {
            await _stationsRepo.MoveUpAsync(id);
            return Redirect(nameof(Index));
        }
        [HttpPost]
        public async Task<ActionResult> Down(string id)
        {
            await _stationsRepo.MoveDownAsync(id);
            return Redirect(nameof(Index));
        }
    }
}