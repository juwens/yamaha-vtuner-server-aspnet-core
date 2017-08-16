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
    public class StationsRepository_Firebase : IStationsRepository
    {
        private class PostResponse
        {
            public string name { get; set; }
        }

        private class StationsContainer
        {
            [JsonProperty("stations")]
            public Dictionary<string, ListOfItemsItem> Stations { get; set; } = new Dictionary<string, ListOfItemsItem>();
            
            [JsonProperty("stations-order")]
            public List<string> StationsOrder { get; set; } = new List<string>();
        }

        private readonly ILogger _logger;
        private readonly FirebaseConfig _conf;
        private readonly IFlurlClient _client;

        public StationsRepository_Firebase(
            ILogger<StationsRepository_Firebase> logger,
            IOptions<FirebaseConfig> conf,
            IFlurlClient client)
        {
            this._logger = logger;
            this._conf = conf.Value;
            this._client = client;
        }

        public async Task Test() {
            await _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations-order.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .PutJsonAsync(new [] {"a", "b", "c", "d"});
        }

        public async Task AddAsync(string name, string url)
        {
             var res = await 
                _client.WithUrl(
                    _conf.databaseURL
                    .AppendPathSegments(_conf.baseRef, "stations.json")
                    .SetQueryParams(new {auth = _conf.dbSecret}))
                .PostJsonAsync(new ListOfItemsItem{
                    StationName = name,
                    StationUrl = url
                })
                .ReceiveJson<PostResponse>();
                
            var keys = (await GetKeysOrderedAsync()).ToList();
            keys.Add(res.name);
            await SetKeyOrderAsync(keys.ToArray());
        }

        private async Task SetKeyOrderAsync(string[] keys)
        {
            var res = await _client.WithUrl(
                    _conf.databaseURL
                    .AppendPathSegments(_conf.baseRef, "stations-order.json")
                    .SetQueryParams(new {
                        auth = _conf.dbSecret
                    }))
                .PutJsonAsync(keys);
        }

        public async Task<IEnumerable<ItemContainer>> GetAllAsync()
        {
            var sw = Stopwatch.StartNew();
            var container = await
                _client.WithUrl(
                        _conf.databaseURL
                        .AppendPathSegments(_conf.baseRef + ".json")
                        .SetQueryParams(new { auth = _conf.dbSecret }))
                    .GetJsonAsync<StationsContainer>();

            // http://tmenier.github.io/Flurl/fluent-http/
            // var items = await 
            //     _flurlClient.WithUrl(
            //         _conf.databaseURL
            //         .AppendPathSegments(_conf.baseRef, "stations.json")
            //         .SetQueryParams(new {auth = _conf.dbSecret}))
            //     .GetJsonAsync<Dictionary<string, ListOfItemsItem>>();
            sw.Stop();
            _logger.LogCritical("stations ms: " + sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            //var orderedKeys = await GetKeysOrderedAsync();
            var items = container?.Stations ?? new Dictionary<string, ListOfItemsItem>();
            var res = (container?.StationsOrder ?? new List<string>())
                .Where(x => items.ContainsKey(x))
                .Select(x => new ItemContainer
                {
                    Key = x,
                    Item = items[x]
                }).ToList();
            sw.Stop();
            _logger.LogCritical("stations-order ms: " + sw.ElapsedMilliseconds);

            return res;
        }

        public async Task DeleteAsync(string id)
        {
            var keys = (await GetKeysOrderedAsync()).ToList();

            keys.Remove(id);
            await SetKeyOrderAsync(keys.ToArray());

            await _client.WithUrl(
                    _conf.databaseURL
                    .AppendPathSegments(_conf.baseRef, "stations", id + ".json")
                    .SetQueryParams(new {auth = _conf.dbSecret}))
                .DeleteAsync();
        }

        private async Task<string[]> GetKeysOrderedAsync(){
            var res = 
                _client.WithUrl(
                    _conf.databaseURL
                    .AppendPathSegments(_conf.baseRef, "stations-order.json")
                    .SetQueryParams(new {auth = _conf.dbSecret}))
                .GetJsonAsync<string[]>();
            return (await res) ?? new string[0];
        }
    }
}
