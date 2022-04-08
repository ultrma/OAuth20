using System.Text.Json.Serialization;

namespace OAuth20.Web.Models
{
    public class LoginUser
    {
        public LoginUser(string oauthProvider = "Line")
        {
            OAuthProvider = oauthProvider;
        }

        public string Id { get; set; }
        public string OAuthProvider { get;}
        public string Name { get; set;}
        public string Picture { get; set; }
        [JsonIgnore]
        public string AccessTokenForLogin { get; set; }
        [JsonIgnore]
        public string AccessTokenForNotification { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; set; }
        public bool HasSubscribed => (string.IsNullOrWhiteSpace(AccessTokenForNotification)) 
            ? false 
            : true;
    }
}
