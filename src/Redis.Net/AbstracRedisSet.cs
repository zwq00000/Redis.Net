using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net {
    /// <summary>
    /// Redis Key 操作封装
    /// </summary>
    public abstract class AbstracRedisKey {
        private TimeSpan? _expire;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="database">Redis Database</param>
        /// <param name="setKey">Redis Key Name</param>
        protected AbstracRedisKey (IDatabase database, string setKey) {
            Database = database ??
                throw new ArgumentNullException (nameof (database));

            if (string.IsNullOrEmpty (setKey)) {
                throw new ArgumentNullException (nameof (setKey));
            }
            SetKey = setKey;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="database">Redis Database</param>
        /// <param name="setKey">Redis Key Name</param>
        /// <param name="keyType"></param>
        protected AbstracRedisKey (IDatabase database, string setKey, RedisType keyType) : this (database, setKey) {
            if (!AssertKeyType (keyType)) {
                throw new Exception ($"Redis Key {setKey} Type Not {keyType}");
            }
        }

        /// <summary>
        /// Redis Database
        /// </summary>
        protected IDatabase Database { get; }

        /// <summary>
        /// Redis Key
        /// </summary>
        public RedisKey SetKey { get; private set; }

        /// <summary>
        /// 检测 KeyType 是否为预期类型
        /// </summary>
        /// <param name="expectation"></param>
        /// <returns></returns>
        protected bool AssertKeyType (RedisType expectation) {
            if (IsExist ()) {
                return Database.KeyType (SetKey) == expectation;
            }

            return true;
        }

        /// <summary>
        /// Key 是否存在
        /// </summary>
        public bool IsExist () {
            return Database.KeyExists (SetKey);
        }

        /// <summary>
        /// 如果 Key 不存在则抛出异常
        /// </summary>
        /// <exception cref="RedisException"></exception>
        protected void CheckKeyExistsAndThrow () {
            if (!IsExist ()) {
                throw new RedisException ($"Redis Key {SetKey} Not Existed");
            }
        }

        /// <summary>
        /// 超期时间
        /// </summary>
        public TimeSpan? Expire {
            get => _expire ?? (_expire = Database.KeyTimeToLive (SetKey));
            set {
                if (Equals (_expire, value)) {
                    return;
                }
                _expire = value;
                Database.KeyExpire (SetKey, value);
            }
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted. 
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="expiry">The timeout to set.</param>
        /// <returns>1 if the timeout was set. 0 if key does not exist or the timeout could not be set.</returns>
        /// <remarks>If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten. So, if key already has an associated timeout, it will do nothing and return 0. Since Redis 2.1.3, you can update the timeout of a key. It is also possible to remove the timeout using the PERSIST command. See the page on key expiry for more information.</remarks>
        /// <remarks>https://redis.io/commands/expire</remarks>
        /// <remarks>https://redis.io/commands/pexpire</remarks>
        /// <remarks>https://redis.io/commands/persist</remarks>
        private void SetExpire (TimeSpan expiry) {
            Database.KeyExpire (SetKey, expiry);
        }

        private async Task<bool> SetExpireAsync (TimeSpan expiry) {
            return await Database.KeyExpireAsync (SetKey, expiry);
        }

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted. 
        /// A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <param name="absoluteTime"></param>
        private void SetExpire (DateTime absoluteTime) {
            Database.KeyExpire (SetKey, absoluteTime);
        }

        /// <summary>
        /// 清除超期设置
        /// </summary>
        public void ClearExpire () {
            Database.KeyExpire (SetKey, (TimeSpan?) null);
        }

        /// <summary>
        /// 重命名 Set Key
        /// </summary>
        /// <param name="newKey"></param>
        protected void Rename (string newKey) {
            if (Database.KeyRename (SetKey, newKey, When.NotExists)) {
                SetKey = newKey;
            } else {
                throw new ArgumentException ($"Rename Fail,newKey '{newKey}' is existed");
            }
        }

        /// <summary>
        /// 异步删除集合.
        /// <seealso cref="IDatabaseAsync.KeyDeleteAsync(RedisKey, CommandFlags)" />
        /// </summary>
        /// <remarks>
        /// Removes the specified key. A key is ignored if it does not exist. 
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </remarks>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public Task<bool> BatchClearAll (IBatch batch) {
            return batch.KeyDeleteAsync (SetKey);
        }

        /// <summary>
        /// 异步删除集合 <seealso cref="IDatabaseAsync.KeyDeleteAsync(RedisKey, CommandFlags)" />
        /// </summary>
        /// <remarks>
        /// Removes the specified key. A key is ignored if it does not exist. 
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </remarks>
        /// <returns></returns>
        public async Task<bool> ClearAllAsync(){
            return await Database.KeyDeleteAsync(SetKey);
        }

        /// <summary>Removes all items.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void ClearAll () {
            Database.KeyDelete (SetKey);
        }
    }
}