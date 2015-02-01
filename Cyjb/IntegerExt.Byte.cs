using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 提供对整数的扩展方法。
	/// </summary>
	public static partial class IntegerExt
	{

		#region 位运算

		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中 <c>1</c> 的个数。</returns>
		public static int CountBits(this byte value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 8);
			int iValue = value;
			iValue -= (iValue >> 1) & 0x55;
			iValue = (iValue & 0x33) + ((iValue >> 2) & 0x33);
			return (iValue + (iValue >> 4)) & 0xF;
		}
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的整数。</param>
		/// <returns>如果指定整数的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；
		/// 否则为 <c>0</c>。</returns>
		public static int Parity(this byte value)
		{
			Contract.Ensures(Contract.Result<int>() == 0 || Contract.Result<int>() == 1);
			int iValue = value;
			iValue = (iValue ^ (iValue >> 4)) & 0xF;
			return (0x6996 >> iValue) & 1;
		}
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的整数。</param>
		/// <returns>指定整数二进制位逆序的结果。</returns>
		public static byte ReverseBits(this byte value)
		{
			uint uValue = value;
			return (byte)(((uValue * 0x0802U & 0x22110U) | (uValue * 0x8020U & 0x88440U)) * 0x10101U >> 16);
		}
		/// <summary>
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>2</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>0</c>，则结果未知。</returns>
		public static int LogBase2(this byte value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 7);
			if (value >= 128) { return 7; }
			if (value >= 64) { return 6; }
			if (value >= 32) { return 5; }
			if (value >= 16) { return 4; }
			if (value >= 8) { return 3; }
			if (value >= 4) { return 2; }
			return value >= 2 ? 1 : 0;
		}
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>10</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>10</c>，则结果未知。</returns>
		public static int LogBase10(this byte value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 2);
			if (value >= 100) { return 2; }
			return (value >= 10) ? 1 : 0;
		}
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于指定整数的最小的 <c>2</c> 的幂次。</returns>
		public static byte CeilingPowerOf2(this byte value)
		{
			int iValue = value - 1;
			iValue |= iValue >> 1;
			iValue |= iValue >> 2;
			iValue |= iValue >> 4;
			return iValue <= 0 ? (byte)1 : (byte)(value + 1);
		}

		#endregion // 位运算

	}
}
