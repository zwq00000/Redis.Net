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
            this.serializer = RedisValueSerializer.Default;
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
            dict.Add(key,instance);
            Assert.True (dict.ContainsKey (key));
            Assert.True (dict.Remove (key));
            Assert.False (dict.ContainsKey (key));
        }
    }
}