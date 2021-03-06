﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;

namespace Redis.Net.Redis.Generic {
    /// <summary>
    /// Redis 多键 Set/Hashset 基类
    /// </summary>
    public abstract class RedisMutiKey<TKey> where TKey : IConvertible {
        /// <summary>
        /// 默认 ShipTracks Redis Key 前缀
        /// </summary>
        public readonly RedisKey BaseKey;

        internal const string IndexSetName = "@__SetIndex";

        internal const string ExpireIndexSetName = "@__ExpireIndex";

        /// <summary>
        /// Redis HashSet Index Set
        /// </summary>
        private readonly RedisSet<TKey> _indexSet;
        /// <summary>
        /// 过时的 Key 索引
        /// </summary>
        private readonly RedisSet<TKey> _expireIndexSet;

        protected IDatabase Database { get; }

        /// <summary>
        /// 重建索引,
        /// </summary>
        /// 该方法根据 <see cref="BaseKey"/> 查找以 {BaseKey}开头的键
        /// 删除当前集合索引并加入所有以 {BaseKey}开头的键(不包括 索引集合键)
        /// <returns></returns>
        public async Task RebuildIndexAsync (Func<string, TKey> convert) {
            var entityKeys = ResolveEntiyKeys ().Select (k => convert (k));
            await _indexSet.DeleteAsync ();
            await _indexSet.AddAsync (entityKeys.ToArray ());
        }

        /// <summary>
        /// 重建索引
        /// </summary>
        /// <remarks>
        /// 该方法根据 <see cref="BaseKey"/> 查找以 {BaseKey}开头的键
        /// 删除当前集合索引并加入所有以 {BaseKey}开头的键(不包括 索引集合键)
        /// </remarks>
        /// <returns></returns>
        public void RebuildIndex (Func<string, TKey> convert) {
            var entityKeys = ResolveEntiyKeys ().Select (k => convert (k));
            var batch = Database.CreateBatch ();
            var batchIndexSet = _indexSet.AsBatch();
            var task1 = batchIndexSet.BatchClear (batch);
            var task2 = batchIndexSet.BatchAdd (batch, entityKeys.ToArray ());
            batch.Execute ();
        }

        /// <summary>
        /// 获取 实体键(排除索引集合键)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ResolveEntiyKeys () {
            var keys = GetKeys ();
            var startIndex = this.BaseKey.ToString ().Length;
            foreach (string key in keys) {
                if (!key.EndsWith (IndexSetName) && !key.EndsWith (ExpireIndexSetName)) {
                    yield return key.Substring (startIndex);
                }
            }
        }

        protected RedisMutiKey (IDatabase database, string baseKey) {
            if (string.IsNullOrWhiteSpace (baseKey)) {
                throw new ArgumentNullException (nameof (baseKey));
            }
            this.Database = database;
            if (!baseKey.EndsWith (":")) {
                baseKey = baseKey + ":";
            }
            this.BaseKey = baseKey;
            this._indexSet = new RedisSet<TKey> (database, BaseKey.Append (IndexSetName));
            this._expireIndexSet = new RedisSet<TKey> (database, BaseKey.Append (ExpireIndexSetName));
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

        /// <summary>
        /// 移除 Key,同时移除从 <c>_indexSet</c> 索引中移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool RemoveKey (TKey key) {
            var setKey = GetEntryKey (key);
            _indexSet.Remove (key);
            return Database.KeyDelete (setKey);
        }

        /// <summary>
        /// 异步移除 Key,同时移除从 <c>_indexSet</c> 索引中移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns>是否删除</returns>
        protected async Task<bool> RemoveKeyAsync (TKey key) {
            var setKey = GetEntryKey (key);
            await _indexSet.RemoveAsync (key);
            return await Database.KeyDeleteAsync (setKey);
        }

        /// <summary>
        /// 异步移除 Key,同时移除从 <c>_indexSet</c> 索引中移除
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>删除数量</returns>
        protected async Task<long> RemoveKeyAsync (params TKey[] keys) {
            var setKeys = keys.Select (k => GetEntryKey (k)).ToArray ();
            await _indexSet.RemoveAsync (keys);
            return await Database.KeyDeleteAsync (setKeys);
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
        protected long AddKeyIndex (params TKey[] keys) {
            return _indexSet.AddRange (keys);
        }

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> AddKeyIndexAsync (params TKey[] keys) {
            return _indexSet.AddAsync (keys);
        }

        /// <summary>
        /// 清除索引
        /// </summary>
        protected void ClearKeyIndex () {
            _indexSet.Clear ();
        }

        /// <summary>
        /// 清除全部索引
        /// </summary>
        protected void ClearAllIndex () {
            _indexSet.Clear ();
            _expireIndexSet.Clear ();
        }

        /// <summary>
        /// 设置过期时间
        /// 如果<c>expiry</c>为空为清除超期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        protected bool SetExpire (TKey key, TimeSpan? expiry) {
            var setKey = GetEntryKey (key);
            if (!Database.KeyExists (setKey)) {
                //Key 不存在
                return false;
            }
            if (expiry.HasValue) {
                //移动setKey 到超期索引
                Database.SetMove (_indexSet.SetKey, _expireIndexSet.SetKey, RedisValue.Unbox (key));
                return Database.KeyExpire (setKey, expiry);
            } else {
                //清除过期时间
                //移动setKey 到SetKey 索引
                Database.SetMove (_expireIndexSet.SetKey, _indexSet.SetKey, RedisValue.Unbox (key));
                return Database.KeyExpire (setKey, expiry);
            }
        }

        protected async Task<bool> SetExpireAsync (TKey key, TimeSpan? expiry) {
            var setKey = GetEntryKey (key);
            if (!await Database.KeyExistsAsync (setKey)) {
                //Key 不存在
                return false;
            }
            if (expiry.HasValue) {
                //移动setKey 到超期索引
                await Database.SetMoveAsync (_indexSet.SetKey, _expireIndexSet.SetKey, RedisValue.Unbox (key));
                return await Database.KeyExpireAsync (setKey, expiry);
            } else {
                //清除过期时间
                //移动setKey 到SetKey 索引
                await Database.SetMoveAsync (_expireIndexSet.SetKey, _indexSet.SetKey, RedisValue.Unbox (key));
                return await Database.KeyExpireAsync (setKey, expiry);
            }
        }

        #region Batch

        /// <summary>
        /// 增加集合时调用此方法,更新索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        protected virtual Task<long> BatchOnAdded (IBatch batch, params TKey[] keys) {
            return _indexSet.AsBatch().BatchAdd (batch, keys);
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
            _indexSet.AsBatch().BatchAdd (batch, key);
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
            _indexSet.AsBatch().BatchRemove (batch, key);
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
            _indexSet.AsBatch().BatchRemove (batch, keys);
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

        /// <summary>
        /// RedisMutiKey 管理的数据库键数量
        /// </summary>
        public int Count => IndexSet.Count;
    }
}