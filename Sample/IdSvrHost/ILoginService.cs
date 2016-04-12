using IdentityServer4.Core.Services.MongoDB;

namespace IdSvrHost.UI.Login
{
    public interface ILoginService
    {
        bool ValidateCredentials(string username, string password);
        IMongoDBUser FindByUsername(string username);
    }
}