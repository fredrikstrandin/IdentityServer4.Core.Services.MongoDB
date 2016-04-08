// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Services.InMemory;
using IdentityServer4.Core.Validation;
using System.Collections.Generic;
using IdentityServer4.Core.Services.MongoDB;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoDBUsers<TUser>(this IIdentityServerBuilder builder) where TUser : IMongoDBUser
        {
            builder.Services.AddTransient<IProfileService, MongoDBProfileService<TUser>>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, MongoDBResourceOwnerPasswordValidator<TUser>>();

            return builder;
        }

        public static IIdentityServerBuilder AddMongoDBClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, MongoDBClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, MongoDBCorsPolicyService>();

            return builder;
        }

        public static IIdentityServerBuilder AddMongoDBScopes(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IScopeStore, MongoDBScopeStore>();

            return builder;
        }        
    }
}