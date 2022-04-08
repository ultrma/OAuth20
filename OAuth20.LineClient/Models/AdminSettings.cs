namespace OAuth20.LineClient.Models
{
    public class AdminUser
    {
        public string Id { get; set; }
        public string Provider { get; set; }
    }

    public class AdminSettings
    {
        public List<AdminUser> AdminUsers { get; set; }
    }
}
