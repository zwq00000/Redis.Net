using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Generic;
using Redis.Net.Serializer;
using StackExchange.Redis;
using Xunit;

namespace Redis.Net.Tests {
    public class ReadOnlyRedisDictionaryTests {
        private readonly RedisFactory _factory;
        private readonly ISerializer serializer;
        private readonly ReadOnlyRedisDictionary<string, ShipName> dict;

        public ReadOnlyRedisDictionaryTests () {
            this._factory = new RedisFactory ();
            this.serializer = DefaultSerializer.Default; //new NewtonsoftJsonSerializer ();
            this.dict = new ReadOnlyRedisDictionary<string, ShipName> (_factory.Database, "ShipInfo:ShipNames", serializer);
        }

        [Fact]
        public void TestGetValues () {
            var keys = dict.Keys.Take (10).ToArray ();
            Assert.NotEmpty (keys);
            var values = dict.GetValues (keys);
            Assert.IsType<ShipName> (values.First ());
            Assert.Equal (10, values.Count ());
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

        [Fact]
        public async Task TestGeoshSetInBoundAsync () {
            var geos = new GeoHashSet (_factory.Database, "ShipTrack:GeoHash");
            Assert.NotEmpty (geos.Keys ());
            var Bounds = new Bounds (118.2, 38.678, 118.96, 39.18);
            // geos.GetRedius
            var keys = await geos.KeysAsync ();
            var poses = geos.Position (keys.Select (k => RedisValue.Unbox (k)).ToArray ());
            Assert.NotEmpty (poses);
            var count = poses.Count (p => Bounds.InBounds (p.Value.Longitude, p.Value.Latitude));
            Assert.True (count < keys.Count ());
        }
    }

    /// <summary>
    /// 区域
    /// </summary>
    public class Bounds {
        public Bounds (double left, double bottom, double right, double top) {
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
        }

        /// <summary>
        /// 左边界
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// 下边界
        /// </summary>
        public double Bottom { get; set; }

        /// <summary>
        /// 右边界
        /// </summary>
        public double Right { get; set; }

        /// <summary>
        /// 上边界
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height => Top - Bottom;

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width => Right - Left;

        public override string ToString () {
            return $"Bounds[{Left} {Top} {Right} {Bottom}]";
        }

        public double[] ToArray () {
            return new [] { Left, Top, Right, Bottom };
        }

        public bool InBounds (double x, double y) {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }
    }
}