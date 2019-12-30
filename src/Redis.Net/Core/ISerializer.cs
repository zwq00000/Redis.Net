using Redis.Net.Generic;

namespace Redis.Net.Core {
    /// <summary>
    /// serialize and deserialize methods,
    /// <see cref="ReadOnlyRedisDicionary{TKey,TEntity}"/>
    /// </summary>
    public interface ISerializer {
        /// <summary>
        /// deserialize bytes to Object
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        object Deserialize (byte[] serializedObject);

        /// <summary>
        /// deserialize to <see cref="T:T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        T Deserialize<T> (byte[] serializedObject);

        /// <summary>
        /// serialize a object to bytes
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        byte[] Serialize (object item);
    }
}