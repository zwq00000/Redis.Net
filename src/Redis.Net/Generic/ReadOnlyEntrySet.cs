using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Redis.Net.Redis.Generic;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// readonly Redis HashSet for Entity Warpper
    /// </summary>
    public class ReadOnlyEntrySet<TKey, TEntity> : RedisMutiKey<TKey>, IReadOnlyDictionary<TKey, TEntity>
        where TKey : IConvertible where TEntity : new () {

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="database">Redis Database</param>
            /// <param name="baseKey">Redis Key Name</param>
            public ReadOnlyEntrySet (IDatabase database, string baseKey) : base (database, baseKey) { }

            #region Implementation of IEnumerable

            /// <summary>Returns an enumerator that iterates through the collection.</summary>
            /// <returns>An enumerator that can be used to iterate through the collection.</returns>
            public IEnumerator<KeyValuePair<TKey, TEntity>> GetEnumerator () {
                foreach (var key in Keys) {
                    var setKey = GetEntryKey (key);
                    var value = Database.HashGetEntity<TEntity> (setKey);
                    yield return new KeyValuePair<TKey, TEntity> (key, value);
                }
            }

            /// <summary>Returns an enumerator that iterates through a collection.</summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
            IEnumerator IEnumerable.GetEnumerator () {
                return GetEnumerator ();
            }

            #endregion

            #region Async Methods
            /// <summary>Async Gets the element that has the specified key in the read-only dictionary.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The element that has the specified key in the read-only dictionary.</returns>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
            /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key">key</paramref> is not found.</exception>
            public async Task<TEntity> GetValueAsync (TKey key) {
                var setKey = GetEntryKey (key);
                return await Database.HashGetEntityAsync<TEntity> (setKey);
            }

            #endregion

            #region Implementation of IReadOnlyDictionary<TKey,TValue>

            /// <summary>Gets the value that is associated with the specified key.</summary>
            /// <param name="key">The key to locate.</param>
            /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
            /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"></see> interface contains an element that has the specified key; otherwise, false.</returns>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
            public bool TryGetValue (TKey key, out TEntity value) {

                if (base.ContainsKey (key)) {
                    var setKey = GetEntryKey (key);
                    value = Database.HashGetEntity<TEntity> (setKey);
                    return true;
                }
                value = default (TEntity);
                return false;
            }

            /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>The element that has the specified key in the read-only dictionary.</returns>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
            /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key">key</paramref> is not found.</exception>
            public TEntity this [TKey key] {
                get {
                    if (TryGetValue (key, out var value)) {
                        return value;
                    }
                    return default (TEntity);
                }
                set => Database.HashSet (GetEntryKey (key), value.ToHashEntries ().ToArray ());
            }

            /// <summary>Gets an enumerable collection that contains the keys in the read-only dictionary.</summary>
            /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
            IEnumerable<TKey> IReadOnlyDictionary<TKey, TEntity>.Keys => base.Keys;

            /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
            /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
            public IEnumerable<TEntity> Values {
                get {
                    foreach (var key in Keys) {
                        var setKey = GetEntryKey (key);
                        yield return Database.HashGetEntity<TEntity> (setKey);
                    }
                }
            }

            #endregion

            #region Property

            /// <summary>
            /// 获取特定的属性
            /// </summary>
            /// <typeparam name="TField"></typeparam>
            /// <param name="key"></param>
            /// <param name="propertyAccess"></param>
            /// <returns></returns>
            public TField GetProperty<TField> (TKey key, Expression<Func<TEntity, TField>> propertyAccess) {
                var field = propertyAccess.MemberName ();
                var setKey = GetEntryKey (key);
                var result = Database.HashGet (setKey, field);
                if (result.HasValue) {
                    return (TField) ((IConvertible) result).ToType (typeof (TField), CultureInfo.InvariantCulture);
                } else {
                    return default (TField);
                }
            }

            /// <summary>
            /// 获取特定的属性
            /// </summary>
            /// <typeparam name="TField"></typeparam>
            /// <param name="key"></param>
            /// <param name="propertyAccess"></param>
            /// <returns></returns>
            public async Task<TField> GetPropertyAsync<TField> (TKey key, Expression<Func<TEntity, TField>> propertyAccess) {
                var field = propertyAccess.MemberName ();
                var setKey = GetEntryKey (key);
                var result = await Database.HashGetAsync (setKey, field);
                if (result.HasValue) {
                    return (TField) ((IConvertible) result).ToType (typeof (TField), CultureInfo.InvariantCulture);
                } else {
                    return default (TField);
                }
            }

            /// <summary>
            /// 获取特定的属性
            /// </summary>
            /// <typeparam name="TField"></typeparam>
            /// <param name="key"></param>
            /// <param name="propertyAccess"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool TryGetProperty<TField> (TKey key, Expression<Func<TEntity, TField>> propertyAccess, out TField value) {
                var field = propertyAccess.MemberName ();
                var setKey = GetEntryKey (key);
                var result = Database.HashGet (setKey, field);
                if (result.HasValue) {
                    value = (TField) ((IConvertible) result).ToType (typeof (TField), CultureInfo.InvariantCulture);
                    return true;
                } else {
                    value = default (TField);
                    return false;
                }
            }

            /// <summary>
            /// 获取泛型属性集合
            /// </summary>
            /// <typeparam name="TV"></typeparam>
            /// <param name="key"></param>
            /// <returns></returns>
            public RedisHashSet<TKey, TV> GetPropertySet<TV> (TKey key) where TV : IConvertible {
                var setKey = GetEntryKey (key);
                return new RedisHashSet<TKey, TV> (Database, setKey);
            }

            /// <summary>
            /// 获取非泛型的属性集合
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public RedisHashSet GetPropertySet (TKey key) {
                var setKey = GetEntryKey (key);
                return new RedisHashSet (Database, setKey);
            }
            #endregion
        }
}