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
    public class StationsRepository_InMemory : IStationsRepository2
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

        public Task MoveUpAsync(string id)
        {
            var idx = _items.FindIndex(x => x.Key == id);
            
            if (idx > 0 && idx < _items.Count) {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx - 1, item);
            }
            
            ItemsChanged?.Invoke();            
            return Task.CompletedTask;
        }

        public Task MoveDownAsync(string id)
        {
            var idx = _items.FindIndex(x => x.Key == id);
            
            if (idx >= 0 && (idx + 1) < _items.Count) {
                var item = _items[idx];
                _items.RemoveAt(idx);
                _items.Insert(idx + 1, item);
            }
            
            ItemsChanged?.Invoke();            
            return Task.CompletedTask;
        }

        public void UpdateItems(IList<ItemContainer> res)
        {
            _items.Clear();
            _items.AddRange(res);
        }

        public Task UpdateAsync(string id, ListOfItemsItem item)
        {
            _items.Single(x => x.Key == id).Item = item;
            ItemsChanged?.Invoke();
            return Task.CompletedTask;
        }
    }
}
