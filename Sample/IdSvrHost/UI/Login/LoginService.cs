using System.Linq;
using System.Collections.Generic;
using IdentityServer4.Core.Services.MongoDB;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using IdentityServer4.Core.Services.MongoDB.Models;

namespace IdSvrHost.UI.Login
{
    public class MongoDBLoginService : ILoginService
    {
        private const string _collectionUsers = "Users";

        private readonly ILogger<MongoDBLoginService> _logger;
        private readonly IMongoDatabase _database;

        public MongoDBLoginService(ILogger<MongoDBLoginService> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }
        
        public bool ValidateCredentials(string username, string password)
        {
            var user = FindByUsername(username);
            if (user != null)
            {
                return user.Password.Equals(password);
            }
            return false;
        }

        public IMongoDBUser FindByUsername(string username)
        {
            var query = _database.GetCollection<MongoDBUser>(_collectionUsers).Find(f => f.Username == username);

            var ret = query.FirstOrDefaultAsync();
            ret.Wait();

            return ret.Result;
        }
    }
}
