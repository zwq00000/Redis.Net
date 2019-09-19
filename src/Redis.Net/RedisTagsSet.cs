using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Set for entityId:{tags} set
    /// </summary>
    public class RedisTagsSet: RedisMutiKey {
        private readonly IDatabase _database;

        /// <summary>
        /// ShipTags Default
        /// </summary>
        private const string DefaultKey = "Tags:";


        public RedisTagsSet(IDatabase database, string baseKey = DefaultKey):base(database,baseKey) {
            _database = database;
        }

        private RedisValue[] FilterTags(string[] tags) {
            return tags.Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => (RedisValue)t.Trim()).ToArray();
        }

        /// <summary>
        /// 增加标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual long AddTags(string entityId, params string[] tags) {
            if (string.IsNullOrEmpty(entityId)) {
                throw new ArgumentNullException(nameof(entityId));
            }

            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            if (values.Any()) {
                return _database.SetAdd(setKey, values);
            }

            return 0;
        }

        /// <summary>
        /// 删除指定标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual bool RemoveTags(string entityId,params string[] tags) {
            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            if (values.Any()) {
                return _database.SetRemove(setKey, values) == tags.Length;
            }

            return false;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual bool RemoveTags(string entityId) {
            return Remove(entityId);
        }

        /// <summary>
        /// 获取 船舶标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTags(string entityId) {
            var setKey = GetSubKey(entityId);
            return _database.SetMembers(setKey)
                .Where(m => m.HasValue)
                .Select(m => m.ToString());
        }

        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(string entityId, string tag) {
            var setKey = GetSubKey(entityId);
            return _database.SetContains(setKey, tag);
        }

        /// <summary>
        /// 获取全部标记
        /// </summary>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        public IEnumerable<(string, IEnumerable<string>)> GetAll(IEnumerable<string> entityIds) {
            foreach (var entityId in entityIds.Where(s => !string.IsNullOrEmpty(s))) {
                yield return (entityId, GetTags(entityId));
            }
        }

        #region Async
        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual async Task<long> AddTagAsync(string entityId, params string[] tags) {
            if (string.IsNullOrEmpty(entityId)) {
                throw new ArgumentNullException(nameof(entityId));
            }

            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            if (values.Any()) {
                return await _database.SetAddAsync(setKey, values);
            }

            return 0;
        }

        /// <summary>
        /// 删除全部标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> RemoveTagsAsync(string entityId) {
            return await RemoveAsync(entityId);
        }

        /// <summary>
        /// 删除指定标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tags"></param>
        public virtual async Task<long> RemoveTagAsync(string entityId, params string[] tags) {
            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            return await Database.SetRemoveAsync(setKey, values);
        }


        /// <summary>
        /// 是否包含标记
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task<bool> HasTagAsync(string entityId, string tag) {
            var setKey = GetSubKey(entityId);
            return await Database.SetContainsAsync(setKey, tag);
        }

        #endregion

        #region batch

        public virtual Task AddTagsBatch(IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            batch.SetAddAsync(setKey, values);
            return Task.CompletedTask;
        }

        public virtual Task RemoveTagsBatch(IBatch batch, string entityId, params string[] tags) {
            var setKey = GetSubKey(entityId);
            var values = FilterTags(tags);
            batch.SetRemoveAsync(setKey, values);
            return Task.CompletedTask;
        }

        #endregion
    }
}