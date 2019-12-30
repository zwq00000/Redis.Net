using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Reids HashSet
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class RedisHashSet<TKey, TValue> : ReadOnlyRedisHashSet<TKey, TValue> where TKey : IConvertible where TValue : IConvertible {

        public RedisHashSet (IDatabase database, RedisKey setKey) : base (database, setKey) { }

        #region Implementation of IRedisHash<TKey,TValue>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Add (TKey key, TValue value) {
            Database.HashSet (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Add (params Tuple<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return;
            }
            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), RedisValue.Unbox ((t.Item2))))
                .ToArray ();
            Database.HashSet (SetKey, entities);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Add (params KeyValuePair<TKey, TValue>[] pairs) {
            if (pairs == null || pairs.Length == 0) {
                return;
            }
            var entities = pairs.Select (t => new HashEntry (RedisValue.Unbox (t.Key), RedisValue.Unbox ((t.Value))))
                .ToArray ();
            Database.HashSet (SetKey, entities);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public async Task AddAsync (TKey key, TValue value) {
            await Database.HashSetAsync (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public async Task AddAsync (params Tuple<TKey, TValue>[] tuples) {
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
        public async Task AddAsync (params KeyValuePair<TKey, TValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return;
            }

            var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Key), RedisValue.Unbox ((t.Value))))
                .ToArray ();
            await Database.HashSetAsync (SetKey, entities);
        }

        /// <summary>
        /// 根据 key 删除Hash条目
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove (TKey key) {
            return Database.HashDelete (SetKey, RedisValue.Unbox (key));
        }

        /// <summary>
        /// 删除键值 的异步方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync (TKey key) {
            return await Database.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
        }

        #endregion

        #region  Increment/Decrement

        /// <summary>
        /// 将存储在键上的哈希的指定字段递减，并用指定的递减量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Decrement (TKey hashField, long value = 1) {
            return Database.HashDecrement (this.SetKey, RedisValue.Unbox (hashField), value);
        }

        /// <summary>
        /// 将存储在键上的哈希的指定字段递减，并用指定的递减量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Decrement (TKey hashField, double value) {
            return Database.HashDecrement (this.SetKey, RedisValue.Unbox (hashField), value);
        }

        /// <summary>
        /// 将存储在键上的哈希的指定字段递增，并用指定的增量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Increment (TKey hashField, double value) {
            return Database.HashIncrement (this.SetKey, RedisValue.Unbox (hashField), value);
        }

        #endregion

        #region Batch

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public Task AddAsync (IBatch batch, TKey key, TValue value) {
            return batch.HashSetAsync (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public Task AddAsync (IBatch batch, params Tuple<TKey, TValue>[] tuples) {
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
        public Task AddAsync (IBatch batch, params KeyValuePair<TKey, TValue>[] tuples) {
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
        public Task<bool> RemoveAsync (IBatch batch, TKey key) {
            return batch.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
        }

        #endregion
    }
}