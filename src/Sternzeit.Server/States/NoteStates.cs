using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.States
{
    [System.Diagnostics.DebuggerDisplay("{{Id}}")]
    public class NoteStates
    {
        [BsonId]
        public Guid Id { get; set; }          
        public string Titel { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string[] Tags { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime LastModifiedAt { get; set; }
        public Guid OwnerId { get; set; }        
        public Guid[] SharedWith { get; set; }
    }
}
