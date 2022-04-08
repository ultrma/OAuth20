using OAuth20.Web.Models;

namespace OAuth20.Web.Services
{
    public interface IUserService
    {
        string GetLineAPIState();
        string GetLineLoginAuthorizeURL(string state);
        string GetLineNotifiyAuthorizeURL(string state);
        Task<LoginUser> LoginCallback(string code, string state, string sessionKey);
        Task<LoginUser> Subscribe(string code, string state, string userId);
        Task Unsubscribe(string sessionKey);
        Task Notify(string message);
        LoginUser GetUser(string userId);
        IDictionary<string, string> GetSubscriptions();
        Task Logout();
    }
}
