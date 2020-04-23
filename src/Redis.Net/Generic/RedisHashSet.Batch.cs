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
        #region Batch

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, TKey key, TValue value) {
            return batch.HashSetAsync (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, params Tuple<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return Task.CompletedTask;
            }

            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), RedisValue.Unbox ((t.Item2))))
                .ToArray ();
            return batch.HashSetAsync (SetKey, entities);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, params KeyValuePair<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return Task.CompletedTask;
            }

            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Key), RedisValue.Unbox ((t.Value))))
                .ToArray ();
            return batch.HashSetAsync (SetKey, entities);
        }

        /// <summary>
        /// 删除键值 的异步方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> IBatchHashSet<TKey, TValue>.BatchRemove (IBatch batch, TKey key) {
            return batch.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
        }

        #endregion
    }
}