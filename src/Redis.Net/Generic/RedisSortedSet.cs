using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis Soretd  Set Wapper
    /// </summary>
    /// <typeparam name="TValue"> assign from <see cref="IConvertible"/> </typeparam>
    public partial class RedisSortedSet<TValue> : ReadOnlySortedSet<TValue>,IAsyncSortSet<TValue>,IBatchSortSet<TValue> where TValue : IConvertible {
        public RedisSortedSet (IDatabase database, string setKey) : base (database, setKey) { }

        #region Sync Methods

        /// <summary>
        /// Adds the specified member with the specified score to the sorted set stored at key. 
        /// If the specified member is already a member of the sorted set,
        ///     the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool Add (TValue member, double score) {
            return Database.SortedSetAdd (this.SetKey, Unbox (member), score);
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. 
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public long Add (params KeyValuePair<TValue, double>[] values) {
            return Database.SortedSetAdd (this.SetKey, values.Select (kv => new SortedSetEntry (Unbox (kv.Key), kv.Value)).ToArray ());
        }

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored at key. 
        /// If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public long Add (params SortedSetEntry<TValue>[] values) {
            return Database.SortedSetAdd (this.SetKey, values.Select (v => v.ToEntry ()).ToArray ());
        }

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing members are ignored.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public long Remove (params TValue[] members) {
            return Database.SortedSetRemove (this.SetKey, members.Select (m => Unbox (m)).ToArray ());
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start and stop. Both start and stop are 0 -based indexes with 0 being the element with the lowest score. These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score.
        ///  For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
        /// </summary>
        /// <param name="start">The minimum rank to remove.</param>
        /// <param name="stop">The maximum rank to remove.</param>
        public long RemoveRangeByRank (long start, long stop) {
            return Database.SortedSetRemoveRangeByRank (this.SetKey, start, stop);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="start">The minimum score to remove.</param>
        /// <param name="stop">The maximum score to remove.</param>
        /// <param name="exclude">Which of <paramref name="start" /> and <paramref name="stop" /> to exclude (defaults to both inclusive).</param>
        public long RemoveRangeByScore (double start, double stop, Exclude exclude = Exclude.None) {
            return Database.SortedSetRemoveRangeByScore (this.SetKey, start, stop, exclude);
        }

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min and max (inclusive by default).
        /// </summary>
        /// <param name="min">The minimum value to remove.</param>
        /// <param name="max">The maximum value to remove.</param>
        /// <param name="exclude">Which of <paramref name="min" /> and <paramref name="max" /> to exclude (defaults to both inclusive).</param>
        public long RemoveRangeByValue (RedisValue min, RedisValue max, Exclude exclude = Exclude.None) {
            return Database.SortedSetRemoveRangeByValue (this.SetKey, min, max, exclude);
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. 
        /// If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Increment (TValue member, double value) {
            return Database.SortedSetIncrement (this.SetKey, Unbox (member), value);
        }

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. 
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Decrement (TValue member, double value) {
            return Database.SortedSetDecrement (this.SetKey, Unbox (member), value);
        }

        #endregion

        public IBatchSortSet<TValue> AsBatch(){
            return this;
        }

        public IAsyncSortSet<TValue> AsAsync(){
            return this;
        }

        public ReadOnlySortedSet<TValue> AsReadonly(){
            return this;
        }
    }
}