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
        Task<IEnumerable<ItemContainer>> GetAllAsync();
        Task AddAsync(string stationName, string stationUrl);
        Task DeleteAsync(string id);
        Task MoveUpAsync(string id);
        Task MoveDownAsync(string id);
        Task UpdateAsync(string id, ListOfItemsItem item);
    }

    public interface IStationsRepository2 : IStationsRepository
    {
        IReadOnlyList<ItemContainer> Items {get;}
        event Action ItemsChanged;

        void UpdateItems(IList<ItemContainer> res);
    }
}
