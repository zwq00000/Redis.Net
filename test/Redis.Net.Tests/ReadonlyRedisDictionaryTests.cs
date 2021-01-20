using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Generic;
using Xunit;

namespace Redis.Net.Tests {
    public class ReadonlyRedisDictionaryTests {
        private readonly RedisFactory _factory;
        private readonly NewtonsoftJsonSerializer serializer;
        private readonly ReadonlyRedisDictionary<string, ShipName> dict;

        public ReadonlyRedisDictionaryTests () {
            this._factory = new RedisFactory ();
            this.serializer = new NewtonsoftJsonSerializer ();
            this.dict = new ReadonlyRedisDictionary<string, ShipName> (_factory.Database, "ShipInfo:ShipNames", serializer);
        }

        [Fact]
        public void TestGetValues () {
            var values = dict.GetValues ("373300000", "412272280");
            Assert.NotEmpty (values);
            Assert.IsType<ShipName> (values.First ());
            Assert.Equal (2, values.Count ());

            values = dict.GetValues ("373300000", "412272280", "NotFoundKey");
            Assert.Equal (3, values.Count ());
            Assert.Null (values.Last ());
        }

        [Fact]
        public async Task TestGetValuesAsync () {
            var values = await dict.GetValuesAsync ("373300000", "412272280");
            Assert.NotEmpty (values);
            Assert.IsType<ShipName> (values.First ());
            Assert.Equal (2, values.Count ());

            values = await dict.GetValuesAsync ("373300000", "412272280", "NotFoundKey");
            Assert.Equal (3, values.Count ());
            Assert.Null (values.Last ());
        }

        [Fact]
        public void TestContainsKey () {
            Assert.True (dict.ContainsKey ("373300000"));
            Assert.False (dict.ContainsKey ("NotExists"));
        }
    }

}