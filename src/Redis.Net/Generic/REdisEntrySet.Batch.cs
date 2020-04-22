using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic
{
    public partial class RedisEntrySet<TKey, TValue> {
        #region Batch Methods

        /// <summary>
        /// 增加实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        Task IBatchEntrySet<TKey, TValue>.BatchAdd (IBatch batch, TKey key, IEnumerable<HashEntry> entries) {
            return base.AddHashSetBatch (batch, key, entries);
        }

        /// <summary>
        /// 更新实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        Task IBatchEntrySet<TKey, TValue>.BatchUpdate (IBatch batch, TKey key, IEnumerable<HashEntry> entries) {
            return base.UpdateHashSetBatch (batch, key, entries);
        }

        /// <summary>
        /// 增加实体集合的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task IBatchEntrySet<TKey, TValue>.BatchAdd (IBatch batch, TKey key, TValue value) {
            return base.AddHashSetBatch (batch, key, value.ToHashEntries ());
        }

        /// <summary>
        /// 删除实体集合的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> IBatchEntrySet<TKey, TValue>.BatchRemove (IBatch batch, TKey key) {
            return RemoveBatch (batch, key);
        }

        /// <summary>
        /// 设置实体集合超时回收时间 的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key">The key to set the expiration for.</param>
        /// <param name="expiry">The exact date to expiry to set.</param>
        /// <remarks>
        /// Set a timeout on key. After the timeout has expired, the key will automatically
        ///     be deleted. A key with an associated timeout is said to be volatile in Redis
        ///     terminology.
        ///</remarks>
        ///  <returns></returns>
        Task<bool> IBatchEntrySet<TKey, TValue>.BatchExpire (IBatch batch, TKey key, TimeSpan? expiry) {
            return ExpireBatch (batch, key, expiry);
        }

        /// <summary>
        /// 清理全部集合的批量方法
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        Task<long> IBatchEntrySet<TKey, TValue>.BatchClear (IBatch batch) {
            var keys = Keys.ToArray ();
            return RemoveBatch (batch, keys);
        }

        #endregion

    }
}