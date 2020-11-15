using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Redis.Net.Converters;

namespace Redis.Net {
    public static partial class RedisHashSetExtensions {
        internal static class PropertiesCache<T> {
            private static PropertyInfo[] _cache;

            public static PropertyInfo[] GetProperties () {
                if (_cache == null) {
                    var type = typeof (T);
                    var props = GetProperties (type);
                    if (type.IsInterface) {
                        props = props.Concat (type.GetInterfaces ().SelectMany (i => GetProperties (i)));
                    }
                    _cache = props.ToArray ();
                }

                return _cache;
            }

            private static IEnumerable<PropertyInfo> GetProperties (Type type) {
                return type.GetRuntimeProperties ()
                    .Where (p => p.CanRead && CanConverted (p.PropertyType));
            }
            /// <summary>
            /// 可以被转换的属性类型
            /// </summary>
            /// <param name="propertyType"></param>
            /// <returns></returns>
            private static bool CanConverted (Type propertyType) {
                return propertyType.IsValueType ||
                    propertyType == typeof (string) ||
                    propertyType.IsEnum ||
                    propertyType == typeof (byte[]) ||
                    propertyType == typeof (ReadOnlyMemory<byte>) ||
                    propertyType == typeof (Memory<byte>) ||
                    RedisConvertFactory.ArrayConverter.CanConverted (propertyType);
            }
        }
    }
}