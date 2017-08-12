using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using VtnrNetRadioServer.Contract;

namespace VtnrNetRadioServer.Repositories
{
    public class StationsRepository_Firebase : IStationsRepository
    {
        private readonly ILogger _logger;

        public StationsRepository_Firebase(ILogger<StationsRepository_Firebase> logger)
        {
            this._logger = logger;
        }

        public IEnumerable<ListOfItemsItem> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
