// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory token handle store
    /// </summary>
    public class MongoDBTokenHandleStore : ITokenHandleStore
    {
        private readonly string _collectionToken = "Token";
        private readonly ILogger<MongoDBTokenHandleStore> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBTokenStore"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public MongoDBTokenHandleStore(ILogger<MongoDBTokenHandleStore> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <summary>
        /// Stores the data.
        /// </summary> 
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Task StoreAsync(string key, Token value)
        {
            Task<ReplaceOneResult> ret = _database.GetCollection<MongoDBToken>(_collectionToken).ReplaceOneAsync(p => p.key == key,
                new MongoDBToken() { key = key, value = value },
                new UpdateOptions() { IsUpsert = true });

            ret.Wait();

            _logger.LogDebug($"StoreAsync modifed: {ret.Result.ModifiedCount}");


            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task<Token> GetAsync(string key)
        {
            Task<MongoDBToken> find = _database.GetCollection<MongoDBToken>(_collectionToken).Find(p => p.key == key).FirstOrDefaultAsync();

            find.Wait();

            if (find.Result != null)
            {
                _logger.LogDebug($"GetAsync Key found: {find.Result.key}");

                return Task.FromResult(find.Result.value);
            }

            _logger.LogDebug($"GetAsync Key is not found: {find.Result.key}");

            return Task.FromResult<Token>(null);
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            Task<DeleteResult> delete = _database.GetCollection<MongoDBToken>(_collectionToken).DeleteOneAsync(p => p.key == key);

            delete.Wait();

            _logger.LogDebug($"RemoveAsync Key {key}, remove count: {delete.Result.DeletedCount}");

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Retrieves all data for a subject identifier.
        /// </summary>
        /// <param name="subject">The subject identifier.</param>
        /// <returns>
        /// A list of token metadata
        /// </returns>
        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var query =
                from item in _database.GetCollection<MongoDBToken>(_collectionToken).AsQueryable()
                where item.value.SubjectId == subject
                select item.value;
            
            var list = query.ToArray();

            _logger.LogDebug($"GetAllAsync: Subject: {subject} found {list?.Count() ?? 0}");

            return Task.FromResult(list.Cast<ITokenMetadata>());
        }

        /// <summary>
        /// Revokes all data for a client and subject id combination.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task RevokeAsync(string subject, string client)
        {
            Task<DeleteResult> ret = _database.GetCollection<MongoDBAuthorizationCode>(_collectionToken)
                .DeleteManyAsync(w => w.value.SubjectId == subject && w.value.ClientId == client);

            ret.Wait();

            _logger.LogDebug($"RevokeAsync: Subject {subject}, client {client}, deleted count {ret.Result.DeletedCount}");

            return Task.FromResult(0);
        }
    }
}