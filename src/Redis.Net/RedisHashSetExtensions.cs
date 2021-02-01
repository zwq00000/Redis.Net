using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Redis.Net.Converters;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis HashSet 扩展方法
    /// </summary>
    public static partial class RedisHashSetExtensions {

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
                var value = values[i];
                instance.SetValue (prop, value);
            }

            return instance;
        }

        /// <summary>
        /// 根据 <see cref="HashEntry" />集合,获取 对象实例 
        /// </summary>
        /// <param name="entries"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToInstance<T> (this IEnumerable<HashEntry> entries) where T : new () {
            var properties = GetProperties<T> ();
            var instance = new T ();
            foreach (var entry in entries) {
                var value = entry.Value;
                if (!value.HasValue) {
                    continue;
                }
                var prop = properties.Single (p => p.Name == entry.Name);
                instance.SetValue (prop, value);
            }

            return instance;
        }

        private static void SetValue<T> (this T instance, PropertyInfo prop, RedisValue value) {
            if (prop == null) {
                return;
            }
            if (!value.HasValue) {
                return;
            }
            if (prop.PropertyType == typeof (TimeSpan)) {
                var timeSpan = TimeSpan.FromTicks ((long) value);
                prop.SetValue (instance, timeSpan);
                return;
            }
            var propValue = RedisConvertFactory.Convert (value, prop.PropertyType, CultureInfo.InvariantCulture);
            prop.SetValue (instance, propValue);
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
                if (RedisConvertFactory.TryParse (value, out var redisValue)) {
                    yield return new HashEntry (property.Name, redisValue);
                }
            }
        }

        /// <summary>
        /// 获取支持 <see cref="RedisValue"/>包装的运行时属性
        /// 支持 <see cref="Type.IsValueType"/> 和 <see cref="T:string"/>
        /// </summary>
        /// <remarks>
        /// 支持类型:
        /// - string
        /// - DateTime
        /// - int
        /// - uint
        /// - double
        /// - byte[]
        /// - bool
        /// - long
        /// - ulong
        /// - float
        /// - ReadOnlyMemory&gt;byte&lt;
        /// - Memory&gt;byte&lt;
        /// - RedisValue
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties<T> () {
            return PropertiesCache<T>.GetProperties ();
        }

        internal static TValue ConvertTo<TValue> (this RedisValue value) where TValue : IConvertible {
            return (TValue) RedisConvertFactory.Convert (value, typeof (TValue), CultureInfo.InvariantCulture);
        }
    }
}