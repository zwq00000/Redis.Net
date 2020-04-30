using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redis.Net.Generic;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Set for entityId:{tags} set
    /// </summary>
    public class RedisTagsSet : RedisMutiKey {
        private readonly IDatabase _database;

        /// <summary>
        /// ShipTags Default
        /// </summary>
        private const string DefaultKey = "Tags:";
        private const string IndexSetName = "__index";

        /// <summary>
        /// 倒排索引
        /// </summary>
        private InvertedIndexSet<string> _indexSet;

        public RedisTagsSet (IDatabase database, string baseKey = DefaultKey) : base (database, baseKey) {
            _database = database;
            _indexSet = new InvertedIndexSet<string> (database, GetIndexSetKey (IndexSetName));
        }

        #region  _invertedIndex

        private string GetIndexSetKey (string indexName) {
            var baseKey = BaseKey.ToString ();
            return baseKey.Substring (0, baseKey.Length - 2) + indexName;
        }

        /// <summary>
        /// 重置全部标记,同时清除索引
        /// </summary>
        /// <returns></returns>
        public async Task ResetAsync () {
            var batch = Database.CreateBatch ();
            await _indexSet.ResetAsync (batch);
            var keys = await this.GetKeysAsync ();
            foreach (var key in keys) {
                // await base.RemoveBatch (batch, key);
                var task = batch.KeyDeleteAsync (key);
            }
            batch.Execute ();
        }

        /// <summary>
        /// 获取全部标记
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllTags () {
            return _indexSet.GetIds ().Select (v => v.ToString ());
        }

        /// <summary>
        /// 获取全部标记的异步方法
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAllTagsAsync () {
            return await _indexSet.GetIdsAsync ();
        }

        /// <summary>
        /// 根据标记获取 <c>entityId</c> 集合
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IEnumerable<string> GetByTag (string tag) {
            return _indexSet.GetValues (tag).Select (v => v.ToString ());
        }

        /// <summary>
        /// 根据标记获取 <c>entityId</c> 集合的异步方法
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetByTagAsync (string tag) {
            var ids = await _indexSet.GetValuesAsync (tag);
            return ids.Select (v => v.ToString ());
        }

        #endregion

        /// <summary>
        /// 过滤合法的标记
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private RedisValue[] FilterTags (string[] tags) {
            return tags.Where (t => !string.IsNullOrWhiteSpace (t))
                .Select (t => (RedisValue) t.Trim ()).ToArray ();
        }

        /// <summary>
        /// 增加标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual long AddTags (string entityId, params string[] tags) {
            if (string.IsNullOrEmpty (entityId)) {
                throw new ArgumentNullException (nameof (entityId));
            }

            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            if (values.Any ()) {
                _indexSet.Add (entityId, values);
                return _database.SetAdd (setKey, values);
            }

            return 0;
        }

        /// <summary>
        /// 删除指定标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual bool RemoveTags (string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            if (values.Any ()) {
                _indexSet.Remove (entityId, values);
                return _database.SetRemove (setKey, values) == tags.Length;
            }

            return false;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual bool RemoveAllTags (string entityId) {
            var tags = this.GetTags (entityId).Select (t => RedisValue.Unbox (t)).ToArray ();
            _indexSet.Remove (entityId, tags);
            return Remove (entityId);
        }

        /// <summary>
        /// 获取 标记,一个Id 可以有多个 Tag
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTags (string entityId) {
            var setKey = GetSubKey (entityId);
            return _database.SetMembers (setKey)
                .Where (m => m.HasValue)
                .Select (m => m.ToString ());
        }

        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag (string entityId, string tag) {
            var setKey = GetSubKey (entityId);
            return _database.SetContains (setKey, tag);
        }

        /// <summary>
        /// 获取全部标记
        /// </summary>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        public IEnumerable < (string, IEnumerable<string>) > GetAll (IEnumerable<string> entityIds) {
            foreach (var entityId in entityIds.Where (s => !string.IsNullOrEmpty (s))) {
                yield return (entityId, GetTags (entityId));
            }
        }

        #region Async
        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual async Task<long> AddTagAsync (string entityId, params string[] tags) {
            if (string.IsNullOrEmpty (entityId)) {
                throw new ArgumentNullException (nameof (entityId));
            }

            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            if (values.Any ()) {
                await _indexSet.AddAsync (entityId, values);
                return await _database.SetAddAsync (setKey, values);
            }

            return 0;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> RemoveAllTagsAsync (string entityId) {
            var tags = this.GetTags (entityId).Select (t => RedisValue.Unbox (t)).ToArray ();
            await _indexSet.RemoveAsync (entityId, tags);
            return await RemoveAsync (entityId);
        }

        /// <summary>
        /// 删除指定标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual async Task<long> RemoveTagAsync (string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            await _indexSet.RemoveAsync (entityId, values);
            return await Database.SetRemoveAsync (setKey, values);
        }

        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<bool> HasTagAsync (string entityId, string tag) {
            var setKey = GetSubKey (entityId);
            return await Database.SetContainsAsync (setKey, tag);
        }

        #endregion

        #region batch

        /// <summary>
        /// 批量增加标记
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual Task BatchAddTags (IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            batch.SetAddAsync (setKey, values);
            _indexSet.BatchAdd (batch, entityId, values);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量移除标记
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual Task BatchRemoveTags (IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            batch.SetRemoveAsync (setKey, values);
            _indexSet.BatchRemove (batch, entityId, values);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量删除全部标记
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual Task BatchAllRemoveTags (IBatch batch, string entityId) {
            var setKey = GetSubKey (entityId);
            var values = this.GetTags (entityId).Select (t => RedisValue.Unbox (t)).ToArray ();
            batch.SetRemoveAsync (setKey, values);
            _indexSet.BatchRemove (batch, entityId, values);
            return Task.CompletedTask;
        }

        #endregion
    }
}