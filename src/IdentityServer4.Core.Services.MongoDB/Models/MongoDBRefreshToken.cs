using IdentityServer4.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.Core.Services.MongoDB
{
    internal class MongoDBRefreshToken
    {
        [BsonId]
        public string key { get; set; }

        public RefreshToken value { get; set; }
    }
}