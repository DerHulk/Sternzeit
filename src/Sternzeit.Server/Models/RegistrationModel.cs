using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    public class RegistrationModel
    {
            public string Id { get; set; }
            public string RawId { get; set; }
            public string Type { get; set; }
            public FidoResponse Response { get; set; }     
    }

    public class FidoResponse
    {
        public string AttestationObject { get; set; }
        public string ClientDataJson { get; set; }
        public string AuthenticatorData { get; set; }
        public string Signature { get; set; }
        public string UserHandle { get; set; }
    }
}
