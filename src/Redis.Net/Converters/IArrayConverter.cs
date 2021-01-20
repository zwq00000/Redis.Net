using System;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    /// <summary>
    /// 数组类型转换器
    /// </summary>
    public interface IArrayConverter {

        bool CanConverted (Type arrayType);

        /// <summary>
        /// 转换数组到 <see cref="RedisValue"/>
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        RedisValue ToRedisValue (Array array);

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        object ToArray (RedisValue value, Type conversionType);
    }
}