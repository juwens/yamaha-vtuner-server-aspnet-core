using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private readonly Task _listenTask;

        public SationsRepository_FirebaseSync(
            IStationsRepository2 stationsRepo,
            ILogger<SationsRepository_FirebaseSync> logger,
            IOptions<FirebaseConfig> conf)
        {
            _stationsRepo = stationsRepo;
            _flurlClient = new FlurlClient();
            _fbConf = conf.Value;
            _log = logger;

            _stationsRepo.ItemsChanged += stationsRepo_ItemsChanged;
            SyncFromFbToRepo().Wait();
            _listenTask = ListenForFirebaseEvents();
        }

        private Task ListenForFirebaseEvents()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("text/event-stream"));
                        var url = 
                        _fbConf.dbUrl
                                .SetQueryParams(new
                                {
                                    auth = _fbConf.dbSecret
                                });
                        client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                        var stream = await client.GetStreamAsync(url);
                        
                        var reader = new StreamReader(stream);

                        while(true) {
                            var @event = await reader.ReadLineAsync();
                            var data = await reader.ReadLineAsync();
                            var emptyLine = await reader.ReadLineAsync();
                            
                            _log.LogTrace("event: " + @event);
                            _log.LogTrace("data: " + data);
                            _log.LogTrace("separator: " + emptyLine);

                            if (!@event.StartsWith("event: ")
                                || !data.StartsWith("data: ")
                                || emptyLine != "")
                            {
                                throw new Exception("cannot read events");
                            }

                            if (@event == ("event: put")
                                || @event == ("event: delete")
                                || @event == ("event: post")) 
                            {
                                await SyncFromFbToRepo();
                            }
                        }
                        
                    }
                    catch (System.Exception ex)
                    {
                        _log.LogError(ex, "exception");
                    }
                    await Task.Delay(1000);
                }
            });
        }

        private async Task SyncFromFbToRepo()
        {
            try
            {
                var res = await _flurlClient.WithUrl(
                        _fbConf.dbUrl
                        .SetQueryParams(new
                        {
                            auth = _fbConf.dbSecret
                        }))
                    .GetJsonAsync<IList<ItemContainer>>();
                _stationsRepo.SetItems(res);
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "exception");
            }
        }

        private void stationsRepo_ItemsChanged()
        {
            UploadAsync().Wait();
        }

        private Task UploadAsync()
        {
            return Task.Run(async () =>
            {
                _log.LogDebug("uploading");

                try
                {
                    var res = await _flurlClient.WithUrl(
                        _fbConf.dbUrl
                        .SetQueryParams(new
                        {
                            auth = _fbConf.dbSecret
                        }))
                    .PutJsonAsync(_stationsRepo.Items.ToArray());
                    _log.LogInformation("uploading finished");
                }
                catch (System.Exception ex)
                {
                    _log.LogError(ex, "uploading failed");
                }
            });
        }
    }
}