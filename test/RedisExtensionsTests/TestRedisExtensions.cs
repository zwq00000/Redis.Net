using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net;
using Redis.Net.Generic;
using StackExchange.Redis;
using Xunit;

namespace RedisExtensionsTests {

    public class TestRedisExtensions : RedisFactory {
        private const int SampleCount = 100;

        [Fact]
        public void CleanAll () {
            base.CleanKeys ("TEST");
        }

        [Fact]
        public async Task Test1 () {
            var client = CreateRedisClient ();
            var SetKey = "TEST:CacheClient:User";

            await client.Db0.HashSetAsync (SetKey, GetUsers (SampleCount).ToDictionary (u => u.UserId.ToString ()));
        }

        [Fact]
        public async Task Test2 () {
            var client = CreateRedisClient ();
            var SetKey = "TEST:Batch:User";

            var batch = client.Db0.Database.CreateBatch ();
            var userEntrys = GetUsers (SampleCount).Select (u => new HashEntry (u.UserId.ToString (), Serializer.Serialize (u)))
                .ToArray ();
            foreach (var entry in userEntrys) {
                await batch.HashSetAsync (SetKey, entry.Name, entry.Value);
            }

            batch.Execute ();

            var users = await client.Db0.HashGetAllAsync<User> (SetKey);
            Assert.Equal (SampleCount, users.Count);
            //Assert.True(users.Values.All(u => u.Firstname == "Test UserName"));
        }

        [Fact]
        public void EntrySet_Add_Test () {
            var client = CreateRedisClient ();
            var SetKey = "TEST:Users:";
            var userSet = new RedisEntrySet<int, User> (client.Db0.Database, SetKey);
            var userDict = GetUsers (SampleCount).ToDictionary (u => u.UserId);
            foreach (var user in userDict) {
                userSet.Add (user);
            }

            //Assert.Equal(SampleCount, userSet.Count);
            //Assert.True(userSet.Values.All(u => u.Firstname == "Test UserName"));
        }

        [Fact]
        public async Task TestEntitySetBatch () {
            var client = CreateRedisClient ();
            var SetKey = "TEST:Users:Batch:";
            var userSet = new RedisEntrySet<int, User> (client.Db0.Database, SetKey);
            var batch = client.Db0.Database.CreateBatch ();
            foreach (var user in GetUsers (SampleCount)) {
                await userSet.AddAsync (batch, user.UserId, user);
            }
            batch.Execute ();

            Assert.Equal (SampleCount, userSet.Count);
            //Assert.True(userSet.Values.All(u => u.Firstname == "Test UserName"));
        }

        private IEnumerable<User> GetUsers (int count = 100) {
            for (int i = 0; i < count; i++) {
                yield return new User () { UserId = i };
            }
        }

    }
}