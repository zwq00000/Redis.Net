using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis HashSet Warpper
    /// </summary>
    public partial class RedisEntrySet<TKey, TValue> : ReadOnlyEntrySet<TKey, TValue>, IEntrySet<TKey, TValue>,
        IAsyncEntrySet<TKey, TValue>, IBatchEntrySet<TKey, TValue>
        where TKey : IConvertible where TValue : new () {
            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="database">Redis Database</param>
            /// <param name="baseKey">Redis Key Name</param>
            public RedisEntrySet (IDatabase database, string baseKey) : base (database, baseKey) { }

            #region Implementation of ICollection<KeyValuePair<TKey,TValue>>

            /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            public void Add (KeyValuePair<TKey, TValue> item) {
                var setKey = GetEntryKey (item.Key);
                Database.HashSet (setKey, item.Value.ToHashEntries ().ToArray ());
                AddKeyIndex (item.Key);
            }

            /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            public void Clear () {
                var keys = Keys.ToArray ();
                foreach (var key in keys) {
                    RemoveKey (key);
                }
            }

            /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.</summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <returns>true if <paramref name="item">item</paramref> is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.</returns>
            public bool Contains (KeyValuePair<TKey, TValue> item) {
                return ContainsKey (item.Key);
            }

            /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
            /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="array">array</paramref> is null.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex">arrayIndex</paramref> is less than 0.</exception>
            /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from <paramref name="arrayIndex">arrayIndex</paramref> to the end of the destination <paramref name="array">array</paramref>.</exception>
            public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
                using (IEnumerator<TKey> enumertor = Keys.GetEnumerator ()) {
                    for (int i = arrayIndex; i < array.Length; i++) {
                        if (enumertor.MoveNext ()) {
                            var key = enumertor.Current;
                            array[i] = new KeyValuePair<TKey, TValue> (key, this [key]);
                        } else {
                            return;
                        }
                    }
                }
            }

            public bool Remove (TKey key) {
                return base.RemoveKey (key);
            }

            /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <returns>true if <paramref name="item">item</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            public bool Remove (KeyValuePair<TKey, TValue> item) {
                return RemoveKey (item.Key);
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
            public void Add (TKey key, TValue value) {
                var setKey = GetEntryKey (key);
                Database.HashSet (setKey, value.ToHashEntries ().ToArray ());
                AddKeyIndex (key);
            }

            /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</summary>
            /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</returns>
            ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToArray ();

            #endregion

            #region Implementation IEntrySet
            public void Add (TKey key, IEnumerable<HashEntry> entries) {
                var setKey = GetEntryKey (key);
                Database.HashSet (setKey, entries.ToArray ());
                AddKeyIndex (key);
            }

            public void Update (TKey key, IEnumerable<HashEntry> entries) {
                var setKey = GetEntryKey (key);
                Database.HashSet (setKey, entries.ToArray ());
                AddKeyIndex (key);
            }

            public void Expire (TKey key, TimeSpan? expiry) {
                base.SetExpire (key, expiry);
            }

            #endregion

            /// <summary>
            /// careate batch methods instance
            /// </summary>
            /// <returns></returns>
            public IBatchEntrySet<TKey, TValue> AsBatch () {
                return (IBatchEntrySet<TKey, TValue>) this;
            }

            public IAsyncEntrySet<TKey, TValue> AsAsync () {
                return (IAsyncEntrySet<TKey, TValue>) this;
            }
        }
}