using Microsoft.AspNetCore.Mvc;
using Sternzeit.Server.Models;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server.Controllers
{
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        public AttestionParser AttestionParser { get; }
        public ClientDataParser ClientDataParser { get; }
        public WebAuthService WebAuthService { get; }
        public RelayingParty RelayingParty { get; }

        public RegisterController(AttestionParser attestionParser, 
                                    ClientDataParser clientDataParser,
                                    WebAuthService webAuthService,
                                    RelayingParty relayingParty)
        {
            this.AttestionParser = attestionParser ?? throw new ArgumentNullException(nameof(attestionParser));
            this.ClientDataParser = clientDataParser ?? throw new ArgumentNullException(nameof(clientDataParser));
            this.WebAuthService = webAuthService ?? throw new ArgumentNullException(nameof(webAuthService));
            this.RelayingParty = relayingParty ?? throw new ArgumentNullException(nameof(relayingParty));
        }        

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
            model.Challenge = IdentityModel.CryptoRandom.CreateUniqueId(16);

            return this.Json(model);
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult Register([FromBody] RegistrationModel model)
        {
            var clientData = this.ClientDataParser.Parse(model.Response.ClientDataJson);
            var attestion = this.AttestionParser.Parse(model.Response.AttestationObject);

            var registration = new RegistrationData() { Attestion = attestion, ClientData = clientData };
            var expectation = new RegistrationExpectations() { RelayingPartyHash }

            this.WebAuthService.ValidateRegistration(registration, )

            return this.Ok();
        }
    }
}
