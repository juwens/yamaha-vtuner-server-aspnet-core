using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VtnrNetRadioServer.Models;

namespace VtnrNetRadioServer.Contract
{
    public interface IStationsRepository
    {
        IEnumerable<ListOfItemsItem> GetAll();
    }
}
