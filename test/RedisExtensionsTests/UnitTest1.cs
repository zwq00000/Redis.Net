using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.MsgPack;
using System.Threading.Tasks;
using Redis.Net;
using Redis.Net.Generic;
using StackExchange.Redis;
using Xunit;

namespace RedisExtensionsTests {

    public class RedisFactory {
        readonly RedisConfiguration redisConfiguration = new RedisConfiguration() {
            AbortOnConnectFail = true,
            KeyPrefix = "_my_key_prefix_",
            Hosts = new RedisHost[]{
                new RedisHost(){Host = "192.168.1.103", Port = 6379}
            },
            AllowAdmin = false,
            ConnectTimeout = 3000,
            Database = 0,
            Ssl = false,
            ServerEnumerationStrategy = new ServerEnumerationStrategy() {
                Mode = ServerEnumerationStrategy.ModeOptions.All,
                TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
            }
        };

        protected MsgPackObjectSerializer Serializer { get; }

        protected IDatabase Database { get; }

        public RedisFactory() {
            Serializer = new MsgPackObjectSerializer();
            var client = CreateRedisClient();
            Database = client.Db0.Database;
        }

        

        public RedisCacheClient CreateRedisClient() {
            return new RedisCacheClient(new RedisCacheConnectionPoolManager(redisConfiguration), Serializer,
                redisConfiguration);
        }

        public void CleanKeys(string setKey) {
            var keys = Database.GetKeys(setKey);
            Database.KeyDelete(keys);
        }
    }

    public class UnitTest1 : RedisFactory {
        private const int SampleCount = 100000;

        [Fact]
        public void CleanAll() {
           base.CleanKeys("TEST");
        }

        [Fact]
        public async Task Test1() {
            var client = CreateRedisClient();
            var SetKey = "TEST:CacheClient:User";

            await client.Db0.HashSetAsync(SetKey, GetUsers(SampleCount).ToDictionary(u=>u.UserId.ToString()));

            //var users = await client.Db0.HashGetAllAsync<User>(SetKey);
            //Assert.Equal(SampleCount, users.Count);
            //Assert.True(users.Values.All(u=>u.Firstname == "Test UserName"));
        }

        [Fact]
        public async Task Test2() {
            var client = CreateRedisClient();
            var SetKey = "TEST:Batch:User";

            var batch = client.Db0.Database.CreateBatch();
            var userEntrys = GetUsers(SampleCount).Select(u => new HashEntry(u.UserId.ToString(), Serializer.Serialize(u)))
                .ToArray();
            foreach (var entry in userEntrys) {
                await batch.HashSetAsync(SetKey,entry.Name,entry.Value);
            }
            
            batch.Execute();

            var users = await client.Db0.HashGetAllAsync<User>(SetKey);
            Assert.Equal(SampleCount, users.Count);
            //Assert.True(users.Values.All(u => u.Firstname == "Test UserName"));
        }

        [Fact]
        public void EntrySet_Add_Test() {
            var client = CreateRedisClient();
            var SetKey = "TEST:Users:";
            var userSet = new RedisEntrySet<int,User>(client.Db0.Database,SetKey);
            var userDict = GetUsers(SampleCount).ToDictionary(u => u.UserId);
            foreach(var user in userDict){
                userSet.Add(user);
            }

            //Assert.Equal(SampleCount, userSet.Count);
            //Assert.True(userSet.Values.All(u => u.Firstname == "Test UserName"));
        }

        [Fact]
        public async Task TestEntitySetBatch() {
            var client = CreateRedisClient();
            var SetKey = "TEST:Users:Batch:";
            var userSet = new RedisEntrySet<int, User>(client.Db0.Database, SetKey);
            var batch = client.Db0.Database.CreateBatch();
            foreach (var user in GetUsers(SampleCount)) {
                await userSet.AddAsync(batch, user.UserId, user);
            }
            batch.Execute();

            Assert.Equal(SampleCount, userSet.Count);
            //Assert.True(userSet.Values.All(u => u.Firstname == "Test UserName"));
        }

        private IEnumerable<User> GetUsers(int count = 100) {
            for (int i = 0; i < count; i++) {
                yield return   new User() { UserId = i };
            }
        }

    }

    public class User {

        public int UserId { get; set; }

        public string Firstname { get; set; } = "Test UserName";
        public string Lastname { get; set; } = "Last Name";
        public string Twitter { get; set; } = "@imperugo";

        public string Blog { get; set; } = "http://tostring.it";
    }
}
