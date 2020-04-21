using System;
using System.Threading.Tasks;

namespace Redis.Net {
    public interface IBatchSet<TValue> where TValue : IConvertible {
        Task<long> Add (params TValue[] values);
        Task<long> Remove (params TValue[] values);
    }
}