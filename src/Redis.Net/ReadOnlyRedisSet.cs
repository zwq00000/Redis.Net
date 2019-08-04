using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Set Warpper
    /// </summary>
    public class ReadOnlyRedisSet:AbstracRedisKey, IReadOnlyCollection<RedisValue> {

        protected ReadOnlyRedisSet(IDatabase database, string setKey) 
            : base(database, setKey, RedisType.Set) {
        }

        /// <summary>Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</returns>
        public ICollection<RedisValue> Values {
            get { return Database.SetMembers(SetKey); }
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public int Count => (int)Database.SetLength(SetKey);

        /// <summary>
        /// 长整型 数量合计
        /// </summary>
        /// <returns></returns>
        public long LongCount => Database.SetLength(SetKey);

        /// <summary>
        /// 集合内是否包含<paramref name="value">指定值</paramref>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(RedisValue value) => Database.SetContains(SetKey, value);

        #region  Async Methods

        /// <summary>
        /// 集合内是否包含<paramref name="value">指定值</paramref>的异步方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> ContainsAsync(RedisValue value) => await Database.SetContainsAsync(SetKey, value);

        /// <summary>
        /// Async method for Values
        /// </summary>
        /// <returns></returns>
        public async Task<RedisValue[]> GetValuesAsync() {
            return await Database.SetMembersAsync(SetKey);
        }

        /// <summary>
        /// Async Count func
        /// </summary>
        /// <returns></returns>
        public async Task<long> CountAsync() {
            return await Database.SetLengthAsync(SetKey);
        }

        #endregion

        /// <summary>
        /// The SSCAN command is used to incrementally iterate over set; note: to resume an iteration via <i>cursor</i>,
        ///  cast the original enumerable or enumerator to <i>IScanningCursor</i>.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IEnumerable<RedisValue> Scan(RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.SetScan(SetKey, pattern, pageSize, cursor, pageOffset);
        }

        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<RedisValue> GetEnumerator() {
            return this.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}