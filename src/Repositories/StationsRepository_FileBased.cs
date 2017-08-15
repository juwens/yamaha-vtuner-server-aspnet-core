using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Repositories
{
    public class StationsRepository_FileBased : IStationsRepository
    {
        private string XmlPath => Path.Combine(System.AppContext.BaseDirectory, "data", "ListOfItems.xml");
        private string ItemsXml => System.IO.File.ReadAllText(XmlPath);
        private XmlSerializer xmlSerializer = new XmlSerializer(typeof(ListOfItems));

        public IEnumerable<ItemContainer> GetAll()
        {
            ListOfItems listOfItems;
            using (var reader = File.OpenRead(XmlPath))
            {
                listOfItems = (ListOfItems)xmlSerializer.Deserialize(reader);
            }

            return listOfItems.Item
                .Where(x => x.ItemType == "Station")
                .Select(x => new ItemContainer
                {
                    Key = x.StationId.ToString(),
                    Item = x
                })
                .ToList();
        }

        public void Add(string stationName, string stationUrl)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
