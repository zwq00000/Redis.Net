using System;

namespace RedisExtensionsTests {
    public class User {

        public User () {

        }

        public User (int uid) {
            this.UserId = uid;
        }
        public int UserId { get; set; }

        public string Firstname { get; set; } = "Test UserName";
        public string Lastname { get; set; } = "Last Name";
        public string Twitter { get; set; } = "@imperugo";

        public string Blog { get; set; } = "http://tostring.it";

        public override bool Equals (object obj) {
            return obj is User user &&
                UserId == user.UserId &&
                Firstname == user.Firstname &&
                Lastname == user.Lastname &&
                Twitter == user.Twitter &&
                Blog == user.Blog;
        }

        public override int GetHashCode () {
            return HashCode.Combine (UserId, Firstname, Lastname, Twitter, Blog);
        }
    }
}