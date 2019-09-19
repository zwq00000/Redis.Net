using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis.Net.Specialized {
    /// <summary>
    /// 时间戳有序集合
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class TimestempSet<TKey> : ReadOnlyTimestempSet<TKey> where TKey : IConvertible {

        ///<summary>
        /// 构造方法
        ///</summary>
        public TimestempSet(IDatabase database, string setKey) : base(database, setKey) {

        }

        ///<summary>
        /// 增加记录
        ///</summary>
        public bool Add(TKey member, DateTime time) {
            return SortedSet.Add(member, time.ToTimestamp());
        }

        ///<summary>
        /// 增加记录
        ///</summary>
        public bool Add(TKey member, int timestemp) {
            return SortedSet.Add(member, timestemp);
        }

        ///<summary>
        /// 增加记录的异步方法
        ///</summary>
        public async Task<bool> AddAsync(TKey member, DateTime time) {
            return await SortedSet.AddAsync(member, time.ToTimestamp());
        }

        /// <summary>
        /// 增加记录的异步方法
        /// </summary>
        /// <param name="member"></param>
        /// <param name="timestemp"></param>
        /// <returns>true 增加成功, false shipId 已经存在,更新时间戳</returns>
        public async Task<bool> AddAsync(TKey member, int timestemp) {
            return await SortedSet.AddAsync(member, timestemp);
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Remove(TKey member) {
            return SortedSet.Remove(member) > 0;
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(TKey member) {
            return await SortedSet.RemoveAsync(member) > 0;
        }

        #region Batch Methods

        /// <summary>
        /// 批量增加时间戳
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member">sorted set member </param>
        /// <param name="timeStamp">AIS 时间戳</param>
        /// <returns></returns>
        public Task<bool> AddAsync(IBatch batch, TKey member, int timeStamp) {
            if (timeStamp == 0) {
                timeStamp = DateTime.UtcNow.ToTimestamp();
            }
            return SortedSet.AddAsync(batch, member, (double)timeStamp);
        }

        /// <summary>
        /// 批量增加时间戳
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Task<bool> AddAsync(IBatch batch, TKey member, DateTime time) {
            return SortedSet.AddAsync(batch, member, time.ToTimestamp());
        }

        /// <summary>
        /// 批量增加成员
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public Task<long> RemoveAsync(IBatch batch, TKey member) {
            return SortedSet.RemoveAsync(batch, member);
        }

        #endregion

    }
}