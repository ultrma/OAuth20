using OAuth20.LineClient.Services;
using OAuth20.LineClient.Models;
using OAuth20.Web.Pages;
using OAuth20.Web.Models;

namespace OAuth20.Web.Services
{
    public class UserService : IUserService
    {
        private readonly ILineLoginClient _lineLoginClient;
        private readonly ILineNotifyClient _lineNotifyClient;

        public UserService (ILineLoginClient lineLoginClient, ILineNotifyClient lineNotifyClient)
        {
            _lineLoginClient = lineLoginClient; 
            _lineNotifyClient = lineNotifyClient;
        }

        public string GetLineLoginAuthorizeURL()
        {
            return _lineLoginClient.GetAuthorizeURL();
        }

        public async Task<User> Login(string code, string state)
        {
            try
            {
                var response = await _lineLoginClient.GetTokenResponse(code, state);
                var lineLoginProfile = response.GetLineLoginProfile();
                return new User { AccessToken = lineLoginProfile.AccessToken, Name = lineLoginProfile.Name, Picture = lineLoginProfile.Picture};
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public string GetLineNotifiyAuthorizeURL()
        {
            return _lineNotifyClient.GetAuthorizeURL();
        }

        public async Task<User> Subscribe(string code, string state)
        {
            try
            {
                var accessToken = await _lineNotifyClient.GetAccessToken(code, state);
                var getTokenStatusResponse = await _lineNotifyClient.GetAccessTokenStatus(accessToken);

                string name = string.Empty;
                if (LineNotifySubscriptions.TryGetValue(accessToken, out name))
                    return new User { Name = name, AccessToken = accessToken };

                name = getTokenStatusResponse.Target;
                LineNotifySubscriptions.Set(accessToken, name);
                return new User { Name = name, AccessToken=accessToken};
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task Unsubscribe(string accessToken)
        {
            try
            {
                var getTokenStatusResponse = await _lineNotifyClient.GetAccessTokenStatus(accessToken);

                if (getTokenStatusResponse.Status == 401)
                    return;

                if (getTokenStatusResponse.Status == 200)
                {
                    LineNotifySubscriptions.Remove(accessToken);
                    await _lineNotifyClient.RevokeAccessToken(accessToken);
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
                var tokens = LineNotifySubscriptions.GetKeys();
                if(tokens.Any())
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

        public User GetUser(string accessToken)
        {
            try
            {
                var name = LineNotifySubscriptions.GetValue(accessToken);
                if (!string.IsNullOrWhiteSpace(name))
                    return new User { AccessToken = accessToken, Name = name };

                return null;                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public IDictionary<string, string> GetSubscriptions()
        {
            return LineNotifySubscriptions.GetAll();
        }
    }
}
