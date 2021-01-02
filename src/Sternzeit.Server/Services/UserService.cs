using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Sternzeit.Server.Services
{
    public class UserService : IUserService
    {
        public IActionContextAccessor ActionContextAccessor { get; }
        public MongoDbContext MongoDbContext { get; }

        public UserService(IActionContextAccessor actionContextAccessor, MongoDbContext mongoDbContext)
        {
            this.ActionContextAccessor = actionContextAccessor ?? throw new ArgumentNullException(nameof(actionContextAccessor));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        }        

        public async Task<Guid?> GetCurrentUserId()
        {
            var user = this.ActionContextAccessor.ActionContext.HttpContext.User;
            return (await this.MongoDbContext.Users.FindAsync(x => x.UserName == user.Identity.Name)).SingleOrDefault()?.Id;
        }
    }
}
