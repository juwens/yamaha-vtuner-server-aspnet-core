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
    public class FirebaseConfig
    {
        public string dbUrl { get; set; }
        public string dbSecret { get; set; }
    }
}