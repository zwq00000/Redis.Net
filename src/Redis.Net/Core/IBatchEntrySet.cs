namespace Redis.Net.Generic {
    public interface IBatchEntrySet<Tkey, TValue> : IEntrySet<Tkey, TValue>, IBatchExecuter { }
}