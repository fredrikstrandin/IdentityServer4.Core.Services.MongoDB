using IdentityServer4.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Core.Services.MongoDB.Models;

namespace IdentityServer4.Core.Services.MongoDB
{
    public class MongoDBResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator 
    {
        private readonly string _collectionUser = "Users";

        private readonly ILogger<MongoDBResourceOwnerPasswordValidator> _logger;
        private readonly IMongoDatabase _database;
        
        public MongoDBResourceOwnerPasswordValidator(ILogger<MongoDBResourceOwnerPasswordValidator> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            Task<MongoDBUser> find = _database.GetCollection<MongoDBUser>(_collectionUser).Find(p => p.Username == userName)
                .FirstOrDefaultAsync();

            find.Wait();

            if (find.Result != null)
            {
                _logger.LogDebug($"ValidateAsync find user: {userName}");

                MongoDBUser user = find.Result;

                //string hashPassword = user.Password.GenerateSHA256Hash(user.Salt);

                if (password == user.Password)
                    return Task.FromResult(new CustomGrantValidationResult(user.Subject.ToString(), "password"));
                else
                    _logger.LogDebug("Password did not match.");
            } else
            {
                _logger.LogDebug($"ValidateAsync can´t find user: {userName}");
            }

            return Task.FromResult(new CustomGrantValidationResult("Invalid username or password."));
        }
    }
}
