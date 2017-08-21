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

        public Task<IEnumerable<ItemContainer>> GetAllAsync()
        {
            ListOfItems listOfItems;
            using (var reader = File.OpenRead(XmlPath))
            {
                listOfItems = (ListOfItems)xmlSerializer.Deserialize(reader);
            }

            var res = listOfItems.Item
                .Where(x => x.ItemType == "Station")
                .Select(x => new ItemContainer
                {
                    Key = x.StationId.ToString(),
                    Item = x
                })
                .AsEnumerable();

            return Task.FromResult(res);
        }

        public Task AddAsync(string stationName, string stationUrl)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task MoveUpAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task MoveDownAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string id, ListOfItemsItem item)
        {
            throw new NotImplementedException();
        }
    }
}
