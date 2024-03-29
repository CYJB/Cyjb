using System.Numerics;

namespace Cyjb;

/// <summary>
/// 提供对 <see cref="long"/> 的扩展方法。
/// </summary>
/// <remarks>位运算的算法来自于 
/// <see href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</see>。</remarks>
/// <seealso href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</seealso>
public static class Int64Util
{

	#region 位运算

	/// <summary>
	/// 判断指定 <see cref="long"/> 是否是 <c>2</c> 的幂。
	/// </summary>
	/// <param name="value">要判断是否是 <c>2</c> 的幂的 <see cref="long"/> 。</param>
	/// <returns>如果 <paramref name="value"/> 是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>对于 <c>0</c> 或负数，结果为 <c>false</c>。</remarks>
	public static bool IsPowerOf2(this long value)
	{
		return BitOperations.IsPow2(value);
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 的二进制表示中 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中 <c>1</c> 的个数。</returns>
	public static int CountBits(this long value)
	{
		return BitOperations.PopCount((ulong)value);
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 的偶校验位。
	/// </summary>
	/// <param name="value">要计算偶校验位的 <see cref="long"/> 。</param>
	/// <returns>如果 <paramref name="value"/> 的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；否则为 <c>0</c>。</returns>
	public static int Parity(this long value)
	{
		value ^= value >> 1;
		value ^= value >> 2;
		value = (value & 0x1111111111111111L) * 0x1111111111111111L;
		return (int)((value >> 60) & 1);
	}

	/// <summary>
	/// 将指定 <see cref="long"/> 的二进制位逆序。
	/// </summary>
	/// <param name="value">要二进制位逆序的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 二进制位逆序的结果。</returns>
	public static long ReverseBits(this long value)
	{
		return (long)UInt64Util.ReverseBits((ulong)value);
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 以 <c>2</c> 为底的对数值，如果 <paramref name="value"/> 小于等于 <c>0</c>，则返回 <c>0</c>。</returns>
	public static int LogBase2(this long value)
	{
		return value <= 0 ? 0 : UInt64Util.LogBase2((ulong)value);
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 以 <c>10</c> 为底的对数值，如果 <paramref name="value"/> 小于等于 <c>0</c>，则返回 <c>0</c>。</returns>
	public static int LogBase10(this long value)
	{
		if (value >= 1000000000000000000L) { return 18; }
		else if (value >= 100000000000000000L) { return 17; }
		else if (value >= 10000000000000000L) { return 16; }
		else if (value >= 1000000000000000L) { return 15; }
		else if (value >= 100000000000000L) { return 14; }
		else if (value >= 10000000000000L) { return 13; }
		else if (value >= 1000000000000L) { return 12; }
		else if (value >= 100000000000L) { return 11; }
		else if (value >= 10000000000L) { return 10; }
		else if (value >= 1000000000L) { return 9; }
		else if (value >= 100000000L) { return 8; }
		else if (value >= 10000000L) { return 7; }
		else if (value >= 1000000L) { return 6; }
		else if (value >= 100000L) { return 5; }
		else if (value >= 10000L) { return 4; }
		else if (value >= 1000L) { return 3; }
		else if (value >= 100L) { return 2; }
		else if (value >= 10L) { return 1; }
		else { return 0; }
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 的二进制表示中末尾连续 <c>0</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
	public static int CountTrailingZeroBits(this long value)
	{
		return BitOperations.TrailingZeroCount(value);
	}

	/// <summary>
	/// 计算指定 <see cref="long"/> 的二进制表示中末尾连续 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
	public static int CountTrailingBits(this long value)
	{
		return UInt64Util.CountTrailingBits((ulong)value);
	}

	/// <summary>
	/// 返回大于或等于指定 <see cref="long"/> 的最小的 <c>2</c> 的幂次。
	/// </summary>
	/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
	/// <returns>大于或等于 <paramref name="value"/> 的最小的 <c>2</c> 的幂次。
	/// 如果 <paramref name="value"/> 小于 <c>0</c> 或结果溢出，则返回 <c>0</c>。</returns>
	public static long CeilingPowerOf2(this long value)
	{
		if (value <= 0L || value > 4611686018427387904L)
		{
			return 0;
		}
		return (long)BitOperations.RoundUpToPowerOf2((ulong)value);
	}

	/// <summary>
	/// 返回指定 <see cref="long"/> 二进制表示的下一字典序排列。
	/// </summary>
	/// <param name="value">要获取下一字典序排列的 <see cref="long"/> 。</param>
	/// <returns><paramref name="value"/> 二进制表示的下一字典序排列。</returns>
	/// <remarks>
	/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
	/// 而且是字典序的下一排列。</para>
	/// <para>例如，<c>102</c> 的二进制表示为 <c>0b1100110</c>，它的下一字典序排列为
	/// <c>0b1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
	/// <c>105</c> 的下一字典序排列为 <c>106</c> (<c>0b1101010</c>)，
	/// 接下来是 <c>108</c> (<c>0b1101100</c>)和 <c>113</c> (<c>0b1110001</c>)。
	/// </para></remarks>
	public static long NextBitPermutation(this long value)
	{
		return (long)UInt64Util.NextBitPermutation((ulong)value);
	}

	#endregion // 位运算

}
