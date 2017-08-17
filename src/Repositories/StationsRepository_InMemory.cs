using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Contract;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Flurl;
using Flurl.Http;
using System.ComponentModel;
using Newtonsoft.Json;

namespace VtnrNetRadioServer.Repositories
{
    public class StationsRepository_InMemory : IStationsRepository
    {
        private readonly ILogger _logger;
        private readonly FirebaseConfig _conf;
        private readonly List<ItemContainer> _items;

        public IReadOnlyList<ItemContainer> Items => _items;
        public event Action ItemsChanged;

        public StationsRepository_InMemory(
            ILogger<StationsRepository_InMemory> logger,
            IOptions<FirebaseConfig> conf)
        {
            this._logger = logger;
            this._conf = conf.Value;
            this._items = new List<ItemContainer>();
        }

        public Task AddAsync(string name, string url)
        {
            _items.Add(new ItemContainer{
                Key = Guid.NewGuid().ToString(),
                Item = new ListOfItemsItem {
                    StationName = name,
                    StationUrl = url
                }
            });
            ItemsChanged?.Invoke();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ItemContainer>> GetAllAsync()
        {
            return Task.FromResult(_items.AsEnumerable());
        }

        public Task DeleteAsync(string id)
        {
            var item = _items.FirstOrDefault(x => x.Key == id);
            if (item != null)
            {
                _items.Remove(item);
            }
            ItemsChanged?.Invoke();            
            return Task.CompletedTask;            
        }
    }
}
