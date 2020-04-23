using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {

    /// <summary>
    /// 只读的 Redis GeoHash Set
    /// </summary>
    public class ReadOnlyGeoHashSet : ReadOnlySortedSet {

        public ReadOnlyGeoHashSet (IDatabase database, string setKey) : base (database, setKey) {

        }

        /// <summary>
        /// 获取成员位置
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public GeoPosition? Position (RedisValue member) {
            return Database.GeoPosition (SetKey, member);
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="members">The members to get.</param>
        /// <returns></returns>
        public GeoPosition?[] Position (params RedisValue[] members) {
            return Database.GeoPosition (SetKey, members);
        }

        /// <summary>
        /// 异步获取位置
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<GeoPosition?> PositionAsync (RedisValue member) {
            return await Database.GeoPositionAsync (SetKey, member);
        }

        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="member1"></param>
        /// <param name="member2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public double? Distance (RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters) {
            return Database.GeoDistance (SetKey, member1, member2, unit);
        }

        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="member1"></param>
        /// <param name="member2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public async Task<double?> DistanceAsync (RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters) {
            return await Database.GeoDistanceAsync (SetKey, member1, member2, unit);
        }

        /// <summary>
        /// 返回 给定实例 和半径内的所有位置
        /// </summary>
        /// <param name="member"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <param name="count"></param>
        /// <param name="order"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public GeoRadiusResult[] GetRedius (RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return Database.GeoRadius (SetKey, member, radius, unit, count, order, options);
        }

        /// <summary>
        /// 根据经纬度计算半径内的所有位置
        /// Return the members of a sorted set populated with geospatial information using GEOADD,
        ///  which are within the borders of the area specified with the center location and the maximum distance from the center (the radius).
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <param name="count"></param>
        /// <param name="order"></param>
        /// <param name="options"></param>
        public GeoRadiusResult[] GetRedius (double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return Database.GeoRadius (SetKey, longitude, latitude, radius, unit, count, order, options);
        }

        /// <summary>
        /// 返回 给定实例 和半径内的所有 ShipId
        /// </summary>
        /// <param name="member"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public async Task<GeoRadiusResult[]> GetRediusAsync (RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters) {
            return await Database.GeoRadiusAsync (SetKey, member, radius, unit);
        }

        public async Task<GeoRadiusResult[]> GetRediusAsync (double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return await Database.GeoRadiusAsync (SetKey, longitude, latitude, radius, unit, count, order, options);
        }
    }
}