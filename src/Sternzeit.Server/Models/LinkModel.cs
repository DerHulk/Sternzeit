using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Models
{
    public class LinkModel
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string HttpMethod { get; set; }
        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public string Rel { get; set; }
    }
}
