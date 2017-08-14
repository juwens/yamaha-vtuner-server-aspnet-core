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

        public void Add(string stationName, string stationUrl)
        {
             Test();
        }

        public IEnumerable<ListOfItemsItem> GetAll()
        {
            var sw = Stopwatch.StartNew();
            
            // http://tmenier.github.io/Flurl/fluent-http/
            var items = _conf.databaseURL
                .AppendPathSegments(_conf.baseRef, "stations.json")
                .SetQueryParams(new {auth = _conf.dbSecret})
                .GetJsonAsync<Dictionary<string, ListOfItemsItem>>()
                .Result.Select(x => x.Value);

            _logger.LogDebug("ms: " + sw.ElapsedMilliseconds);

            return items;
        }
    }
}
