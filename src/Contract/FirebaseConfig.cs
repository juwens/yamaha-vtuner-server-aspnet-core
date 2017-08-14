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
        public string apiKey { get; set; }
        public string authDomain { get; set; }
        public string databaseURL { get; set; }
        public string projectId { get; set; }
        public string storageBucket { get; set; }
        public string messagingSenderId { get; set; }
        public string baseRef { get; set; }
        public string dbSecret { get; set; }
    }
}