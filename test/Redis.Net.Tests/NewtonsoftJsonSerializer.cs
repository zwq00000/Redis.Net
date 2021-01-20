using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Net.Tests {
    internal class NewtonsoftJsonSerializer : ISerializer {

        /// <summary>
        /// The _settings
        /// </summary>
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        public NewtonsoftJsonSerializer () : this (new JsonSerializerSettings ()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public NewtonsoftJsonSerializer (JsonSerializerSettings settings) {
            _settings = settings;
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public RedisValue Serialize<T> (T value) {
            return JsonConvert.SerializeObject (value, _settings);
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public T Deserialize<T> (RedisValue value) {
            return JsonConvert.DeserializeObject<T> (value, _settings);
        }
    }
}