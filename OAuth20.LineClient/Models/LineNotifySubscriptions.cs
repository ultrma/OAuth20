using System.Linq;

namespace OAuth20.LineClient.Models
{
    public sealed class LineNotifySubscriptions
    {
        private static IDictionary<string, string> _subscriptions;

        static LineNotifySubscriptions()
        {
            _subscriptions = new Dictionary<string, string>();
        }

        public static bool TryGetValue(string key, out string name)
        {
            return _subscriptions.TryGetValue(key, out name);
        }

        public static void Set(string key, string value)
        {
            _subscriptions.Add(key, value);
        }

        public static void Remove(string key)
        {
            _subscriptions.Remove(key);
        }

        public static string GetValue(string key)
        {
            if (!_subscriptions.TryGetValue(key, out string value))
                return string.Empty;
            else
                return value;
        }

        public static List<string> GetKeys() { 
            return _subscriptions.Select(s => s.Key).ToList();
        }
          
        public static IDictionary<string, string> GetAll()
        {
            return _subscriptions;
        }
    }
}
