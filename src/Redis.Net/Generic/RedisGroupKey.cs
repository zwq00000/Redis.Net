using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis 分组容器基类
    /// </summary>
    public abstract class RedisGroupKey<TKey> where TKey:IConvertible {
        /// <summary>
        /// 默认 Redis Key 前缀, ':' 字符结尾
        /// </summary>
        public readonly RedisKey PrefixKey;

        /// <summary>
        /// Keys Index set
        /// </summary>
        private readonly RedisSet<TKey> _indexSet;

        protected IDatabase Database { get; }

        protected RedisGroupKey(IDatabase database, string prefixKey) {
            if (string.IsNullOrWhiteSpace(prefixKey)) {
                throw new ArgumentNullException(nameof(prefixKey));
            }
            this.Database = database;
            if (!prefixKey.EndsWith(":")) {
                prefixKey = prefixKey + ":";
            }
            this.PrefixKey = prefixKey;
            this._indexSet = new RedisSet<TKey>(database, PrefixKey.Append("@__SetIndex"));
        }

        /// <summary>
        /// 根据 Id 获取 RedisKey
        /// <see cref="RedisKey.Append"/>,生成 "{baseKey}:{id}" 的二级结构键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected RedisKey GetEntryKey(TKey key) {
            return PrefixKey.Append(key.ToString());
        }

        public bool Remove(TKey key) {
            var setKey = GetEntryKey(key);
            _indexSet.Remove(key);
            return Database.KeyDelete(setKey);
        }

        protected async Task<bool> RemoveAsync(TKey key) {
            var setKey = GetEntryKey(key);
            await _indexSet.RemoveAsync(key);
            return await Database.KeyDeleteAsync(setKey);
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) {
            return _indexSet.Contains(key);
        }

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected long OnAdded(params TKey[] keys) {
            return _indexSet.AddRange(keys);
        }

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> OnAddedAsync(params TKey[] keys) {
            return _indexSet.AddAsync(keys);
        }

        #region Batch

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> OnAddedAsync(IBatch batch, params TKey[] keys) {
            return _indexSet.AddAsync(batch, keys);
        }

        protected Task<bool> RemoveBatch(IBatch batch, TKey key) {
            var setKey = GetEntryKey(key);
            _indexSet.RemoveAsync(batch, key);
            return batch.KeyDeleteAsync(setKey);
        }

        protected Task<long> RemoveBatch(IBatch batch, TKey[] keys) {
            var setKeys = keys.Select(GetEntryKey).ToArray();
            _indexSet.RemoveAsync(batch, keys);
            return batch.KeyDeleteAsync(setKeys);
        }

        #endregion

        public ICollection<TKey> Keys => IndexSet.Values;

        /// <summary>
        /// 索引集合
        /// </summary>
        protected ReadOnlyRedisSet<TKey> IndexSet {
            get { return _indexSet; }
        }

        /// <summary>
        /// 根据 <see cref="PrefixKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        protected RedisKey[] GetKeys() {
            return Database.GetKeys(this.PrefixKey);
        }

        /// <summary>
        /// 根据 <see cref="PrefixKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        protected async Task<RedisKey[]> GetKeysAsync() {
            return await Database.GetKeysAsync(this.PrefixKey);
        }

        public int Count => IndexSet.Count;
    }
}