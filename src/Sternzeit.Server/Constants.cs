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
    }
}
