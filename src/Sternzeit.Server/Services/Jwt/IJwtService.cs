using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services.Jwt
{
    public interface IJwtService
    {
        /// <summary>
        /// Generates an Jwt-Token for the given User.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        string CreateToken(string username);
    }
}
