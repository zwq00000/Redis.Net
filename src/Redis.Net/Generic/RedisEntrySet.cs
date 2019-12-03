using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis HashSet Warpper
    /// </summary>
    public class RedisEntrySet<TKey, TValue> : ReadOnlyEntrySet<TKey, TValue>, IDictionary<TKey, TValue>
        where TKey : IConvertible where TValue : new() {

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="database">Redis Database</param>
        /// <param name="baseKey">Redis Key Name</param>
        public RedisEntrySet(IDatabase database, string baseKey) : base(database, baseKey) {
        }

        #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add(KeyValuePair<TKey, TValue> item) {
            var setKey = GetEntryKey(item.Key);
            Database.HashSet(setKey, item.Value.ToHashEntries().ToArray());
            OnAdded(item.Key);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Clear() {
            var keys = Keys.ToArray();
            foreach (var key in keys) {
                Remove(key);
            }
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>true if <paramref name="item">item</paramref> is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return ContainsKey(item.Key);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array">array</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex">arrayIndex</paramref> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from <paramref name="arrayIndex">arrayIndex</paramref> to the end of the destination <paramref name="array">array</paramref>.</exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            using (IEnumerator<TKey> enumertor = Keys.GetEnumerator()) {
                for (int i = arrayIndex; i < array.Length; i++) {
                    if (enumertor.MoveNext()) {
                        var key = enumertor.Current;
                        array[i] = new KeyValuePair<TKey, TValue>(key, this[key]);
                    } else {
                        return;
                    }
                }
            }
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>true if <paramref name="item">item</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item) {
            return Remove(item.Key);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly => false;

        #endregion

        #region Implementation of IDictionary<TKey,TValue>

        /// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"></see> is read-only.</exception>
        public void Add(TKey key, TValue value) {
            var setKey = GetEntryKey(key);
            Database.HashSet(setKey, value.ToHashEntries().ToArray());
            OnAdded(key);
        }

        /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</returns>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToArray();

        #endregion

        #region Batch


        /// <summary>
        /// 增加实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public Task AddAsync(IBatch batch, TKey key, IEnumerable<HashEntry> entries) {
            return base.AddHashSetBatch(batch,key,entries);
        }

        /// <summary>
        /// 增加实体集合的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task AddAsync(IBatch batch, TKey key, TValue value) {
            return base.AddHashSetBatch(batch,key,value.ToHashEntries());
        }

        /// <summary>
        /// 删除实体集合的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> RemoveAsync(IBatch batch, TKey key) {
            return RemoveBatch(batch,key);
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
        public Task<bool> ExpireAsync(IBatch batch, TKey key,TimeSpan? expiry) {
            return ExpireBatch(batch,key,expiry);
        }

        /// <summary>
        /// 清理全部集合的批量方法
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public Task<long> ClearAsync(IBatch batch) {
            var keys = Keys.ToArray();
            return RemoveBatch(batch,keys);
        }

        #endregion
    }
}