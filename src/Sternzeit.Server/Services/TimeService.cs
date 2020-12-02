using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public class TimeService : ITimeService
    {        
        public DateTime Now()
        {
            return DateTime.Now;
        }
        
        public long ToUnixTimeMilliseconds(DateTime value)
        {
            return (new DateTimeOffset(value)).ToUnixTimeMilliseconds();                        
        }
    }
}
