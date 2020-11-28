using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Sternzeit.Server.Services.Jwt;

namespace Sternzeit.Server
{
    public static class IoC
    {
        public static IServiceCollection AddWebAuth(this IServiceCollection services, string relyingParyId, string relyingPartyName, params string[] additionalOrigins)
        {
            services.AddSingleton<ISerializationService>(new SerializationService());
            services.AddSingleton<RelyingParty>(new RelyingParty(relyingParyId, relyingPartyName, additionalOrigins));
            services.AddSingleton<AttestionParser>(x => new AttestionParser(x.GetService<ISerializationService>()));
            services.AddSingleton<AssertionParser>(x => new AssertionParser());
            services.AddSingleton<ClientDataParser>(x => new ClientDataParser(x.GetService<ISerializationService>()));
            services.AddSingleton<WebAuthService>(new WebAuthService());
            services.AddSingleton<ITimeService>(new TimeService());

            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            return services.AddScoped<MongoDbContext>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <remarks>
        /// https://medium.com/dev-genius/jwt-authentication-in-asp-net-core-e67dca9ae3e8
        /// </remarks>
        public static IServiceCollection AddJwtToken(this IServiceCollection services, IConfiguration configuration)
        {            
            var rsaKey = JwtService.GetPublicKey(configuration);

            services.AddAuthentication()
               .AddJwtBearer("Asymmetric", options =>
               {                  
                   options.IncludeErrorDetails = true; // <- great for debugging

                   // Configure the actual Bearer validation
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       IssuerSigningKey = rsaKey,
                       ValidAudience = JwtService.Audience,
                       ValidIssuer = JwtService.Issuer,
                       RequireSignedTokens = true,
                       RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
                       ValidateLifetime = true, // <- the "exp" will be validated
                       ValidateAudience = true,
                       ValidateIssuer = true,
                   };
               });

            services.AddSingleton<IJwtService, JwtService>();

            return services;
        }
    }
}
