using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="UInt32Util"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestUInt32Util
	{
		/// <summary>
		/// 对 <see cref="UInt32Util.IsPowerOf2(uint)"/> 方法进行测试。
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
		[DataRow(3221225472, false)]
		public void TestIsPowerOf2(object value, bool target)
		{
			Assert.AreEqual(target, UInt32Util.IsPowerOf2(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.CountBits(uint)"/> 方法进行测试。
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
		[DataRow(4294967294, 31)]
		[DataRow(4294967295, 32)]
		public void TestCountBits(object value, int countBits)
		{
			Assert.AreEqual(countBits, UInt32Util.CountBits(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.Parity(uint)"/> 方法进行测试。
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
		[DataRow(4294967294, 1)]
		[DataRow(4294967295, 0)]
		public void TestParity(object value, int countBits)
		{
			Assert.AreEqual(countBits, UInt32Util.Parity(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.ReverseBits(uint)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(1, 2147483648)]
		[DataRow(2, 1073741824)]
		[DataRow(3, 3221225472)]
		[DataRow(4, 536870912)]
		[DataRow(7, 3758096384)]
		[DataRow(8, 268435456)]
		[DataRow(15, 4026531840)]
		[DataRow(16, 134217728)]
		[DataRow(31, 4160749568)]
		[DataRow(32, 67108864)]
		[DataRow(63, 4227858432)]
		[DataRow(64, 33554432)]
		[DataRow(99, 3321888768)]
		[DataRow(107, 3590324224)]
		[DataRow(127, 4261412864)]
		[DataRow(128, 16777216)]
		[DataRow(164, 620756992)]
		[DataRow(243, 3472883712)]
		[DataRow(247, 4009754624)]
		[DataRow(255, 4278190080)]
		[DataRow(256, 8388608)]
		[DataRow(257, 2155872256)]
		[DataRow(511, 4286578688)]
		[DataRow(1073741823, 4294967292)]
		[DataRow(1073741824, 2)]
		[DataRow(1160070801, 2304943266)]
		[DataRow(2147483646, 2147483646)]
		[DataRow(2147483647, 4294967294)]
		[DataRow(2147483648, 1)]
		[DataRow(2147483649, 2147483649)]
		[DataRow(4294967294, 2147483647)]
		[DataRow(4294967295, 4294967295)]
		public void TestReverseBits(object value, object target)
		{
			Assert.AreEqual(Convert.ToUInt32(target), UInt32Util.ReverseBits(Convert.ToUInt32(value)));
			Assert.AreEqual(Convert.ToUInt32(value), UInt32Util.ReverseBits(Convert.ToUInt32(target)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.LogBase2(uint)"/> 方法进行测试。
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
		[DataRow(4294967295, 31)]
		public void TestLogBase2(object value, int target)
		{
			Assert.AreEqual(target, UInt32Util.LogBase2(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.LogBase10(uint)"/> 方法进行测试。
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
		[DataRow(2147483648, 9)]
		[DataRow(4294967295, 9)]
		public void TestLogBase10(object value, int target)
		{
			Assert.AreEqual(target, UInt32Util.LogBase10(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.CountTrailingZeroBits(uint)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 32)]
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
		[DataRow(4294967294, 1)]
		[DataRow(4294967295, 0)]

		public void TestCountTrailingZeroBits(object value, int target)
		{
			Assert.AreEqual(target, UInt32Util.CountTrailingZeroBits(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.CountTrailingBits(uint)"/> 方法进行测试。
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
		[DataRow(4026531839, 28)]
		[DataRow(4294967279, 4)]
		[DataRow(4294967294, 0)]
		[DataRow(4294967295, 32)]

		public void TestCountTrailingBits(object value, int target)
		{
			Assert.AreEqual(target, UInt32Util.CountTrailingBits(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.CountTrailingBits(uint)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(0x80000000, 1)]
		[DataRow(0x40000000, 0)]
		[DataRow(0xC0000000, 2)]
		[DataRow(0x20000000, 0)]
		[DataRow(0xA0000000, 1)]
		[DataRow(0xE0000000, 3)]
		[DataRow(0x10000000, 0)]
		[DataRow(0x90000000, 1)]
		[DataRow(0xF0000000, 4)]
		[DataRow(0x8000000, 0)]
		[DataRow(0x88000000, 1)]
		[DataRow(0xF8000000, 5)]
		[DataRow(0x4000000, 0)]
		[DataRow(0x84000000, 1)]
		[DataRow(0x24000000, 0)]
		[DataRow(0xD4000000, 2)]
		[DataRow(0xFC000000, 6)]
		[DataRow(0x2000000, 0)]
		[DataRow(0x82000000, 1)]
		[DataRow(0xFE000000, 7)]
		[DataRow(0x1000000, 0)]
		[DataRow(0x81000000, 1)]
		[DataRow(0xF9000000, 5)]
		[DataRow(0x5000000, 0)]
		[DataRow(0xC5000000, 2)]
		[DataRow(0xE5000000, 3)]
		[DataRow(0xF5000000, 4)]
		[DataRow(0x6D000000, 0)]
		[DataRow(0xFD000000, 6)]
		[DataRow(0xFF000000, 8)]
		[DataRow(0x800000, 0)]
		[DataRow(0x80800000, 1)]
		[DataRow(0xFF800000, 9)]
		[DataRow(0x400000, 0)]
		[DataRow(0xFFC00000, 10)]
		[DataRow(0x200000, 0)]
		[DataRow(0xFFFFFFFC, 30)]
		[DataRow(0x2, 0)]
		[DataRow(0x80000002, 1)]
		[DataRow(0xF7FFFFFE, 4)]
		[DataRow(0xFFFFFFFE, 31)]
		[DataRow(0x1, 0)]
		[DataRow(0xFFFFFFF7, 28)]
		[DataRow(0xF7FFFFFF, 4)]
		[DataRow(0x7FFFFFFF, 0)]
		[DataRow(0xFFFFFFFF, 32)]

		public void TestCountLeadingBits(object value, int target)
		{
			Assert.AreEqual(target, UInt32Util.CountLeadingBits(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.CeilingPowerOf2(uint)"/> 方法进行测试。
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
		[DataRow(2147483649, 0)]
		[DataRow(3000048955, 0)]
		[DataRow(4026531839, 0)]
		[DataRow(4294967295, 0)]
		public void TestCeilingPowerOf2(object value, object target)
		{
			Assert.AreEqual(Convert.ToUInt32(target), UInt32Util.CeilingPowerOf2(Convert.ToUInt32(value)));
		}

		/// <summary>
		/// 对 <see cref="UInt32Util.NextBitPermutation(uint)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(0U, 0U)]
		[DataRow(21U, 22U, 25U, 26U, 28U, 35U)]
		[DataRow(4294967295U, 4294967295U)]
		[DataRow(1U, 2U, 4U, 8U, 16U, 32U, 64U, 128U, 256U, 512U, 1024U, 2048U, 4096U, 8192U,
			16384U, 32768U, 65536U, 131072U, 262144U, 524288U, 1048576U, 2097152U, 4194304U,
			8388608U, 16777216U, 33554432U, 67108864U, 134217728U, 268435456U, 536870912U,
			1073741824U, 2147483648U, 1U)]
		[DataRow(2145386494U, 2145910783U, 2146172927U, 2146303999U, 2146369535U, 2146402303U,
			2146418687U, 2146426879U)]
		[DataRow(2146435069U, 2146435070U, 2146697215U)]
		[DataRow(2147483644U, 2415919103U, 2550136831U, 2617245695U, 2650800127U)]
		[DataRow(4294967293U, 4294967294U, 2147483647U, 3221225471U, 3758096383U)]
		[DataRow(4294967289U, 4294967290U, 4294967292U, 1073741823U, 1610612735U, 1879048191U, 2013265919U)]
		public void TestNextBitPermutation(params uint[] values)
		{
			for (int i = 1; i < values.Length; i++)
			{
				Assert.AreEqual(values[i], UInt32Util.NextBitPermutation(values[i - 1]), "NextBitPermutation of {0}", values[i - 1]);
			}
		}
	}
}
