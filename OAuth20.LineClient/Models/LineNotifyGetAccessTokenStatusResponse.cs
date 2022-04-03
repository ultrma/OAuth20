using System.Text.Json.Serialization;

namespace OAuth20.LineClient.Models
{
    public class LineNotifyGetAccessTokenResponse : LineNotifyBaseResponse
    {
        [JsonPropertyName("targetType")]
        public string TargetType { get; set; }

        [JsonPropertyName("target")]
        public string Target { get; set; }
    }
}

