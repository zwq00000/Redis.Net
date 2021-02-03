using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Converters;
using StackExchange.Redis;

namespace Redis.Net.Generic {

    /// <summary>
    /// Redis Set 泛型集合
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ReadOnlyRedisSet<TValue> : AbstracRedisKey, IReadOnlyCollection<TValue> where TValue : IConvertible {
        public ReadOnlyRedisSet (IDatabase database, string setKey) : base (database, setKey, RedisType.Set) { }

        protected TValue ConvertValue (RedisValue key) {
            return (TValue) ((IConvertible) key).ToType (typeof (TValue), CultureInfo.CurrentCulture);
        }

        protected RedisValue Unbox (TValue value) {
            return RedisConvertFactory.ToRedisValue (value);
        }

        /// <summary>
        /// 返回非泛型 HashSet
        /// </summary>
        /// <returns></returns>
        public ReadOnlyRedisSet GetNormal () {
            return new ReadOnlyRedisSet (this.Database, this.SetKey);
        }

        public int Count => (int) Database.SetLength (SetKey);

        /// <summary>
        /// 长整型 数量合计
        /// </summary>
        /// <returns></returns>
        public long LongCount => Database.SetLength (SetKey);

        /// <summary>
        /// 集合内是否包含<paramref name="value">指定值</paramref>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains (TValue value) => Database.SetContains (SetKey, RedisValue.Unbox (value));

        public ICollection<TValue> Values {
            get { return Array.ConvertAll (Database.SetMembers (SetKey), ConvertValue); }
        }

        #region Implementation of IEnumerable
        public IEnumerator<TValue> GetEnumerator () {
            return this.Values.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return this.GetEnumerator ();
        }
        #endregion

        #region  Async Methods

        /// <summary>
        /// 集合内是否包含<paramref name="value">指定值</paramref>的异步方法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> ContainsAsync (TValue value) => await Database.SetContainsAsync (SetKey, Unbox (value));

        /// <summary>
        /// Async method for Values
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TValue>> GetValuesAsync () {
            var values = await Database.SetMembersAsync (SetKey);
            return Array.ConvertAll (values, ConvertValue);
        }

        /// <summary>
        /// Async Count func
        /// </summary>
        /// <returns></returns>
        public async Task<long> CountAsync () {
            return await Database.SetLengthAsync (SetKey);
        }

        #endregion

        #region extend

        /// <summary>
        /// The SSCAN command is used to incrementally iterate over set; note: to resume an iteration via <i>cursor</i>,
        ///  cast the original enumerable or enumerator to <i>IScanningCursor</i>.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IEnumerable<TValue> Scan (TValue pattern = default (TValue), int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.SetScan (SetKey, RedisValue.Unbox (pattern), pageSize, cursor, pageOffset).Select (ConvertValue);
        }

        #endregion
    }
}