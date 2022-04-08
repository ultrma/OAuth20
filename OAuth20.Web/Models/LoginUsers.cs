using System.Linq;
namespace OAuth20.Web.Models
{
    public sealed class LoginUsers
    {
        // SessionKey(Guid), LoginUser
        private static Dictionary<string, LoginUser> Users { get; set; }
        
        // LoginUser.Id, SessionKey(Guid)
        private static Dictionary<string, string> SessionKeys { get; set; }

        static LoginUsers()
        {
            Users = new Dictionary<string, LoginUser>();
            SessionKeys = new Dictionary<string, string>();
        }

        public static bool TryGetUser(string userId, out LoginUser name)
        {
            return Users.TryGetValue(userId, out name);
        }

        public static void Set(string sessionKey, LoginUser value)
        {
            Users.Add(sessionKey, value);
            SessionKeys.Add(value.Id, sessionKey);
        }

        public static void RemoveBySessionKey(string sessionKey)
        {
            var loginUser = Users[sessionKey];
            SessionKeys.Remove(loginUser.Id);
            Users.Remove(sessionKey);
        }

        public static void RemoveByIdentity(string userId)
        {
            var guid = SessionKeys[userId];
            SessionKeys.Remove(userId);
            Users.Remove(guid);
        }

        public static LoginUser GetValue(string sessionKey)
        {
            if (sessionKey == null || !Users.TryGetValue(sessionKey, out LoginUser loginUser))
                return null;
            else
                return loginUser;
        }

        public static List<string> GetSessionKeys()
        {
            return Users.Select(s => s.Key).ToList();
        }

        public static IDictionary<string, LoginUser> GetAllUsers()
        {
            return Users;
        }

        public static string GetSessionKey(string userId)
        {
            if (SessionKeys.TryGetValue(userId, out string sessionKey))
                return sessionKey;
            
            return null;
        }

        public static void UpdsertLoginUser(LoginUser updatedUser)
        {
            if (SessionKeys.TryGetValue(updatedUser.Id, out string sessionKey))
            {
                var existingUser = Users[sessionKey];
                RemoveByIdentity(existingUser.Id);
                updatedUser.AccessTokenForNotification = existingUser.AccessTokenForNotification;
            }

            Set(Guid.NewGuid().ToString(), updatedUser);
        }

        public static void UpdateAccessTokenForNotification(string sessionKey, string accessTokenForNotification)
        {
            var loginUser = GetValue(sessionKey);
            loginUser.AccessTokenForNotification = accessTokenForNotification;

            RemoveBySessionKey(sessionKey);
            Set(sessionKey, loginUser);
        }


        public static List<string> GetAccessTokensForNotification()
        {
            return Users.Values
                .Where(u => !string.IsNullOrWhiteSpace(u.AccessTokenForNotification))
                .Select(u => u.AccessTokenForNotification)
                .ToList();
        }

        public static List<string> GetAllUserNames()
        {
            return Users.Values
                .Select(u => u.Name)
                .ToList();
        }

        public static IDictionary<string, string> GetAllSubscriptions()
        {
            return Users
                .Where(u => !string.IsNullOrWhiteSpace(u.Value.AccessTokenForNotification))
                .ToDictionary(u => u.Key, u => u.Value.Name);
        }
    }

}
