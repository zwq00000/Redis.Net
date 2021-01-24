using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Reids HashSet
    /// </summary>
    public partial class RedisHashSet<TKey, TValue> {
        #region Implementation IHashSetAsync

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        async Task IAsyncHashSet<TKey, TValue>.AddAsync (TKey key, TValue value) {
            await Database.HashSetAsync (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        async Task IAsyncHashSet<TKey, TValue>.AddAsync (params Tuple<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return;
            }

            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), RedisValue.Unbox ((t.Item2))))
                .ToArray ();
            await Database.HashSetAsync (SetKey, entities);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        async Task IAsyncHashSet<TKey, TValue>.AddAsync (params KeyValuePair<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return;
            }

            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Key), RedisValue.Unbox ((t.Value))))
                .ToArray ();
            await Database.HashSetAsync (SetKey, entities);
        }

        /// <summary>
        /// 删除键值 的异步方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        async Task<bool> IAsyncHashSet<TKey, TValue>.RemoveAsync (TKey key) {
            return await Database.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
        }

        // /// <summary>
        // /// 指定字段递减的异步方法
        // /// </summary>
        // /// <param name="hashField"></param>
        // /// <param name="value"></param>
        // /// <returns></returns>
        // async Task<long> IAsyncHashSet<TKey, TValue>.DecrementAsync (TKey hashField, long value) {
        //     return await Database.HashDecrementAsync (this.SetKey, RedisValue.Unbox (hashField), value);
        // }

        // /// <summary>
        // /// 指定字段递减的异步方法
        // /// </summary>
        // /// <param name="hashField"></param>
        // /// <param name="value"></param>
        // /// <returns></returns>
        // async Task<double> IAsyncHashSet<TKey, TValue>.DecrementAsync (TKey hashField, double value) {
        //     return await Database.HashDecrementAsync (this.SetKey, RedisValue.Unbox (hashField), value);
        // }

        // /// <summary>
        // /// 指定字段递增的异步方法
        // /// </summary>
        // /// <param name="hashField"></param>
        // /// <param name="value"></param>
        // /// <returns></returns>
        // async Task<double> IAsyncHashSet<TKey, TValue>.IncrementAsync (TKey hashField, double value) {
        //     return await Database.HashIncrementAsync (this.SetKey, RedisValue.Unbox (hashField), value);
        // }

        #endregion
    }
}