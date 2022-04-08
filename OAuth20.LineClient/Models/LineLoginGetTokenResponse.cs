using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace OAuth20.LineClient.Models
{
    public class LineLoginGetTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("expire_in")]
        public int ExpireIn { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }

        public LineLoginProfile GetLineLoginProfile()
        {
            var stream = IdToken;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            LineLoginProfile lineLoginProfile = new LineLoginProfile();
            lineLoginProfile.Name = tokenS.Claims.First(claim => claim.Type == "name").Value;
            lineLoginProfile.Picture = tokenS.Claims.First(claim => claim.Type == "picture").Value;
            lineLoginProfile.Id = tokenS.Claims.First(claim => claim.Type == "sub").Value;
            lineLoginProfile.AccessToken = AccessToken;

            return lineLoginProfile;
        }
    }    
}
