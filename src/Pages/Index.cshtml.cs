using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;
using VtnrNetRadioServer.Repositories;

namespace VtnrNetRadioServer.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private readonly IStationsRepository _stationsRepo;

        public IndexModel(IStationsRepository stationsRepo, ILogger<IndexModel> logger)
        {
            this._stationsRepo = stationsRepo;
        }

        public IEnumerable<ItemContainer> Stations { get; private set; }

        public async Task OnGetAsync()
        {
            Stations = await _stationsRepo.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAddAsync(string name, string url)
        {
            await _stationsRepo.AddAsync(name, url);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            await _stationsRepo.DeleteAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpAsync(string id)
        {
            await _stationsRepo.MoveUpAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDownAsync(string id)
        {
            await _stationsRepo.MoveDownAsync(id);
            return RedirectToPage();
        }
    }
}