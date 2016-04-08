using IdentityServer4.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.Core.Services.MongoDB
{
    internal class MongoDBToken
    {
        [BsonId]
        public string key { get; set; }

        public Token value { get; set; }
    }
}