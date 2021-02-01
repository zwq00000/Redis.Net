using System.Text.Json;
using StackExchange.Redis;

namespace Redis.Net.Serializer {
    /// <summary>
    /// 默认序列化/反序列化工具, 使用 <see cref="JsonSerializer" />作为序列化方法
    /// </summary>
    public class DefaultSerializer : ISerializer {

        internal static ISerializer Default = new DefaultSerializer ();

        /// <summary>
        /// The _settings
        /// </summary>
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializer"/> class.
        /// </summary>
        public DefaultSerializer () : this (new JsonSerializerOptions () {
            PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
        }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializer"/> class.
        /// </summary>
        /// <param name="options">The settings.</param>
        public DefaultSerializer (JsonSerializerOptions options) {
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