namespace IdentityServer4.Core.Services.MongoDB
{
    public interface IMongoDBSetup
    {
        string ConectionString { get; set; }
    }
}