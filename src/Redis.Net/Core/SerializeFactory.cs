using StackExchange.Redis;

namespace Redis.Net.Core
{
    public static class SerializeFactory {

        public static T DeserializeObject<T> (this byte[] rawValue) {
            return Serializer.Deserialize<T> (rawValue);
        }

        public static T DeserializeObject<T> (this RedisValue rawValue) {
            return Serializer.Deserialize<T> (rawValue);
        }

        public static byte[] SerializeObject<T> (this T obj) {
            return Serializer.Serialize (obj);
        }

        public static ISerializer Serializer { get; set; }

    }
}