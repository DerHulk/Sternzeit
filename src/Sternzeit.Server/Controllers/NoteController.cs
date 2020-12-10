using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;
using Sternzeit.Server.States;

namespace Sternzeit.Server.Controllers
{
    [Authorize]
    [Route("Node")]
    public class NodeController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return Json(new string[] { "Hallo", "Welt" });
        }
        
        [HttpPut]
        public ActionResult Create(string titel)
        {
            //validate
            //saven
            var state = new NoteStates() { 
             Id = Guid.NewGuid(),
             CreatedAt = null,
             OwnerId = null,
             Tags = model.Tags,
             Text = model.Text,
             Titel = titel,
            };
            return this.Ok();
        }
    }
}
