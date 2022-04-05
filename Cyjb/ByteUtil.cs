namespace Cyjb
{
	/// <summary>
	/// 提供对 <see cref="byte"/> 的扩展方法。
	/// </summary>
	/// <remarks>位运算的算法来自于 
	/// <see href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</see>。</remarks>
	/// <seealso href="http://graphics.stanford.edu/~seander/bithacks.html">Bit Twiddling Hacks</seealso>
	public static class ByteUtil
	{

		#region 位运算

		/// <summary>
		/// 判断指定 <see cref="byte"/> 是否是 <c>2</c> 的幂。
		/// </summary>
		/// <param name="value">要判断是否是 <c>2</c> 的幂的 <see cref="byte"/>。</param>
		/// <returns>如果 <paramref name="value"/> 是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>对于 <c>0</c>，结果为<c>false</c>。</remarks>
		public static bool IsPowerOf2(this byte value)
		{
			return value != 0 & (value & (value - 1)) == 0;
		}

		/// <summary>
		/// 计算指定 <see cref="byte"/> 的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 的二进制表示中 <c>1</c> 的个数。</returns>
		public static int CountBits(this byte value)
		{
			int iValue = value;
			iValue -= (iValue >> 1) & 0x55;
			iValue = (iValue & 0x33) + ((iValue >> 2) & 0x33);
			return (iValue + (iValue >> 4)) & 0xF;
		}

		/// <summary>
		/// 计算指定 <see cref="byte"/> 的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的 <see cref="byte"/>。</param>
		/// <returns>如果 <paramref name="value"/> 的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；否则为 <c>0</c>。</returns>
		public static int Parity(this byte value)
		{
			int iValue = value;
			iValue = (iValue ^ (iValue >> 4)) & 0xF;
			return (0x6996 >> iValue) & 1;
		}

		/// <summary>
		/// 将指定 <see cref="byte"/> 的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 二进制位逆序的结果。</returns>
		public static byte ReverseBits(this byte value)
		{
			uint uValue = value;
			return (byte)(((uValue * 0x0802U & 0x22110U) | (uValue * 0x8020U & 0x88440U)) * 0x10101U >> 16);
		}

		/// <summary>
		/// 用于计算以 2 为底的对数值的数组。
		/// </summary>
		private static readonly int[] logBase2Map = { 0, 5, 1, 6, 4, 3, 2, 7 };

		/// <summary>
		/// 计算指定正 <see cref="byte"/> 以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 以 <c>2</c> 为底的对数值，如果 <paramref name="value"/> 等于 <c>0</c>，则返回 <c>0</c>。</returns>
		public static int LogBase2(this byte value)
		{
			int result = value;
			// 先向下取整到 2^n-1
			result |= result >> 1;
			result |= result >> 2;
			result |= result >> 4;
			return logBase2Map[((result * 0x1D) & 0xFF) >> 5];
		}

		/// <summary>
		/// 计算指定正 <see cref="byte"/> 以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 以 <c>10</c> 为底的对数值。</returns>
		public static int LogBase10(this byte value)
		{
			if (value >= 100) { return 2; }
			else if (value >= 10) { return 1; }
			else { return 0; }
		}

		/// <summary>
		/// 用于计算末尾连续零的个数的数组。
		/// </summary>
		private static readonly int[] trailingZeroBitsMap = { 0, 1, 6, 2, 7, 5, 4, 3 };

		/// <summary>
		/// 计算指定 <see cref="byte"/> 的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
		public static int CountTrailingZeroBits(this byte value)
		{
			if (value == 0) { return 8; }
			return trailingZeroBitsMap[(((value & -value) * 0x1D) & 0xFF) >> 5];
		}

		/// <summary>
		/// 计算指定 <see cref="byte"/> 的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
		public static int CountTrailingBits(this byte value)
		{
			return ((value ^ (value + 1)) >> 1).CountBits();
		}

		/// <summary>
		/// 返回大于或等于指定 <see cref="byte"/> 的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于 <paramref name="value"/> 的最小的 <c>2</c> 的幂次。
		/// 由于 <c>256</c> 会超出 <see cref="byte"/> 的范围，此时会返回 <c>0</c>。</returns>
		public static byte CeilingPowerOf2(this byte value)
		{
			int result = value - 1;
			result |= result >> 1;
			result |= result >> 2;
			result |= result >> 4;
			return result <= 0 ? (byte)1 : (byte)(result + 1);
		}

		/// <summary>
		/// 返回指定 <see cref="byte"/> 二进制表示的下一字典序排列。
		/// </summary>
		/// <param name="value">要获取下一字典序排列的 <see cref="byte"/>。</param>
		/// <returns><paramref name="value"/> 二进制表示的下一字典序排列。</returns>
		/// <remarks>
		/// <para>方法的返回值与 <paramref name="value"/> 的二进制表示包含相同个数的 <c>1</c>，
		/// 而且是字典序的下一排列。</para>
		/// <para>例如，<c>102</c> 的二进制表示为 <c>0b1100110</c>，它的下一字典序排列为
		/// <c>0b1101001</c>，同样包含 <c>4</c> 个 <c>1</c>，而相应的值为 <c>105</c>。
		/// <c>105</c> 的下一字典序排列为 <c>106</c> (<c>0b1101010</c>)，
		/// 接下来是 <c>108</c> (<c>0b1101100</c>)和 <c>113</c> (<c>0b1110001</c>)。
		/// </para></remarks>
		public static byte NextBitPermutation(this byte value)
		{
			int t = value | (value - 1);
			int r = ((~t & -~t) - 1) >> (value.CountTrailingZeroBits() + 1);
			if (t < 255)
			{
				t = r | (t + 1);
			}
			else
			{
				// 结果会超出 byte 有效范围，循环。
				t = r | (r + 1);
			}
			return (byte)t;
		}

		#endregion // 位运算

	}
}

