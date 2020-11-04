using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class LoginData
    {
        public ClientDataParser.ClientData ClientData { get; set; }
        public AssertionParser.AssertionData Assertion { get; set; }        

        public string Signatur { get; set; }
    }
}
