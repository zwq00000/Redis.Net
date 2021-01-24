using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// /// 基于 redis Hashset 的 只读泛型字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadonlyRedisDictionary<TKey, TValue> : AbstracRedisKey, IReadOnlyDictionary<TKey, TValue>, IEnumerable
    where TKey : IConvertible {
        public ReadonlyRedisDictionary (IDatabase database, RedisKey setKey) : base (database, setKey) { }

        private static readonly Type KeyType = typeof (TKey);
        private static readonly Type ValueType = typeof (TValue);
        private readonly Func<RedisValue, TValue> _valueConvert;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="database">Redis Database</param>
        /// <param name="setKey">Redis Key Name</param>
        /// <param name="serializer">Value Converter</param>
        public ReadonlyRedisDictionary (IDatabase database, string setKey, ISerializer serializer) : base (database, setKey, RedisType.Hash) {
            if (serializer == null) {
                throw new ArgumentNullException (nameof (serializer));
            }

            this._valueConvert =serializer.Deserialize<TValue>;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="database">Redis Database</param>
        /// <param name="setKey">Redis Key Name</param>
        /// <param name="valueConvert">Value Converter</param>
        public ReadonlyRedisDictionary (IDatabase database, string setKey, Func<RedisValue, TValue> valueConvert) : base (database, setKey, RedisType.Hash) {
            if (valueConvert == null) {
                throw new ArgumentNullException (nameof (valueConvert));
            }

            this._valueConvert = valueConvert;
        }

        protected TKey ConvertKey (RedisValue key) {
            return (TKey) ((IConvertible) key).ToType (KeyType, CultureInfo.CurrentCulture);
        }

        protected TValue ConvertValue (RedisValue value) {
            return _valueConvert (value);
        }

        /// <summary>
        /// 根据键值批量返回数据
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetValues (params TKey[] keys) {
            if (keys == null || keys.Length == 0) {
                return new TValue[0];
            }
            return Database.HashGet (SetKey, keys.Select (k => RedisValue.Unbox (k)).ToArray ())
                .Select (v => v.HasValue ? ConvertValue (v) : default (TValue));
        }

        /// <summary>
        /// 根据键值批量返回数据的异步方法
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TValue>> GetValuesAsync (params TKey[] keys) {
            if (keys == null || keys.Length == 0) {
                return new TValue[0];
            }
            var result = await Database.HashGetAsync (SetKey, keys.Select (k => RedisValue.Unbox (k)).ToArray ());
            return result.Select (v => v.HasValue ? ConvertValue (v) : default (TValue));
        }

        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator () {
            var entities = Database.HashGetAll (SetKey);

            foreach (var entry in entities) {
                yield return new KeyValuePair<TKey, TValue> (ConvertKey (entry.Name), ConvertValue (entry.Value));
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out KeyValuePair<TKey,TValue>>

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public int Count => (int) Database.HashLength (SetKey);

        #endregion

        #region Implementation of IReadOnlyDictionary<TKey,TValue>

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool ContainsKey (TKey key) {
            return Database.HashExists (SetKey, RedisValue.Unbox (key));
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"></see> interface contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool TryGetValue (TKey key, out TValue value) {
            var result = Database.HashGet (SetKey, RedisValue.Unbox (key));
            if (result.HasValue) {
                value = ConvertValue (result);
                return true;
            }

            value = default (TValue);
            return false;
        }

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key">key</paramref> is not found.</exception>
        public TValue this [TKey key] {
            get {
                if (TryGetValue (key, out var value)) {
                    return value;
                }
                return default (TValue);
            }
            set => Database.HashSet (SetKey, RedisValue.Unbox (key), RedisValue.Unbox (value));
        }

        /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        public IEnumerable<TKey> Keys => Database.HashKeys (SetKey).Select (ConvertKey);

        /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        public IEnumerable<TValue> Values => Database.HashValues (SetKey).Select (ConvertValue);

        #endregion
    }

}