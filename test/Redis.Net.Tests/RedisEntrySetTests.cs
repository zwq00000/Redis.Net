using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisEntrySetTests : RedisFactory, IDisposable {

        public class MockEntity {
            static Random random = new Random ();

            public MockEntity () {

            }

            public MockEntity (int id, string msg) {
                this.Id = id;
                this.Message = msg;
                this.IntValue = random.Next ();
                this.Date = DateTime.Now;
                this.Time = TimeSpan.FromMilliseconds (1 + id);
                Bytes = Encoding.UTF8.GetBytes (msg);
            }
            public int Id { get; set; }

            public string Message { get; set; }
            public int IntValue { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }

            public byte[] Bytes { get; set; }
        }

        string SetKey = "_Test:RedisEntrySet";

        private IEnumerable<MockEntity> GetEntities (int count = 10) {
            for (int i = 0; i < count; i++) {
                yield return new MockEntity (i, "Test Entity " + i);
            }
        }

        private void InitSet (int count = 10) {
            for (int i = 0; i < count; i++) {
                var key = $"{SetKey}:{i}";
                var value = new MockEntity (i, key);
                base.Database.HashSet (key, value.ToHashEntries ().ToArray ());
            }
        }

        [Fact]
        public void TestAdd () {
            var set = new RedisEntrySet<int, MockEntity> (base.Database, SetKey);
            set.Clear ();
            foreach (var item in GetEntities (10)) {
                set.Add (item.Id, item);
            }
            Assert.Equal (10, set.Count);
        }

        [Fact]
        public void TestAddBatch () {
            var set = new RedisEntrySet<int, MockEntity> (base.Database, SetKey);
            set.Clear ();
            Assert.Equal (0, set.Count);
            var batch = Database.CreateBatch();
            var batchSet = set.AsBatch();
            foreach (var item in GetEntities (10)) {
                 batchSet.BatchAdd(batch, item.Id, item);
            }
            batch.Execute();
            Assert.Equal (10, set.Count);
        }

        [Fact]
        public void TestValues () {
            TestAdd ();
            var set = new RedisEntrySet<int, MockEntity> (base.Database, SetKey);
            Assert.NotEmpty (set.Values);
            foreach (var entity in set.Values) {
                Assert.True (entity.Date > DateTime.Today);
                Assert.True (entity.Time.Ticks > 0);
                Assert.NotNull (entity.Bytes);
                Assert.Equal (entity.Message, Encoding.UTF8.GetString (entity.Bytes));
            }
        }

        [Fact]
        public async Task TestRebuildIndexAsync () {
            var count = 1000;
            var set = new RedisEntrySet<string, MockEntity> (base.Database, SetKey);
            set.Clear ();
            Assert.Equal (0, set.Count);
            InitSet (count);
            Assert.Equal (0, set.Count);
            await set.RebuildIndexAsync (s => s);
            Assert.Equal (count, set.Count);
        }

        [Fact]
        public void TestRebuildIndex () {
            var count = 10000;
            var set = new RedisEntrySet<string, MockEntity> (base.Database, SetKey);
            set.Clear ();
            Assert.Equal (0, set.Count);
            InitSet (count);
            Assert.Equal (0, set.Count);
            set.RebuildIndex (s => s);
            Assert.Equal (count, set.Count);
        }

        public void Dispose () {
            base.CleanKeys (SetKey);
        }
    }
}