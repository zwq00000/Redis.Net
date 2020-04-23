using Redis.Net;
using StackExchange.Redis;

namespace Redis.Net.Tests {
    public class RedisFactory {

#if DEBUG
        const string RedisServerName = "192.168.1.15";
#else
        const string RedisServerName = "localhost";
#endif
        protected IDatabase Database { get; }

        public RedisFactory () {
            var client = this.Connection ();
            Database = client.GetDatabase ();
        }

        public ConnectionMultiplexer Connection () {
            return ConnectionMultiplexer.Connect (RedisServerName);
        }

        public void CleanKeys (string setKey) {
            var keys = Database.GetKeys (setKey);
            Database.KeyDelete (keys);
        }
    }
}