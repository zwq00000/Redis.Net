using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis Soretd  Set Wapper
    /// </summary>
    public partial class RedisSortedSet<TValue> {
        #region  Batch
        Task<bool> IBatchSortSet<TValue>.BatchAdd (IBatch batch, TValue member, double score) {
            return batch.SortedSetAddAsync (this.SetKey, RedisValue.Unbox (member), score);
        }

        Task<long> IBatchSortSet<TValue>.BatchAdd (IBatch batch, params KeyValuePair<TValue, double>[] values) {
            return batch.SortedSetAddAsync (this.SetKey, values.Select (kv => new SortedSetEntry (RedisValue.Unbox (kv.Key), kv.Value)).ToArray ());
        }

        Task<long> IBatchSortSet<TValue>.BatchAdd (IBatch batch, params SortedSetEntry<TValue>[] values) {
            return batch.SortedSetAddAsync (this.SetKey, values.Select (v => v.ToEntry ()).ToArray ());
        }

        Task<long> IBatchSortSet<TValue>.BatchRemove (IBatch batch, params TValue[] members) {
            return batch.SortedSetRemoveAsync (this.SetKey, members.Select (m => RedisValue.Unbox (m)).ToArray ());
        }

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment. 
        /// If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double> IBatchSortSet<TValue>.BatchIncrement (IBatch batch, TValue member, double value) {
            return batch.SortedSetIncrementAsync (this.SetKey, RedisValue.Unbox (member), value);
        }

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement. 
        /// If member does not exist in the sorted set, it is added with -decrement as its score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double> IBatchSortSet<TValue>.BatchDecrement (IBatch batch, TValue member, double value) {
            return batch.SortedSetDecrementAsync (this.SetKey, RedisValue.Unbox (member), value);
        }

        #endregion
    }
}