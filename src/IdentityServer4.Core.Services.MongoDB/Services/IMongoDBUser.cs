using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4.Core.Services.MongoDB.Models;
using MongoDB.Bson;

namespace IdentityServer4.Core.Services.MongoDB
{
    /// <summary>
    /// In-memory user
    /// </summary>
    public interface IMongoDBUser
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        ObjectId Subject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="InMemoryUser"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        string Salt { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        string Provider { get; set; }

        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the claims.
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        IEnumerable<MongoDBClaim> Claims { get; set; }        
    }
}