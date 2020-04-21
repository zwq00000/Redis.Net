using System;
using System.Globalization;
using System.Linq;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace Redis.Net.Tests {
    public class RedisHashSetExtensionsTests {
        private readonly ITestOutputHelper _output;

        public RedisHashSetExtensionsTests (ITestOutputHelper outputHelper) {
            this._output = outputHelper;
        }
        public class Model {
            public string Str { get; set; }
            public DateTime Date { get; set; }
            public int Int { get; set; }
            public uint Uint { get; set; }
            public double Double { get; set; }
            public byte[] bytes { get; set; }
            public bool Bool { get; set; }
            public long Long { get; set; }
            public ulong Ulong { get; set; }
            public float Float { get; set; }
            public DateTime? DateNullable { get; set; }
            public int? IntNullable { get; set; }
            public uint? UintNullable { get; set; }
            public double? DoubleNullable { get; set; }
            public bool? BoolNullable { get; set; }
            public long? LongNullable { get; set; }
            public ulong? UlongNullable { get; set; }
            public float? FloatNullable { get; set; }

            public UriKind Kind{get;set;}

            public static Model CreateNew () {
                return new Model () {
                    Str = "TEST",
                        bytes = new byte[]{1,2,3},
                        Date = DateTime.Now,
                        DateNullable = DateTime.Now,
                        IntNullable = 1,
                        UintNullable = 1,
                        DoubleNullable = 1,
                        BoolNullable = true,
                        LongNullable = 1,
                        UlongNullable = 1,
                        FloatNullable = 1f,
                        Kind = UriKind.Relative
                };
            }
        }

        [Fact]
        public void TestToHashEntries () {
            var model = Model.CreateNew ();
            var entries = model.ToHashEntries ();
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
            Assert.Equal (model.Kind, instance.Kind);
        }

        [Fact]
        public void TestDateTime () {
            var value = DateTime.Now;
            var redisValue = value.ToString("O");//“O”或“o”标准格式说明符表示使用保留时区信息的模式的自定义日期和时间格式字符串，并发出符合 ISO8601 的结果字符串。
            Assert.IsType<string>(redisValue);
            _output.WriteLine (redisValue.ToString ());
            var date = ((IConvertible)redisValue).ToType(typeof(DateTime),CultureInfo.InvariantCulture);
            Assert.Equal(value,date);
        }
    }
}