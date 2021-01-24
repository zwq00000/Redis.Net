using System;

namespace Redis.Net.Tests
{

    public class Model {
        public string Str { get; set; }
        public DateTime Date { get; set; }
        public int Int { get; set; }
        public uint Uint { get; set; }
        public double Double { get; set; }
        public byte[] bytes { get; set; }
        public bool Bool { get; set; }
        public long Long { get; set; }
        public ulong Ulong { get; set; }
        public float Float { get; set; }
        public DateTime? DateNullable { get; set; }
        public int? IntNullable { get; set; }
        public uint? UintNullable { get; set; }
        public double? DoubleNullable { get; set; }
        public bool? BoolNullable { get; set; }
        public long? LongNullable { get; set; }
        public ulong? UlongNullable { get; set; }
        public float? FloatNullable { get; set; }

        public float[] FloatArray { get; set; }
        public double[] DoubleArray { get; set; }
        public int[] IntArray { get; set; }
        public long[] LongArray { get; set; }

        public UriKind Kind { get; set; }

        public static Model CreateNew () {
            var random = new Random();
            return new Model () {
                Str = "TEST",
                    bytes = new byte[] { 1, 2, 3 },
                    Date = DateTime.Now,
                    DateNullable = null,
                    IntNullable = random.Next(),
                    UintNullable = (uint)random.Next(),
                    DoubleNullable = random.NextDouble(),
                    BoolNullable = true,
                    LongNullable = random.Next(),
                    UlongNullable = (ulong)random.Next(),
                    FloatNullable = random.Next(),
                    Kind = UriKind.Relative,
                    FloatArray = new float[] { (float)random.NextDouble(), (float)random.NextDouble(),(float)random.NextDouble(), (float)random.NextDouble() },
                    DoubleArray = new double[] { random.NextDouble(),random.NextDouble(),random.NextDouble(),random.NextDouble() },
                    IntArray = new int[] { random.Next(),random.Next(), random.Next(), random.Next() },
                    LongArray = new long[] { 9L, 10L, 11L, 12L, 13, 14 }
            };
        }
    }
}