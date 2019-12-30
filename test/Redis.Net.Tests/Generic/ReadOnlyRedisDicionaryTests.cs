using Redis.Net.Core;
using Redis.Net.Serializer;
using Redis.Net.Tests;
using RedisExtensionsTests;
using Xunit;

namespace Redis.Net.Generic.Tests
{
    public class ReadOnlyRedisDicionaryTests:RedisFactory{
        const string SetKey = "TEST:ReadOnlyRedisDicionaryTests";

        public ReadOnlyRedisDicionaryTests(){
            SerializeFactory.Serializer = new JsonSerializer();
        }

        [Fact]
        public void TestAdd(){
            var dict = new RedisDicionary<string,User>(base.Database,SetKey);
            var user = new User();
            dict.Add("abc",user);
            Assert.Equal(user,dict["abc"]);
        }
    }
}