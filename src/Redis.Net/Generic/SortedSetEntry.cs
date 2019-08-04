using System;
using System.Globalization;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Generic SorteSetEntry
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public struct SortedSetEntry<TValue>  where TValue : IConvertible {
        private SortedSetEntry _entry;

        /// <summary>
        /// Initializes a <see cref="SortedSetEntry"/> value.
        /// </summary>
        /// <param name="element">The <see cref="RedisValue"/> to get an entry for.</param>
        /// <param name="score">The redis score for <paramref name="element"/>.</param>
        public SortedSetEntry(TValue element, double score) {
            this._entry = new SortedSetEntry(RedisValue.Unbox(element),score);
        }

        /// <summary>
        /// The unique element stored in the sorted set
        /// </summary>
        public TValue Element => (TValue)((IConvertible)_entry.Element).ToType(typeof(TValue), CultureInfo.CurrentCulture);

        /// <summary>
        /// The score against the element
        /// </summary>
        public double Score => _entry.Score;

        public SortedSetEntry ToEntry() {
            return this._entry;
        }
    }
}