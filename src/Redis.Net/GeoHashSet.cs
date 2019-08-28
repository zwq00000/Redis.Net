using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// 可更新的 Redis GeoHash Set
    /// </summary>
    public class GeoHashSet : ReadOnlyGeoHashSet {

        public GeoHashSet(IDatabase database, string setKey) : base(database, setKey) {
        }

        /// <summary>
        /// Add the specified member to the set stored at key. 
        /// Specified members that are already a member of this set are ignored. 
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="entry">The geo value to store.</param>
        /// <returns></returns>
        public bool Add(GeoEntry entry) {
            return Database.GeoAdd(SetKey, entry);
        }

        /// <summary>
        /// Add the specified member to the set stored at key. 
        /// Specified members that are already a member of this set are ignored. 
        /// If key does not exist, a new set is created before adding the specified members.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(GeoEntry entry) {
            return await Database.GeoAddAsync(SetKey, entry);
        }

        /// <summary>
        /// 增加 一个 位置索引
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Add(double lng, double lat, RedisValue member) {
            return Add(new GeoEntry(lng, lat, member));
        }

        /// <summary>
        /// 批量增加
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public long AddRange(IEnumerable<GeoEntry> entries) {
            return Database.GeoAdd(SetKey, entries.ToArray());
        }

        /// <summary>
        /// 异步批量增加
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public async Task<long> AddRangeAsync(IEnumerable<GeoEntry> entries) {
            return await Database.GeoAddAsync(SetKey, entries.ToArray());
        }

        public async Task<bool> AddAsync(double lng, double lat, RedisValue member) {
            return await this.AddAsync(new GeoEntry(lng, lat, member));
        }

        /// <summary>
        /// 从 GeoHash set 中移除一个位置
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Remove(RedisValue member) {
            return Database.GeoRemove(SetKey, member);
        }

        public async Task<bool> RemoveAsync(RedisValue member) {
            return await Database.GeoRemoveAsync(SetKey, member);
        }

        public bool Clear() {
            return Database.KeyDelete(SetKey);
        }

        #region Batch Method

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="shipId"></param>
        /// <returns></returns>
        public Task RemoveAsync(IBatch batch, string shipId) {
            return batch.GeoRemoveAsync(SetKey, shipId);
        }


        /// <summary>
        /// 增加 GeoHash 批处理
        /// </summary>
        /// <example>
        /// var batch = database.CreateBatch();
        /// set.AddAsync(batch,entry);
        /// batch.Execute();
        /// </example>
        /// <param name="batch"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Task AddAsync(IBatch batch, GeoEntry entry) {
            return batch.GeoAddAsync(SetKey,entry);
        }

        /// <summary>
        /// 增加 GeoHash 批处理
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public Task AddAsync(IBatch batch,double lng, double lat, RedisValue member) {
            return AddAsync(batch, new GeoEntry(lng, lat, member));
        }

        #endregion
    }
}