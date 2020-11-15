using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Sternzeit.Server.Models;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Sternzeit.Server.States;

namespace Sternzeit.Server.Controllers
{
    [Route("Register")]
    public class RegisterController : Controller
    {
        private MongoDbContext MongoDbContext { get; }
        private AttestionParser AttestionParser { get; }
        private ClientDataParser ClientDataParser { get; }
        private WebAuthService WebAuthService { get; }
        private ITimeService TimeService { get; }
        private RelyingParty RelayingParty { get; }

        public RegisterController(MongoDbContext mongoDbContext,
                                    AttestionParser attestionParser,
                                    ClientDataParser clientDataParser,
                                    WebAuthService webAuthService,
                                    ITimeService timeService,
                                    RelyingParty relayingParty)
        {
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
            this.AttestionParser = attestionParser ?? throw new ArgumentNullException(nameof(attestionParser));
            this.ClientDataParser = clientDataParser ?? throw new ArgumentNullException(nameof(clientDataParser));
            this.WebAuthService = webAuthService ?? throw new ArgumentNullException(nameof(webAuthService));
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(TimeService));
            this.RelayingParty = relayingParty ?? throw new ArgumentNullException(nameof(relayingParty));
        }

        [HttpGet]
        [Route("Preconditions")]
        public async Task<ActionResult> GetPreconditions(string userName)
        {
            var filter = Builders<UserStates>.Filter.Regex(nameof(UserStates.UserName), new BsonRegularExpression($"^{userName}$"));

            if ((await this.MongoDbContext.Users.CountDocumentsAsync(filter)) > 0)
                return this.BadRequest();

            var userId = Guid.NewGuid();
            var model = new RegisterPreconditionsModel();
            model.UserName = userName;
            model.UserId = userId.ToString();
            model.UserDisplayName = userName;

            model.RegisterUrl = this.Url.Action(nameof(Register), "Register", null, this.Request.Scheme);
            model.RelayingPartyId = this.RelayingParty.Id;
            model.RelayingPartyName = this.RelayingParty.Name;
            model.Challenge = IdentityModel.CryptoRandom.CreateUniqueId(16, IdentityModel.CryptoRandom.OutputFormat.Base64);

            var state = new UserStates() 
            {
                Id = userId,
                Challenge =  model.Challenge,
                UserName = model.UserName,
                CreationTime = this.TimeService.Now(),                
            };

            await this.MongoDbContext.Users.InsertOneAsync(state);

            return this.Json(model);
        }

        [HttpPost]
        [Route("Finish")]
        public async Task<ActionResult> Register([FromBody] RegistrationModel model)
        {            
            var clientData = this.ClientDataParser.Parse(model.Response.ClientDataJson);
            var attestion = this.AttestionParser.Parse(model.Response.AttestationObject);         
            //HACK: Todo 'Why are the == missing in response?'
            var state = await (await this.MongoDbContext.Users.FindAsync(x => (x.Challenge == clientData.Challenge || x.Challenge == clientData.Challenge + "==") && 
                                                                                x.LoginCounter == 0 && 
                                                                                    x.RegistrationTime == null))
                                                                                    .SingleOrDefaultAsync();

            if (state == null)
                return this.NotFound();

            var registration = new RegistrationData() { Attestion = attestion, ClientData = clientData };
            var expectation = new RegistrationExpectations()
            {
                RelayingPartyHash = this.RelayingParty.Hash.Value,
                RelayingPartyId = this.RelayingParty.Id,
                Challenge = state.Challenge,
                Origin = this.Request.Host.Value
            };

            var validation = this.WebAuthService.ValidateRegistration(registration, expectation);

            if (validation == ValidationResult.Success)
            {
                state.PublicKey = attestion.Key;
                state.RegistrationTime = this.TimeService.Now();
                await this.MongoDbContext.Users.ReplaceOneAsync(x=> x.Id == state.Id, state);
                return this.Ok();
            }

            return this.BadRequest();
        }       
    }
}
