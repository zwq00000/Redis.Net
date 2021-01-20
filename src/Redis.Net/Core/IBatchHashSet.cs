using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {

    /// <summary>
    /// HashSet 批量访问方法
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IBatchHashSet<TKey, TValue>
        where TKey : IConvertible {

            Task BatchAdd (IBatch batch, TKey key, TValue value);
            Task BatchAdd (IBatch batch, params Tuple<TKey, TValue>[] tuples);
            Task BatchAdd (IBatch batch, params KeyValuePair<TKey, TValue>[] tuples);
            Task<bool> BatchRemove (IBatch batch, TKey key);

        }

    /// <summary>
    /// HashSet 同步访问方法
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IHashSet<TKey, TValue> where TKey : IConvertible {
        /// <summary>
        /// Add one to Hashset
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add (TKey key, TValue value);

        /// <summary>
        /// Add to Hashset
        /// </summary>
        /// <param name="tuples"></param>
        void Add (params Tuple<TKey, TValue>[] tuples);

        /// <summary>
        /// Add to Hashset
        /// </summary>
        /// <param name="pairs"></param>
        void Add (params KeyValuePair<TKey, TValue>[] pairs);

        /// <summary>
        /// remove Set field
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove (TKey key);

    }

    /// <summary>
    /// Hashset 异步访问方法
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IAsyncHashSet<TKey, TValue> where TKey : IConvertible {
        Task AddAsync (TKey key, TValue value);
        Task AddAsync (params Tuple<TKey, TValue>[] tuples);
        Task AddAsync (params KeyValuePair<TKey, TValue>[] tuples);
        Task<bool> RemoveAsync (TKey key);

        Task<long> DecrementAsync (TKey hashField, long value = 1);
        Task<double> DecrementAsync (TKey hashField, double value);
        Task<double> IncrementAsync (TKey hashField, double value);
    }
}