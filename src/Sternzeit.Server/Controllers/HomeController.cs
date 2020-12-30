using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;

namespace Sternzeit.Server.Controllers
{
    [Authorize]
    [Route("Home")]
    public class HomeController : Controller
    {
        [Route("Index", Name= Constants.Routes.Home)]
        public IActionResult Index()
        {            
            var links = new [] { 
                new LinkModel(){ Description = "Create", HttpMethod="PUT", Rel="Note", Url = this.Url.Link(Constants.Routes.CreateNote, new { titel="" }) }
            };
            return Json(links);
        }
    }
}
