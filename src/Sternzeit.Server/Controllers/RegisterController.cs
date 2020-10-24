using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Controllers
{
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        [HttpGet]
        [Route("Preconditions")]        
        public ActionResult GetPreconditions(string userName)
        {
            var model = new RegisterPreconditionsModel();
            model.UserName = userName;
            model.UserId = Guid.NewGuid().ToString();
            model.UserDisplayName = userName;

            model.RegisterUrl = this.Url.Action(nameof(Register));
            model.RelayingPartyId = "localhost";
            model.RelayingPartyName = "Sternzeit";

            return this.Json(model);
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Register()
        {
            return this.Ok();
        }
    }
}
