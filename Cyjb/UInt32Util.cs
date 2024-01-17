using System.Numerics;

namespace Cyjb;

/// <summary>
/// 提供对 <see cref="uint"/>  的扩展方法。
/// </summary>
/// <remarks>位运算的算法来自于 
/// <see href="https://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</see>。</remarks>
/// <seealso href="https://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</seealso>
public static class UInt32Util
{

	#region Times

	/// <summary>
	/// 将指定操作执行多次。
	/// </summary>
	/// <param name="source">要执行操作的次数。</param>
	/// <param name="action">要执行的操作。</param>
	/// <overloads>
	/// <summary>
	/// 将指定操作执行多次，或者得到指定值重复多次的序列。
	/// </summary>
	/// </overloads>
	[CLSCompliant(false)]
	public static void Times(this uint source, Action action)
	{
		for (uint i = 0; i < source; i++)
		{
			action();
		}
	}
	/// <summary>
	/// 返回将指定值重复多次的序列。
	/// </summary>
	/// <typeparam name="T">要重复的值的类型。</typeparam>
	/// <param name="source">要重复的次数。</param>
	/// <param name="value">要重复的值。</param>
	/// <returns>将指定值重复多次的序列。</returns>
	[CLSCompliant(false)]
	public static IEnumerable<T> Times<T>(this uint source, T value)
	{
		for (uint i = 0; i < source; i++)
		{
			yield return value;
		}
	}
	/// <summary>
	/// 返回将指定函数的返回值重复多次的序列。
	/// </summary>
	/// <typeparam name="T">要重复的值的类型。</typeparam>
	/// <param name="source">要重复的次数。</param>
	/// <param name="value">返回要重复的值的函数。</param>
	/// <returns>将指定函数的返回值重复多次的序列。</returns>
	[CLSCompliant(false)]
	public static IEnumerable<T> Times<T>(this uint source, Func<T> value)
	{
		for (uint i = 0; i < source; i++)
		{
			yield return value();
		}
	}

	#endregion // Times

	#region To

	/// <summary>
	/// 返回从当前值递增（递减）到指定值的序列。
	/// </summary>
	/// <param name="source">要执行操作的起始值。</param>
	/// <param name="destination">要执行操作的目标值。</param>
	/// <returns>数值递增（递减）的序列。</returns>
	/// <overloads>
	/// <summary>
	/// 返回从当前值递增（递减）到指定值的序列。
	/// </summary>
	/// </overloads>
	[CLSCompliant(false)]
	public static IEnumerable<uint> To(this uint source, uint destination)
	{
		if (source < destination)
		{
			while (source < destination)
			{
				yield return source;
				source++;
			}
			yield return source;
		}
		else
		{
			while (source > destination)
			{
				yield return source;
				source--;
			}
			yield return source;
		}
	}
	/// <summary>
	/// 从当前值递增（递减）到指定值并执行操作。
	/// </summary>
	/// <param name="source">要执行操作的起始值。</param>
	/// <param name="destination">要执行操作的目标值。</param>
	/// <param name="action">要执行的操作，参数为当前的值。</param>
	[CLSCompliant(false)]
	public static void To(this uint source, uint destination, Action<uint> action)
	{
		if (source < destination)
		{
			while (source < destination)
			{
				action(source);
				source++;
			}
			action(source);
		}
		else
		{
			while (source > destination)
			{
				action(source);
				source--;
			}
			action(source);
		}
	}

	#endregion // To

	#region 位运算

	/// <summary>
	/// 判断指定 <see cref="uint"/> 是否是 <c>2</c> 的幂。
	/// </summary>
	/// <param name="value">要判断是否是 <c>2</c> 的幂的 <see cref="uint"/> 。</param>
	/// <returns>如果果 <paramref name="value"/> 是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>对于 <c>0</c>，结果为 <c>false</c>。</remarks>
	[CLSCompliant(false)]
	public static bool IsPowerOf2(this uint value)
	{
		return BitOperations.IsPow2(value);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的二进制表示中 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中 <c>1</c> 的个数。</returns>
	[CLSCompliant(false)]
	public static int CountBits(this uint value)
	{
		return BitOperations.PopCount(value);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的偶校验位。
	/// </summary>
	/// <param name="value">要计算偶校验位的 <see cref="uint"/> 。</param>
	/// <returns>如果 <paramref name="value"/> 的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；否则为 <c>0</c>。</returns>
	[CLSCompliant(false)]
	public static int Parity(this uint value)
	{
		value ^= value >> 16;
		value ^= value >> 8;
		value ^= value >> 4;
		value &= 0xFU;
		return (0x6996 >> (int)value) & 1;
	}

	/// <summary>
	/// 将指定 <see cref="uint"/> 的二进制位逆序。
	/// </summary>
	/// <param name="value">要二进制位逆序的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 二进制位逆序的结果。</returns>
	[CLSCompliant(false)]
	public static uint ReverseBits(this uint value)
	{
		value = ((value >> 1) & 0x55555555U) | ((value & 0x55555555U) << 1);
		value = ((value >> 2) & 0x33333333U) | ((value & 0x33333333U) << 2);
		value = ((value >> 4) & 0x0F0F0F0FU) | ((value & 0x0F0F0F0FU) << 4);
		value = ((value >> 8) & 0x00FF00FFU) | ((value & 0x00FF00FFU) << 8);
		value = (value >> 16) | (value << 16);
		return value;
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 以 <c>2</c> 为底的对数值，如果 <paramref name="value"/> 等于 <c>0</c>，则返回 <c>0</c>。</returns>
	[CLSCompliant(false)]
	public static int LogBase2(this uint value)
	{
		return BitOperations.Log2(value);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 以 <c>10</c> 为底的对数值。</returns>
	[CLSCompliant(false)]
	public static int LogBase10(this uint value)
	{
		if (value >= 1000000000U) { return 9; }
		else if (value >= 100000000U) { return 8; }
		else if (value >= 10000000U) { return 7; }
		else if (value >= 1000000U) { return 6; }
		else if (value >= 100000U) { return 5; }
		else if (value >= 10000U) { return 4; }
		else if (value >= 1000U) { return 3; }
		else if (value >= 100U) { return 2; }
		else if (value >= 10U) { return 1; }
		else { return 0; }
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的二进制表示中末尾（低位）连续 <c>0</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中末尾（低位）连续 <c>0</c> 的个数。</returns>
	[CLSCompliant(false)]
	public static int CountTrailingZeroBits(this uint value)
	{
		return BitOperations.TrailingZeroCount(value);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的二进制表示中末尾（低位）连续 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 二进制表示中末尾（低位）连续 <c>1</c> 的个数。</returns>
	[CLSCompliant(false)]
	public static int CountTrailingBits(this uint value)
	{
		if (value == uint.MaxValue)
		{
			// value + 1 会溢出，因此需要单独处理。
			return 32;
		}
		return BitOperations.PopCount((value ^ (value + 1U)) >> 1);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的二进制表示中开头（高位）连续 <c>0</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 的二进制表示中开头（高位）连续 <c>0</c> 的个数。</returns>
	[CLSCompliant(false)]
	public static int CountLeadingZeroBits(this uint value)
	{
		return BitOperations.LeadingZeroCount(value);
	}

	/// <summary>
	/// 计算指定 <see cref="uint"/> 的二进制表示中开头（高位）连续 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 二进制表示中开头（高位）连续 <c>1</c> 的个数。</returns>
	[CLSCompliant(false)]
	public static int CountLeadingBits(this uint value)
	{
		if ((value & 0x80000000) == 0)
		{
			return 0;
		}
		value ^= value >> 1;
		value &= 0x7FFFFFFF;
		return BitOperations.LeadingZeroCount(value);
	}

	/// <summary>
	/// 返回大于或等于指定 <see cref="uint"/> 的最小的 <c>2</c> 的幂次。
	/// </summary>
	/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
	/// <returns>大于或等于 <paramref name="value"/> 的最小的 <c>2</c> 的幂次。
	/// 如果 <paramref name="value"/> 为 <c>0</c> 或结果溢出，则返回 <c>0</c>。</returns>
	[CLSCompliant(false)]
	public static uint CeilingPowerOf2(this uint value)
	{
		return BitOperations.RoundUpToPowerOf2(value);
	}

	/// <summary>
	/// 返回指定 <see cref="uint"/> 二进制表示的下一字典序排列。
	/// </summary>
	/// <param name="value">要获取下一字典序排列的 <see cref="uint"/> 。</param>
	/// <returns><paramref name="value"/> 二进制表示的下一字典序排列。</returns>
	/// <remarks>
	/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
	/// 而且是字典序的下一排列。</para>
	/// <para>例如，<c>102</c> 的二进制表示为 <c>0b1100110</c>，它的下一字典序排列为
	/// <c>0b1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
	/// <c>105</c> 的下一字典序排列为 <c>106</c> (<c>0b1101010</c>)，
	/// 接下来是 <c>108</c> (<c>0b1101100</c>)和 <c>113</c> (<c>0b1110001</c>)。
	/// </para></remarks>
	[CLSCompliant(false)]
	public static uint NextBitPermutation(this uint value)
	{
		if (value == 0)
		{
			return 0;
		}
		uint t = value | (value - 1U);
		uint r = ((~t & (uint)(-(int)~t)) - 1U);
		int shift = BitOperations.TrailingZeroCount(value);
		// 这里分两次右移，这样在 >> 32 时可以正确得到结果 0。
		r = r >> shift >> 1;
		if (t < uint.MaxValue)
		{
			t = r | (t + 1U);
		}
		else
		{
			// 结果会超出 uint 有效范围，循环。
			t = r | (r + 1U);
		}
		return t;
	}

	#endregion // 位运算

}
