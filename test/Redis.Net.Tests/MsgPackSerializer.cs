using System.IO;
using MsgPack.Serialization;
using StackExchange.Redis;

namespace Redis.Net.Tests
{
    /// <summary>
    /// Class MsgPackSerializer.
    /// </summary>
    public class MsgPackSerializer : ISerializer {
        public MsgPackSerializer () {

        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public RedisValue Serialize<T> (T value) {
            if (value == null) {
                return RedisValue.Null;
            }
            var serializer = MessagePackSerializer.Get<T> ();
            using (var stream = new MemoryStream ()) {
                serializer.Pack (stream, value);
                return stream.ToArray ();
            }
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public T Deserialize<T> (RedisValue value) {
            if (value.IsNull) {
                return default (T);
            }
            var serializer = MessagePackSerializer.Get<T> ();
            using (var stream = new MemoryStream (value)) {
                return serializer.Unpack (stream);
            }
        }
    }
}