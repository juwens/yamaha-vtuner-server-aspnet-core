using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Repositories
{
    public class SationsRepository_FirebaseSync
    {
        private readonly IStationsRepository2 _stationsRepo;
        private readonly IFlurlClient _flurlClient;
        private readonly FirebaseConfig _fbConf;
        private readonly ILogger<SationsRepository_FirebaseSync> _log;

        public SationsRepository_FirebaseSync(
            IStationsRepository2 stationsRepo,
            ILogger<SationsRepository_FirebaseSync> logger,
            IOptions<FirebaseConfig> conf,
            IFlurlClient client)
        {
            _stationsRepo = stationsRepo;
            _flurlClient = client;
            _fbConf = conf.Value;
            _log = logger;

            _stationsRepo.ItemsChanged += stationsRepo_ItemsChanged;
            DownloadOnceAsync().Wait();
        }

        private async Task DownloadOnceAsync()
        {
            try
            {
                var res = await _flurlClient.WithUrl(
                        _fbConf.databaseURL
                        .AppendPathSegments(_fbConf.baseRef, ".json")
                        .SetQueryParams(new
                        {
                            auth = _fbConf.dbSecret
                        }))
                    .GetJsonAsync<IList<ItemContainer>>();
                _stationsRepo.UpdateItems(res);
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "exception");
            }
        }

        private void stationsRepo_ItemsChanged()
        {
            Task.Run(async () =>
            {
                try
                {
                    await UploadAsync();
                }
                catch (System.Exception ex)
                {
                    _log.LogError(ex, "exception");
                }
            });
        }

        private Task UploadAsync()
        {
            return _flurlClient.WithUrl(
                        _fbConf.databaseURL
                        .AppendPathSegments(_fbConf.baseRef, ".json")
                        .SetQueryParams(new
                        {
                            auth = _fbConf.dbSecret
                        }))
                    .PutJsonAsync(_stationsRepo.Items.ToArray());
        }
    }
}