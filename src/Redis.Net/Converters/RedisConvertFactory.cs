using System;
using System.Collections.Generic;
using System.Globalization;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    /// <summary>
    /// Redis 数值转换工厂,用于 Redis HashSet 类型转换
    /// </summary>
    internal static class RedisConvertFactory {
        static IRedisValueConverter _converter = new RedisValueConverter ();
        static IArrayConverter _arrayConverter = new ArrayConverter ();

        internal static IArrayConverter ArrayConverter { get => _arrayConverter; }

        public static bool TryParse (object obj, out RedisValue value) {
            return _converter.TryParse (obj, out value);
        }

        public static RedisValue ToRedisValue<TVal> (TVal obj) where TVal : IConvertible {
            if (obj == null) {
                return RedisValue.Null;
            }

            switch (obj) {
                case DateTime v:
                    //符合 ISO8601
                    return v.ToString ("O");
                case Enum v:
                     var underlyingType = Enum.GetUnderlyingType (v.GetType());
                    return RedisValue.Unbox (System.Convert.ChangeType (v, underlyingType));
                default:
                    return RedisValue.Unbox (obj);
            }
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

        // static class EnumValueConvert<TValue> where TValue : IConvertible {

        //     private static Func<TValue, RedisValue> conv;
        //     public static Func<TValue, RedisValue> GetConvert () {
        //         if (conv == null) {
        //             var underlyingType = Enum.GetUnderlyingType (typeof (TValue));
        //             conv = new Func<TValue, RedisValue> (e => RedisValue.Unbox (System.Convert.ChangeType (e, underlyingType)));
        //         }
        //         return conv;
        //     }

        //     private static Func<TValue, RedisValue> CreateConvert () {
        //         switch(TValue){

        //         }
        //     }

        //     public static RedisValue Unbox(TValue e){
        //         return GetConvert()(e);
        //     }
        // }
    }
}