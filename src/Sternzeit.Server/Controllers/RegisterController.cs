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
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;

namespace Sternzeit.Server.Controllers
{
    [Route("Register")]
    public class RegisterController : Controller
    {
        public ILogger<RegisterController> Logger { get; }
        private MongoDbContext MongoDbContext { get; }
        private AttestionParser AttestionParser { get; }
        private ClientDataParser ClientDataParser { get; }
        private WebAuthService WebAuthService { get; }
        private ITimeService TimeService { get; }
        private RelyingParty RelayingParty { get; }

        public RegisterController(ILogger<RegisterController> logger,
                                    MongoDbContext mongoDbContext,
                                    AttestionParser attestionParser,
                                    ClientDataParser clientDataParser,
                                    WebAuthService webAuthService,
                                    ITimeService timeService,
                                    RelyingParty relayingParty)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            model.Challenge = Base64UrlEncoder.Encode(CryptoRandom.CreateUniqueId(16));

            var state = new UserStates() 
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Challenge =  model.Challenge,
                UserName = model.UserName,
                CreationTime = this.TimeService.Now(),                                
            };

            await this.MongoDbContext.Users.InsertOneAsync(state);

            return this.Json(model);
        }

        [HttpPost]
        [Route("Finish")]
        public async Task<ActionResult> Register([FromBody] CredentialsModel model)
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
                Origin = new[] { this.Request.Host.Value }.Union(this.RelayingParty.Origins).ToArray()
            };

            var validation = this.WebAuthService.ValidateRegistration(registration, expectation);

            if (validation == ValidationResult.Success)
            {
                state.PublicKey = attestion.Key;
                state.RegistrationTime = this.TimeService.Now();
                state.CredentialId = attestion.GetCredintailIdAsString();
                state.LoginCounter = attestion.Counter;

                if (await this.MongoDbContext.Users.CountDocumentsAsync(x => x.CredentialId == state.CredentialId) > 0)
                {
                    this.Logger.LogError("Duplicate credential-Id detected.");
                    return this.BadRequest();
                }
                    
                await this.MongoDbContext.Users.ReplaceOneAsync(x=> x.Id == state.Id, state);
                return this.Ok();
            }

            this.Logger.LogError(validation.ErrorMessage);
            return this.BadRequest();
        }       
    }
}
