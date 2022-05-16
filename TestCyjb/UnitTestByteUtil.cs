using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// <see cref="ByteUtil"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestByteUtil
{
	/// <summary>
	/// 对 <see cref="ByteUtil.IsPowerOf2(byte)"/> 方法进行测试。
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
	public void TestIsPowerOf2(int value, bool target)
	{
		Assert.AreEqual(target, ByteUtil.IsPowerOf2((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.CountBits(byte)"/> 方法进行测试。
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
	public void TestCountBits(int value, int countBits)
	{
		Assert.AreEqual(countBits, ByteUtil.CountBits((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.Parity(byte)"/> 方法进行测试。
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
	public void TestParity(int value, int countBits)
	{
		Assert.AreEqual(countBits, ByteUtil.Parity((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.ReverseBits(byte)"/> 方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(0, 0)]
	[DataRow(1, 128)]
	[DataRow(2, 64)]
	[DataRow(3, 192)]
	[DataRow(4, 32)]
	[DataRow(7, 224)]
	[DataRow(8, 16)]
	[DataRow(15, 240)]
	[DataRow(16, 8)]
	[DataRow(31, 248)]
	[DataRow(32, 4)]
	[DataRow(63, 252)]
	[DataRow(64, 2)]
	[DataRow(99, 198)]
	[DataRow(107, 214)]
	[DataRow(127, 254)]
	[DataRow(128, 1)]
	[DataRow(164, 37)]
	[DataRow(243, 207)]
	[DataRow(247, 239)]
	[DataRow(255, 255)]
	public void TestReverseBits(int value, int target)
	{
		Assert.AreEqual((byte)target, ByteUtil.ReverseBits((byte)value));
		Assert.AreEqual((byte)value, ByteUtil.ReverseBits((byte)target));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.LogBase2(byte)"/> 方法进行测试。
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
	public void TestLogBase2(int value, int target)
	{
		Assert.AreEqual((byte)target, ByteUtil.LogBase2((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.LogBase10(byte)"/> 方法进行测试。
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
	public void TestLogBase10(int value, int target)
	{
		Assert.AreEqual(target, ByteUtil.LogBase10((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.CountTrailingZeroBits(byte)"/> 方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(0, 8)]
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

	public void TestCountTrailingZeroBits(int value, int target)
	{
		Assert.AreEqual(target, ByteUtil.CountTrailingZeroBits((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.CountTrailingBits(byte)"/> 方法进行测试。
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

	public void TestCountTrailingBits(int value, int target)
	{
		Assert.AreEqual(target, ByteUtil.CountTrailingBits((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.CeilingPowerOf2(byte)"/> 方法进行测试。
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
	[DataRow(129, 0)]
	[DataRow(255, 0)]
	public void TestCeilingPowerOf2(int value, int target)
	{
		Assert.AreEqual(target, ByteUtil.CeilingPowerOf2((byte)value));
	}

	/// <summary>
	/// 对 <see cref="ByteUtil.NextBitPermutation(byte)"/> 方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(new byte[] { 0, 0 })]
	[DataRow(new byte[] { 255, 255 })]
	[DataRow(new byte[] { 1, 2, 4, 8, 16, 32, 64, 128, 1 })]
	[DataRow(new byte[] { 40, 48, 65, 66, 68, 72, 80, 96, 129, 130, 132, 136, 144, 160, 192, 3, 5, 6, 9, 10, 12, 17, 18, 20, 24, 33, 34, 36, 40 })]
	public void TestNextBitPermutation(byte[] values)
	{
		for (int i = 1; i < values.Length; i++)
		{
			Assert.AreEqual(values[i], ByteUtil.NextBitPermutation(values[i - 1]), "NextBitPermutation of {0}", values[i - 1]);
		}
	}
}
