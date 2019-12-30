using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// Redis 多键 Set/Hashset 基类
    /// </summary>
    public abstract class RedisMutiKey<TKey> where TKey : IConvertible {
        /// <summary>
        /// 默认 ShipTracks Redis Key 前缀
        /// </summary>
        public readonly RedisKey BaseKey;

        /// <summary>
        /// Redis HashSet Index Set
        /// </summary>
        private readonly RedisSet<TKey> _indexSet;
        /// <summary>
        /// 过时的 Key 索引
        /// </summary>
        private readonly RedisSet<TKey> _expireIndexSet;

        protected IDatabase Database { get; }

        protected RedisMutiKey (IDatabase database, string baseKey) {
            if (string.IsNullOrWhiteSpace (baseKey)) {
                throw new ArgumentNullException (nameof (baseKey));
            }
            this.Database = database;
            if (!baseKey.EndsWith (":")) {
                baseKey = baseKey + ":";
            }
            this.BaseKey = baseKey;
            this._indexSet = new RedisSet<TKey> (database, BaseKey.Append ("@__SetIndex"));
            this._expireIndexSet = new RedisSet<TKey> (database, BaseKey.Append ("@__ExpireIndex"));
        }

        /// <summary>
        /// 根据 Id 获取 RedisKey
        /// <see cref="RedisKey.Append"/>,生成 "{baseKey}:{id}" 的二级结构键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected RedisKey GetEntryKey (TKey key) {
            return BaseKey.Append (key.ToString ());
        }

        public bool Remove (TKey key) {
            var setKey = GetEntryKey (key);
            _indexSet.Remove (key);
            return Database.KeyDelete (setKey);
        }

        protected async Task<bool> RemoveAsync (TKey key) {
            var setKey = GetEntryKey (key);
            await _indexSet.RemoveAsync (key);
            return await Database.KeyDeleteAsync (setKey);
        }

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey (TKey key) {
            var result = _indexSet.Contains (key);;
            if (!result) {
                return this.CheckExpired (key);
            }
            return result;
        }

        /// <summary>
        /// 检查过去 Set 是否存在,如果不存在,则清除 过期索引
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckExpired (TKey key) {
            if (!_expireIndexSet.Contains (key)) {
                return false;
            }
            var setKey = GetEntryKey (key);
            var setExiste = Database.KeyExists (setKey);
            if (!setExiste) {
                _expireIndexSet.Remove (key);
            }
            return setExiste;
        }

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected long OnAdded (params TKey[] keys) {
            return _indexSet.AddRange (keys);
        }

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> OnAddedAsync (params TKey[] keys) {
            return _indexSet.AddAsync (keys);
        }

        #region Batch

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> OnAddedAsync (IBatch batch, params TKey[] keys) {
            return _indexSet.AddAsync (batch, keys);
        }

        /// <summary>
        /// 增加 Hashset ,同时更新索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        protected Task AddHashSetBatch (IBatch batch, TKey key, IEnumerable<HashEntry> entries) {
            var setKey = GetEntryKey (key);
            _indexSet.AddAsync (batch, key);
            batch.HashSetAsync (setKey, entries.ToArray ());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新 Hashset ,不更新索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        protected Task UpdateHashSetBatch (IBatch batch, TKey key, IEnumerable<HashEntry> entries) {
            var setKey = GetEntryKey (key);
            batch.HashSetAsync (setKey, entries.ToArray ());
            return Task.CompletedTask;
        }

        /// <summary>
        /// 删除集合时 同时从索引中删除键 的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected Task<bool> RemoveBatch (IBatch batch, TKey key) {
            var setKey = GetEntryKey (key);
            _indexSet.RemoveAsync (batch, key);
            return batch.KeyDeleteAsync (setKey);
        }

        /// <summary>
        /// 删除多个索引键
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected Task<long> RemoveBatch (IBatch batch, TKey[] keys) {
            var setKeys = keys.Select (GetEntryKey).ToArray ();
            _indexSet.RemoveAsync (batch, keys);
            return batch.KeyDeleteAsync (setKeys);
        }

        /// <summary>
        /// 删除集合时 同时从索引中迁移键值到 过期索引中 的批处理方法
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        protected Task<bool> ExpireBatch (IBatch batch, TKey key, TimeSpan? expiry) {
            var setKey = GetEntryKey (key);
            batch.SetMoveAsync (_indexSet.SetKey, _expireIndexSet.SetKey, RedisValue.Unbox (key));
            return batch.KeyExpireAsync (setKey, expiry);
        }

        /// <summary>
        /// 检查 Redis Key 是否存在,并从过期索引中清除
        /// </summary>
        /// <returns></returns>
        protected async Task ClearExpiredIndexAsync () {
            var keys = _expireIndexSet.ToArray ();
            var expiredKeys = new List<TKey> ();
            foreach (var key in keys) {
                var setKey = GetEntryKey (key);
                if (!await Database.KeyExistsAsync (setKey)) {
                    expiredKeys.Add (key);
                }
            }
            _expireIndexSet.RemoveRange (expiredKeys.ToArray ());
        }

        #endregion

        protected TKey KeyConvert (RedisValue value) {
            return (TKey) ((IConvertible) value).ToType (typeof (TKey), CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// 返回 合并索引键值
        /// </summary>
        /// <returns>An enumerable collection that contains the keys in the read-only dictionary.</returns>
        public ICollection<TKey> Keys {
            get {
                var members = Database.SetCombine (SetOperation.Union, _indexSet.SetKey, _expireIndexSet.SetKey);
                return Array.ConvertAll (members, KeyConvert);
            }
        }

        /// <summary>
        /// 索引集合
        /// </summary>
        protected ReadOnlyRedisSet<TKey> IndexSet {
            get { return _indexSet; }
        }

        /// <summary>
        /// 根据 <see cref="BaseKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        protected RedisKey[] GetKeys () {
            return Database.GetKeys (this.BaseKey);
        }

        /// <summary>
        /// 根据 <see cref="BaseKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        protected async Task<RedisKey[]> GetKeysAsync () {
            return await Database.GetKeysAsync (this.BaseKey);
        }

        public int Count => IndexSet.Count;
    }
}