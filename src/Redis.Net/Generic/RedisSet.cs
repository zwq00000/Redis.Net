using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis Set 集合
    /// </summary>
    public class RedisSet<TValue> : ReadOnlyRedisSet<TValue>, ICollection<TValue> where TValue : IConvertible {
        public RedisSet (IDatabase database, string setKey) : base (database, setKey) { }

        /// <inheritdoc />
        public void Clear () {
            Database.KeyDelete(SetKey);
        }

        /// <summary>
        /// 异步向集合中增加元素的
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> AddAsync(params TValue[] value) {
            return await Database.SetAddAsync(SetKey,Array.ConvertAll(value,v=>RedisValue.Unbox(v)));
        }

        /// <inheritdoc />
        public void Add(TValue item) {
            Database.SetAdd(SetKey, RedisValue.Unbox (item));
        }

        /// <summary>
        /// 向集合中增加元素
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public long AddRange(TValue[] items) {
            RedisValue[] values = Array.ConvertAll(items,Unbox);
            return Database.SetAdd (SetKey, values);
        }

        /// <inheritdoc />
        public bool Remove (TValue value) {
            return Database.SetRemove (SetKey, RedisValue.Unbox (value));
        }

        public long RemoveRange(TValue[] items) {
            RedisValue[] values = Array.ConvertAll(items, Unbox);
            return Database.SetRemove(SetKey, values);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly {
            get => false;
        }

        public async Task<long> RemoveAsync (params TValue[] items) {
            if (items.Length == 1) {
                return await Database.SetRemoveAsync (SetKey, RedisValue.Unbox (items[0])) ? 1 : 0;
            }
            RedisValue[] values = Array.ConvertAll(items, Unbox);
            return await Database.SetRemoveAsync (SetKey, values);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array">array</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex">arrayIndex</paramref> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from <paramref name="arrayIndex">arrayIndex</paramref> to the end of the destination <paramref name="array">array</paramref>.</exception>

        public void CopyTo (TValue[] array, int arrayIndex) {
            var size = array.Length;
            var values = Database.SetMembers(SetKey);
            for (int i = arrayIndex ; i < size; i++) {
                if (i < values.Length) {
                    array[i] = ConvertValue(values[i]);
                }
            }
        }

        #region Batch

        public Task<long> AddAsync(IBatch batch, params TValue[] values) {
            return batch.SetAddAsync(this.SetKey,Array.ConvertAll(values,v=>RedisValue.Unbox(v)));
        }

        /// <summary>
        /// Remove the specified members from the set stored at key. Specified members that are not a member of this set are ignored.
        /// </summary>
        /// <param name="batch"><see cref="IBatch"/></param>
        /// <param name="values">The values to remove.</param>
        /// <returns>The number of members that were removed from the set, not including non existing members.</returns>
        /// <remarks>https://redis.io/commands/srem</remarks>
        public Task<long> RemoveAsync(IBatch batch, params TValue[] values) {
            return batch.SetRemoveAsync(this.SetKey, Array.ConvertAll(values, v => RedisValue.Unbox(v)));
        }

        #endregion
    }
}