﻿using Microsoft.AspNetCore.WebUtilities;

namespace OAuth20.LineClient.Models
{
    public class LineNotifySettings : LineBaseSettings
    {
        public string AuthourizeURL => GetAuthorizeURL();
        public string NotifyEndpoint { get; set; }
        public string GetAccessTokenStatusEndpoint { get; set; }
        public string RevokeAccessTokenEndpoint { get; set; }

        private bool HasValidPropertiesForAuthorizeURL =>
               !string.IsNullOrEmpty(ClientID) &&
               !string.IsNullOrEmpty(State) &&
               !string.IsNullOrEmpty(Scope) &&
               !string.IsNullOrEmpty(CallBackURL);

        private string GetAuthorizeURL()
        {
            if (!HasValidPropertiesForAuthorizeURL) 
                return string.Empty;

            var query = new Dictionary<string, string>();
            query.Add("response_type", "code");
            query.Add("client_id", ClientID);
            query.Add("state", State);
            query.Add("scope", Scope);
            query.Add("redirect_uri", CallBackURL);
            var url = QueryHelpers.AddQueryString(AuthorizeEndpoint, query);
            return url;
        }
    }
}
