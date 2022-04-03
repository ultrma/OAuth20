using OAuth20.LineClient.Models;
using OAuth20.Web.Pages;
using OAuth20.Web.Models;

namespace OAuth20.Web.Services
{
    public interface IUserService
    {
        string GetLineLoginAuthorizeURL();
        string GetLineNotifiyAuthorizeURL();
        Task<User> Login(string code, string state);
        Task<User> Subscribe(string code, string state);
        Task Unsubscribe(string accessToken);
        Task Notify(string message);
        User GetUser(string accessToken);
        IDictionary<string, string> GetSubscriptions();
    }
}
