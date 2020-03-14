namespace Redis.Net.Generic
{
    public interface IBatchExecuter {
        /// <summary>
        ///     Execute the batch operation, sending all queued commands to the server. Note
        ///     that this operation is neither synchronous nor truly asyncronous - it simply
        ///     enqueues the buffered messages. To check on completion, you should check the
        ///     individual responses.
        /// </summary>
        void Execute ();
    }
}