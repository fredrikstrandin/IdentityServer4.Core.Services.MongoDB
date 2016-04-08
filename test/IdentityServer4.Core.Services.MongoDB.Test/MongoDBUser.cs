using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Core.Services.MongoDB.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace IdentityServer4.Core.Services.MongoDB.Test
{
    public class MongoDBUser : IMongoDBUser
    {
        public bool Enabled { get; set; }

        public string Password { get; set; }

        public string Provider { get; set; }

        public string ProviderId { get; set; }

        public string Salt { get; set; }
        
        [BsonId]
        public ObjectId Subject { get; set; }

        public string Username { get; set; }

        public IEnumerable<MongoDBClaim> Claims { get; set; }

    }
}
