using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Keys 扩展方法
    /// </summary>
    public static class RedisKeysExtensions {

        /// <summary>
        /// 根据条件查询 RedisKey
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static RedisKey[] GetKeys(this IDatabase database, string keyPrefix) {
            if (string.IsNullOrEmpty(keyPrefix)) {
                throw new ArgumentNullException(nameof(keyPrefix));
            }

            var partten = $"{keyPrefix}*";
            return (RedisKey[])database.Execute("KEYS", partten);
        }

        /// <summary>
        /// 根据条件查询 RedisKey
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static RedisKey[] GetKeys(this IDatabase database, RedisKey keyPrefix) {
            if (string.IsNullOrEmpty(keyPrefix)) {
                throw new ArgumentNullException(nameof(keyPrefix));
            }
            var partten = keyPrefix.Append("*");
            return (RedisKey[])database.Execute("KEYS", partten);
        }

        /// <summary>
        ///  根据条件异步查询 RedisKey
        /// </summary>
        /// <param name="database"></param>
        /// <param name="keyPrefix"></param>
        /// <returns></returns>
        public static async Task<RedisKey[]> GetKeysAsync(this IDatabaseAsync database, string keyPrefix) {
            if (string.IsNullOrEmpty(keyPrefix)) {
                throw new ArgumentNullException(nameof(keyPrefix));
            }

            var partten = $"{keyPrefix}*";
            return (RedisKey[])await database.ExecuteAsync("KEYS", partten);
        }
    }
}