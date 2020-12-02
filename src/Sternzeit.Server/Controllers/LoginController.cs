using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Sternzeit.Server.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Sternzeit.Server.Services.Jwt;

namespace Sternzeit.Server.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {

        public ILogger<LoginController> Logger { get; }
        private MongoDbContext MongoDbContext { get; }
        private AssertionParser AssertionParser { get; }
        private ClientDataParser ClientDataParser { get; }
        private WebAuthService WebAuthService { get; }
        private ITimeService TimeService { get; }
        private RelyingParty RelayingParty { get; }
        public IJwtService JwtService { get; }

        public LoginController(ILogger<LoginController> logger,
                                    MongoDbContext mongoDbContext,
                                    AssertionParser assertionParser,
                                    ClientDataParser clientDataParser,
                                    WebAuthService webAuthService,
                                    ITimeService timeService,
                                    RelyingParty relayingParty,
                                    IJwtService jwtService)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
            this.AssertionParser = assertionParser ?? throw new ArgumentNullException(nameof(assertionParser));
            this.ClientDataParser = clientDataParser ?? throw new ArgumentNullException(nameof(clientDataParser));
            this.WebAuthService = webAuthService ?? throw new ArgumentNullException(nameof(webAuthService));
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(TimeService));
            this.RelayingParty = relayingParty ?? throw new ArgumentNullException(nameof(relayingParty));
            this.JwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        [HttpGet]
        [Route("Preconditions")]
        public async Task<IActionResult> GetPreconditions(string userName, string redirectUrl=null)
        {
            // generate challenge
            var challenge = Base64UrlEncoder.Encode(CryptoRandom.CreateUniqueId(16));
            var user = await (await this.MongoDbContext.Users.FindAsync(x => x.UserName == userName)).SingleOrDefaultAsync();

            user.Challenge = challenge;
            await this.MongoDbContext.Users.ReplaceOneAsync(x => x.Id == user.Id, user);

            var callbackUrl = this.Url.Action(nameof(Login), "Login", null, this.Request.Scheme);

            return this.Json(new LoginPreconditionModel(user.CredentialId, challenge, RelayingParty.Id, callbackUrl));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CredentialsModel model)
        {            
            var assertion = this.AssertionParser.Parse(model.Response.AuthenticatorData);
            var clientData = this.ClientDataParser.Parse(model.Response.ClientDataJson);

            var state = await (await this.MongoDbContext.Users.FindAsync(x => (x.Challenge == clientData.Challenge || x.Challenge == clientData.Challenge + "==") &&
                                                                               x.CredentialId == model.RawId))
                                                                                   .SingleOrDefaultAsync();

            if (state == null)
                return this.NotFound();

            var login = new LoginData() { Assertion = assertion, ClientData = clientData, Signatur = model.Response.Signature };
            var expectation = new LoginExpectations()
            {
                RelayingPartyHash = this.RelayingParty.Hash.Value,
                Challenge = state.Challenge,
                PublicUserKey = state.PublicKey,
                Counter = state.LoginCounter,
                Origins = new[] { this.Request.Host.Value }.Union(this.RelayingParty.Origins).ToArray()
            };
            var validation = this.WebAuthService.IsValidLogin(login, expectation);

            if (validation == ValidationResult.Success)
            {
                state.LastLoginTime = this.TimeService.Now();
                state.LoginCounter++;
                state.Challenge = null;

                await this.TryUpdateModelAsync(state);            

                var expriresAt = this.TimeService.Now().AddHours(1);
                var token = this.JwtService.CreateToken(state.UserName, expriresAt);
                return this.Ok( new { token, expriresAt= this.TimeService.ToUnixTimeMilliseconds(expriresAt) });
            }

            return this.BadRequest();
        }
    }

   
}
