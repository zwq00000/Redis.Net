namespace Redis.Net {
    public interface IBatchEntrySet<Tkey, TValue> : IEntrySet<Tkey, TValue>, IBatchExecuter { }
}