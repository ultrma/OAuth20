using OAuth20.LineClient.Models;

namespace OAuth20.LineClient.Services
{
    public interface ILineLoginClient
    {
        string GetAuthorizeURL(string state);
        Task<LineLoginGetTokenResponse> GetTokenResponse(string code, string state);
    }
}
