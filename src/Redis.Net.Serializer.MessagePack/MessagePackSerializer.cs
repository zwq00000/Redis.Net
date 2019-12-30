using MessagePack;
using Redis.Net.Core;
using System;

namespace Redis.Net.Serializer {
    public class MessagePackSerializer : ISerializer {
        private readonly MessagePackSerializerOptions _options;

        public MessagePackSerializer() :this(MessagePackSerializerOptions.Standard){

        }

        public MessagePackSerializer(MessagePackSerializerOptions options) {
            this._options = options;
        }

        #region Implementation of ISerializer

        /// <summary>
        /// deserialize bytes to Object
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public object Deserialize(byte[] serializedObject) {
            return this.Deserialize<object>(serializedObject);
        }

        /// <summary>
        /// deserialize to <see cref="T:T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] serializedObject) {
            var memory = new ReadOnlyMemory<byte>(serializedObject);
            return MessagePack.MessagePackSerializer.Deserialize<T>(memory, _options);
        }

        /// <summary>
        /// serialize a object to bytes
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public byte[] Serialize(object item) {
            return MessagePack.MessagePackSerializer.Serialize(item.GetType(), _options);
        }

        #endregion
    }
}
