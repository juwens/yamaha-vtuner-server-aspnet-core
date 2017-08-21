using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Repositories;

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

        public async Task<ViewResult> Index()
        {
            var sw = Stopwatch.StartNew();
            var stations = await _stationsRepo.GetAllAsync();
            sw.Stop();
            _logger.LogCritical("GetAll ms: " + sw.ElapsedMilliseconds);
            return View(stations);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, string url)
        {
            await _stationsRepo.AddAsync(name, url);
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _stationsRepo.DeleteAsync(id);
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Up(string id)
        {
            await _stationsRepo.MoveUpAsync(id);
            return Redirect(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Down(string id)
        {
            await _stationsRepo.MoveDownAsync(id);
            return Redirect(nameof(Index));
        }

        [HttpGet]
        public async Task<ViewResult> Edit(string id)
        {
            var item = (await _stationsRepo.GetAllAsync()).Single(x => x.Key == id);
            return View(item.Item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ListOfItemsItem item)
        {
            await _stationsRepo.UpdateAsync(id, item);
            return RedirectToAction(nameof(Index));
        }
    }
}