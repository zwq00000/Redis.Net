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
        /// Returns the remaining time to live of a key that has a timeout
        /// </summary>
        /// <returns></returns>
        public TimeSpan? TTL => Database.KeyTimeToLive (SetKey);

        /// <summary>
        /// 在更新操作中检查超期设置
        /// </summary>
        protected void CheckExpire (IBatch batch = null) {
            if (_expire.HasValue) {
                if (batch != null) {
                    batch.KeyExpireAsync (SetKey, _expire.Value);
                } else {
                    Database.KeyExpire (SetKey, _expire.Value);
                }
            }
        }

        /// <summary>
        /// 在更新操作中检查超期设置
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        protected async Task CheckExpireAsync (IBatch batch = null) {
            if (_expire.HasValue) {
                await Database.KeyExpireAsync (SetKey, _expire.Value);
            }
        }

        /// <summary>
        /// 清除超期设置
        /// </summary>
        public void ClearExpire () {
            this._expire = null;
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
        /// 异步删除集合 <seealso cref="IDatabaseAsync.KeyDeleteAsync(RedisKey, CommandFlags)" />
        /// </summary>
        /// <remarks>
        /// Removes the specified key. A key is ignored if it does not exist. 
        /// If UNLINK is available (Redis 4.0+), it will be used.
        /// </remarks>
        /// <returns></returns>
        public async Task<bool> DeleteAsync () {
            return await Database.KeyDeleteAsync (SetKey);
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Delete () {
            return Database.KeyDelete (SetKey);
        }

        /// <summary>
        /// 在批命令上下文中执行清理操作
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public Task<bool> BatchDelete (IBatch batch) {
            return batch.KeyDeleteAsync (SetKey);
        }
    }
}