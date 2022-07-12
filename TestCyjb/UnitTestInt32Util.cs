using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// <see cref="Int32Util"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestInt32Util
{
	/// <summary>
	/// 对 <see cref="Int32Util.IsPowerOf2(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, false)]
	[DataRow(-2147483647, false)]
	[DataRow(-2, false)]
	[DataRow(-1, false)]
	public void TestIsPowerOf2(int value, bool target)
	{
		Assert.AreEqual(target, Int32Util.IsPowerOf2(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.CountBits(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 1)]
	[DataRow(-2147483647, 2)]
	[DataRow(-2, 31)]
	[DataRow(-1, 32)]
	public void TestCountBits(int value, int countBits)
	{
		Assert.AreEqual(countBits, Int32Util.CountBits(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.Parity(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 1)]
	[DataRow(-2147483647, 0)]
	[DataRow(-2, 1)]
	[DataRow(-1, 0)]
	public void TestParity(int value, int countBits)
	{
		Assert.AreEqual(countBits, Int32Util.Parity(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.ReverseBits(int)"/> 方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(0, 0)]
	[DataRow(1, -2147483648)]
	[DataRow(2, 1073741824)]
	[DataRow(3, -1073741824)]
	[DataRow(4, 536870912)]
	[DataRow(7, -536870912)]
	[DataRow(8, 268435456)]
	[DataRow(15, -268435456)]
	[DataRow(16, 134217728)]
	[DataRow(31, -134217728)]
	[DataRow(32, 67108864)]
	[DataRow(63, -67108864)]
	[DataRow(64, 33554432)]
	[DataRow(99, -973078528)]
	[DataRow(107, -704643072)]
	[DataRow(127, -33554432)]
	[DataRow(128, 16777216)]
	[DataRow(164, 620756992)]
	[DataRow(243, -822083584)]
	[DataRow(247, -285212672)]
	[DataRow(255, -16777216)]
	[DataRow(256, 8388608)]
	[DataRow(257, -2139095040)]
	[DataRow(511, -8388608)]
	[DataRow(1073741823, -4)]
	[DataRow(1073741824, 2)]
	[DataRow(1160070801, -1990024030)]
	[DataRow(2147483646, 2147483646)]
	[DataRow(2147483647, -2)]
	[DataRow(-2147483648, 1)]
	[DataRow(-2147483647, -2147483647)]
	[DataRow(-2, 2147483647)]
	[DataRow(-1, -1)]
	public void TestReverseBits(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.ReverseBits(value));
		Assert.AreEqual(value, Int32Util.ReverseBits(target));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.LogBase2(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 0)]
	[DataRow(-1, 0)]
	public void TestLogBase2(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.LogBase2(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.LogBase10(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 0)]
	[DataRow(-1, 0)]
	public void TestLogBase10(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.LogBase10(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.CountTrailingZeroBits(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 31)]
	[DataRow(-2, 1)]
	[DataRow(-1, 0)]

	public void TestCountTrailingZeroBits(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.CountTrailingZeroBits(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.CountTrailingBits(int)"/> 方法进行测试。
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
	[DataRow(-2147483648, 0)]
	[DataRow(-2147483647, 1)]
	[DataRow(-268435457, 28)]
	[DataRow(-17, 4)]
	[DataRow(-2, 0)]
	[DataRow(-1, 32)]

	public void TestCountTrailingBits(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.CountTrailingBits(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.CeilingPowerOf2(int)"/> 方法进行测试。
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
	[DataRow(1073741825, 0)]
	[DataRow(2147483647, 0)]
	[DataRow(-2147483648, 0)]
	[DataRow(-1294918341, 0)]
	[DataRow(-268435457, 0)]
	[DataRow(-1, 0)]
	public void TestCeilingPowerOf2(int value, int target)
	{
		Assert.AreEqual(target, Int32Util.CeilingPowerOf2(value));
	}

	/// <summary>
	/// 对 <see cref="Int32Util.NextBitPermutation(int)"/> 方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(0, 0)]
	[DataRow(-1, -1)]
	[DataRow(21, 22, 25, 26, 28, 35)]
	[DataRow(1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072,
		262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864, 134217728, 268435456,
		536870912, 1073741824, -2147483648, 1)]
	[DataRow(2145386494, 2145910783, 2146172927, 2146303999, 2146369535, 2146402303, 2146418687, 2146426879)]
	[DataRow(2146435069, 2146435070, 2146697215)]
	[DataRow(2147483644, -1879048193, -1744830465, -1677721601, -1644167169)]
	[DataRow(-3, -2, 2147483647, -1073741825, -536870913)]
	[DataRow(-7, -6, -4, 1073741823, 1610612735, 1879048191, 2013265919)]
	public void TestNextBitPermutation(params int[] values)
	{
		for (int i = 1; i < values.Length; i++)
		{
			Assert.AreEqual(values[i], Int32Util.NextBitPermutation(values[i - 1]), "NextBitPermutation of {0}", values[i - 1]);
		}
	}
}
