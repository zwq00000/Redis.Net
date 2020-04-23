namespace RedisExtensionsTests
{
    public class User {

        public int UserId { get; set; }

        public string Firstname { get; set; } = "Test UserName";
        public string Lastname { get; set; } = "Last Name";
        public string Twitter { get; set; } = "@imperugo";

        public string Blog { get; set; } = "http://tostring.it";
    }
}
