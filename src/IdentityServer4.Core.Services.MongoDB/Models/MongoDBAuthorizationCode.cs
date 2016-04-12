// Copyright (c) Fredrik Strandin. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer4.Core.Services.MongoDB
{
    internal class MongoDBAuthorizationCode 
    {
        [BsonId]
        public string key { get; set; }
        public AuthorizationCode value { get; set; }
    }
}
