using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services
{
    public interface IUserService
    {
        Task<Guid?> GetCurrentUserId();
    }
}
