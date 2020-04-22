using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IBatchGeoHashSet {
        Task BatchRemove (IBatch batch, string shipId);
        Task BatchAdd (IBatch batch, GeoEntry entry);
        Task BatchAdd (IBatch batch, double lng, double lat, RedisValue member);
    }

    public interface IAsyncGeoHashSet {
        Task<bool> AddAsync (GeoEntry entry);
        Task<long> AddRangeAsync (IEnumerable<GeoEntry> entries);
        Task<bool> RemoveAsync (RedisValue member);
    }
}