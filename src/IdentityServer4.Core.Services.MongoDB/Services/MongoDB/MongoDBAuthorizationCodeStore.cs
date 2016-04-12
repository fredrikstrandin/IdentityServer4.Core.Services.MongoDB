// Copyright (c) Fredrik Strandin. All rights reserved.
// This is based on InMemoryAuthorizationCodeStore from IdentityServer
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Extensions;
using IdentityServer4.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// MongoDB authorization code store
    /// </summary>
    public class MongoDBAuthorizationCodeStore : IAuthorizationCodeStore
    {
        private const string _collectionAuthorizationCode = "AuthorizationCode";

        private readonly ILogger<MongoDBAuthorizationCodeStore> _logger;
        private readonly IMongoDatabase _database;

        public MongoDBAuthorizationCodeStore(ILogger<MongoDBAuthorizationCodeStore> logger,
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
        public Task StoreAsync(string key, AuthorizationCode value)
        {
            Task<ReplaceOneResult> ret = _database.GetCollection<MongoDBAuthorizationCode>(_collectionAuthorizationCode).ReplaceOneAsync(p => p.key == key, 
                new MongoDBAuthorizationCode() { key = key, value = value }, 
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
        public Task<AuthorizationCode> GetAsync(string key)
        {
            Task<MongoDBAuthorizationCode> find = _database.GetCollection<MongoDBAuthorizationCode>(_collectionAuthorizationCode).Find(p => p.key == key).FirstOrDefaultAsync();

            find.Wait();

            if (find.Result != null)
            {
                _logger.LogDebug($"GetAsync Key found: {find.Result.key}");

                return Task.FromResult(find.Result.value);
            }

            _logger.LogDebug($"GetAsync Key is not found: {find.Result.key}");

            return Task.FromResult<AuthorizationCode>(null);
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            Task<DeleteResult> delete = _database.GetCollection<MongoDBAuthorizationCode>(_collectionAuthorizationCode).DeleteOneAsync(p => p.key == key);
            
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
                from item in _database.GetCollection<MongoDBAuthorizationCode>(_collectionAuthorizationCode).AsQueryable()
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
            Task<DeleteResult> ret =  _database.GetCollection<MongoDBAuthorizationCode>(_collectionAuthorizationCode)
                .DeleteManyAsync(w => w.value.Subject.GetSubjectId() == subject && w.value.ClientId == client);

            ret.Wait();

            _logger.LogDebug($"RevokeAsync: Subject {subject}, client {client}, deleted count {ret.Result.DeletedCount}");
            return Task.FromResult(0);
        }
    }
}