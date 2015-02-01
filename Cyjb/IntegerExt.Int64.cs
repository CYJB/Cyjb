using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 提供对整数的扩展方法。
	/// </summary>
	public static partial class IntegerExt
	{

		#region 常量定义

		/// <summary>
		/// 用于计算末尾连续零的个数的数组。
		/// </summary>
		private static readonly int[] multiplyDeBruijnBitPosition64 =
		{
			0, 1, 2, 56, 3, 32, 57, 46, 29, 4, 20, 33, 7, 58, 11, 47, 
			62, 30, 18, 5, 16, 21, 34, 23, 53, 8, 59, 36, 25, 12, 48, 39,
			63, 55, 31, 45, 28, 19, 6, 10, 61, 17, 15, 22, 52, 35, 24, 38, 
			54, 44, 27, 9, 60, 14, 51, 37, 43, 26, 13, 50, 42, 49, 41, 40 
		};

		#endregion // 常量定义

		#region 位运算

		/// <summary>
		/// 判断指定整数是否是 <c>2</c> 的幂。
		/// </summary>
		/// <param name="value">要判断是否是 <c>2</c> 的幂的整数。</param>
		/// <returns>如果指定整数是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>对于负数和 <c>0</c>，结果为 <c>false</c>。</remarks>
		public static bool IsPowerOf2(this long value)
		{
			return value > 0L && (value & (value - 1L)) == 0L;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中 <c>1</c> 的个数。</returns>
		public static int CountBits(this long value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			value -= (value >> 1) & 0x5555555555555555L;
			value = (value & 0x3333333333333333L) + ((value >> 2) & 0x3333333333333333L);
			value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FL;
			return (int)((value * 0x0101010101010101L) >> 56);
		}
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的整数。</param>
		/// <returns>如果指定整数的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；
		/// 否则为 <c>0</c>。</returns>
		public static int Parity(this long value)
		{
			Contract.Ensures(Contract.Result<int>() == 0 || Contract.Result<int>() == 1);
			value ^= value >> 1;
			value ^= value >> 2;
			value = (value & 0x1111111111111111L) * 0x1111111111111111L;
			return (int)((value >> 60) & 1);
		}
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的整数。</param>
		/// <returns>指定整数二进制位逆序的结果。</returns>
		public static long ReverseBits(this long value)
		{
			value = ((value >> 1) & 0x5555555555555555L) | ((value & 0x5555555555555555L) << 1);
			value = ((value >> 2) & 0x3333333333333333L) | ((value & 0x3333333333333333L) << 2);
			value = ((value >> 4) & 0x0F0F0F0F0F0F0F0FL) | ((value & 0x0F0F0F0F0F0F0F0FL) << 4);
			value = ((value >> 8) & 0x00FF00FF00FF00FFL) | ((value & 0x00FF00FF00FF00FFL) << 8);
			value = ((value >> 16) & 0x0000FFFF0000FFFFL) | ((value & 0x0000FFFF0000FFFFL) << 16);
			value = (value >> 32) | (value << 32);
			return value;
		}
		/// <summary>
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>2</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 小于等于 <c>0</c>，则结果未知。</returns>
		public static int LogBase2(this long value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			return LogBase2((ulong)value);
		}
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>10</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 小于等于 <c>10</c>，则结果未知。</returns>
		public static int LogBase10(this long value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 18);
			if (value >= 1000000000000000000L) { return 18; }
			if (value >= 100000000000000000L) { return 17; }
			if (value >= 10000000000000000L) { return 16; }
			if (value >= 1000000000000000L) { return 15; }
			if (value >= 100000000000000L) { return 14; }
			if (value >= 10000000000000L) { return 13; }
			if (value >= 1000000000000L) { return 12; }
			if (value >= 100000000000L) { return 11; }
			if (value >= 10000000000L) { return 10; }
			if (value >= 1000000000L) { return 9; }
			if (value >= 100000000L) { return 8; }
			if (value >= 10000000L) { return 7; }
			if (value >= 1000000L) { return 6; }
			if (value >= 100000L) { return 5; }
			if (value >= 10000L) { return 4; }
			if (value >= 1000L) { return 3; }
			if (value >= 100L) { return 2; }
			return (value >= 10L) ? 1 : 0;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
		public static int CountTrailingZeroBits(this long value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			return multiplyDeBruijnBitPosition64[((ulong)((value & -value) * 0x26752B916FC7B0DL)) >> 58];
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
		public static int CountTrailingBits(this long value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 64);
			return ((value ^ (value + 1)) >> 1).CountBits();
		}
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于指定整数的最小的 <c>2</c> 的幂次。</returns>
		public static long CeilingPowerOf2(this long value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			return value <= 0L ? 1L : value + 1L;
		}
		/// <summary>
		/// 返回指定整数二进制表示的下一字典序排列。
		/// </summary>
		/// <param name="value">要获取下一字典序排列的整数。</param>
		/// <returns>指定整数二进制表示的下一字典序排列。</returns>
		/// <remarks>
		/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
		/// 而且是字典序的下一排列。</para>
		/// <para>例如，<c>102</c> 的二进制表示为 <c>1100110</c>，它的下一字典序排列为
		/// <c>1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
		/// <c>105</c> 的下一字典序排列为 <c>106</c>（二进制为 <c>1101010</c>），
		/// 接下来是 <c>108</c>（二进制为 <c>1101100</c>）和 <c>113</c>（二进制为 <c>1110001</c>）。
		/// </para></remarks>
		public static long NextBitPermutation(this long value)
		{
			long t = value | (value - 1);
			return (t + 1L) | (((~t & -~t) - 1L) >> (value.CountTrailingZeroBits() + 1));
		}

		#endregion // 位运算

	}
}
