using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class RegistrationExpectations
    {
        public string Challenge { get; set; }
        public string RelayingPartyId { get; set; }
        public byte[] RelayingPartyHash { get; set; }

        public string Origin { get; set; }      
    }
}
