// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Extensions;
using IdentityServer4.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// CORS policy service that configures the allowed origins from a list of clients' redirect URLs.
    /// </summary>
    public class MongoDBCorsPolicyService : ICorsPolicyService
    {
        private readonly string _collectionClients = "Clients";
        private readonly ILogger<MongoDBCorsPolicyService> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBCorsPolicyService"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public MongoDBCorsPolicyService(ILogger<MongoDBCorsPolicyService> logger, 
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var query =
                from client in _database.GetCollection<Client>(_collectionClients).AsQueryable()
                from url in client.AllowedCorsOrigins
                select url.GetOrigin();

            var result = query.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (result)
            {
                _logger.LogInformation("Client list checked and origin: {0} is allowed", origin);
            }
            else
            {
                _logger.LogInformation("Client list checked and origin: {0} is not allowed", origin);
            }
            
            return Task.FromResult(result);
        }
    }
}
