using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VtnrNetRadioServer.Repositories;

namespace VtnrNetRadioServer.Contract
{
    public interface IStationsRepository
    {
        IEnumerable<ItemContainer> GetAll();
        void Add(string stationName, string stationUrl);
        void Delete(string id);
    }
}
