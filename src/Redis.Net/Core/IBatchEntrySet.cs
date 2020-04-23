using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IBatchEntrySet<TKey, TValue> where TKey : IConvertible {

        Task BatchAdd (IBatch batch, TKey key, IEnumerable<HashEntry> entries);
        Task BatchUpdate (IBatch batch, TKey key, IEnumerable<HashEntry> entries);
        Task BatchAdd (IBatch batch, TKey key, TValue value);
        Task<bool> BatchRemove (IBatch batch, TKey key);
        Task<bool> BatchExpire (IBatch batch, TKey key, TimeSpan? expiry);
        Task<long> BatchClear (IBatch batch);
    }
}