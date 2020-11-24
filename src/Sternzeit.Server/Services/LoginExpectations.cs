using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sternzeit.Server.AttestionParser;

namespace Sternzeit.Server.Services
{
    public class LoginExpectations
    {
        public string Challenge { get; set; }
        public string[] Origins { get; set; }
        public byte[] RelayingPartyHash { get; set; }

        public long Counter { get; set; }

        public PublicKey PublicUserKey { get; set; }
    }
}
