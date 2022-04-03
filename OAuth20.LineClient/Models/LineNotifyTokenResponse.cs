using System.Text.Json.Serialization;

namespace OAuth20.LineClient.Models
{
    public class LineNotifyTokenResponse : LineNotifyBaseResponse
    {

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}

