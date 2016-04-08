// Copyright (c) Fredrik Strandin. All rights reserved.
// This is based on InMemoryAuthorizationCodeStore from IdentityServer
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory client store
    /// </summary>
    public class MongoDBClientStore : IClientStore
    {
        private const string _collectionClient = "Client";

        private readonly ILogger<MongoDBClientStore> _logger;
        private readonly IMongoDatabase _database;

        public MongoDBClientStore(ILogger<MongoDBClientStore> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBClientStore"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public MongoDBClientStore(IEnumerable<Client> clients)
        {
            foreach (Client client in clients)
            {
                Task<ReplaceOneResult> ret = _database.GetCollection<Client>(_collectionClient).ReplaceOneAsync(c => c.ClientId == client.ClientId,
                    client,
                    new UpdateOptions() { IsUpsert = true });

                ret.Wait();

                _logger.LogInformation($"Client {client.ClientId} is added.");
            }
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var query = _database.GetCollection<Client>(_collectionClient).Find(f => f.ClientId == clientId && f.Enabled).SingleOrDefaultAsync();

            query.Wait();

            _logger.LogDebug($"FindClientByIdAsync: clientId {clientId}, is found {query?.Result != null}");

            return Task.FromResult(query.Result);
        }
    }
}