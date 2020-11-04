using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class RelayingParty
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

        public RelayingParty(string relayingPartyId, string relayingPartyName)
        {
            if (string.IsNullOrEmpty(relayingPartyId))
                throw new ArgumentException("message", nameof(relayingPartyId));

            if (string.IsNullOrEmpty(relayingPartyName))
                throw new ArgumentException("message", nameof(relayingPartyName));

            this.Id = relayingPartyId;
            this.Name = relayingPartyName;

            this.Hash = new Lazy<byte[]>(() => {
                using (var hasher = new SHA256Managed())
                {
                    return hasher.ComputeHash(Encoding.UTF8.GetBytes(this.Id));
                }                
            });
        }
       
    }
}
