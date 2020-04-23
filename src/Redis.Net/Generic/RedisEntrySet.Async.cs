using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic
{
    public partial class RedisEntrySet<TKey, TValue> {
        #region Async Methods

        /// <summary>
        /// 增加实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        async Task IAsyncEntrySet<TKey, TValue>.AddAsync (TKey key, IEnumerable<HashEntry> entries) {
            var setKey = GetEntryKey (key);
            await Database.HashSetAsync (setKey, entries.ToArray ());
            await AddKeyIndexAsync (key);
        }

        /// <summary>
        /// 更新实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        async Task IAsyncEntrySet<TKey, TValue>.UpdateAsync (TKey key, IEnumerable<HashEntry> entries) {
            var setKey = GetEntryKey (key);
            await Database.HashSetAsync (setKey, entries.ToArray ());
        }

        async Task IAsyncEntrySet<TKey, TValue>.AddAsync (TKey key, TValue value) {
            var setKey = GetEntryKey (key);
            await Database.HashSetAsync (setKey, value.ToHashEntries ().ToArray ());
            await AddKeyIndexAsync (key);
        }

        async Task<bool> IAsyncEntrySet<TKey, TValue>.RemoveAsync (TKey key) {
            return await base.RemoveKeyAsync (key);
        }

        async Task<bool> IAsyncEntrySet<TKey, TValue>.ExpireAsync (TKey key, TimeSpan? expiry) {
            return await base.SetExpireAsync (key, expiry);
        }

        /// <summary>
        /// 清理全部集合的异步方法
        /// </summary>
        /// <returns></returns>
        async Task IAsyncEntrySet<TKey, TValue>.ClearAsync () {
            var keys = Keys.ToArray ();
            await RemoveKeyAsync (keys);
        }

        #endregion

    }
}