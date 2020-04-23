using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IBatchHashSet<TKey, TValue>
        where TKey : IConvertible where TValue : IConvertible {

            Task BatchAdd (IBatch batch, TKey key, TValue value);
            Task BatchAdd (IBatch batch, params Tuple<TKey, TValue>[] tuples);
            Task BatchAdd (IBatch batch, params KeyValuePair<TKey, TValue>[] tuples);
            Task<bool> BatchRemove (IBatch batch, TKey key);

        }

    public interface IHashSet<TKey, TValue> where TKey : IConvertible where TValue : IConvertible {
        void Add (TKey key, TValue value);
        void Add (params Tuple<TKey, TValue>[] tuples);
        void Add (params KeyValuePair<TKey, TValue>[] pairs);
        bool Remove (TKey key);

        long Decrement (TKey hashField, long value = 1);
        double Decrement (TKey hashField, double value);
        double Increment (TKey hashField, double value);
    }

    public interface IAsyncHashSet<TKey, TValue> where TKey : IConvertible where TValue : IConvertible {
        Task AddAsync (TKey key, TValue value);
        Task AddAsync (params Tuple<TKey, TValue>[] tuples);
        Task AddAsync (params KeyValuePair<TKey, TValue>[] tuples);
        Task<bool> RemoveAsync (TKey key);

        Task<long> DecrementAsync (TKey hashField, long value = 1);
        Task<double> DecrementAsync (TKey hashField, double value);
        Task<double> IncrementAsync (TKey hashField, double value);
    }
}