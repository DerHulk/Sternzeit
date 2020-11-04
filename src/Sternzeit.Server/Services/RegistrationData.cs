using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class RegistrationData
    {
        public ClientDataParser.ClientData ClientData { get; set; }
        public AttestionParser.AttestionData Attestion { get; set; }
    }
}
