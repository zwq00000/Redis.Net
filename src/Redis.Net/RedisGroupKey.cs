using System;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis 分组容器基类
    /// </summary>
    public abstract class RedisGroupKey {
        /// <summary>
        /// 默认 Redis Key 前缀
        /// </summary>
        public readonly RedisKey PrefixKey;

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
        }

        /// <summary>
        /// 根据 Id 获取 RedisKey
        /// <see cref="RedisKey.Append"/>,生成 "{baseKey}:{id}" 的二级结构键值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected RedisKey GetSubKey(string id) {
            return PrefixKey.Append(id);
        }

        /// <summary>
        ///  删除容器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected bool Remove(string id) {
            var key = GetSubKey(id);
            return Database.KeyDelete(key);
        }

        /// <summary>
        ///  异步删除容器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task<bool> RemoveAsync(string id) {
            var key = GetSubKey(id);
            return await Database.KeyDeleteAsync(key);
        }

        /// <summary>
        ///  批处理删除容器
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Task<bool> RemoveBatch(IBatch batch, string id) {
            var key = GetSubKey(id);
            return batch.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 批处理删除多个容器
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected Task<long> RemoveBatch(IBatch batch, string[] ids) {
            var keys = ids.Select(GetSubKey).ToArray();
            return batch.KeyDeleteAsync(keys);
        }

        /// <summary>
        /// 根据 <see cref="PrefixKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        public RedisKey[] GetKeys() {
            return Database.GetKeys(this.PrefixKey);
        }

        /// <summary>
        /// 根据 <see cref="PrefixKey"/> 查找全部 RedisKey 
        /// </summary>
        /// <returns></returns>
        public async Task<RedisKey[]> GetKeysAsync() {
            return await Database.GetKeysAsync(this.PrefixKey);
        }

        /// <summary>
        /// Get Redis Keys Count start with prefixKey
        /// </summary>
        /// <returns></returns>
        public long Count() {
            return Database.GetKeys(this.PrefixKey).Count();
        }

        public long LongCount() {
            return Database.GetKeys(this.PrefixKey).LongCount();
        }
    }
}