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
    /// 只读的 Redis SortSet
    /// </summary>
    public class ReadOnlySortedSet<TValue> : AbstracRedisKey, IReadOnlyDictionary<TValue, double> where TValue : IConvertible {

        public ReadOnlySortedSet (IDatabase database, string setKey) : base (database, setKey, RedisType.SortedSet) {

        }

        protected TValue ConvertValue (RedisValue key) {
            return (TValue) ((IConvertible) key).ToType (typeof (TValue), CultureInfo.CurrentCulture);
        }

        protected RedisValue Unbox (TValue value) {
            return RedisConvertFactory.ToRedisValue (value);
        }

        /// <summary>
        /// 获取集合全部键值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TValue> Keys => this.GetRangeByRank ();

        /// <summary>
        /// 异步获取集合全部键值
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TValue>> KeysAsync () {
            return await this.GetRangeByRankAsync ();
        }

        ///<summary>
        /// Returns the score of member in the sorted set at key; If member does not exist
        ///     in the sorted set, or key does not exist, nil is returned.
        ///</summary>
        ///<returns>The score of the member.</returns>
        ///<remarks>
        /// see https://redis.io/commands/zscore
        ///</remarks>
        public double? GetScore (TValue member) {
            return Database.SortedSetScore (SetKey, Unbox (member));
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key; 
        /// If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<double?> GetScoreAsync (TValue member) {
            return await Database.SortedSetScoreAsync (SetKey, Unbox (member));
        }

        /// <summary>
        /// 扫描集合
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IEnumerable<SortedSetEntry> Scan (TValue pattern = default (TValue), int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.SortedSetScan (SetKey, Unbox (pattern), pageSize, cursor, pageOffset);
        }

        /// <summary>
        /// 返回全部匹配项的 Member和Score
        /// see IDatabase.SortedSetScan
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TValue, double>> GetEntries (TValue pattern = default (TValue), int pageSize = int.MaxValue, long cursor = 0, int pageOffset = 0) {
            var values = Database.SortedSetScan (SetKey, Unbox (pattern), pageSize, cursor, pageOffset);
            return values.Select (v => new KeyValuePair<TValue, double> (ConvertValue (v.Element), v.Score));
        }

        #region GetRange

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering,
        ///  this command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="exclude"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public TValue[] GetRangeByValue (TValue min = default (TValue), TValue max = default (TValue), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            return Array.ConvertAll (Database.SortedSetRangeByValue (SetKey, Unbox (min), Unbox (max), exclude, order, skip, take), ConvertValue);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering,
        ///  this command returns all the elements in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="exclude"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<TValue[]> GetRangeByValueAsync (TValue min = default (TValue), TValue max = default (TValue), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            var result = await Database.SortedSetRangeByValueAsync (SetKey, Unbox (min), Unbox (max), exclude, order, skip, take);
            return Array.ConvertAll (result, ConvertValue);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Start and stop are used to specify the min and max range for score values. Similar to other range methods the values are inclusive.
        /// </summary>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by.</param>
        /// <param name="exclude">Which of <paramref name="start" /> and <paramref name="stop" /> to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip.</param>
        /// <param name="take">How many items to take.</param>
        /// <returns>List of elements in the specified score range.</returns>
        public TValue[] GetRangeByScore (double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            return Array.ConvertAll (Database.SortedSetRangeByScore (SetKey, start, stop, exclude, order, skip, take), ConvertValue);
        }

        /// <summary>
        ///  Returns the specified range of elements in the sorted set stored at key. By default
        ///     the elements are considered to be ordered from the lowest to the highest score.
        ///     Lexicographical order is used for elements with equal score. Start and stop are
        ///     used to specify the min and max range for score values. Similar to other range
        ///     methods the values are inclusive.
        /// </summary>
        /// <param name="start">The minimum score to filter by.</param>
        /// <param name="stop">The maximum score to filter by</param>
        /// <param name="exclude">Which of start and stop to exclude (defaults to both inclusive).</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <param name="skip">How many items to skip</param>
        /// <param name="take">How many items to take.</param>
        /// <returns></returns>
        public async Task<TValue[]> GetRangeByScoreAsync (double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            var result = await Database.SortedSetRangeByScoreAsync (SetKey, start, stop, exclude, order, skip, take);
            return Array.ConvertAll (result, ConvertValue);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>https://redis.io/commands/zrange</remarks>
        /// <remarks>https://redis.io/commands/zrevrange</remarks>
        public TValue[] GetRangeByRank (long start = 0, long stop = -1, Order order = Order.Ascending) {
            return Array.ConvertAll (Database.SortedSetRangeByRank (SetKey, start, stop, order), ConvertValue);
        }

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default the elements are considered to be ordered from the lowest to the highest score. Lexicographical order is used for elements with equal score.
        /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on. They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
        /// </summary>
        /// <param name="start">The start index to get.</param>
        /// <param name="stop">The stop index to get.</param>
        /// <param name="order">The order to sort by (defaults to ascending).</param>
        /// <returns>List of elements in the specified range.</returns>
        /// <remarks>https://redis.io/commands/zrange</remarks>
        /// <remarks>https://redis.io/commands/zrevrange</remarks>
        public async Task<TValue[]> GetRangeByRankAsync (long start = 0, long stop = -1, Order order = Order.Ascending) {
            var result = await Database.SortedSetRangeByRankAsync (SetKey, start, stop, order);
            return Array.ConvertAll (result, ConvertValue);
        }

        #endregion

        #region Count

        /// <summary>
        /// 获取集合数量
        /// </summary>
        /// <returns></returns>
        public int GetCount (double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) {
            return (int) Database.SortedSetLength (SetKey, min, max, exclude);
        }

        /// <summary>
        /// 获取集合数量
        /// </summary>
        /// <param name="min">The min score to filter by (defaults to negative infinity).</param>
        /// <param name="max">The max score to filter by (defaults to positive infinity).</param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public long GetLongCount (double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) {
            return Database.SortedSetLength (SetKey, min, max, exclude);
        }

        /// <summary>
        /// 获取集合数量的异步方法
        /// </summary>
        /// <param name="min">The min score to filter by (defaults to negative infinity).</param>
        /// <param name="max">The max score to filter by (defaults to positive infinity).</param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public async Task<long> CountAsync (double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) {
            return await Database.SortedSetLengthAsync (SetKey, min, max, exclude);
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TValue, double>> GetEnumerator () {
            var entites = this.Database.SortedSetRangeByRankWithScores (SetKey);
            foreach (var entry in entites) {
                yield return new KeyValuePair<TValue, double> (ConvertValue (entry.Element), entry.Score);
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out KeyValuePair<TValue,double>>

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public int Count => this.GetCount ();

        #endregion

        #region Implementation of IReadOnlyDictionary<TValue,double>

        /// <summary>Determines whether the read-only dictionary contains an element that has the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the read-only dictionary contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool ContainsKey (TValue key) {
            var member = Unbox (key);
            var result = Database.SortedSetScore (SetKey, member);
            return result.HasValue;
        }

        /// <summary>Gets the value that is associated with the specified key.</summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"></see> interface contains an element that has the specified key; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        public bool TryGetValue (TValue key, out double value) {
            var member = Unbox (key);
            var result = Database.SortedSetScore (SetKey, member);
            if (result.HasValue) {
                value = result.Value;
                return true;
            }
            value = default (double);
            return false;
        }

        /// <summary>Gets the element that has the specified key in the read-only dictionary.</summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key">key</paramref> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key">key</paramref> is not found.</exception>
        public double this [TValue key] {
            get {
                var member = Unbox (key);
                var result = Database.SortedSetScore (SetKey, member);
                if (result.HasValue) {
                    return result.Value;
                }
                throw new KeyNotFoundException ();
            }
        }

        /// <summary>Gets an enumerable collection that contains the values in the read-only dictionary.</summary>
        /// <returns>An enumerable collection that contains the values in the read-only dictionary.</returns>
        public IEnumerable<double> Values {
            get {
                return this.Database.SortedSetRangeByRankWithScores (SetKey)
                    .Select (e => e.Score);
            }
        }

        #endregion
    }
}