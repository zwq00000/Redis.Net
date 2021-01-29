using System;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    internal class RedisValueConverter : IRedisValueConverter {

        /// <summary>
        /// 尝试解析 <see cref="IConvertible">对象数据</see> 为 <see cref="RedisValue"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryParse (object obj, out RedisValue value) {
            switch (obj) {
                case null:
                    value = RedisValue.Null;
                    break;
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
                    value =  (int) obj;
                    break;
                case Array array:
                    value = RedisConvertFactory.ArrayConverter.ToRedisValue (array);
                    break;
                default:
                    value = RedisValue.Null;
                    return false;
            }

            return true;
        }

        public object Convert (RedisValue value, Type conversionType, IFormatProvider provider) {
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
            if (conversionType.IsArray) return RedisConvertFactory.ArrayConverter.ToArray (value, conversionType);

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
}