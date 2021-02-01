using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis 多键 Set/Hashset 基类
    /// </summary>
    public abstract class RedisMutiKey {
        /// <summary>
        /// 默认 ShipTracks Redis Key 前缀
        /// </summary>
        public readonly RedisKey BaseKey;

        private string _baseKeyString;

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
            _baseKeyString = BaseKey.ToString ();
        }

        /// <summary>
        /// 根据 Id 获取 RedisKey
        /// <see cref="RedisKey.Append"/>,生成 "{baseKey}:{id}" 的二级结构键值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected RedisKey GetSubKey (string id) {
            return BaseKey.Append (id);
        }

        /// <summary>
        /// 根据 RedisKey 获取 Id,去除 <see cref="BaseKey" />
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetId (RedisKey key) {
            var keyStr = key.ToString ();
            if (keyStr.StartsWith (_baseKeyString)) {
                return keyStr.Substring (_baseKeyString.Length);
            } else {
                return keyStr;
            }
        }

        protected bool Remove (string id) {
            var key = GetSubKey (id);
            return Database.KeyDelete (key);
        }

        protected async Task<bool> RemoveAsync (string id) {
            var key = GetSubKey (id);
            return await Database.KeyDeleteAsync (key);
        }

        protected Task<bool> RemoveBatch (IBatch batch, string id) {
            var key = GetSubKey (id);
            return batch.KeyDeleteAsync (key);
        }

        protected Task<long> RemoveBatch (IBatch batch, string[] ids) {
            var keys = ids.Select (GetSubKey).ToArray ();
            return batch.KeyDeleteAsync (keys);
        }

        /// <summary>
        /// 获取全部Id
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetIds () {
            return GetKeys ().Select (k => GetId (k));
        }

        /// <summary>
        /// 获取全部Id 的异步方法
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetIdsAsync () {
            var keys = await GetKeysAsync ();
            return keys.Select (k => GetId (k));
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

        public long Count () {
            return Database.GetKeys (this.BaseKey).Count ();
        }

        public long LongCount () {
            return Database.GetKeys (this.BaseKey).LongCount ();
        }
    }

}