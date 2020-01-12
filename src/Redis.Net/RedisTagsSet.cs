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

        private RedisKey __AllTagsSetKey;

        /// <summary>
        /// All Tags 
        /// </summary>
        private RedisSet<string> _allTags;

        public RedisTagsSet (IDatabase database, string baseKey = DefaultKey) : base (database, baseKey) {
            _database = database;
            this.__AllTagsSetKey = base.GetSubKey("__ALLTAGS");
            this._allTags = new RedisSet<string>(database,__AllTagsSetKey);
        }

        private RedisValue[] FilterTags (string[] tags) {
            return tags.Where (t => !string.IsNullOrWhiteSpace (t))
                .Select (t => (RedisValue) t.Trim ()).ToArray ();
        }

        #region  Tags Methods

        /// <summary>
        /// 获取全部标记
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllTags () {
            return _allTags.ToArray();
        }

        /// <summary>
        /// remove tag and remove releated entity tag
        /// </summary>
        /// <param name="tag"></param>
        public void DeleteTag(string tag){
            var entities = this.GetAllEntities();
            var batch = Database.CreateBatch();
            foreach(var id in entities){
                this.RemoveTagsBatch(batch,id,tag);
            }
            batch.SetRemoveAsync(__AllTagsSetKey,tag);
            batch.Execute();
        }

        #endregion

        /// <summary>
        /// 增加标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual long AddTags (string entityId, params string[] tags) {
            if (string.IsNullOrEmpty (entityId)) {
                throw new ArgumentNullException (nameof (entityId));
            }

            var values = FilterTags (tags);
            if (values.Any ()) {
                var setKey = GetSubKey (entityId);
                _database.SetAdd(__AllTagsSetKey,values);
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
                return _database.SetRemove (setKey, values) == tags.Length;
            }

            return false;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual bool RemoveTags (string entityId) {
            return Remove (entityId);
        }

        /// <summary>
        /// 获取 船舶标记
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

        public IEnumerable<string> GetAllEntities(){
            var len = this.BaseKey.ToString().Length;
            return base.GetKeys().Where(k=>k!=__AllTagsSetKey)
            .Select(k=>k.ToString().Substring(len));
        }

        #region Async
        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual async Task<long> AddTagsAsync (string entityId, params string[] tags) {
            if (string.IsNullOrEmpty (entityId)) {
                throw new ArgumentNullException (nameof (entityId));
            }

            var values = FilterTags (tags);
            if (values.Any ()) {
                var setKey = GetSubKey (entityId);
                return await _database.SetAddAsync (setKey, values);
            }

            return 0;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> RemoveTagsAsync (string entityId) {
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

        public virtual Task AddTagsBatch (IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            batch.SetAddAsync (setKey, values);
            return Task.CompletedTask;
        }

        public virtual Task RemoveTagsBatch (IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey (entityId);
            var values = FilterTags (tags);
            batch.SetRemoveAsync (setKey, values);
            return Task.CompletedTask;
        }

        #endregion
    }
}