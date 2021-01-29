using System;
using System.Linq;
using Redis.Net.Generic;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisHashSetTests : RedisFactory, IDisposable {
        const string SetKey = "_Test:HashSet";

        public RedisHashSetTests () {

        }

        [Fact]
        public void TestAddEnum () {
            var hashSet = new RedisHashSet<string, DayOfWeek> (base.Database, SetKey);
            Assert.Empty (hashSet.Values);

            foreach (DayOfWeek item in Enum.GetValues (typeof (DayOfWeek))) {
                hashSet.Add (item.ToString (), item);
            }
            Assert.NotEmpty (hashSet.Values);

            Assert.IsType<DayOfWeek> (hashSet.Values.First());
        }

        [Fact]
        public async System.Threading.Tasks.Task TestAddEnumAsync () {
            var hashSet = new RedisHashSet<string, DayOfWeek> (base.Database, SetKey);
            Assert.Empty (hashSet.Values);

            foreach (DayOfWeek item in Enum.GetValues (typeof (DayOfWeek))) {
                await hashSet.AsAsync ().AddAsync (item.ToString (), item);
            }
            Assert.NotEmpty (hashSet.Values);

            Assert.All (hashSet.Values, t => Assert.IsType<DayOfWeek> (t));
        }

        public void Dispose () {
            base.CleanKeys (SetKey);
        }
    }
}