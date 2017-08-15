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

namespace VtnrNetRadioServer.Repositories
{
    public class StationsRepository_Firebase : IStationsRepository
    {
        private class PostResponse
        {
            public string name { get; set; }
        }

        private readonly ILogger _logger;
        private readonly FirebaseConfig _conf;

        public StationsRepository_Firebase(
            ILogger<StationsRepository_Firebase> logger,
            IOptions<FirebaseConfig> conf)
        {
            this._logger = logger;
            this._conf = conf.Value;
        }

        public void Test() {
            _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations-order.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .PutJsonAsync(new [] {"a", "b", "c", "d"})
                .Wait();
        }

        public void Add(string name, string url)
        {
             var res = _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .PostJsonAsync(new ListOfItemsItem{
                    StationName = name,
                    StationUrl = url
                })
                .ReceiveJson<PostResponse>()
                .Result;
            var keys = GetKeysOrderedAsync().Result.ToList();
            keys.Add(res.name);
            SetKeyOrderAsync(keys.ToArray()).Wait();
        }

        private async Task<List<string>> GetAllKeysUnorderedAsync()
        {
            var items = await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {
                    auth = _conf.dbSecret,
                    shallow = true
                })
                .GetJsonAsync<Dictionary<string, bool>>();
            return items.Keys.ToList();
        }

        private async Task SetKeyOrderAsync(string[] keys)
        {
            var res = await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations-order.json")
                .SetQueryParams(new {
                    auth = _conf.dbSecret
                })
                .PutJsonAsync(keys);
        }

        public IEnumerable<ItemContainer> GetAll()
        {
            var sw = Stopwatch.StartNew();
            
            // http://tmenier.github.io/Flurl/fluent-http/
            var items = _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .GetJsonAsync<Dictionary<string, ListOfItemsItem>>()
                .Result;
            
            var orderedKeys = GetKeysOrderedAsync().Result;
            var res = orderedKeys
                .Select(x => new ItemContainer {
                    Key = x,
                    Item = items[x]
                }).ToList();

            _logger.LogDebug("ms: " + sw.ElapsedMilliseconds);

            return res;
        }

        public void Delete(string id)
        {
            var keys = GetKeysOrderedAsync().Result.ToList();
            keys.Remove(id);
            SetKeyOrderAsync(keys.ToArray()).Wait();

            _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations", id + ".json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .DeleteAsync()
                .Wait();
        }

        private async Task<string[]> GetKeysOrderedAsync(){
            var res = _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations-order.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .GetJsonAsync<string[]>();
            return (await res) ?? new string[0];
        }
    }
}
