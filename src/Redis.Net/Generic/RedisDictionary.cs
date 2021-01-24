using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Serializer;
using StackExchange.Redis;

namespace Redis.Net.Generic {

    /// <summary>
    /// 基于 redis Hashset 的 泛型字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class RedisDictionary<TKey, TValue> : ReadonlyRedisDictionary<TKey, TValue>, IHashSet<TKey, TValue>,
        IAsyncHashSet<TKey, TValue>, IBatchHashSet<TKey, TValue>
        where TKey : IConvertible {

            public RedisDictionary (IDatabase database, RedisKey setKey, ISerializer serializer) 
            : base (database, setKey, serializer.Deserialize<TValue>) {
                this.Serializer = serializer;
            }

            public RedisDictionary (IDatabase database, RedisKey setKey) : this (database, setKey, DefaultSerializer.Default) { }

            private ISerializer Serializer { get; }

            /// <summary>
            /// careate batch methods instance
            /// </summary>
            /// <returns></returns>
            public IBatchEntrySet<TKey, TValue> AsBatch () {
                return (IBatchEntrySet<TKey, TValue>) this;
            }

            public IAsyncEntrySet<TKey, TValue> AsAsync () {
                return (IAsyncEntrySet<TKey, TValue>) this;
            }

            public void Add (TKey key, TValue value) {
                Database.HashSet (SetKey, RedisValue.Unbox (key), Serializer.Serialize (value));
            }

            public void Add (params Tuple<TKey, TValue>[] tuples) {
                var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), Serializer.Serialize (t.Item2)))
                    .ToArray ();
                Database.HashSet (SetKey, entities);
            }

            public void Add (params KeyValuePair<TKey, TValue>[] pairs) {
                var entities = pairs.Select (t => new HashEntry (RedisValue.Unbox (t.Key), Serializer.Serialize (t.Value)))
                    .ToArray ();
                Database.HashSet (SetKey, entities);
            }

            public bool Remove (TKey key) {
                return Database.HashDelete (SetKey, RedisValue.Unbox (key));
            }

            async Task IAsyncHashSet<TKey, TValue>.AddAsync (TKey key, TValue value) {
                await Database.HashSetAsync (SetKey, RedisValue.Unbox (key), Serializer.Serialize (value));
            }

            async Task IAsyncHashSet<TKey, TValue>.AddAsync (params Tuple<TKey, TValue>[] tuples) {
                var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), Serializer.Serialize (t.Item2)))
                    .ToArray ();
                await Database.HashSetAsync (SetKey, entities);
            }

            async Task IAsyncHashSet<TKey, TValue>.AddAsync (params KeyValuePair<TKey, TValue>[] pairs) {
                var entities = pairs.Select (t => new HashEntry (RedisValue.Unbox (t.Key), Serializer.Serialize (t.Value)))
                    .ToArray ();
                await Database.HashSetAsync (SetKey, entities);
            }

            async Task<bool> IAsyncHashSet<TKey, TValue>.RemoveAsync (TKey key) {
                return await Database.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
            }

            Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, TKey key, TValue value) {
                return batch.HashSetAsync (SetKey, RedisValue.Unbox (key), Serializer.Serialize (value));
            }

            Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, params Tuple<TKey, TValue>[] tuples) {
                if (tuples == null || tuples.Length == 0) {
                    return Task.CompletedTask;
                }

                var entities = tuples.Select (t => new HashEntry (RedisValue.Unbox (t.Item1), Serializer.Serialize (t.Item2)))
                    .ToArray ();
                return batch.HashSetAsync (SetKey, entities);
            }

            Task IBatchHashSet<TKey, TValue>.BatchAdd (IBatch batch, params KeyValuePair<TKey, TValue>[] pairs) {
                if (pairs == null || pairs.Length == 0) {
                    return Task.CompletedTask;
                }

                var entities = pairs.Select (t => new HashEntry (RedisValue.Unbox (t.Key), Serializer.Serialize (t.Value)))
                    .ToArray ();
                return batch.HashSetAsync (SetKey, entities);
            }

            Task<bool> IBatchHashSet<TKey, TValue>.BatchRemove (IBatch batch, TKey key) {
                return batch.HashDeleteAsync (SetKey, RedisValue.Unbox (key));
            }

        }

}