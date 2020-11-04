using IdentityModel;
using Sternzeit.Server.Models;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sternzeit.Server
{
    public class ClientDataParser
    {
        public ISerializationService SerializationService { get; }

        public class ClientData
        {
            public string Type { get; set; }
            public string Challenge { get; set; }
            public string Origin { get; set; }

            /// <summary>
            /// SHA-256-Hash over the source Json-Object. 
            /// </summary>
            /// <remarks>
            /// Should be 32 Byte.
            /// </remarks>
            public byte[] Hash { get; set; }
        }

        public ClientDataParser(ISerializationService serializationService)
        {
            this.SerializationService = serializationService ??
                throw new ArgumentNullException(nameof(serializationService));
        }

        public ClientData Parse(string toParse)
        {
            // 1. Let JSONtext be the result of running UTF-8 decode on the value of response.clientDataJSON
            var jsonText = Encoding.UTF8.GetString(Base64Url.Decode(toParse));

            // 2. Let C, the client data claimed as collected during the credential creation, be the result of running an implementation-specific JSON parser on JSONtext
            var result = this.SerializationService.Deserialize<ClientData>(jsonText);

            using (var hasher = new SHA256Managed())
            {
                result.Hash = hasher.ComputeHash(Base64Url.Decode(toParse));
            }

            return result;
        }
    }
}
