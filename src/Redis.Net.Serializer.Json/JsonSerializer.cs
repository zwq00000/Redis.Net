using Newtonsoft.Json;
using Redis.Net.Core;
using System.Text;

namespace Redis.Net.Serializer {
    public class JsonSerializer : ISerializer {

        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly JsonSerializer DefaultInstance = new JsonSerializer();

        /// <inheritdoc />
        public object Deserialize(byte[] serializedObject) {
            var jsonStr = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject(jsonStr);
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedObject) {
            var jsonStr = Encoding.UTF8.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <inheritdoc />
        public byte[] Serialize(object item) {
            var jsonStr = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(jsonStr);
        }
    }
}