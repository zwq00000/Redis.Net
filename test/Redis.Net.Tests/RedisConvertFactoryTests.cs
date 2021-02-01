using System;
using Redis.Net.Converters;
using StackExchange.Redis;
using Xunit;

namespace Redis.Net.Tests {
    public class RedisConvertFactoryTests {

        [Fact]
        public void TestToRedisValue () {
            Assert.Equal (RedisValue.Null, RedisConvertFactory.ToRedisValue<string> (null));
            Assert.Equal (RedisValue.Null, RedisValue.Unbox (null));
            Assert.Equal (5, RedisConvertFactory.ToRedisValue<DayOfWeek> (DayOfWeek.Friday));
        }

        [Fact]
        public void TestUnbox () {
            object obj = null;
            RedisValue value = RedisValue.Null;
            Assert.Equal (value, RedisValue.Unbox (obj));

            obj = DayOfWeek.Friday;
            value = 5;
            Assert.Equal (value, RedisValue.Unbox (obj));

        }
    }
}