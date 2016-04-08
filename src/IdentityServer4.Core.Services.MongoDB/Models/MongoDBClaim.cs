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
            this.type = type;
            this.value = value;
        }

        public MongoDBClaim(string type, string value, string valueType)
        {
            this.type = type;
            this.value = value;
            this.valueType = valueType;
        }

        public MongoDBClaim(string type, string value, string valueType, string issuer)
        {
            this.type = type;
            this.value = value;
            this.valueType = valueType;
            this.issuer = issuer;
        }
        public MongoDBClaim(string type, string value, string valueType, string issuer, string originalIssuer)
        {
            this.type = type;
            this.value = value;
            this.valueType = valueType;
            this.issuer = issuer;
            this.originalIssuer = originalIssuer;
        }


        public string type { get; set; }
        public string value { get; set; }
        public string valueType { get; set; }
        public string issuer { get; set; }
        public string originalIssuer { get; set; }

        public static implicit operator Claim(MongoDBClaim claim)
        {
            return new Claim(claim.type, claim.value, claim.valueType, claim.issuer, claim.originalIssuer);
        }
    }
}
