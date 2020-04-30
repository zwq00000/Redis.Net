using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Generic {
    /// <summary>
    /// 倒排索引集合
    /// 通常用作 RedisSet集合索引,避免使用 Keys 命令 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InvertedIndexSet<T> : RedisMutiKey where T : IConvertible {

        internal const string IndexSetName = "@__Index";
        public InvertedIndexSet (IDatabase database, string baseKey) : base (database, baseKey) { }

        public IEnumerable<T> GetValues (RedisValue index) {
            var setKey = base.GetSubKey (index);
            return Database.SetMembers (setKey).Where(m=>m.HasValue).Select (m => m.ConvertTo<T>());
        }

        public async Task<IEnumerable<T>> GetValuesAsync (RedisValue index) {
            var setKey = base.GetSubKey (index);
            var members = await Database.SetMembersAsync (setKey);
            return members.Where(m=>m.HasValue).Select (m => m.ConvertTo<T>());
        }

        public void Add (T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                Database.SetAdd (subKey, keyValue);
            }
        }

        public void Remove (T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                Database.SetRemove (subKey, keyValue);
            }
        }

        public async Task AddAsync (T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                await Database.SetAddAsync (subKey, keyValue);
            }
        }

        public async Task RemoveAsync (T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                await Database.SetRemoveAsync (subKey, keyValue);
            }
        }

        /// <summary>
        /// 批量增加索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="id"></param>
        /// <param name="values"></param>
        public void BatchAdd (IBatch batch, T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                batch.SetAddAsync (subKey, keyValue);
            }
        }

        /// <summary>
        /// 批量删除索引
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="id"></param>
        /// <param name="values"></param>
        public void BatchRemove (IBatch batch, T id, params RedisValue[] values) {
            var subKeys = values.Select (v => base.GetSubKey (v));
            var keyValue = RedisValue.Unbox (id);
            foreach (var subKey in subKeys) {
                batch.SetRemoveAsync (subKey, keyValue);
            }
        }

    }

}