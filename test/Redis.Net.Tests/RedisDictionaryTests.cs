using System.Linq;
using Redis.Net.Generic;
using Redis.Net.Serializer;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisDictionaryTests {
        private readonly RedisFactory _factory;
        private readonly ISerializer serializer;
        private readonly RedisDictionary<string, Model> dict;

        public RedisDictionaryTests () {
            this._factory = new RedisFactory ();
            this.serializer = DefaultSerializer.Default;
            this.dict = new RedisDictionary<string, Model> (_factory.Database, "_TEST:Model", serializer);
        }

        [Fact]
        public void TestAdd () {
            var instance = Model.CreateNew ();
            dict.Add (instance.Str, instance);
            Assert.True (dict.ContainsKey (instance.Str));
            var serialized = dict[instance.Str];
            Assert.Equal (instance, serialized);
        }

        [Fact]
        public void TestRemove () {
            var instance = Model.CreateNew ();
            var key = "TestRemove";
            dict.Add (key, instance);
            Assert.True (dict.ContainsKey (key));
            Assert.True (dict.Remove (key));
            Assert.False (dict.ContainsKey (key));
        }

        [Fact]
        public void TestAsBanch () {
            var batch = dict.AsBatch ();
            Assert.NotNull (batch);
            Assert.Same (dict, batch);

             var key = "TestAsBatch";
             var b = _factory.Database.CreateBatch();
            batch.BatchAdd(b, key,Model.CreateNew());
            b.Execute();
            Assert.True(dict.ContainsKey(key));
            dict.Remove(key);
        }

        [Fact]
        public async System.Threading.Tasks.Task TestAsAsync () {
            var asy = dict.AsAsync ();
            Assert.NotNull (asy);
            Assert.Same (dict, asy);
            var key = "TestAsAsync";
            await asy.AddAsync(key,Model.CreateNew());
            Assert.True(dict.ContainsKey(key));
            dict.Remove(key);
        }
    }
}