using Microsoft.Extensions.Options;
using OAuth20.LineClient.Models;
using System.Text.Json;

namespace OAuth20.LineClient.Services
{
    public class LineLoginClient : ILineLoginClient
    {
        private readonly LineLoginSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public LineLoginClient(IOptions<LineLoginSettings> settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public string GetAuthorizeURL(string state)
        {
            return _settings.GetAuthorizeURL(state);
        }

        public async Task<LineLoginGetTokenResponse> GetTokenResponse(string code, string state)
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
                throw new Exception("Unable to get token from Line Login");

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<LineLoginGetTokenResponse>(responseString);

            return tokenResponse;
        }

    }
}
