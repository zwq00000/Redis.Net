using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IBatchGeoHashSet:IBatchGeoHashSet<RedisValue> {

    }

        public interface IBatchGeoHashSet<TKey> {
        Task BatchRemove (IBatch batch, TKey member);
        Task BatchAdd (IBatch batch, GeoEntry entry);
        Task BatchAdd (IBatch batch, double lng, double lat, TKey member);
    }

    public interface IAsyncGeoHashSet:IAsyncGeoHashSet<RedisValue> {

    }

    public interface IAsyncGeoHashSet<TKey> {
        Task<bool> AddAsync (GeoEntry entry);
        Task<long> AddRangeAsync (IEnumerable<GeoEntry> entries);
        Task<bool> RemoveAsync (TKey member);
    }
}