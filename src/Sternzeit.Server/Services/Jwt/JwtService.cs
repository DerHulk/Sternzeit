using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sternzeit.Server.Services.Jwt
{
    /// <summary>
    /// Service for using Jwt-Tokens.
    /// </summary>
    /// <remarks>
    /// https://medium.com/dev-genius/jwt-authentication-in-asp-net-core-e67dca9ae3e8
    /// https://www.scottbrady91.com/C-Sharp/JWT-Signing-using-ECDSA-in-dotnet-Core
    /// </remarks>
    public class JwtService : IJwtService
    {
        public const string Audience = "";
        public const string Issuer = "";
        
        public static readonly TimeSpan DefaultTokenDuration = new TimeSpan(0, 1, 0, 0, 0);

        private IConfiguration Configuration { get; }
        private ITimeService TimeService { get; }

        public JwtService(IConfiguration configuration, ITimeService timeService)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.TimeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
        }
        
        public string CreateToken(string username)
        {
            return this.CreateToken(username, this.TimeService.Now().Add(DefaultTokenDuration));
        }

        public string CreateToken(string username, DateTime expriresDate)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));

            if (this.TimeService.Now() > expriresDate)
                throw new ArgumentOutOfRangeException(nameof(expriresDate));

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(
                source: Convert.FromBase64String(this.Configuration["Jwt:Asymmetric:PrivateKey"]), // Use the private key to sign tokens
                bytesRead: out int _); // Discard the out variable 

            var signingCredentials = new SigningCredentials(
                key: new RsaSecurityKey(rsa),
                algorithm: SecurityAlgorithms.RsaSha256
            );

            var jwtDate = this.TimeService.Now();

            var jwt = new JwtSecurityToken(
                audience: Audience,
                issuer: Issuer,
                claims: new Claim[] { new Claim(ClaimTypes.NameIdentifier, username) },
                notBefore: jwtDate,
                expires: expriresDate,
                signingCredentials: signingCredentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;

        }

        public static RsaSecurityKey GetPublicKey(IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            var publicKey = Convert.FromBase64String(configuration["Jwt:Asymmetric:PublicKey"]);
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(
                source: publicKey,
                bytesRead: out int _
            );

            return new RsaSecurityKey(rsa);
        }
    }
}
