using System;
using System.Globalization;
using System.Linq;
using Redis.Net.Converters;
using Xunit;
using Xunit.Abstractions;

namespace Redis.Net.Tests {
    public partial class RedisHashSetExtensionsTests {
        private readonly ITestOutputHelper _output;

        public RedisHashSetExtensionsTests (ITestOutputHelper outputHelper) {
            this._output = outputHelper;
        }

        [Fact]
        public void TestToHashEntries () {
            var model = Model.CreateNew ();
            var entries = model.ToHashEntries ().ToArray ();
            Assert.NotEmpty (entries);
            Assert.Contains (entries, e => e.Name == nameof (Model.DateNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.DateNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.IntNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.UintNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.DoubleNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.BoolNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.LongNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.UlongNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.FloatNullable));
            Assert.Contains (entries, e => e.Name == nameof (Model.FloatArray));
            Assert.Contains (entries, e => e.Name == nameof (Model.DoubleArray));
            Assert.Contains (entries, e => e.Name == nameof (Model.IntArray));
            Assert.Contains (entries, e => e.Name == nameof (Model.LongArray));
        }

        [Fact]
        public void TestToInstance () {
            var model = Model.CreateNew ();
            var entries = model.ToHashEntries ();
            var instance = entries.ToInstance<Model> ();

            Assert.Equal (model.Str, instance.Str);
            Assert.Equal (model.Date, instance.Date);
            Assert.Equal (model.Int, instance.Int);
            Assert.Equal (model.Uint, instance.Uint);
            Assert.Equal (model.Double, instance.Double);
            Assert.Equal (model.bytes, instance.bytes);
            Assert.Equal (model.Bool, instance.Bool);
            Assert.Equal (model.Long, instance.Long);
            Assert.Equal (model.Ulong, instance.Ulong);
            Assert.Equal (model.Float, instance.Float);
            Assert.Equal (model.DateNullable, instance.DateNullable);
            Assert.Equal (model.IntNullable, instance.IntNullable);
            Assert.Equal (model.UintNullable, instance.UintNullable);
            Assert.Equal (model.DoubleNullable, instance.DoubleNullable);
            Assert.Equal (model.BoolNullable, instance.BoolNullable);
            Assert.Equal (model.LongNullable, instance.LongNullable);
            Assert.Equal (model.UlongNullable, instance.UlongNullable);
            Assert.Equal (model.FloatNullable, instance.FloatNullable);
            Assert.Equal (model.FloatArray, instance.FloatArray);
            Assert.Equal (model.Kind, instance.Kind);
            Assert.Equal (model.DoubleArray, instance.DoubleArray);
            Assert.Equal (model.IntArray, instance.IntArray);
            Assert.Equal (model.LongArray, instance.LongArray);
        }

        [Fact]
        public void TestDateTime () {
            var value = DateTime.Now;
            var redisValue = value.ToString ("O"); //“O”或“o”标准格式说明符表示使用保留时区信息的模式的自定义日期和时间格式字符串，并发出符合 ISO8601 的结果字符串。
            Assert.IsType<string> (redisValue);
            _output.WriteLine (redisValue.ToString ());
            var date = ((IConvertible) redisValue).ToType (typeof (DateTime), CultureInfo.InvariantCulture);
            Assert.Equal (value, date);
        }

        [Theory]
        [InlineData (new float[] { 1, 2, 3, 4 })]
        [InlineData (new int[] { 1, 2, 3, 4 })]
        // [InlineData(new string[]{"a","B","123456"})]
        public void TestArrayConvert (Array array) {
            var val = RedisConvertFactory.ArrayConverter.ToRedisValue (array);
            Assert.NotNull (val);
            Assert.False (val.IsNull);

            var converted = (Array) RedisConvertFactory.ArrayConverter.ToArray (val, array.GetType ());
            Assert.NotNull (converted);
            Assert.Equal (array.Length, converted.Length);
            Assert.Equal (array, converted);
        }
    }
}