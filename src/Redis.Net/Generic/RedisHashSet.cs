using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Redis.Net.Converters;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Reids HashSet
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public partial class RedisHashSet<TKey, TValue> : ReadOnlyRedisHashSet<TKey, TValue>, IHashSet<TKey, TValue>,
        IAsyncHashSet<TKey, TValue>, IBatchHashSet<TKey, TValue>
        where TKey : IConvertible where TValue : IConvertible {

            public RedisHashSet (IDatabase database, RedisKey setKey) : base (database, setKey) {
                if (typeof (TValue).IsEnum) {

                }
            }

            [MethodImpl (MethodImplOptions.AggressiveInlining)]
            private RedisValue Unbox<T> (T value) where T : IConvertible {
                return RedisConvertFactory.ToRedisValue<T> (value);
            }

            #region Implementation of IRedisHash<TKey,TValue>

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
            /// </summary>
            public void Add (TKey key, TValue value) {
                Database.HashSet (SetKey, Unbox (key), Unbox (value));
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
            /// </summary>
            public void Add (params Tuple<TKey, TValue>[] tuples) {
                if (tuples == null || tuples.Length == 0) {
                    return;
                }
                var entities = tuples.Select (t => new HashEntry (Unbox (t.Item1), Unbox ((t.Item2))))
                    .ToArray ();
                Database.HashSet (SetKey, entities);
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
            /// </summary>
            public void Add (params KeyValuePair<TKey, TValue>[] pairs) {
                if (pairs == null || pairs.Length == 0) {
                    return;
                }
                var entities = pairs.Select (t => new HashEntry (Unbox (t.Key), Unbox ((t.Value))))
                    .ToArray ();
                Database.HashSet (SetKey, entities);
            }

            /// <summary>
            /// 根据 key 删除Hash条目
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public bool Remove (TKey key) {
                return Database.HashDelete (SetKey, Unbox (key));
            }

            #endregion

            // #region  Increment/Decrement

            // /// <summary>
            // /// 将存储在键上的哈希的指定字段递减，并用指定的递减量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
            // /// </summary>
            // /// <param name="hashField"></param>
            // /// <param name="value"></param>
            // /// <returns></returns>
            // public long Decrement (TKey hashField, long value = 1) {
            //     return Database.HashDecrement (this.SetKey, ToRedisValue (hashField), value);
            // }

            // /// <summary>
            // /// 将存储在键上的哈希的指定字段递减，并用指定的递减量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
            // /// </summary>
            // /// <param name="hashField"></param>
            // /// <param name="value"></param>
            // /// <returns></returns>
            // public double Decrement (TKey hashField, double value) {
            //     return Database.HashDecrement (this.SetKey, ToRedisValue (hashField), value);
            // }

            // /// <summary>
            // /// 将存储在键上的哈希的指定字段递增，并用指定的增量表示浮点数。如果该字段不存在，则在执行操作之前将其设置为0。
            // /// </summary>
            // /// <param name="hashField"></param>
            // /// <param name="value"></param>
            // /// <returns></returns>
            // public double Increment (TKey hashField, double value) {
            //     return Database.HashIncrement (this.SetKey, ToRedisValue (hashField), value);
            // }

            // #endregion

            /// <summary>
            /// careate batch methods instance
            /// </summary>
            /// <returns></returns>
            public IBatchHashSet<TKey, TValue> AsBatch () {
                return (IBatchHashSet<TKey, TValue>) this;
            }

            public IAsyncHashSet<TKey, TValue> AsAsync () {
                return this;
            }

            public ReadOnlyRedisHashSet<TKey, TValue> AsReadonly () {
                return this;
            }
        }
}