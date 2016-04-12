using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Core.Services.MongoDB.Models
{
    public class MongoDBClaim
    {
        public MongoDBClaim(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public MongoDBClaim(string type, string value, string valueType)
        {
            this.Type = type;
            this.Value = value;
            this.ValueType = valueType;
        }

        public MongoDBClaim(string type, string value, string valueType, string issuer)
        {
            this.Type = type;
            this.Value = value;
            this.ValueType = valueType;
            this.Issuer = issuer;
        }
        public MongoDBClaim(string type, string value, string valueType, string issuer, string originalIssuer)
        {
            this.Type = type;
            this.Value = value;
            this.ValueType = valueType;
            this.Issuer = issuer;
            this.OriginalIssuer = originalIssuer;
        }


        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string Issuer { get; set; }
        public string OriginalIssuer { get; set; }

        public static implicit operator Claim(MongoDBClaim claim)
        {
            return new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer);
        }
    }
}
