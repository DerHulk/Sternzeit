using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Sternzeit.Server.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sternzeit.Server
{
    public class MongoDbContext
    {
        private IMongoDatabase Database { get; }

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDB"));

            if (client != null)
                this.Database = client.GetDatabase("Sternzeit");
        }

        protected MongoDbContext(IMongoDatabase database)
        {
            this.Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public IMongoCollection<UserStates> Users
        {
            get
            {
                return this.Database.GetCollection<UserStates>("Users");
            }
        }

        public IMongoCollection<NoteStates> Notes
        {
            get
            {
                return this.Database.GetCollection<NoteStates>("Notes");
            }
        }
    }
}
