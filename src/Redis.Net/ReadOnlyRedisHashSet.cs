using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis HashSet Warpper
    /// </summary>
    public class ReadOnlyRedisHashSet : AbstracRedisKey, IReadOnlyDictionary<RedisValue, RedisValue> {
        public ReadOnlyRedisHashSet (IDatabase database, string setKey) : base (database, setKey, RedisType.Hash) { }

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool ContainsKey (RedisValue key) {
            return Database.HashExists (SetKey, key);
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"></see> interface contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool TryGetValue (RedisValue key, out RedisValue value) {
            var result = Database.HashGet (SetKey, key);
            value = result;
            return result.HasValue;
        }

        public RedisValue GetValue (RedisValue hashField) {
            return Database.HashGet (SetKey, hashField);
        }

        public RedisValue[] GetValues (params RedisValue[] hashField) {
            return Database.HashGet (SetKey, hashField);
        }

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key">key</paramref> is not found.</exception>
        public RedisValue this [RedisValue key] {
            get {
                if (TryGetValue (key, out var value)) {
                    return value;
                }
                return RedisValue.Null;
            }
            set => Database.HashSet (SetKey, key, value);
        }

        public IEnumerable<RedisValue> Keys => Database.HashKeys (SetKey);

        /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</returns>
        public IEnumerable<RedisValue> Values => Database.HashValues (SetKey);

        ///<summary>
        /// 当前集合数量
        ///</summary>    
        public int Count => (int) Database.HashLength (SetKey);

        public long LongCount => Database.HashLength (SetKey);

        public IEnumerable<HashEntry> Scan (RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.HashScan (SetKey, pattern, pageSize, cursor, pageOffset);
        }

        #region  Async Methods

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public async Task<bool> ContainsKeyAsync (RedisValue key) {
            return await Database.HashExistsAsync (SetKey, key);
        }

        public async Task<RedisValue> GetValueAsync (RedisValue key) {
            return await Database.HashGetAsync (SetKey, key);
        }

        public async Task<RedisValue[]> GetValuesAsync (params RedisValue[] keys) {
            return await Database.HashGetAsync (SetKey, keys);
        }

        public async Task<RedisValue[]> GetKeysAsync () {
            return await Database.HashKeysAsync (SetKey);
        }

        ///<summary>
        /// 获取全部数值
        ///</summary>
        public async Task<RedisValue[]> ValuesAsync () {
            return await Database.HashValuesAsync (SetKey);
        }

        public async Task<long> CountAsync () {
            return await Database.HashLengthAsync (SetKey);
        }
        #endregion

        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<RedisValue, RedisValue>> GetEnumerator () {
            var entities = Database.HashGetAll (SetKey);

            foreach (var entry in entities) {
                yield return new KeyValuePair<RedisValue, RedisValue> (entry.Name, entry.Value);
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        #endregion
    }

}