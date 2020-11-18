using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class RelyingParty
    {
        /// <summary>
        /// Get the Id. For example "localhost".
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get the Name. For example the name of the web-app.
        /// </summary>
        public string Name { get; }
        
        public Lazy<byte[]> Hash { get; }

        /// <summary>
        /// List of origins to check. Can be helpfull if you have a single page application which
        /// is the UI. The orgin would then be the spa url.
        /// </summary>
        public string[] Origins { get; }

        public RelyingParty(string relayingPartyId, string relayingPartyName, params string[] externalOrigins)
        {
            if (string.IsNullOrEmpty(relayingPartyId))
                throw new ArgumentException("message", nameof(relayingPartyId));

            if (string.IsNullOrEmpty(relayingPartyName))
                throw new ArgumentException("message", nameof(relayingPartyName));
            
            this.Id = relayingPartyId;
            this.Name = relayingPartyName;
            this.Origins = externalOrigins;

            this.Hash = new Lazy<byte[]>(() => {
                using (var hasher = new SHA256Managed())
                {
                    return hasher.ComputeHash(Encoding.UTF8.GetBytes(this.Id));
                }                
            });
        }
       
    }
}
