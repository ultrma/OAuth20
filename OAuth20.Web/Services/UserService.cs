using Microsoft.Extensions.Options;
using OAuth20.LineClient.Models;
using OAuth20.LineClient.Services;
using OAuth20.Web.Models;

namespace OAuth20.Web.Services
{
    public class UserService : IUserService
    {
        private readonly ILineLoginClient _lineLoginClient;
        private readonly ILineNotifyClient _lineNotifyClient;
        private readonly AdminSettings _adminSettings;
        public UserService (ILineLoginClient lineLoginClient, ILineNotifyClient lineNotifyClient, IOptions<AdminSettings> adminSettings)
        {
            _lineLoginClient = lineLoginClient; 
            _lineNotifyClient = lineNotifyClient;
            _adminSettings = adminSettings.Value;
        }

        public string GetLineLoginAuthorizeURL(string state)
        {
            return _lineLoginClient.GetAuthorizeURL(state);
        }

        public async Task<LoginUser> LoginCallback(string code, string state, string sessionKey)
        {
            try
            {
                var response = await _lineLoginClient.GetTokenResponse(code, state);
                var lineLoginProfile = response.GetLineLoginProfile();

                var loginUser = LoginUsers.GetValue(sessionKey);

                if (loginUser == null)
                {
                    // add this User
                    loginUser = new LoginUser(oauthProvider: "Line");
                    loginUser.Id = lineLoginProfile.Id;
                    loginUser.Name = lineLoginProfile.Name;
                    loginUser.Picture = lineLoginProfile.Picture;
                    loginUser.AccessTokenForLogin = lineLoginProfile.AccessToken;
                    loginUser.IsAdmin = 
                        _adminSettings.AdminUsers.Any(a => a.Provider == "Line" && a.Id == lineLoginProfile.Id)
                            ? true 
                            : false;    
                }

                LoginUsers.UpdsertLoginUser(loginUser);

                return loginUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string GetLineNotifiyAuthorizeURL(string state)
        {
            return _lineNotifyClient.GetAuthorizeURL(state);
        }

        public async Task<LoginUser> Subscribe(string code, string state, string userId)
        {
            try
            {
                var accessToken = await _lineNotifyClient.GetAccessToken(code, state);
                var getTokenStatusResponse = await _lineNotifyClient.GetAccessTokenStatus(accessToken);

                LoginUsers.UpdateAccessTokenForNotification(userId, accessToken);
                return LoginUsers.GetValue(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task Logout()
        {
            string accessToken = "";
            await Unsubscribe(accessToken);
        }

        public async Task Unsubscribe(string sessionKey)
        {
            try
            {
                var loginUser = LoginUsers.GetValue(sessionKey);

                var getTokenStatusResponse = await _lineNotifyClient.GetAccessTokenStatus(loginUser.AccessTokenForNotification);

                if (getTokenStatusResponse.Status == 401)
                    return;

                if (getTokenStatusResponse.Status == 200)
                {
                    LoginUsers.UpdateAccessTokenForNotification(sessionKey, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task Notify(string message)
        {
            try
            {
                var tokens = LoginUsers.GetAccessTokensForNotification();
                if (tokens.Any())
                {
                    foreach(var token in tokens)
                    {
                        await _lineNotifyClient.Notify(token, message);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public LoginUser GetUser(string userId)
        {
            try
            {
                var loginUser = LoginUsers.GetValue(userId);
                
                return loginUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public IDictionary<string, string> GetSubscriptions()
        {
            return LoginUsers.GetAllSubscriptions();
        }

        public string GetLineAPIState()
        {
            return DateTime.Now.ToString("sssddMMyyyy");
        }

    }
}
