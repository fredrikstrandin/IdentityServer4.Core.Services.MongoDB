using IdentityServer4.Core.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Core.Services.MongoDB
{
    public class MongoDBResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator where TUser : IMongoDBUser
    {
        private readonly string _collectionUser = "Users";

        private readonly ILogger<MongoDBResourceOwnerPasswordValidator<TUser>> _logger;
        private readonly IMongoDatabase _database;
        
        public MongoDBResourceOwnerPasswordValidator(ILogger<MongoDBResourceOwnerPasswordValidator<TUser>> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            Task<TUser> find = _database.GetCollection<TUser>(_collectionUser).Find(p => p.Username == userName).FirstOrDefaultAsync();

            find.Wait();

            if (find.Result != null)
            {
                _logger.LogDebug($"ValidateAsync find user: {userName}");

                TUser user = find.Result;

                string hashPassword = user.Password.GenerateSHA256Hash(user.Salt);

                if (password == hashPassword)
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
