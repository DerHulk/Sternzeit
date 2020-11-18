using Microsoft.Extensions.DependencyInjection;
using Sternzeit.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server
{
    public static class IoC
    {
        public static IServiceCollection AddWebAuth(this IServiceCollection services, string relyingParyId, string relyingPartyName, params string[] additionalOrigins)
        {
            services.AddSingleton<ISerializationService>(new SerializationService());
            services.AddSingleton<RelyingParty>(new RelyingParty( relyingParyId, relyingPartyName, additionalOrigins));            
            services.AddSingleton<AttestionParser>(x=> new AttestionParser(x.GetService<ISerializationService>()));
            services.AddSingleton<AssertionParser>(x => new AssertionParser());
            services.AddSingleton<ClientDataParser>(x=> new ClientDataParser(x.GetService<ISerializationService>()));
            services.AddSingleton<WebAuthService>(new WebAuthService());
            services.AddSingleton<ITimeService>(new TimeService());

            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            return services.AddScoped<MongoDbContext>();
        }
    }
}
