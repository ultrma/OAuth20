namespace OAuth20.LineClient.Models
{
    public class LineBaseSettings
    {
        public readonly string GrantType = "authorization_code";
        public string AuthorizeEndpoint { get; set; }
        public string BaseAddress { get; set; }
        public string CallBackURL { get; set; }
        public string TokenURL { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
    }
}

