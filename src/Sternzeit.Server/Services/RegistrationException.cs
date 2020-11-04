using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    [System.Serializable]
    public class RegistrationException : Exception
    {
        public RegistrationException() { }
        public RegistrationException(string message) : base(message) { }
        public RegistrationException(string message, Exception inner) : base(message, inner) { }
        protected RegistrationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
