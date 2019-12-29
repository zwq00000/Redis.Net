using Redis.Net;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.MsgPack;

namespace RedisExtensionsTests {
    public class RedisFactory {
#if DEBUG
        const string RedisServerName = "192.168.1.105";
#else
        const string RedisServerName = "localhost";
#endif
        readonly RedisConfiguration redisConfiguration = new RedisConfiguration () {
            AbortOnConnectFail = true,
            KeyPrefix = "_my_key_prefix_",
            Hosts = new RedisHost[] {
            new RedisHost () { Host = RedisServerName, Port = 6379 }
            },
            AllowAdmin = false,
            ConnectTimeout = 3000,
            Database = 0,
            Ssl = false,
            ServerEnumerationStrategy = new ServerEnumerationStrategy () {
            Mode = ServerEnumerationStrategy.ModeOptions.All,
            TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
            UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
            }
        };

        protected MsgPackObjectSerializer Serializer { get; }

        protected IDatabase Database { get; }

        public RedisFactory () {
            Serializer = new MsgPackObjectSerializer ();
            var client = CreateRedisClient ();
            Database = client.Db0.Database;
        }

        public RedisCacheClient CreateRedisClient () {
            return new RedisCacheClient (new RedisCacheConnectionPoolManager (redisConfiguration), Serializer,
                redisConfiguration);
        }

        public void CleanKeys (string setKey) {
            var keys = Database.GetKeys (setKey);
            Database.KeyDelete (keys);
        }
    }
}