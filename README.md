Redis.Net A Redis Collection/Set Wapper
=======================================
C# Wapper Redis Collection and Hashset


# ����
��Ŀ���� StackExchange.Redis �ṩ���� Api,�����򵥵ķ�װ����ʹ��.

���� ��Ϊ ֻ�������Ϳ�д����,���� ReadonlyRedisSet/RedisSet.

## Redis ����

��ֵ����Ϊ ReidsValue


## ��������

- RedisSet<T>
- RedisHashSet<T>
- RedisSortedSet<T>
- ReadonlyEntrySet<TEntry> / RedisEntrySet<TEntry>

## ������������




## ֧�ֵ�����

### ��������
����������֧�ֵĻ������ͺ� Redis ֧�ֵ���������һ��.
ת������ �ο� RedisValue ת������,Ҫ�� ����������Ҫ֧�� IConverable �ӿ�.

### ʵ���������
RedisEntrySet ��һ���������,ͨ�� Redis Hashset �洢���Լ�/ֵ�� (KeyValuePair)

## ʵ�ֵĽӿ�
 
 - ReadOnlyRedisSet<TValue> : IReadOnlyCollection<TValue>

 - ReadOnlyRedisHashSet<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>