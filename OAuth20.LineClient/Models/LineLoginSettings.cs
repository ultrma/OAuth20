using Microsoft.AspNetCore.WebUtilities;

namespace OAuth20.LineClient.Models
{
    public class LineLoginSettings : LineBaseSettings
    {
        private bool HasValidPropertiesForAuthorizeURL =>
               !string.IsNullOrEmpty(ClientID) &&
               !string.IsNullOrEmpty(Scope) &&
               !string.IsNullOrEmpty(CallBackURL);

        public string GetAuthorizeURL(string state)
        {
            if (!HasValidPropertiesForAuthorizeURL)
                return string.Empty;

            var query = new Dictionary<string, string>();
            query.Add("response_type", "code");
            query.Add("client_id", ClientID);
            query.Add("state", state);
            query.Add("scope", Scope);
            query.Add("redirect_uri", CallBackURL);
            var url = QueryHelpers.AddQueryString(AuthorizeEndpoint, query);
            return url;
        }
    }
}
