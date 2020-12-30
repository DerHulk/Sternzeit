using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    [System.Diagnostics.DebuggerDisplay("{{Value}}")]
    public class NoteIdModel
    {        
        public Guid Value { get; set; }
    }
}
