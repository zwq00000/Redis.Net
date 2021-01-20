using System;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    /// <summary>
    /// Redis 数值转换工厂,用于 Redis HashSet 类型转换
    /// </summary>
    public static class RedisConvertFactory {
        static IRedisValueConverter _converter = new RedisValueConverter();
        static IArrayConverter _arrayConverter = new ArrayConverter();

        internal static IArrayConverter ArrayConverter{get=>_arrayConverter;}

        public static bool TryParse (object obj, out RedisValue value) {
            return _converter.TryParse (obj, out value);
        }

        public static object Convert (RedisValue value, Type conversionType, IFormatProvider provider) {
            return _converter.Convert (value, conversionType, provider);
        }

        public static void SetConverter (IRedisValueConverter converter) {
            if (converter == null) {
                throw new ArgumentNullException (nameof (converter));
            }
            _converter = converter;
        }

        public static void SetArrayConvert (IArrayConverter arrayConvert) {
            _arrayConverter = arrayConvert;
        }
    }
}