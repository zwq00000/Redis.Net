using System;

namespace Redis.Net.Tests {

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
            return new Model () {
                Str = "TEST",
                    bytes = new byte[] { 1, 2, 3 },
                    Date = DateTime.Now,
                    DateNullable = DateTime.Now,
                    IntNullable = 1,
                    UintNullable = 1,
                    DoubleNullable = 1,
                    BoolNullable = true,
                    LongNullable = 1,
                    UlongNullable = 1,
                    FloatNullable = 1f,
                    Kind = UriKind.Relative,
                    FloatArray = new float[] { 1f, 2f, 3f, 4f, 5, 6, 7 },
                    DoubleArray = new double[] { 3.1d, 4.2d, 5.3d },
                    IntArray = new int[] { 5, 6, 7, 8 },
                    LongArray = new long[] { 9L, 10L, 11L, 12L, 13, 14 }
            };
        }
    }
}