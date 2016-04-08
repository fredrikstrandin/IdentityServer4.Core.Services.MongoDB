// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory refresh token store
    /// </summary>
    public class MongoDBRefreshTokenStore : IRefreshTokenStore
    {
        private readonly string _collectionRefreshToken = "RefreshToken";
        private readonly ILogger<MongoDBRefreshTokenStore> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBTokenStore"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public MongoDBRefreshTokenStore(ILogger<MongoDBRefreshTokenStore> logger,
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
        public Task StoreAsync(string key, RefreshToken value)
        {
            Task<ReplaceOneResult> ret = _database.GetCollection<MongoDBRefreshToken>(_collectionRefreshToken).ReplaceOneAsync(p => p.key == key,
                new MongoDBRefreshToken() { key = key, value = value },
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
        public Task<RefreshToken> GetAsync(string key)
        {
            Task<MongoDBRefreshToken> find = _database.GetCollection<MongoDBRefreshToken>(_collectionRefreshToken).Find(p => p.key == key).FirstOrDefaultAsync();

            find.Wait();

            if (find.Result != null)
            {
                _logger.LogDebug($"GetAsync Key found: {find.Result.key}");

                return Task.FromResult(find.Result.value);
            }

            _logger.LogDebug($"GetAsync Key is not found: {find.Result.key}");

            return Task.FromResult<RefreshToken>(null);
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            Task<DeleteResult> delete = _database.GetCollection<MongoDBRefreshToken>(_collectionRefreshToken).DeleteOneAsync(p => p.key == key);

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
                from item in _database.GetCollection<MongoDBRefreshToken>(_collectionRefreshToken).AsQueryable()
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
            Task<DeleteResult> ret = _database.GetCollection<MongoDBAuthorizationCode>(_collectionRefreshToken)
                .DeleteManyAsync(w => w.value.SubjectId == subject && w.value.ClientId == client);

            ret.Wait();

            _logger.LogDebug($"RevokeAsync: Subject {subject}, client {client}, deleted count {ret.Result.DeletedCount}");

            return Task.FromResult(0);
        }
    }
}