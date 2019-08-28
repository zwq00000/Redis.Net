using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis HashSet 扩展方法
    /// </summary>
    public static class RedisHashSetExtensions {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBase"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public static async Task<T> HashGetEntityAsync<T> (this IDatabase dataBase, RedisKey key) where T : new () {
            if (dataBase == null) {
                throw new ArgumentNullException (nameof (dataBase));
            }

            var properties = GetProperties<T> ().ToArray ();
            var values = await dataBase.HashGetAsync (key, properties.Select (p => (RedisValue) p.Name).ToArray ());
            var instance = new T ();
            for (var i = 0; i < values.Length; i++) {
                var prop = properties[i];
                var val = values[i];

                if (val.HasValue) {
                    prop.SetValue (instance, ((IConvertible) val).ToType (prop.PropertyType, CultureInfo.InvariantCulture));
                }
            }

            return instance;
        }

        public static T HashGetEntity<T> (this IDatabase dataBase, RedisKey key) where T : new () {
            if (dataBase == null) {
                throw new ArgumentNullException (nameof (dataBase));
            }

            var properties = GetProperties<T> ().ToArray ();
            var values = dataBase.HashGet (key, properties.Select (p => (RedisValue) p.Name).ToArray ());
            var instance = new T ();
            for (int i = 0; i < values.Length; i++) {
                var prop = properties[i];
                var val = values[i];

                if (val.HasValue) {
                    prop.SetValue (instance, ((IConvertible) val).ToType (prop.PropertyType, CultureInfo.InvariantCulture));
                }
            }

            return instance;
        }

        public static T ToInstance<T> (this IEnumerable<HashEntry> entries) where T : new () {
            var properties = GetProperties<T> ().ToArray ();
            var instance = new T ();
            foreach (var entry in entries) {
                var value = entry.Value;
                if (!value.HasValue) {
                    continue;
                }
                var prop = properties.FirstOrDefault (p => p.Name == entry.Name);
                if (prop != null) {
                    prop.SetValue (instance,
                        ((IConvertible) value).ToType (prop.PropertyType, CultureInfo.InvariantCulture));
                }
            }

            return instance;
        }

        /// <summary>
        /// 获取 实体对象数值属性 <see cref="HashEntry"/> 集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<HashEntry> ToHashEntries<T> (this T entity) {
            var properties = GetProperties<T> ();
            foreach (var property in properties) {
                var value = property.GetValue (entity);
                if (TryParse (value, out var redisValue)) {
                    yield return new HashEntry (property.Name, redisValue);
                }
            }
        }

        /// <summary>
        /// 获取支持 <see cref="RedisValue"/>包装的运行时属性
        /// 支持 <see cref="Type.IsValueType"/> 和 <see cref="T:string"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties<T> () {
            return PropertiesCache<T>.GetProperties ();
        }

        private static bool TryParse (object obj, out RedisValue value) {
            if (obj == null) {
                value = RedisValue.Null;
                return false;
            }

            if (obj.GetType ().IsEnum) {
                value = (int) obj;
                return true;
            }
            switch (obj) {
                case string v:
                    value = v;
                    break;
                case int v:
                    value = v;
                    break;
                case uint v:
                    value = v;
                    break;
                case double v:
                    value = v;
                    break;
                case byte[] v:
                    value = v;
                    break;
                case bool v:
                    value = v;
                    break;
                case long v:
                    value = v;
                    break;
                case ulong v:
                    value = v;
                    break;
                case float v:
                    value = v;
                    break;
                case ReadOnlyMemory<byte> v:
                    value = v;
                    break;
                case Memory<byte> v:
                    value = v;
                    break;
                case RedisValue v:
                    value = v;
                    break;
                default:
                    value = RedisValue.Null;
                    return false;
            }

            return true;
        }

        static class PropertiesCache<T> {
            private static PropertyInfo[] _cache;

            public static PropertyInfo[] GetProperties () {
                if (_cache == null) {
                    var type = typeof(T);
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
                    .Where (p => p.PropertyType.IsValueType || p.PropertyType == typeof (string) || p.PropertyType.IsEnum);
            }
        }
    }
}