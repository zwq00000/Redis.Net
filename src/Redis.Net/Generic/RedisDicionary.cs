using Redis.Net.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis Hash 字典
    /// 通过对类型序列化,建立 K/V 字典, 整个集合存放在一个 Redis HashSet 中
    /// 必须实现 <see ref="SerializeFactory.Serialize" />
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class RedisDicionary<TKey, TEntity> : ReadOnlyRedisDicionary<TKey, TEntity>, IDictionary<TKey, TEntity> where TKey : IConvertible where TEntity : class {
        public RedisDicionary(IDatabase database, string setKey) : base(database, setKey) { }

        TEntity IDictionary<TKey, TEntity>.this[TKey key] {
            get {
                if (key == null) {
                    throw new ArgumentNullException(nameof(key));
                }
                var redisKey = RedisValue.Unbox(key);
                return base.Deserialize(InnerSet[redisKey]);
            }
            set {
                if (key == null) {
                    throw new ArgumentNullException(nameof(key));
                }
                if (value == null) {
                    InnerSet.Remove(RedisValue.Unbox(key));
                }
                this.Add(key, value);
            }
        }

        public bool IsReadOnly => false;

        /// <inheritdoc />
        ICollection<TKey> IDictionary<TKey, TEntity>.Keys => base.Keys.ToList();

        /// <inheritdoc />
        ICollection<TEntity> IDictionary<TKey, TEntity>.Values =>
            base.Values.ToList();

        /// <inheritdoc />
        public void Add(TKey key, TEntity value) {
            InnerSet.Add(RedisValue.Unbox(key), base.Serialize(value));
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TEntity> item) {
            this.Add(item.Key, item.Value);
        }

        public async Task AddAsync(TKey key, TEntity value) {
            await InnerSet.AddAsync(RedisValue.Unbox(key), base.Serialize(value));
        }

        /// <inheritdoc />
        public void Clear() {
            InnerSet.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TEntity> item) {
            if (InnerSet.TryGetValue(RedisValue.Unbox(item.Key), out var value)) {
                var serial = value.DeserializeObject<TEntity>();
                return Equals(item.Value, serial);
            }
            return false;
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<TKey, TEntity>>.CopyTo(KeyValuePair<TKey, TEntity>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Remove(TKey key) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }
            return InnerSet.Remove(RedisValue.Unbox(key));
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TEntity> item) {
            return InnerSet.Remove(RedisValue.Unbox(item.Key));
        }
    }
}