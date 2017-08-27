using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.ViewModels.Stations
{
    public class EditStationVm
    {
        public ListOfItemsItem Item { get; set; }
        public List<SelectListItem> ItemTypes { get; } = new List<SelectListItem>()
        {
            new SelectListItem{Value = "Station", Text = "Station"},
            new SelectListItem{Value = "Dir", Text = "Directory"}
        };
    }
}
