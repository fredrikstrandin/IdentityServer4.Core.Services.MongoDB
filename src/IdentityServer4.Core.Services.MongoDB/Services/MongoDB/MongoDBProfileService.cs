// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel;
using IdentityServer4.Core.Extensions;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using IdentityServer4.Core.Services.MongoDB.Models;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory user service
    /// </summary>
    public class MongoDBProfileService : IProfileService 
    {
        private readonly string _collectionUser = "Users";

        private readonly ILogger<MongoDBProfileService> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBProfileService"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public MongoDBProfileService(ILogger<MongoDBProfileService> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = new ObjectId(context.Subject.GetSubjectId());

            var query =
                from u in _database.GetCollection<MongoDBUser>(_collectionUser).AsQueryable()
                where u.Subject == subject
                select u;

            var user = query.SingleOrDefault();

            if (user != null)
            {
                var claims = new List<Claim>{
                new Claim(JwtClaimTypes.Subject, user.Subject.ToString()),
            };

                foreach (var item in user.Claims)
                {
                    claims.Add(item);
                }

                if (!context.AllClaimsRequested)
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }

                context.IssuedClaims = claims;
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. during token issuance or validation)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        public Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null) throw new ArgumentNullException("subject");

            var subject = new ObjectId(context.Subject.GetSubjectId());

            var query =
                from u in _database.GetCollection<MongoDBUser>(_collectionUser).AsQueryable()
                where
                    u.Subject == subject
                select new MongoDBUser() { Enabled = u.Enabled };

            var user = query.SingleOrDefault();

            context.IsActive = (user != null) && user.Enabled;

            return Task.FromResult(0);
        }
    }
}