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
                new MongoDBUser{Subject = ObjectId.GenerateNewId(818727), Username = "alice", Password = "alice",
                    Claims = new MongoDBClaim[]
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
                new MongoDBUser{Subject = ObjectId.GenerateNewId(88421113), Username = "bob", Password = "bob",
                    Claims = new MongoDBClaim[]
                    {
                        new MongoDBClaim(JwtClaimTypes.Name, "Bob Smith"),
                        new MongoDBClaim(JwtClaimTypes.GivenName, "Bob"),
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
                ///////////////////////////////////////////
                // Console Client Credentials Flow Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.ResourceOwner,

                    AllowRememberConsent = true,

                    AllowedCorsOrigins = new List<string>() { "www.dn.se", "www.test.se" },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OfflineAccess.Name,
                        "api1", "api2"
                    }
                },

                ///////////////////////////////////////////
                // Console Resource Owner Flow Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "roclient",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.ResourceOwner,

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Email.Name,
                        StandardScopes.OfflineAccess.Name,

                        "api1", "api2"
                    }
                },

                ///////////////////////////////////////////
                // Console Client Credentials Flow Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "client.custom",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.Custom,

                    AllowedCustomGrantTypes = new List<string>
                    {
                        "custom"
                    },

                    AllowedScopes = new List<string>
                    {
                        "api1", "api2"
                    }
                },

                ///////////////////////////////////////////
                // Introspection Client Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "roclient.reference",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.ResourceOwner,

                    AllowedScopes = new List<string>
                    {
                        "api1", "api2"
                    },

                    AccessTokenType = AccessTokenType.Reference
                },

                ///////////////////////////////////////////
                // MVC Implicit Flow Samples
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "mvc_implicit",
                    ClientName = "MVC Implicit",
                    ClientUri = "http://identityserver.io",

                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:44077/signin-oidc"
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.Email.Name,
                        StandardScopes.Roles.Name,

                        "api1", "api2"
                    }
                },

                ///////////////////////////////////////////
                // JS OAuth 2.0 Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "js_oauth",
                    ClientName = "JavaScript OAuth 2.0 Client",
                    ClientUri = "http://identityserver.io",

                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:28895/index.html"
                    },

                    AllowedScopes = new List<string>
                    {
                        "api1", "api2"
                    }
                },
                
                ///////////////////////////////////////////
                // JS OIDC Sample
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "js_oidc",
                    ClientName = "JavaScript OIDC Client",
                    ClientUri = "http://identityserver.io",

                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:7017/index.html",
                        "http://localhost:7017/silent_renew.html",
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:7017/index.html",
                    },

                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:7017"
                    },

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.Email.Name,
                        StandardScopes.Roles.Name,
                        "api1", "api2"
                    }
                }

        };

            _database.GetCollection<Client>("Clients").InsertMany(client);

            List<Scope> scope = new List<Scope>()
            {
                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.EmailAlwaysInclude,
                StandardScopes.OfflineAccess,
                StandardScopes.RolesAlwaysInclude,
                StandardScopes.OfflineAccess,

                new Scope
                {
                    Name = "api1",
                    DisplayName = "API 1",
                    Description = "API 1 features and data",
                    Type = ScopeType.Resource,

                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                },
                new Scope
                {
                    Name = "api2",
                    DisplayName = "API 2",
                    Description = "API 2 features and data, which are better than API 1",
                    Type = ScopeType.Resource
                }
            };

            _database.GetCollection<Scope>("Scopes").InsertMany(scope);
        }


        [Fact]
        public void LoadTestUser()
        {
            string line;

            // Read the file and display it line by line.
            using (System.IO.StreamReader file =
               new System.IO.StreamReader("Persondata.csv"))
            {
                List<MongoDBUser> users = new List<MongoDBUser>();

                string[] fields = file.ReadLine().Split(',');
                
                while ((line = file.ReadLine()) != null)
                {
                    string[] items = line.Split(';');

                    users.Add( new MongoDBUser()
                    {
                        Subject = ObjectId.GenerateNewId(),
                        Enabled = true,
                        Username = items[14],
                        Password = items[15],
                        Claims = new MongoDBClaim[]
                        {
                            new MongoDBClaim(JwtClaimTypes.Name, $"{items[4]} {items[6]}"),
                            new MongoDBClaim(JwtClaimTypes.GivenName, items[6]),
                            new MongoDBClaim(JwtClaimTypes.FamilyName, items[4]),
                            new MongoDBClaim(JwtClaimTypes.Email, items[13])
                        }
                    });

                    
                    if (users.Count() > 500)
                    {
                        _database.GetCollection<MongoDBUser>("Users").InsertManyAsync(users);

                        users = new List<MongoDBUser>();
                    }
                }
            }
        }
    }
}