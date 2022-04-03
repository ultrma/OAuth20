using OAuth20.LineClient.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace OAuth20.LineClient.Services
{
    public class LineNotifyClient : ILineNotifyClient
    {
        private readonly LineNotifySettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public LineNotifyClient(IOptions<LineNotifySettings> settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public string GetAuthorizeURL()
        {
            return _settings.AuthourizeURL;
        }

        public async Task<string> GetAccessToken(string code, string state)
        {
            var client = _httpClientFactory.CreateClient();
            var requestBody = new[]
            {
                new KeyValuePair<string,string>("grant_type" , _settings.GrantType),
                new KeyValuePair<string,string>("code", code),
                new KeyValuePair<string,string>("client_id", _settings.ClientID),
                new KeyValuePair<string,string>("client_secret", _settings.ClientSecret),
                new KeyValuePair<string,string>("redirect_uri",  _settings.CallBackURL)
            };

            var content = new FormUrlEncodedContent(requestBody);

            var response = await client.PostAsync(_settings.TokenURL, content);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to get access token from Line Notify");

            var responseString = await response.Content.ReadAsStringAsync();
            var lineNotifyTokenResponse= JsonSerializer.Deserialize<LineNotifyTokenResponse>(responseString);

            return (lineNotifyTokenResponse == null) ? String.Empty : lineNotifyTokenResponse.AccessToken;
        }

        public async Task Notify(string accessToken, string message)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new { message = message };
            var requestBody = new[]
            {
                new KeyValuePair<string,string>("message" ,message)
            };
            var content = new FormUrlEncodedContent(requestBody);

            var response = await client.PostAsync(_settings.NotifyEndpoint, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to send notification from Line Notify");

            var responseString = await response.Content.ReadAsStringAsync();
            var lineNotifyTokenResponse = JsonSerializer.Deserialize<LineNotifySendNotificationResponse>(responseString);
        }

        public async Task<LineNotifyGetAccessTokenResponse> GetAccessTokenStatus(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(_settings.GetAccessTokenStatusEndpoint);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to get access token status from Line Notify");

            var responseString = await response.Content.ReadAsStringAsync();
            var lineNotifyGetAccessTokenResponse = JsonSerializer.Deserialize<LineNotifyGetAccessTokenResponse>(responseString);

            return lineNotifyGetAccessTokenResponse;
        }

        public async Task RevokeAccessToken(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.PostAsync(_settings.RevokeAccessTokenEndpoint, null);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to remove access token from Line Notify");

            var responseString = await response.Content.ReadAsStringAsync();
            var lineNotifyRevokeAccessTokenResponse = JsonSerializer.Deserialize<LineNotifyRevokeAccessTokenResponse>(responseString);
        }
    }
}
