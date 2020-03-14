using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Redis.Net {
    public interface IEntrySet<TKey, TValue> {
        /// <summary>
        /// 增加实体集合的批处理方法
        /// </summary>
        void Add (TKey key, TValue value);
        /// <summary>
        /// 增加实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        void Add (TKey key, IEnumerable<HashEntry> entries);
        /// <summary>
        /// 更新实体集合的批处理方法,用户可以自定义 Entity to HashEntry[] 的方式
        /// </summary>
        void Update (TKey key, IEnumerable<HashEntry> entries);
        /// <summary>
        /// 删除实体集合的批处理方法
        /// </summary>
        void Remove (TKey key);
        /// <summary>
        /// 设置实体集合超时回收时间 的批处理方法
        /// </summary>
        void Expire (TKey key, TimeSpan? expiry);
        /// <summary>
        /// 清理全部集合的批量方法
        /// </summary>
        void Clear ();
    }
}