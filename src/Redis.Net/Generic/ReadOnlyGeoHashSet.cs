using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {

    /// <summary>
    /// 只读的 Redis GeoHash Set
    /// </summary>
    public class ReadOnlyGeoHashSet<TKey> : ReadOnlySortedSet<TKey> where TKey : IConvertible {

        public ReadOnlyGeoHashSet (IDatabase database, string setKey) : base (database, setKey) {

        }

        /// <summary>
        /// 获取成员位置
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public GeoPosition? Position (TKey member) {
            return Database.GeoPosition (SetKey, Unbox (member));
        }

        /// <summary>
        /// Return the positions (longitude,latitude) of all the specified members of the geospatial index represented by the sorted set at key.
        /// </summary>
        /// <param name="members">The members to get.</param>
        /// <returns></returns>
        public GeoPosition?[] Position (params TKey[] members) {
            return Database.GeoPosition (SetKey, members.Select (m => Unbox (m)).ToArray ());
        }

        /// <summary>
        /// 异步获取位置
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<GeoPosition?> PositionAsync (TKey member) {
            return await Database.GeoPositionAsync (SetKey, Unbox (member));
        }

        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="member1"></param>
        /// <param name="member2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public double? GetDistance (TKey member1, TKey member2, GeoUnit unit = GeoUnit.Meters) {
            return Database.GeoDistance (SetKey, Unbox (member1), Unbox (member2), unit);
        }

        /// <summary>
        /// 计算两点间距离
        /// </summary>
        /// <param name="member1"></param>
        /// <param name="member2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public async Task<double?> GetDistanceAsync (TKey member1, TKey member2, GeoUnit unit = GeoUnit.Meters) {
            return await Database.GeoDistanceAsync (SetKey, Unbox (member1), Unbox (member2), unit);
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
        public GeoRadiusResult[] GetByRedius (TKey member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return Database.GeoRadius (SetKey, Unbox (member), radius, unit, count, order, options);
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
        public GeoRadiusResult[] GetByRedius (double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return Database.GeoRadius (SetKey, longitude, latitude, radius, unit, count, order, options);
        }

        /// <summary>
        /// 返回 给定实例 和半径内的所有 ShipId
        /// </summary>
        /// <param name="member"></param>
        /// <param name="radius"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public async Task<GeoRadiusResult[]> GetByRediusAsync (TKey member, double radius, GeoUnit unit = GeoUnit.Meters) {
            return await Database.GeoRadiusAsync (SetKey, Unbox (member), radius, unit);
        }

        public async Task<GeoRadiusResult[]> GetByRediusAsync (double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = Order.Ascending, GeoRadiusOptions options = GeoRadiusOptions.Default) {
            return await Database.GeoRadiusAsync (SetKey, longitude, latitude, radius, unit, count, order, options);
        }
    }

    /// <summary>
    /// 可更新的 Redis GeoHash Set
    /// </summary>
    public class GeoHashSet<TKey> : ReadOnlyGeoHashSet<TKey>, IAsyncGeoHashSet<TKey>,IBatchGeoHashSet<TKey> where TKey:IConvertible {

        public GeoHashSet (IDatabase database, string setKey) : base (database, setKey) { }

        /// <summary>
        /// Add the specified member to the set stored at key. 
        /// Specified members that are already a member of this set are ignored. 
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="entry">The geo value to store.</param>
        /// <returns></returns>
        public bool Add (GeoEntry entry) {
            return Database.GeoAdd (SetKey, entry);
        }

        /// <summary>
        /// 增加 一个 位置索引
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Add (double lng, double lat, TKey member) {
            return Add (new GeoEntry (lng, lat, Unbox(member)));
        }

        /// <summary>
        /// 批量增加
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public long AddRange (IEnumerable<GeoEntry> entries) {
            return Database.GeoAdd (SetKey, entries.ToArray ());
        }

        /// <summary>
        /// 从 GeoHash set 中移除一个位置
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Remove (TKey member) {
            return Database.GeoRemove (SetKey, Unbox(member));
        }

        public bool Clear () {
            return base.Delete();
        }

        #region  Async Methods

        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are
        /// already a member of this set are ignored. If key does not exist, a new set is
        /// created before adding the specified members.
        /// </summary>
        /// <param name="entry">The geo value to store.</param>
        /// <returns></returns>
        async Task<bool> IAsyncGeoHashSet<TKey>.AddAsync (GeoEntry entry) {
            return await Database.GeoAddAsync (SetKey, entry);
        }

        /// <summary>
        /// 异步批量增加
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        async Task<long> IAsyncGeoHashSet<TKey>.AddRangeAsync (IEnumerable<GeoEntry> entries) {
            return await Database.GeoAddAsync (SetKey, entries.ToArray ());
        }

        async Task<bool> IAsyncGeoHashSet<TKey>.RemoveAsync (TKey member) {
            return await Database.GeoRemoveAsync (SetKey, Unbox(member));
        }
        #endregion

        #region Batch Method

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task IBatchGeoHashSet<TKey>.BatchRemove (IBatch batch, TKey member) {
            return batch.GeoRemoveAsync (SetKey, Unbox(member));
        }

        /// <summary>
        /// 增加 GeoHash 批处理
        /// </summary>
        /// <example>
        /// var batch = database.CreateBatch();
        /// set.BatchAdd(batch,entry);
        /// batch.Execute();
        /// </example>
        /// <param name="batch"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        Task IBatchGeoHashSet<TKey>.BatchAdd (IBatch batch, GeoEntry entry) {
            return batch.GeoAddAsync (SetKey, entry);
        }

        /// <summary>
        /// 增加 GeoHash 批处理
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task IBatchGeoHashSet<TKey>.BatchAdd (IBatch batch, double lng, double lat, TKey member) {
            return ((IBatchGeoHashSet) this).BatchAdd (batch, new GeoEntry (lng, lat, Unbox(member)));
        }

        #endregion

        /// <summary>
        /// 获取 <see cref="IBatchGeoHashSet">批处理接口</see>
        /// </summary>
        /// <returns></returns>
        public IBatchGeoHashSet<TKey> AsBatch () {
            return this;
        }

        /// <summary>
        /// 获取 <see cref="IAsyncGeoHashSet">异步接口</see>
        /// </summary>
        /// <returns></returns>
        public IAsyncGeoHashSet<TKey> AsAsync(){
            return this;
        }
    }
}