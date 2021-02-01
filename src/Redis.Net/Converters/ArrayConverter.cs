using System;
using StackExchange.Redis;

namespace Redis.Net.Converters {
    internal class ArrayConverter : IArrayConverter {

        public bool CanConverted (Type arrayType) {
            return arrayType.HasElementType && arrayType.GetElementType ().IsValueType;
        }

        public RedisValue ToRedisValue (Array array) {
            // return (RedisValue) ConvertToBytes (array);
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes (array);
        }

        public object ToArray (RedisValue value, Type conversionType) {
            return System.Text.Json.JsonSerializer.Deserialize ((byte[]) value, conversionType);
            // if (!conversionType.HasElementType) {
            //     return null;
            // }
            // var elementType = conversionType.GetElementType ();
            // var itemSize = SizeOf (elementType);
            // var bytes = (byte[]) value;
            // var len = bytes.Length / itemSize;
            // var array = Array.CreateInstance (elementType, len);
            // Buffer.BlockCopy (bytes, 0, array, 0, bytes.Length);
            // return array;
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

}