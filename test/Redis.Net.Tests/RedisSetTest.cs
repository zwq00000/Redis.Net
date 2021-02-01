using System;
using System.Threading.Tasks;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisSetTest : RedisFactory, IDisposable {
        const string SetKey = "_Test:Set1";
        private readonly RedisSet _set;

        public RedisSetTest () {
            _set = new RedisSet (base.Database, SetKey);
            Assert.Empty (_set.Values);
        }

        public void Dispose () {
            Database.KeyDelete (SetKey);
        }

        [Fact]
        public void TestAdd () {
            _set.Add ("Test1");
            Assert.Contains ("Test1", _set.Values);
        }

        [Fact]
        public async Task TestAddAsync () {
            await _set.AddAsync ("Test2");
            Assert.Contains ("Test2", _set.Values);
        }
    }
}