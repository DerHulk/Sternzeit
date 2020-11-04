using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class SerializationService : ISerializationService
    {
        public T Deserialize<T>(string toConvert)
        {
            return JsonConvert.DeserializeObject<T>(toConvert);
        }
    }
}
