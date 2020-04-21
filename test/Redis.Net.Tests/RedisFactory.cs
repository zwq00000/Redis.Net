using Redis.Net;
using StackExchange.Redis;

namespace Redis.Net.Tests {
    public class RedisFactory {
        const string RedisServerName = "localhost";
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