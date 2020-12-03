using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services.Jwt
{
    /// <summary>
    /// Key for the token service.
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/60434420/asp-net-core-3-1-jwt-signature-invalid-when-using-addjwtbearer
    /// </remarks>
    public class PrivateTokenKey
    {        
        public RsaSecurityKey Key { get; }

        public PrivateTokenKey(RsaSecurityKey key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
