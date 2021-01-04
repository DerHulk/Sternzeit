using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server
{
    public class Constants
    {
        public const string OriginsPolicy = "546E9691-5488-4E84-983F-089E399BF607";

        public class Routes
        {
            public const string CreateNote = "Sternzeit.Create.Note";
            public const string GetNote = "Sternzeit.Get.Note";
            public const string Home = "Sternzeit.Home";
        }

        /// <summary>
        /// Relation-Types
        /// </summary>
        public class RelTypes
        {
            public const string Note = "Note";
        }

        public class HttpVerbs
        {
            public const string Get = "GET";
            public const string Put = "PUT";
            public const string Post = "POST";
            public const string Delete = "DELETE";
            public const string Patch = "PATCH";
        }
    }
}
