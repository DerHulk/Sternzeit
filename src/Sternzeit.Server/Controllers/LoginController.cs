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

        public LoginController(ILogger<LoginController> logger,
                                    MongoDbContext mongoDbContext,
                                    AssertionParser assertionParser,
                                    ClientDataParser clientDataParser,
                                    WebAuthService webAuthService,
                                    ITimeService timeService,
                                    RelyingParty relayingParty)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.MongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
            this.AssertionParser = assertionParser ?? throw new ArgumentNullException(nameof(assertionParser));
            this.ClientDataParser = clientDataParser ?? throw new ArgumentNullException(nameof(clientDataParser));
            this.WebAuthService = webAuthService ?? throw new ArgumentNullException(nameof(webAuthService));
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(TimeService));
            this.RelayingParty = relayingParty ?? throw new ArgumentNullException(nameof(relayingParty));
        }

        [HttpGet]
        [Route("Preconditions")]
        public async Task<IActionResult> GetPreconditions(string userName, string redirectUrl=null)
        {
            // generate challenge
            var challenge = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Base64);
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

            var login = new LoginData() { Assertion = assertion, ClientData = clientData };
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
                await HttpContext.SignInAsync("cookie",
                       new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("name", state.UserName) }, "cookie")));

                return this.Ok();
            }

            return this.BadRequest();
        }
    }

   
}
