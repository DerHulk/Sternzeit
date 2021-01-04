using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;
using MongoDB.Driver;
using Sternzeit.Server.Services;

namespace Sternzeit.Server.Controllers
{
    [Authorize]
    [Route("Home")]
    public class HomeController : Controller
    {
        private IUserService UserService { get; }
        private MongoDbContext MongoDbContext { get; }

        public HomeController(IUserService userService, MongoDbContext mongoDbContext)
        {
            this.UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        }

        [Route("Index", Name= Constants.Routes.Home)]
        public async Task<IActionResult> Index()
        {
            var links = new List<LinkModel>();
            var userId = await this.UserService.GetCurrentUserId();
            var notes = await (await this.MongoDbContext.Notes.FindAsync(x => x.OwnerId == userId)).ToListAsync();

            links.AddRange(notes.Select(x=> new LinkModel() { Description = x.Titel, HttpMethod = Constants.HttpVerbs.Get, Rel = Constants.RelTypes.Note, Url = this.Url.Link(Constants.Routes.GetNote, new { id = x.Id }) }).ToArray());
            links.Add(new LinkModel() { Description = "Create", HttpMethod = Constants.HttpVerbs.Put, Rel = Constants.RelTypes.Note, Url = this.Url.Link(Constants.Routes.CreateNote, new { titel = "" }) });            

            return Json(links.ToArray());
        }
    }
}
