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

        public async Task Test() {
            await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations-order.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .PutJsonAsync(new [] {"a", "b", "c", "d"});
        }

        public async Task AddAsync(string name, string url)
        {
             var res = await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .PostJsonAsync(new ListOfItemsItem{
                    StationName = name,
                    StationUrl = url
                })
                .ReceiveJson<PostResponse>();
            var keys = GetKeysOrderedAsync().Result.ToList();
            keys.Add(res.name);
            await SetKeyOrderAsync(keys.ToArray());
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

        public async Task<IEnumerable<ItemContainer>> GetAllAsync()
        {
            var sw = Stopwatch.StartNew();
            // http://tmenier.github.io/Flurl/fluent-http/
            var items = await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .GetJsonAsync<Dictionary<string, ListOfItemsItem>>();
            sw.Stop();
            _logger.LogCritical("stations ms: " + sw.ElapsedMilliseconds);
            
            sw.Reset();
            sw.Start();
            var orderedKeys = await GetKeysOrderedAsync();
            var res = orderedKeys
                .Select(x => new ItemContainer {
                    Key = x,
                    Item = items[x]
                }).ToList();
            sw.Stop();
            _logger.LogCritical("stations-order ms: " + sw.ElapsedMilliseconds);

            return res;
        }

        public async Task DeleteAsync(string id)
        {
            var keys = GetKeysOrderedAsync().Result.ToList();
            keys.Remove(id);
            SetKeyOrderAsync(keys.ToArray()).Wait();

            await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations", id + ".json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .DeleteAsync();
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
