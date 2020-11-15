using MongoDB.Bson.Serialization.Attributes;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sternzeit.Server.AttestionParser;

namespace Sternzeit.Server.States
{
    public class UserStates
    {
        [BsonId]
        public Guid Id { get; set; }
        
        public string UserName { get; set; }

        public PublicKey PublicKey { get; set; }

        public string CredentialId { get; set; }

        public string Challenge { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? RegistrationTime{ get; set; }

        public int LoginCounter { get; set; } = 0;
    }
}
