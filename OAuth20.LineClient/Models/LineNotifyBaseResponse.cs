using System.Text.Json.Serialization;

namespace OAuth20.LineClient.Models
{
    public class LineNotifyBaseResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }

    }
}

