using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Pages
{
    public class EditStationVm : PageModel
    {
        private readonly IStationsRepository stationsRepo;
        private readonly ILogger<EditStationVm> logger;

        public EditStationVm(IStationsRepository stationsRepo, ILogger<EditStationVm> logger)
        {
            this.stationsRepo = stationsRepo;
            this.logger = logger;
        }

        public ListOfItemsItem Item { get; set; }

        public List<SelectListItem> ItemTypes { get; } = new List<SelectListItem>()
        {
            new SelectListItem{Value = "Station", Text = "Station"},
            new SelectListItem{Value = "Dir", Text = "Directory"}
        };

        public async void OnGetAsync(string id)
        {
            Item = (await stationsRepo.GetAllAsync()).Single(x => x.Key == id).Item;
        }

        public async void OnPostAsync(string id, ListOfItemsItem item)
        {
            await stationsRepo.UpdateAsync(id, item);
            RedirectToPage("/Index");
        }
    }
}
