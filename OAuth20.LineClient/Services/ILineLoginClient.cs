using OAuth20.LineClient.Models;

namespace OAuth20.LineClient.Services
{
    public interface ILineLoginClient
    {
        string GetAuthorizeURL();
        Task<LineLoginGetTokenResponse> GetTokenResponse(string code, string state);
    }
}
