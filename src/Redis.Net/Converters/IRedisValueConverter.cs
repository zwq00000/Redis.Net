using System;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    /// <summary>
    /// <see cref="RedisValue"/> 类型转换器
    /// </summary>
    public interface IRedisValueConverter {

        /// <summary>
        /// 尝试序列化数据对象到 <see cref="RedisValue"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryParse (object obj, out RedisValue value);

        /// <summary>
        /// 从 <see cref="RedisValue"/> 转换为 对象实例
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        object Convert (RedisValue value, Type conversionType, IFormatProvider provider);
    }
}