// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory scope store
    /// </summary>
    public class MongoDBScopeStore : IScopeStore
    {
        private readonly string _collectionScope = "Scopes";

        private readonly ILogger<MongoDBScopeStore> _logger;
        private readonly IMongoDatabase _database;


        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBScopeStore"/> class.
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        public MongoDBScopeStore(ILogger<MongoDBScopeStore> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// Gets all scopes.
        /// </summary>
        /// <returns>
        /// List of scopes
        /// </returns>
        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            try
            {
                if (scopeNames == null)
                {
                    _logger.LogDebug("FindScopesAsync argument scopeNames is null.");
                    throw new ArgumentNullException("scopeNames");
                }

                if (scopeNames.Count() == 0)
                {
                    _logger.LogDebug("FindScopesAsync argument scopeNames is empty.");

                    return Task.FromResult<IEnumerable<Scope>>(new List<Scope>());
                }

                var query = _database.GetCollection<Scope>(_collectionScope).Find(f => scopeNames.Contains(f.Name) && f.Enabled)
                    .Project<Scope>(Builders<Scope>.Projection
                                                .Exclude("_id"));
                var ret = query.ToListAsync();

                ret.Wait();
                //IEnumerable<Scope> scopes = (from s in _database.GetCollection<Scope>(_collectionScope).AsQueryable()
                //             where scopeNames.Contains(s.Name)
                //             select s).AsEnumerable();

                //var ret = scopes.ToList();

                return Task.FromResult<IEnumerable<Scope>>(ret.Result);


            } catch (Exception)
            {
                _logger.LogCritical("FindScopesAsync has failed.");
                throw;
            }
        }


        /// <summary>
        /// Gets all defined scopes.
        /// </summary>
        /// <param name="publicOnly">if set to <c>true</c> only public scopes are returned.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            Task<List<Scope>> ret;

            if (publicOnly)
            {
                ret = _database.GetCollection<Scope>(_collectionScope).Find(s => s.ShowInDiscoveryDocument)
                    .Project<Scope>(Builders<Scope>.Projection
                                                .Exclude("_id"))
                    .ToListAsync();

                ret.Wait();

                return Task.FromResult((IEnumerable<Scope>)ret.Result);
            }

            ret = _database.GetCollection<Scope>(_collectionScope).Find(s => true)
                .Project<Scope>(Builders<Scope>.Projection
                                                .Exclude("_id"))
                .ToListAsync();

            ret.Wait();

            return Task.FromResult((IEnumerable<Scope>)ret.Result);
        }
    }
}