using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Redis.Net.Core;
using StackExchange.Redis;

namespace Redis.Net.Generic
{
    /// <summary>
    /// 只读的 Redis Hash 字典
    /// 通过对类型序列化,建立 K/V 字典, 整个集合存放在一个 Redis HashSet 中
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class ReadOnlyRedisDicionary<TKey, TEntity> : IReadOnlyDictionary<TKey, TEntity> where TKey : IConvertible where TEntity : class {
        public ReadOnlyRedisDicionary (IDatabase database, string setKey) {
            InnerSet = new RedisHashSet (database, setKey);
        }

        protected TKey ConvertKey (RedisValue key) {
            return (TKey) ((IConvertible) key).ToType (typeof (TKey), CultureInfo.CurrentCulture);
        }

        public IEnumerable<TKey> Keys => InnerSet.Keys.Select (ConvertKey);

        public IEnumerable<TEntity> Values =>
            InnerSet.Values.Select (Deserialize);

        public int Count => InnerSet.Count;

        protected RedisHashSet InnerSet { get; }

        public TEntity this [TKey key] {
            get {
                if (this.TryGetValue (key, out var value)) {
                    return value;
                }
                return null;
            }
        }

        /// <summary>
        /// 序列化实体对象,默认采用 <see ref="JsonConvert.SerializeObject" />
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual byte[] Serialize (TEntity entity) {
            return entity.SerializeObject<TEntity> ();
        }

        /// <summary>
        /// 反序列化实体对象,默认采用 <see ref="JsonConvert.DeserializeObject:(TEntity)" />
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        protected virtual TEntity Deserialize (RedisValue rawValue) {
            return rawValue.DeserializeObject<TEntity> ();
        }

        public bool ContainsKey (TKey key) {
            return InnerSet.ContainsKey (RedisValue.Unbox (key));
        }

        public bool TryGetValue (TKey key, out TEntity value) {
            if (InnerSet.TryGetValue (RedisValue.Unbox (key), out var rawValue)) {
                value = this.Deserialize (rawValue);
                return true;
            }
            value = default (TEntity);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TEntity>> GetEnumerator () {
            foreach (var pair in InnerSet) {
                yield return new KeyValuePair<TKey, TEntity> (this.ConvertKey (pair.Key), this.Deserialize (pair.Value));
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }
    }
}