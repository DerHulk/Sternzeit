using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    public class NoteModel
    {
        public NoteIdModel Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titel { get; set; }        
        public string Text { get; set; }
        public string[] Tags { get; set; }
    }
}
