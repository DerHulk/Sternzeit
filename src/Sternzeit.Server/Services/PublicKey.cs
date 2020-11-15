using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class PublicKey
    {
        [JsonProperty("1")]
        public string KeyType { get; set; }

        [JsonProperty("3")]
        public string Algorithm { get; set; }

        [JsonProperty("-1")]
        public string Curve { get; set; }

        [JsonProperty("-2")]
        public string X { get; set; }

        [JsonProperty("-3")]
        public string Y { get; set; }
    }
}
