Redis.Net A Redis Collection/Set Wapper
=======================================
C# Wapper Redis Collection and Hashset


# 概述
项目依赖 StackExchange.Redis 提供基础 Api,经过简单的封装方便使用.

容器 分为 只读容器和可写容器,比如 ReadonlyRedisSet/RedisSet.

## Redis 容器

数值类型为 ReidsValue


## 泛型容器

- RedisSet<T>
- RedisHashSet<T>
- RedisSortedSet<T>
- ReadonlyEntrySet<TEntry> / RedisEntrySet<TEntry>

## 分组容器管理




## 支持的类型

### 基本类型
泛型容器中支持的基本类型和 Redis 支持的数据类型一致.
转换方法 参考 RedisValue 转换方法,要求 基本类型需要支持 IConverable 接口.

### 实体对象类型
RedisEntrySet 是一种组合类型,通过 Redis Hashset 存储属性键/值对 (KeyValuePair)

## 实现的接口
 
 - ReadOnlyRedisSet<TValue> : IReadOnlyCollection<TValue>

 - ReadOnlyRedisHashSet<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>