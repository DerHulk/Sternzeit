using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    internal class LoginPreconditionModel
    {
        public string KeyId { get; }
        public string Challenge { get; }
        public string RelyingPartyId { get; }
        public string LoginCallbackUrl { get; }

        public LoginPreconditionModel(string keyId, string challenge, string relyingPartyId, string callbackUrl)
        {
            KeyId = keyId;
            Challenge = challenge;
            RelyingPartyId = relyingPartyId;
            LoginCallbackUrl = callbackUrl;
        }

        public override bool Equals(object obj)
        {
            return obj is LoginPreconditionModel other &&
                   KeyId == other.KeyId &&
                   Challenge == other.Challenge &&
                   RelyingPartyId == other.RelyingPartyId &&
                   LoginCallbackUrl == other.LoginCallbackUrl;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeyId, Challenge, RelyingPartyId, LoginCallbackUrl);
        }
    }
}
