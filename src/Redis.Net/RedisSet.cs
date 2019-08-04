using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Set 集合
    /// </summary>
    public class RedisSet : ReadOnlyRedisSet , ICollection<RedisValue> {
        public RedisSet(IDatabase database, string setKey) : base(database, setKey) {
        }


        /// <inheritdoc />
        public void Clear() {
            Database.SetRemove(SetKey,this.Values.ToArray());
        }

        /// <summary>
        /// 异步向集合中增加元素的
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> AddAsync(params RedisValue[] value) {
            return await base.Database.SetAddAsync(SetKey, value);
        }

        /// <inheritdoc />
        public void Add(RedisValue item) {
            base.Database.SetAdd(SetKey, item);
        }

        /// <summary>
        /// 向集合中增加元素
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public long Add(RedisValue[] items) {
            return base.Database.SetAdd(SetKey, items);
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array">array</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex">arrayIndex</paramref> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from <paramref name="arrayIndex">arrayIndex</paramref> to the end of the destination <paramref name="array">array</paramref>.</exception>
        public void CopyTo(RedisValue[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool Remove(RedisValue value) {
            return Database.SetRemove(SetKey, value);
        }

        public long Remove(RedisValue[] value) {
            return Database.SetRemove(SetKey, value);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly {
            get => false;
        }

        public async Task<long> RemoveAsync(params RedisValue[] value) {
            return await Database.SetRemoveAsync(SetKey,value);
        }
    }
}