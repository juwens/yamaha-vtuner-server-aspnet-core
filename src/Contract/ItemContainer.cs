using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Contract
{
    public class ItemContainer
    {
        public ListOfItemsItem Item { get; set; }
        public string Key { get; set; }
    }
}