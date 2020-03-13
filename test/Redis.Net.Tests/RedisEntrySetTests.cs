using System;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisEntrySetTests : RedisFactory, IDisposable {

        public class EntityMock {
            public string Id { get; set; }
            public int IntValue { get; set; }
        }

        string SetKey = "_Test:RedisEntrySet";
        private readonly RedisSet _set;

        private void InitSet (int count = 10) {
            for (int i = 0; i < count; i++) {
                var key = $"{SetKey}:{i:000}";
                base.Database.HashSet (key, new [] { new HashEntry ("test", "value" + i) });
            }
        }

        [Fact]
        public async Task TestRebuildIndexAsync () {
            var set = new RedisEntrySet<string, EntityMock> (base.Database, SetKey);
            set.Clear();
            Assert.Equal (0, set.Count);
            InitSet (10);
            Assert.Equal (0, set.Count);
            await set.RebuldIndexAsync (s => s);
            Assert.Equal (10, set.Count);
        }

        public void Dispose () {
            base.CleanKeys("SetKey:");
            base.CleanKeys(SetKey);
        }
    }
}