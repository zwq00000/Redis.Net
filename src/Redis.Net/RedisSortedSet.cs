using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis ZSet
    /// </summary>
    public class RedisSortedSet : ReadOnlySortedSet {
        public RedisSortedSet(IDatabase database, string setKey) : base(database, setKey) {
        }

        /// <summary>
        /// 增加成员
        /// </summary>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool Add(string member, double score) {
            return Database.SortedSetAdd(base.SetKey, member, score);
        }

        /// <summary>
        /// 增加成员的异步方法
        /// </summary>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(string member, double score) {
            return await Database.SortedSetAddAsync(base.SetKey, member, score);
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public long Remove(params RedisValue[] member) {
            return Database.SortedSetRemove(SetKey, member);
        }

        /// <summary>
        /// 删除成员的异步方法
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<long> RemoveAsync(params RedisValue[] member) {
            return await Database.SortedSetRemoveAsync(SetKey, member);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebyrank</remarks>
        public long RemoveByRank(long start,long stop) {
            return Database.SortedSetRemoveRangeByRank(SetKey, start, stop);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start" /> and <paramref name="stop" /> to exclude (defaults to both inclusive).</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebyscore</remarks>
        public long RemoveByScore(double start, double stop, Exclude exclude = Exclude.None) {
            return Database.SortedSetRemoveRangeByScore(SetKey, start, stop,exclude);
        }

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min" /> and <paramref name="max" /> to exclude (defaults to both inclusive).</param>
        /// <returns>the number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebylex</remarks>
        public long RemoveByValue(RedisValue min, RedisValue max, Exclude exclude = Exclude.None) {
            return Database.SortedSetRemoveRangeByValue(SetKey, min, max,exclude);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score. For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebyrank</remarks>
        public async Task<long> RemoveByRankAsync(long start, long stop) {
            return await Database.SortedSetRemoveRangeByRankAsync(SetKey, start, stop);
        }
        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start" /> and <paramref name="stop" /> to exclude (defaults to both inclusive).</param>
        /// <returns>The number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebyscore</remarks>
        public async Task<long> RemoveByScoreAsync(double start, double stop, Exclude exclude = Exclude.None) {
            return await Database.SortedSetRemoveRangeByScoreAsync(SetKey, start, stop,exclude);
        }
        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order to force lexicographical ordering, this command removes all elements in the sorted set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min" /> and <paramref name="max" /> to exclude (defaults to both inclusive).</param>
        /// <returns>the number of elements removed.</returns>
        /// <remarks>https://redis.io/commands/zremrangebylex</remarks>
        public async Task<long> RemoveByValueAsync(RedisValue min, RedisValue max, Exclude exclude = Exclude.None) {
            return await Database.SortedSetRemoveRangeByValueAsync(SetKey, min, max,exclude);
        }

    }
}