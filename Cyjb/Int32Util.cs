using System.Numerics;

namespace Cyjb;

/// <summary>
/// 提供对 <see cref="int"/> 的扩展方法。
/// </summary>
/// <remarks>位运算的算法来自于 
/// <see href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</see>。</remarks>
/// <seealso href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</seealso>
public static class Int32Util
{

	#region Times

	/// <summary>
	/// 将指定操作执行多次。
	/// </summary>
	/// <param name="source">要执行操作的次数。只有大于 <c>0</c> 时才有效。</param>
	/// <param name="action">要执行的操作。</param>
	/// <overloads>
	/// <summary>
	/// 将指定操作执行多次，或者得到指定值重复多次的序列。
	/// </summary>
	/// </overloads>
	public static void Times(this int source, Action action)
	{
		for (int i = 0; i < source; i++)
		{
			action();
		}
	}

	/// <summary>
	/// 返回将指定值重复多次的序列。
	/// </summary>
	/// <typeparam name="T">要重复的值的类型。</typeparam>
	/// <param name="source">要重复的次数。只有大于 <c>0</c> 时才有效。</param>
	/// <param name="value">要重复的值。</param>
	/// <returns>将指定值重复多次的序列。</returns>
	public static IEnumerable<T> Times<T>(this int source, T value)
	{
		for (int i = 0; i < source; i++)
		{
			yield return value;
		}
	}
	/// <summary>
	/// 返回将指定函数的返回值重复多次的序列。
	/// </summary>
	/// <typeparam name="T">要重复的值的类型。</typeparam>
	/// <param name="source">要重复的次数。只有大于 <c>0</c> 时才有效。</param>
	/// <param name="value">返回要重复的值的函数。</param>
	/// <returns>将指定函数的返回值重复多次的序列。</returns>
	public static IEnumerable<T> Times<T>(this int source, Func<T> value)
	{
		for (int i = 0; i < source; i++)
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
	public static IEnumerable<int> To(this int source, int destination)
	{
		if (source < destination)
		{
			while (source <= destination)
			{
				yield return source;
				source++;
			}
		}
		else
		{
			while (source >= destination)
			{
				yield return source;
				source--;
			}
		}
	}

	/// <summary>
	/// 从当前值递增（递减）到指定值并执行指定操作。
	/// </summary>
	/// <param name="source">要执行操作的起始值。</param>
	/// <param name="destination">要执行操作的目标值。</param>
	/// <param name="action">要执行的操作，参数为当前的值。</param>
	public static void To(this int source, int destination, Action<int> action)
	{
		if (source < destination)
		{
			while (source <= destination)
			{
				action(source);
				source++;
			}
		}
		else
		{
			while (source >= destination)
			{
				action(source);
				source--;
			}
		}
	}

	#endregion // To

	#region 位运算

	/// <summary>
	/// 判断指定 <see cref="int"/> 是否是 <c>2</c> 的幂。
	/// </summary>
	/// <param name="value">要判断是否是 <c>2</c> 的幂的 <see cref="int"/>。</param>
	/// <returns>如果 <paramref name="value"/> 是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <remarks>对于 <c>0</c> 或负数，结果为 <c>false</c>。</remarks>
	public static bool IsPowerOf2(this int value)
	{
		return BitOperations.IsPow2(value);
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 的二进制表示中 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 的二进制表示中 <c>1</c> 的个数。</returns>
	public static int CountBits(this int value)
	{
		return BitOperations.PopCount((uint)value);
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 的偶校验位。
	/// </summary>
	/// <param name="value">要计算偶校验位的 <see cref="int"/>。</param>
	/// <returns>如果 <paramref name="value"/> 的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；否则为 <c>0</c>。</returns>
	public static int Parity(this int value)
	{
		value ^= value >> 1;
		value ^= value >> 2;
		value = (value & 0x11111111) * 0x11111111;
		return (value >> 28) & 1;
	}

	/// <summary>
	/// 将指定 <see cref="int"/> 的二进制位逆序。
	/// </summary>
	/// <param name="value">要二进制位逆序的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 二进制位逆序的结果。</returns>
	public static int ReverseBits(this int value)
	{
		return (int)UInt32Util.ReverseBits((uint)value);
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 以 <c>2</c> 为底的对数值，如果 <paramref name="value"/> 小于等于 <c>0</c>，则返回 <c>0</c>。</returns>
	public static int LogBase2(this int value)
	{
		return value <= 0 ? 0 : UInt32Util.LogBase2((uint)value);
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
	/// </summary>
	/// <param name="value">要计算对数的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 以 <c>10</c> 为底的对数值，如果 <paramref name="value"/> 小于等于 <c>0</c>，则返回 <c>0</c>。</returns>
	public static int LogBase10(this int value)
	{
		if (value >= 1000000000) { return 9; }
		else if (value >= 100000000) { return 8; }
		else if (value >= 10000000) { return 7; }
		else if (value >= 1000000) { return 6; }
		else if (value >= 100000) { return 5; }
		else if (value >= 10000) { return 4; }
		else if (value >= 1000) { return 3; }
		else if (value >= 100) { return 2; }
		else if (value >= 10) { return 1; }
		else { return 0; }
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 的二进制表示中末尾连续 <c>0</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
	public static int CountTrailingZeroBits(this int value)
	{
		return BitOperations.TrailingZeroCount(value);
	}

	/// <summary>
	/// 计算指定 <see cref="int"/> 的二进制表示中末尾连续 <c>1</c> 的个数。
	/// </summary>
	/// <param name="value">要计算的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
	public static int CountTrailingBits(this int value)
	{
		return UInt32Util.CountTrailingBits((uint)value);
	}

	/// <summary>
	/// 返回大于或等于指定 <see cref="int"/> 的最小的 <c>2</c> 的幂次。
	/// </summary>
	/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
	/// <returns>大于或等于 <paramref name="value"/> 的最小的 <c>2</c> 的幂次。
	/// 如果 <paramref name="value"/> 小于 <c>0</c> 或结果溢出，则返回 <c>0</c>。</returns>
	public static int CeilingPowerOf2(this int value)
	{
		if (value <= 0 || value > 1073741824)
		{
			return 0;
		}
		return (int)BitOperations.RoundUpToPowerOf2((uint)value);
	}

	/// <summary>
	/// 返回指定 <see cref="int"/> 二进制表示的下一字典序排列。
	/// </summary>
	/// <param name="value">要获取下一字典序排列的 <see cref="int"/>。</param>
	/// <returns><paramref name="value"/> 二进制表示的下一字典序排列。</returns>
	/// <remarks>
	/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
	/// 而且是字典序的下一排列。</para>
	/// <para>例如，<c>102</c> 的二进制表示为 <c>0b1100110</c>，它的下一字典序排列为
	/// <c>0b1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
	/// <c>105</c> 的下一字典序排列为 <c>106</c> (<c>0b1101010</c>)，
	/// 接下来是 <c>108</c> (<c>0b1101100</c>)和 <c>113</c> (<c>0b1110001</c>)。
	/// </para></remarks>
	public static int NextBitPermutation(this int value)
	{
		return (int)UInt32Util.NextBitPermutation((uint)value);
	}

	#endregion // 位运算

}
