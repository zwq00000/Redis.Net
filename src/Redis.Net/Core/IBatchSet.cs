using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IBatchSet<TValue> where TValue : IConvertible {
        Task<long> BatchAdd (IBatch batch, params TValue[] values);
        Task<long> BatchRemove (IBatch batch, params TValue[] values);
        Task<bool> BatchClear (IBatch batch);
    }

    public interface IBatchSortSet<TValue> where TValue : IConvertible {
        Task<bool> BatchAdd (IBatch batch, TValue member, double score);
        Task<long> BatchAdd (IBatch batch, params KeyValuePair<TValue, double>[] values);
        Task<long> BatchAdd (IBatch batch, params SortedSetEntry<TValue>[] values);
        Task<long> BatchRemove (IBatch batch, params TValue[] members);
        Task<double> BatchIncrement (IBatch batch, TValue member, double value);
        Task<double> BatchDecrement (IBatch batch, TValue member, double value);
    }

    public interface IAsyncSortSet<TValue> where TValue : IConvertible {
        Task<bool> AddAsync (TValue member, double score);
        Task<long> AddAsync (params KeyValuePair<TValue, double>[] values);
        Task<long> AddAsync (params SortedSetEntry<TValue>[] values);
        Task<long> RemoveAsync (params TValue[] members);
        Task<long> RemoveRangeByRankAsync (long start, long stop);
        Task<long> RemoveRangeByScoreAsync (double start, double stop, Exclude exclude = Exclude.None);
        Task<long> RemoveRangeByValueAsync (RedisValue min, RedisValue max, Exclude exclude = Exclude.None);
        Task<double> IncrementAsync (TValue member, double value);
        Task<double> DecrementAsync (TValue member, double value);
    }
}