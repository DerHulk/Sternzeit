using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    public class RegisterPreconditionsModel
    {
        public string Challenge { get; set; }
        public string RelayingPartyId { get; set; }
        public string RelayingPartyName { get; set; }

        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string UserId { get; set; }
        public string RegisterUrl { get; set; }
    }
}
