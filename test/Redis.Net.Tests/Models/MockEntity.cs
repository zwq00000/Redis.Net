using System;
using System.Text;

namespace Redis.Net.Tests {
    public class MockEntity {
        static Random random = new Random ();

        public MockEntity () {

        }

        public MockEntity (int id, string msg) {
            this.Id = id;
            this.Message = msg;
            this.IntValue = random.Next ();
            this.Date = DateTime.Now;
            this.Time = TimeSpan.FromMilliseconds (1 + id);
            Bytes = Encoding.UTF8.GetBytes (msg);
            Floats = new float[] { 1, 2, 3, 4, 5 };
        }
        public int Id { get; set; }

        public string Message { get; set; }
        public int IntValue { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        public byte[] Bytes { get; set; }

        public float[] Floats { get; set; }
    }
}