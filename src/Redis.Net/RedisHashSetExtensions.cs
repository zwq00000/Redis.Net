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
            var propValue = RedisValueConverter.Convert (value, prop.PropertyType, CultureInfo.InvariantCulture);
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
                if (RedisValueConverter.TryParse (value, out var redisValue)) {
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
            return (TValue) RedisValueConverter.Convert (value, typeof (TValue), CultureInfo.InvariantCulture);
        }

        static class RedisValueConverter {

            /// <summary>
            /// 尝试解析 <see cref="IConvertible">对象数据</see> 为 <see cref="RedisValue"/>
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public static bool TryParse (object obj, out RedisValue value) {
                if (obj == null) {
                    value = RedisValue.Null;
                    return false;
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
                    case DateTime date:
                        //符合 ISO8601
                        value = date.ToString ("O");
                        break;
                    case TimeSpan time:
                        value = time.Ticks;
                        break;
                    case Enum enumVal:
                        value = (int) obj;
                        break;
                    case Array array:
                        value = ArrayConverter.ToRedisValue (array);
                        break;
                    default:
                        value = RedisValue.Null;
                        break;
                }

                return value != RedisValue.Null;
            }

            public static object Convert (RedisValue value, Type conversionType, IFormatProvider provider) {
                if (value.IsNull) {
                    return null;
                }
                if (conversionType == null) throw new ArgumentNullException (nameof (conversionType));
                var underlyingType = Nullable.GetUnderlyingType (conversionType);
                if (underlyingType != null) {
                    conversionType = underlyingType;
                }

                if (conversionType == typeof (byte[])) return (byte[]) value;
                if (conversionType == typeof (ReadOnlyMemory<byte>)) return (ReadOnlyMemory<byte>) value;
                if (conversionType == typeof (RedisValue)) return value;
                //转换 Array 类型
                if (conversionType.IsArray) return ArrayConverter.ToArray (value, conversionType);

                switch (System.Type.GetTypeCode (conversionType)) {
                    case TypeCode.Boolean:
                        return (bool) value;
                    case TypeCode.Byte:
                        return checked ((byte) (uint) value);
                    case TypeCode.Char:
                        return checked ((char) (uint) value);
                    case TypeCode.DateTime:
                        return DateTime.Parse ((string) value, provider);
                    case TypeCode.Decimal:
                        return (decimal) value;
                    case TypeCode.Double:
                        return (double) value;
                    case TypeCode.Int16:
                        return (short) value;
                    case TypeCode.Int32:
                        return (int) value;
                    case TypeCode.Int64:
                        return (long) value;
                    case TypeCode.SByte:
                        return (sbyte) value;
                    case TypeCode.Single:
                        return (float) value;
                    case TypeCode.String:
                        return (string) value;
                    case TypeCode.UInt16:
                        return checked ((ushort) (uint) value);
                    case TypeCode.UInt32:
                        return (uint) value;
                    case TypeCode.UInt64:
                        return (ulong) value;
                    case TypeCode.Object:
                        return value;
                    default:
                        throw new NotSupportedException (conversionType.Name);
                }
            }

        }

        static class ArrayConverter {

            public static bool CanConverted (Type arrayType) {
                return arrayType.HasElementType && arrayType.GetElementType ().IsValueType;
            }

            public static RedisValue ToRedisValue (Array array) {
                return (RedisValue) ConvertToBytes (array);
            }

            public static object ToArray (RedisValue value, Type conversionType) {
                if (!conversionType.HasElementType) {
                    return null;
                }
                var elementType = conversionType.GetElementType ();
                var itemSize = SizeOf (elementType);
                var bytes = (byte[]) value;
                var len = bytes.Length / itemSize;
                var array = Array.CreateInstance (elementType, len);
                Buffer.BlockCopy (bytes, 0, array, 0, bytes.Length);
                return array;
            }

            private static int SizeOf (Type valueType) {
                switch (System.Type.GetTypeCode (valueType)) {
                    case TypeCode.Boolean:
                        return sizeof (bool);
                    case TypeCode.Byte:
                        return sizeof (byte);
                    case TypeCode.Char:
                        return sizeof (char);
                    case TypeCode.Decimal:
                        return sizeof (decimal);
                    case TypeCode.Double:
                        return sizeof (double);
                    case TypeCode.Int16:
                        return sizeof (short);
                    case TypeCode.Int32:
                        return sizeof (int);
                    case TypeCode.Int64:
                        return sizeof (long);
                    case TypeCode.SByte:
                        return sizeof (sbyte);
                    case TypeCode.Single:
                        return sizeof (float);
                    case TypeCode.UInt16:
                        return sizeof (ushort);
                    case TypeCode.UInt32:
                        return sizeof (uint);
                    case TypeCode.UInt64:
                        return sizeof (ulong);
                    default:
                        throw new NotSupportedException (valueType.Name);
                }
            }

            private static byte[] ConvertToBytes (Array array) {
                var elementType = array.GetType ().GetElementType ();
                var itemSize = SizeOf (elementType);
                var byteArray = new byte[array.Length * itemSize];
                Buffer.BlockCopy (array, 0, byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        static class PropertiesCache<T> {
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
                    ArrayConverter.CanConverted (propertyType);
            }
        }
    }
}