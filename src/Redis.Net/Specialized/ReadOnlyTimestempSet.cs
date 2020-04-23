using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.Net;
using Redis.Net.Generic;
using StackExchange.Redis;

namespace Redis.Net.Specialized {
    /// <summary>
    /// 只读的 时间戳 有序集合
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ReadOnlyTimestempSet<TKey> : AbstracRedisKey where TKey : IConvertible {

        protected readonly RedisSortedSet<TKey> SortedSet;

        ///<summary>
        /// 构造方法
        ///</summary>
        public ReadOnlyTimestempSet(IDatabase database, string setKey) : base(database, setKey, RedisType.SortedSet) {
            this.SortedSet = new RedisSortedSet<TKey>(database, setKey);
        }

        /// <summary>
        /// 返回基础集合
        /// </summary>
        /// <value></value>
        public ReadOnlySortedSet<TKey> BaseSet{
            get{
                return SortedSet;
            }
        } 

        /// <summary>
        /// 获取集合全部键值
        /// </summary>
        /// <value></value>
        public IEnumerable<TKey> Keys{get{
            return SortedSet.Keys;
        }}

        ///<summary>
        /// 获取指定 shipId 的时间戳 (unix time sec)
        ///</summary>
        ///<param name="member"></param>
        public int? GetTimestamp(TKey member) {
            if (SortedSet.TryGetValue(member, out var value)) {
                return (int)value;
            } else {
                return null;
            }
        }

        ///<summary>
        /// 获取指定 shipId 的时间戳 (unix time sec)
        ///</summary>
        ///<param name="member"></param>
        public async Task<int?> GetTimestampAsync(TKey member) {
            var result = await SortedSet.GetScoreAsync(member);
            if (result.HasValue) {
                return (int)result.Value;
            }
            return null;
        }

        /// <summary>
        /// 指定时间范围内的成员列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<TKey> GetByRange(DateTime start, DateTime end) {
            return SortedSet.GetRangeByScore(start.ToTimestamp(), end.ToTimestamp(), Exclude.None, Order.Descending);
        }

        /// <summary>
        /// 指定时间范围内的成员列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TKey>> GetByRangeAsync(DateTime start, DateTime end) {
            return await SortedSet.GetRangeByScoreAsync(start.ToTimestamp(), end.ToTimestamp(), Exclude.None,
                Order.Descending);
        }

        /// <summary>
        /// 查找超时成员
        /// </summary>
        /// <param name="time">过期期限</param>
        /// <returns></returns>
        public IEnumerable<TKey> GetByOverTime(DateTime time) {
            return SortedSet.GetRangeByScore(0, time.ToTimestamp(), Exclude.None, Order.Descending);
        }

        /// <summary>
        /// 查找超时船舶的异步方法
        /// </summary>
        /// <param name="time">过期期限</param>
        /// <param name="order">排序方法</param>
        /// <returns></returns>
        public async Task<IEnumerable<TKey>> GetByOverTimeAsync(DateTime time, Order order = Order.Ascending) {
            var result = await SortedSet.GetRangeByScoreAsync(0, time.ToTimestamp(), Exclude.None, order);
            return result;
        }

        /// <summary>
        /// 删除超时记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long RemoveByOvertime(DateTime time) {
            return SortedSet.RemoveRangeByScore(0, time.ToTimestamp());
        }

        /// <summary>
        /// 删除超时记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task<long> RemoveByOvertimeAsync(DateTime time) {
            return await ((IAsyncSortSet<TKey>)SortedSet).RemoveRangeByScoreAsync(0, time.ToTimestamp());
        }

        /// <summary>
        /// 获取记录数量
        /// </summary>
        /// <returns></returns>
        public long Count() {
            return SortedSet.GetLongCount();
        }

        /// <summary>
        /// 获取记录数量的异步方法
        /// </summary>
        /// <returns></returns>
        public long CountByRange(DateTime start, DateTime end) {
            return SortedSet.GetLongCount(start.ToTimestamp(), end.ToTimestamp());
        }
    }
}