using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace IdentityServer4.Core.Services.MongoDB
{
    public class ClaimProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Claim))
            {
                return new ClaimSerializer();
            }

            return null;
        }
    }

    public class ClaimSerializer : SerializerBase<Claim>, IBsonDocumentSerializer
    {
        public override Claim Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();

            string type = context.Reader.ReadString();
            string value = context.Reader.ReadString();
            string valueType = context.Reader.ReadString();
            string issuer = context.Reader.ReadString();
            string originalIssuer = context.Reader.ReadString();

            context.Reader.ReadEndDocument();

            return new Claim(type, value, valueType, issuer, originalIssuer);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Claim value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteName("type");
            context.Writer.WriteString(value.Type);

            context.Writer.WriteName("value");
            context.Writer.WriteString(value.Value);

            context.Writer.WriteName("valueType");
            context.Writer.WriteString(value.ValueType);

            context.Writer.WriteName("issuer");
            context.Writer.WriteString(value.Issuer);

            context.Writer.WriteName("originalIssuer");
            context.Writer.WriteString(value.OriginalIssuer);

            context.Writer.WriteEndDocument();
        }

        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            switch (memberName)
            {
                case "type":
                    serializationInfo = new BsonSerializationInfo("type", new StringSerializer(), typeof(string));
                    return true;
                case "valueType":
                    serializationInfo = new BsonSerializationInfo("valueType", new StringSerializer(), typeof(string));
                    return true;
                case "issuer":
                    serializationInfo = new BsonSerializationInfo("issuer", new StringSerializer(), typeof(string));
                    return true;
                case "originalIssuer":
                    serializationInfo = new BsonSerializationInfo("originalIssuer", new StringSerializer(), typeof(string));
                    return true;

                default:
                    serializationInfo = null;
                    return false;
            }
        }
    }
}
