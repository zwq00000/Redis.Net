using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// 只读的 Redis SortSet
    /// </summary>
    public class ReadOnlySortedSet : AbstracRedisKey {

        public ReadOnlySortedSet (IDatabase database, string setKey) : base (database, setKey, RedisType.SortedSet) {

        }

        /// <summary>
        /// 获取集合全部键值
        /// </summary>
        /// <returns></returns>
        public RedisValue[] Keys () {
            return GetRangeByRank ();
        }

        /// <summary>
        /// 异步获取集合全部键值
        /// </summary>
        /// <returns></returns>
        public async Task<RedisValue[]> KeysAsync () {
            return await GetRangeByRankAsync ();
        }

        ///<summary>
        /// Returns the score of member in the sorted set at key; If member does not exist
        ///     in the sorted set, or key does not exist, nil is returned.
        ///</summary>
        ///<returns>The score of the member.</returns>
        ///<remarks>
        /// see https://redis.io/commands/zscore
        ///</remarks>
        public double? GetScore (RedisValue member) {
            return Database.SortedSetScore (SetKey, member);
        }

        /// <summary>
        /// Returns the score of member in the sorted set at key; 
        /// If member does not exist in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<double?> GetScoreAsync (RedisValue member) {
            return await Database.SortedSetScoreAsync (SetKey, member);
        }

        /// <summary>
        /// 扫描集合
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IEnumerable<SortedSetEntry> Scan (RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.SortedSetScan (SetKey, pattern, pageSize, cursor, pageOffset);
        }

        /// <summary>
        /// 扫描集合的异步方法
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="pageSize"></param>
        /// <param name="cursor"></param>
        /// <param name="pageOffset"></param>
        /// <returns></returns>
        public IAsyncEnumerable<SortedSetEntry> ScanAsync (RedisValue pattern = default, int pageSize = 10, long cursor = 0, int pageOffset = 0) {
            return Database.SortedSetScanAsync (SetKey, pattern, pageSize, cursor, pageOffset);
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
        public RedisValue[] GetRangeByScore (double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            return Database.SortedSetRangeByScore (SetKey, start, stop, exclude, order, skip, take);
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
        public async Task<RedisValue[]> GetRangeByScoreAsync (double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) {
            return await Database.SortedSetRangeByScoreAsync (SetKey, start, stop, exclude, order, skip, take);
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
        public RedisValue[] GetRangeByRank (long start = 0, long stop = -1, Order order = Order.Ascending) {
            return Database.SortedSetRangeByRank (SetKey, start, stop, order);
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
        public async Task<RedisValue[]> GetRangeByRankAsync (long start = 0, long stop = -1, Order order = Order.Ascending) {
            return await Database.SortedSetRangeByRankAsync (SetKey, start, stop, order);
        }

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
        /// <returns></returns>
        public long GetLongCount (double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) {
            return Database.SortedSetLength (SetKey, min, max, exclude);
        }

        /// <summary>
        /// 获取集合数量的异步方法
        /// </summary>
        /// <returns></returns>
        public async Task<long> CountAsync (double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) {
            return await Database.SortedSetLengthAsync (SetKey, min, max, exclude);
        }
    }
}