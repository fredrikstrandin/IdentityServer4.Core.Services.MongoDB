using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core.Services.InMemory;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using IdentityServer4.Core.Services.MongoDB.Models;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Core.Services.MongoDB.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class UserTest
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public UserTest()
        {
            try
            {
                _client = new MongoClient();
                _database = _client.GetDatabase("IdentityServer");
            } catch (Exception)
            {
                throw;
            }
        }

        [Fact]
        public void CreateTestUser()
        {
            //_database.CreateCollection("Users");
            
            string salt = string.Empty;
            salt = salt.CreateSalt(34);

            string password = "vemvet";

            password = password.GenerateSHA256Hash(salt);

            var users = new List<MongoDBUser>
            {
                new MongoDBUser{Subject = ObjectId.GenerateNewId(), Username = "alice", Password = password, Salt = salt,
                    Claims = new List<MongoDBClaim>()
                    {
                        new MongoDBClaim(JwtClaimTypes.Name, "Alice Smith"),
                        new MongoDBClaim(JwtClaimTypes.GivenName, "Alice"),
                        new MongoDBClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new MongoDBClaim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                        new MongoDBClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new MongoDBClaim(JwtClaimTypes.Role, "Admin"),
                        new MongoDBClaim(JwtClaimTypes.Role, "Geek"),
                        new MongoDBClaim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new MongoDBClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", Constants.ClaimValueTypes.Json)
                    }
                },
                new MongoDBUser{Subject = ObjectId.GenerateNewId(), Username = "Bob", Password = password, Salt = salt,
                    Claims = new List<MongoDBClaim>()
                    {
                        new MongoDBClaim(JwtClaimTypes.Name, "Bob Smith"),
                        new MongoDBClaim(JwtClaimTypes.GivenName, "Bob" ),
                        new MongoDBClaim(JwtClaimTypes.FamilyName, "Smith"),
                        new MongoDBClaim(JwtClaimTypes.Email, "BobSmith@email.com"),
                        new MongoDBClaim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new MongoDBClaim(JwtClaimTypes.Role, "Developer"),
                        new MongoDBClaim(JwtClaimTypes.Role, "Geek"),
                        new MongoDBClaim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new MongoDBClaim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", Constants.ClaimValueTypes.Json)
                    }
                }
            };

            _database.GetCollection<IMongoDBUser>("Users").InsertMany(users);

            List<Client> client = new List<Client>
            {
                new Client
                {
                    ClientName = "Test Client",
                    ClientId = "test",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    // server to server communication
                    Flow = Flows.ClientCredentials,

                    // only allowed to access api1
                    AllowedScopes = new List<string>
                    {
                        "api1"
                    }
                },

                new Client
                {
                    ClientName = "MVC6 Demo Client",
                    ClientId = "mvc6",

                    // human involved
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:19276/signin-oidc",
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:19276/",
                    },

                    // access to identity data and api1
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "email",
                        "profile",
                        "api1"
                    }
                }
            };

            _database.GetCollection<Client>("Client").InsertMany(client);

            List<Scope> scope =  new List<Scope>()
            {
                // standard OpenID Connect scopes
                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.EmailAlwaysInclude,

                // API - access token will 
                // contain roles of user
                new Scope
                {
                    Name = "api1",
                    DisplayName = "API 1",
                    Type = ScopeType.Resource,

                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                }
            };

            _database.GetCollection<Scope>("Scope").InsertMany(scope);
        }
    }
}
