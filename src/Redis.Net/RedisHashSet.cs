using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    public class RedisHashSet : ReadOnlyRedisHashSet, IHashSet<RedisValue, RedisValue> {

        public RedisHashSet (IDatabase database, RedisKey setKey) : base (database, setKey) { }

        protected virtual HashEntry ToEntry (RedisValue key, RedisValue value) {
            return new HashEntry (key, value);
        }

        #region Implementation of IRedisHash<TKey,TValue>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Add (RedisValue key, RedisValue value) {
            Database.HashSet (SetKey, new [] { ToEntry (key, value) });
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Add (params HashEntry[] entries) {
            if (entries == null || entries.Length == 0) {
                return;
            }
            Database.HashSet (SetKey, entries);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public async Task AddAsync (RedisValue key, RedisValue value) {
            await AddAsync (ToEntry (key, value));
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public async Task AddAsync (params HashEntry[] entries) {
            if (entries == null || entries.Length == 0) {
                return;
            }
            await Database.HashSetAsync (SetKey, entries);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Clear () {
            Database.KeyDelete (SetKey);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
        /// <param name="key">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>true if <paramref name="key">key</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; 
        /// otherwise, false. This method also returns false if <paramref name="key">key</paramref> is not found in the original 
        /// <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove (RedisValue key) {
            return Database.HashDelete (SetKey, key);
        }

        /// <summary>
        /// 删除键值 的异步方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync (RedisValue key) {
            return await Database.HashDeleteAsync (SetKey, key);
        }

        #endregion

        #region  Increment/Decrement

        /// <summary>
        /// 将存储在键上的哈希的指定字段递减，并用指定的递减量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Decrement (RedisValue hashField, double value) {
            return Database.HashDecrement (SetKey, hashField, value);
        }

        /// <summary>
        /// 将存储在键上的哈希的指定字段递增，并用指定的增量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Increment (RedisValue hashField, double value) {
            return Database.HashIncrement (SetKey, hashField, value);
        }

        public void Add (params Tuple<RedisValue, RedisValue>[] tuples) {
            if (tuples == null || tuples.Length == 0) {
                return;
            }
            var entities = tuples.Select (t => new HashEntry (t.Item1, t.Item2))
                .ToArray ();
            Database.HashSet (SetKey, entities);
        }

        public void Add (params KeyValuePair<RedisValue, RedisValue>[] pairs) {
            if (pairs == null || pairs.Length == 0) {
                return;
            }
            var entities = pairs.Select (t => new HashEntry (t.Key, t.Value))
                .ToArray ();
            Database.HashSet (SetKey, entities);
        }

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating
        ///     point number, by the specified decrement. If the field does not exist, it is
        ///     set to 0 before performing the operation.
        /// </summary>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Decrement (RedisValue hashField, long value = 1) {
            return Database.HashDecrement (this.SetKey, RedisValue.Unbox (hashField), value);
        }

        #endregion
    }

}