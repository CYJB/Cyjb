using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="UInt64Util"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestUInt64Util
	{
		/// <summary>
		/// 对 <see cref="UInt64Util.IsPowerOf2(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, false)]
		[DataRow(1, true)]
		[DataRow(2, true)]
		[DataRow(3, false)]
		[DataRow(4, true)]
		[DataRow(7, false)]
		[DataRow(8, true)]
		[DataRow(15, false)]
		[DataRow(16, true)]
		[DataRow(31, false)]
		[DataRow(32, true)]
		[DataRow(63, false)]
		[DataRow(64, true)]
		[DataRow(99, false)]
		[DataRow(107, false)]
		[DataRow(127, false)]
		[DataRow(128, true)]
		[DataRow(164, false)]
		[DataRow(247, false)]
		[DataRow(255, false)]
		[DataRow(256, true)]
		[DataRow(257, false)]
		[DataRow(511, false)]
		[DataRow(512, true)]
		[DataRow(513, false)]
		[DataRow(1023, false)]
		[DataRow(1024, true)]
		[DataRow(1025, false)]
		[DataRow(1073741823, false)]
		[DataRow(1073741824, true)]
		[DataRow(1073741825, false)]
		[DataRow(2147483646, false)]
		[DataRow(2147483647, false)]
		[DataRow(2147483648, true)]
		[DataRow(2147483649, false)]
		[DataRow(4611686018427387903, false)]
		[DataRow(4611686018427387904, true)]
		[DataRow(4611686018427387905, false)]
		[DataRow(9223372036854775808, true)]
		[DataRow(9223372036854775809, false)]
		[DataRow(18446744073709551614, false)]
		[DataRow(18446744073709551615, false)]
		public void TestIsPowerOf2(object value, bool target)
		{
			Assert.AreEqual(target, UInt64Util.IsPowerOf2(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.CountBits(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 1)]
		[DataRow(2, 1)]
		[DataRow(3, 2)]
		[DataRow(4, 1)]
		[DataRow(7, 3)]
		[DataRow(8, 1)]
		[DataRow(15, 4)]
		[DataRow(16, 1)]
		[DataRow(31, 5)]
		[DataRow(32, 1)]
		[DataRow(63, 6)]
		[DataRow(64, 1)]
		[DataRow(99, 4)]
		[DataRow(107, 5)]
		[DataRow(127, 7)]
		[DataRow(128, 1)]
		[DataRow(164, 3)]
		[DataRow(243, 6)]
		[DataRow(247, 7)]
		[DataRow(255, 8)]
		[DataRow(256, 1)]
		[DataRow(257, 2)]
		[DataRow(511, 9)]
		[DataRow(1073741823, 30)]
		[DataRow(1073741824, 1)]
		[DataRow(1160070801, 12)]
		[DataRow(2147483646, 30)]
		[DataRow(2147483647, 31)]
		[DataRow(2147483648, 1)]
		[DataRow(2147483649, 2)]
		[DataRow(4611686018427387903, 62)]
		[DataRow(4611686018427387904, 1)]
		[DataRow(4611686018427387905, 2)]
		[DataRow(9223372036854775808, 1)]
		[DataRow(9223372036854775809, 2)]
		[DataRow(18446744073709551614, 63)]
		[DataRow(18446744073709551615, 64)]
		public void TestCountBits(object value, int countBits)
		{
			Assert.AreEqual(countBits, UInt64Util.CountBits(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.Parity(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 1)]
		[DataRow(2, 1)]
		[DataRow(3, 0)]
		[DataRow(4, 1)]
		[DataRow(7, 1)]
		[DataRow(8, 1)]
		[DataRow(15, 0)]
		[DataRow(16, 1)]
		[DataRow(31, 1)]
		[DataRow(32, 1)]
		[DataRow(63, 0)]
		[DataRow(64, 1)]
		[DataRow(99, 0)]
		[DataRow(107, 1)]
		[DataRow(127, 1)]
		[DataRow(128, 1)]
		[DataRow(164, 1)]
		[DataRow(243, 0)]
		[DataRow(247, 1)]
		[DataRow(255, 0)]
		[DataRow(256, 1)]
		[DataRow(257, 0)]
		[DataRow(511, 1)]
		[DataRow(1073741823, 0)]
		[DataRow(1073741824, 1)]
		[DataRow(1160070801, 0)]
		[DataRow(2147483646, 0)]
		[DataRow(2147483647, 1)]
		[DataRow(2147483648, 1)]
		[DataRow(2147483649, 0)]
		[DataRow(4611686018427387903, 0)]
		[DataRow(4611686018427387904, 1)]
		[DataRow(4611686018427387905, 0)]
		[DataRow(9223372036854775808, 1)]
		[DataRow(9223372036854775809, 0)]
		[DataRow(18446744073709551614, 1)]
		[DataRow(18446744073709551615, 0)]
		public void TestParity(object value, int countBits)
		{
			Assert.AreEqual(countBits, UInt64Util.Parity(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.ReverseBits(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 9223372036854775808)]
		[DataRow(2, 4611686018427387904)]
		[DataRow(3, 13835058055282163712)]
		[DataRow(4, 2305843009213693952)]
		[DataRow(7, 16140901064495857664)]
		[DataRow(8, 1152921504606846976)]
		[DataRow(15, 17293822569102704640)]
		[DataRow(16, 576460752303423488)]
		[DataRow(31, 17870283321406128128)]
		[DataRow(32, 288230376151711744)]
		[DataRow(63, 18158513697557839872)]
		[DataRow(64, 144115188075855872)]
		[DataRow(99, 14267403619509731328)]
		[DataRow(107, 15420325124116578304)]
		[DataRow(127, 18302628885633695744)]
		[DataRow(128, 72057594037927936)]
		[DataRow(164, 2666130979403333632)]
		[DataRow(243, 14915921965851082752)]
		[DataRow(247, 17221764975064776704)]
		[DataRow(255, 18374686479671623680)]
		[DataRow(256, 36028797018963968)]
		[DataRow(257, 9259400833873739776)]
		[DataRow(511, 18410715276690587648)]
		[DataRow(1073741823, 18446744056529682432)]
		[DataRow(1073741824, 8589934592)]
		[DataRow(1160070801, 9899655946605428736)]
		[DataRow(2147483646, 9223372028264841216)]
		[DataRow(2147483647, 18446744065119617024)]
		[DataRow(18446744071562067968, 8589934591)]
		[DataRow(18446744071562067969, 9223372045444710399)]
		[DataRow(13835058055282163696, 1152921504606846973)]
		[DataRow(18446744073709551614, 9223372036854775807)]
		[DataRow(18446744073709551615, 18446744073709551615)]
		public void TestReverseBits(object value, object target)
		{
			Assert.AreEqual(Convert.ToUInt64(target), UInt64Util.ReverseBits(Convert.ToUInt64(value)));
			Assert.AreEqual(Convert.ToUInt64(value), UInt64Util.ReverseBits(Convert.ToUInt64(target)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.LogBase2(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 0)]
		[DataRow(2, 1)]
		[DataRow(3, 1)]
		[DataRow(4, 2)]
		[DataRow(7, 2)]
		[DataRow(8, 3)]
		[DataRow(15, 3)]
		[DataRow(16, 4)]
		[DataRow(31, 4)]
		[DataRow(32, 5)]
		[DataRow(63, 5)]
		[DataRow(64, 6)]
		[DataRow(99, 6)]
		[DataRow(107, 6)]
		[DataRow(127, 6)]
		[DataRow(128, 7)]
		[DataRow(164, 7)]
		[DataRow(243, 7)]
		[DataRow(247, 7)]
		[DataRow(255, 7)]
		[DataRow(256, 8)]
		[DataRow(257, 8)]
		[DataRow(511, 8)]
		[DataRow(512, 9)]
		[DataRow(1024, 10)]
		[DataRow(1073741824, 30)]
		[DataRow(1073741825, 30)]
		[DataRow(2147483647, 30)]
		[DataRow(2147483648, 31)]
		[DataRow(4294967296, 32)]
		[DataRow(4611686018427387904, 62)]
		[DataRow(9223372036854775807, 62)]
		[DataRow(9223372036854775808, 63)]
		[DataRow(18446744073709551615, 63)]
		public void TestLogBase2(object value, int target)
		{
			Assert.AreEqual(target, UInt64Util.LogBase2(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.LogBase10(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 0)]
		[DataRow(2, 0)]
		[DataRow(9, 0)]
		[DataRow(10, 1)]
		[DataRow(11, 1)]
		[DataRow(99, 1)]
		[DataRow(100, 2)]
		[DataRow(101, 2)]
		[DataRow(255, 2)]
		[DataRow(256, 2)]
		[DataRow(1024, 3)]
		[DataRow(1073741824, 9)]
		[DataRow(10737418240, 10)]
		[DataRow(10737418240, 10)]
		[DataRow(1000000000000000000, 18)]
		[DataRow(9223372036854775807, 18)]
		[DataRow(9223372036854775808, 18)]
		[DataRow(18446744073709551615, 19)]
		public void TestLogBase10(object value, int target)
		{
			Assert.AreEqual(target, UInt64Util.LogBase10(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.CountTrailingZeroBits(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 64)]
		[DataRow(1, 0)]
		[DataRow(2, 1)]
		[DataRow(3, 0)]
		[DataRow(4, 2)]
		[DataRow(5, 0)]
		[DataRow(7, 0)]
		[DataRow(8, 3)]
		[DataRow(9, 0)]
		[DataRow(15, 0)]
		[DataRow(16, 4)]
		[DataRow(17, 0)]
		[DataRow(31, 0)]
		[DataRow(32, 5)]
		[DataRow(33, 0)]
		[DataRow(36, 2)]
		[DataRow(40, 3)]
		[DataRow(63, 0)]
		[DataRow(64, 6)]
		[DataRow(65, 0)]
		[DataRow(127, 0)]
		[DataRow(128, 7)]
		[DataRow(129, 0)]
		[DataRow(160, 5)]
		[DataRow(164, 2)]
		[DataRow(176, 4)]
		[DataRow(182, 1)]
		[DataRow(192, 6)]
		[DataRow(255, 0)]
		[DataRow(256, 8)]
		[DataRow(257, 0)]
		[DataRow(512, 9)]
		[DataRow(1024, 10)]
		[DataRow(1073741824, 30)]
		[DataRow(1073741825, 0)]
		[DataRow(1107296512, 8)]
		[DataRow(2147483648, 31)]
		[DataRow(1000000000000000000, 18)]
		[DataRow(9223372036854775807, 0)]
		[DataRow(9223372036854775808, 63)]
		[DataRow(18446744073709551614, 1)]
		[DataRow(18446744073709551615, 0)]

		public void TestCountTrailingZeroBits(object value, int target)
		{
			Assert.AreEqual(target, UInt64Util.CountTrailingZeroBits(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.CountTrailingBits(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 1)]
		[DataRow(2, 0)]
		[DataRow(3, 2)]
		[DataRow(4, 0)]
		[DataRow(5, 1)]
		[DataRow(7, 3)]
		[DataRow(8, 0)]
		[DataRow(9, 1)]
		[DataRow(15, 4)]
		[DataRow(16, 0)]
		[DataRow(17, 1)]
		[DataRow(31, 5)]
		[DataRow(32, 0)]
		[DataRow(33, 1)]
		[DataRow(36, 0)]
		[DataRow(43, 2)]
		[DataRow(63, 6)]
		[DataRow(64, 0)]
		[DataRow(65, 1)]
		[DataRow(127, 7)]
		[DataRow(128, 0)]
		[DataRow(129, 1)]
		[DataRow(159, 5)]
		[DataRow(160, 0)]
		[DataRow(163, 2)]
		[DataRow(167, 3)]
		[DataRow(175, 4)]
		[DataRow(182, 0)]
		[DataRow(191, 6)]
		[DataRow(255, 8)]
		[DataRow(256, 0)]
		[DataRow(257, 1)]
		[DataRow(511, 9)]
		[DataRow(512, 0)]
		[DataRow(1023, 10)]
		[DataRow(1024, 0)]
		[DataRow(1073741823, 30)]
		[DataRow(1073741824, 0)]
		[DataRow(1073741825, 1)]
		[DataRow(2147483631, 4)]
		[DataRow(2147483647, 31)]
		[DataRow(2147483648, 0)]
		[DataRow(2147483649, 1)]
		[DataRow(4026531839, 28)]
		[DataRow(1000000000000000000, 0)]
		[DataRow(9223372036854775807, 63)]
		[DataRow(9223372036854775808, 0)]
		[DataRow(9223372036854775809, 1)]
		[DataRow(18446726481523507199, 44)]
		[DataRow(18446744073709551599, 4)]
		[DataRow(18446744073709551614, 0)]
		[DataRow(18446744073709551615, 64)]

		public void TestCountTrailingBits(object value, int target)
		{
			Assert.AreEqual(target, UInt64Util.CountTrailingBits(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.CeilingPowerOf2(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 1)]
		[DataRow(2, 2)]
		[DataRow(3, 4)]
		[DataRow(4, 4)]
		[DataRow(5, 8)]
		[DataRow(7, 8)]
		[DataRow(8, 8)]
		[DataRow(9, 16)]
		[DataRow(15, 16)]
		[DataRow(16, 16)]
		[DataRow(17, 32)]
		[DataRow(31, 32)]
		[DataRow(32, 32)]
		[DataRow(33, 64)]
		[DataRow(63, 64)]
		[DataRow(64, 64)]
		[DataRow(65, 128)]
		[DataRow(127, 128)]
		[DataRow(128, 128)]
		[DataRow(129, 256)]
		[DataRow(255, 256)]
		[DataRow(256, 256)]
		[DataRow(268435456, 268435456)]
		[DataRow(268435457, 536870912)]
		[DataRow(536870913, 1073741824)]
		[DataRow(1073741824, 1073741824)]
		[DataRow(1073741825, 2147483648)]
		[DataRow(2147483647, 2147483648)]
		[DataRow(2147483648, 2147483648)]
		[DataRow(2147483649, 4294967296)]
		[DataRow(1369097585255514112, 2305843009213693952)]
		[DataRow(4611686018427387904, 4611686018427387904)]
		[DataRow(4899916394579099648, 9223372036854775808)]
		[DataRow(18446744072414633275, 0)]
		[DataRow(18446744073441116159, 0)]
		[DataRow(18446744072414633275, 0)]
		[DataRow(18446744073441116159, 0)]
		[DataRow(18446744073709551615, 0)]
		public void TestCeilingPowerOf2(object value, object target)
		{
			Assert.AreEqual(Convert.ToUInt64(target), UInt64Util.CeilingPowerOf2(Convert.ToUInt64(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt64Util.NextBitPermutation(ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(new ulong[] { 0, 0 })]
		[DataRow(new ulong[] { 18446744073709551615, 18446744073709551615 })]
		[DataRow(new ulong[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072,
			262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864, 134217728, 268435456 })]
		[DataRow(new ulong[] { 2305843009213693952, 4611686018427387904, 9223372036854775808, 1, 2, 4, 8, 16 })]
		[DataRow(new ulong[] { 2145386494, 2145910783, 2146172927, 2146303999, 2146369535, 2146402303, 2146418687, 2146426879 })]
		[DataRow(new ulong[] { 2146435069, 2146435070, 2146959358 })]
		[DataRow(new ulong[] { 2147483644, 2415919103, 2550136831, 2617245695, 2650800127 })]
		[DataRow(new ulong[] { 9223372036854775804, 10376293541461622783, 10952754293765046271, 11240984669916758015 })]
		[DataRow(new ulong[] { 18446744073709551613, 18446744073709551614, 9223372036854775807, 13835058055282163711 })]
		[DataRow(new ulong[] { 18446744073709551609, 18446744073709551610, 18446744073709551612, 4611686018427387903,
			6917529027641081855, 8070450532247928831 })]
		public void TestNextBitPermutation(ulong[] values)
		{
			for (int i = 1; i < values.Length; i++)
			{
				Assert.AreEqual(values[i], UInt64Util.NextBitPermutation(values[i - 1]), "NextBitPermutation of {0}", values[i - 1]);
			}
		}
	}
}
