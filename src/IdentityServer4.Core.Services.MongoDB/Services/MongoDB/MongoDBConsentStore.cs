// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// MongoDB consent store
    /// </summary>
    public class MongoDBConsentStore : IConsentStore
    {
        private const string _collectionConsent = "Consent";

        private readonly ILogger<MongoDBConsentStore> _logger;
        private readonly IMongoDatabase _database;

        public MongoDBConsentStore(ILogger<MongoDBConsentStore> logger,
            IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }


        /// <summary>
        /// Loads all permissions the subject has granted to all clients.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The permissions.</returns>
        public Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var query = _database.GetCollection<Consent>(_collectionConsent).Find(f => f.Subject == subject).ToListAsync();

            query.Wait();

            return Task.FromResult<IEnumerable<Consent>>(query.Result);
        }

        /// <summary>
        /// Loads the subject's prior consent for the client.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns>The persisted consent.</returns>
        public Task<Consent> LoadAsync(string subject, string client)
        {
            var query = _database.GetCollection<Consent>(_collectionConsent).Find(c => c.Subject == subject && c.ClientId == client).FirstOrDefaultAsync();

            query.Wait();

            return Task.FromResult(query.Result);
        }


        /// <summary>
        /// Persists the subject's consent.
        /// </summary>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        public Task UpdateAsync(Consent consent)
        {
            // makes a snapshot as a DB would
            //consent.Scopes = consent.Scopes.ToArray();

            var query =  _database.GetCollection<Consent>(_collectionConsent).ReplaceOneAsync(c => c.Subject == consent.Subject && c.ClientId == consent.ClientId, 
                consent, 
                new UpdateOptions() { IsUpsert = true });

            //query.Wait();
            
            return Task.FromResult(0);
        }

        /// <summary>
        /// Revokes all permissions the subject has given to a client.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task RevokeAsync(string subject, string client)
        {
            var query = _database.GetCollection<Consent>(_collectionConsent).DeleteOneAsync(c => c.Subject == subject && c.ClientId == client);

            return Task.FromResult(0);
        }
    }
}