using OAuth20.LineClient.Models;

namespace OAuth20.LineClient.Services
{ 
    public interface ILineNotifyClient
    {
        string GetAuthorizeURL(string state);
        Task<string> GetAccessToken(string code, string state);
        Task Notify(string accessToken, string message);
        Task<LineNotifyGetAccessTokenResponse> GetAccessTokenStatus(string accessToken);
        Task RevokeAccessToken(string accessToken);
    }
}
