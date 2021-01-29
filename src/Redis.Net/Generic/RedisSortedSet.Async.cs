using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis Soretd  Set Wapper
    /// </summary>
    public partial class RedisSortedSet<TValue> {
        #region  Async Funcs
        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key.
        ///  If the specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        async Task<bool> IAsyncSortSet<TValue>.AddAsync (TValue member, double score) {
            return await Database.SortedSetAddAsync (this.SetKey, Unbox (member), score);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. 
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        async Task<long> IAsyncSortSet<TValue>.AddAsync (params KeyValuePair<TValue, double>[] values) {
            return await Database.SortedSetAddAsync (this.SetKey, values.Select (kv => new SortedSetEntry (Unbox (kv.Key), kv.Value)).ToArray ());
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. 
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        async Task<long> IAsyncSortSet<TValue>.AddAsync (params SortedSetEntry<TValue>[] values) {
            return await Database.SortedSetAddAsync (this.SetKey, values.Select (v => v.ToEntry ()).ToArray ());
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. 
        /// Non existing members are ignored.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        async Task<long> IAsyncSortSet<TValue>.RemoveAsync (params TValue[] members) {
            return await Database.SortedSetRemoveAsync (this.SetKey, members.Select (m => Unbox (m)).ToArray ());
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score.
        ///  For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        async Task<long> IAsyncSortSet<TValue>.RemoveRangeByRankAsync (long start, long stop) {
            return await Database.SortedSetRemoveRangeByRankAsync (this.SetKey, start, stop);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start" /> and <paramref name="stop" /> to exclude (defaults to both inclusive).</param>
        async Task<long> IAsyncSortSet<TValue>.RemoveRangeByScoreAsync (double start, double stop, Exclude exclude) {
            return await Database.SortedSetRemoveRangeByScoreAsync (this.SetKey, start, stop, exclude);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min" /> and <paramref name="max" /> to exclude (defaults to both inclusive).</param>
        async Task<long> IAsyncSortSet<TValue>.RemoveRangeByValueAsync (RedisValue min, RedisValue max, Exclude exclude) {
            return await Database.SortedSetRemoveRangeByValueAsync (this.SetKey, min, max, exclude);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. 
        /// If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double> IAsyncSortSet<TValue>.IncrementAsync (TValue member, double value) {
            return Database.SortedSetIncrementAsync (this.SetKey, Unbox (member), value);
        }

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. 
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double> IAsyncSortSet<TValue>.DecrementAsync (TValue member, double value) {
            return Database.SortedSetDecrementAsync (this.SetKey, Unbox (member), value);
        }

        #endregion

    }
}