using System.Text.Json;
using StackExchange.Redis;

namespace Redis.Net.Serializer {
    public class JsonSerializer : ISerializer {

        /// <summary>
        /// The _settings
        /// </summary>
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        public JsonSerializer () : this (new JsonSerializerOptions ()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="options">The settings.</param>
        public JsonSerializer (JsonSerializerOptions options) {
            _options = options;
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public RedisValue Serialize<T> (T value) {
            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T> (value, _options);
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public T Deserialize<T> (RedisValue value) {
            return System.Text.Json.JsonSerializer.Deserialize<T> (value, _options);
        }
    }
}