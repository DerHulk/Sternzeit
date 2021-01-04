using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    [System.Diagnostics.DebuggerDisplay("{Value}")]    
    public class NoteIdModel
    {
        //[JsonPropertyName("id1")]
        [FromQuery(Name = "id")]        
        public Guid Value { get; set; }
                
        public string test { get; set; }
    }
}
