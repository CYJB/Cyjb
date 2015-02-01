using System;
using System.Collections.Generic;
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
		/// 用于计算以 2 为底的对数值的数组。
		/// </summary>
		private static readonly int[] logBase2_32 =
		{ 
			0,  9,  1, 10, 13, 21,  2, 29, 11, 14, 16, 18, 22, 25, 3, 30, 
			8, 12, 20, 28, 15, 17, 24,  7, 19, 27, 23,  6, 26,  5, 4, 31 
		};

		#endregion // 常量定义

		#region Times

		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作。</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void Times(this uint source, Action action)
		{
			if (action == null)
			{
				throw CommonExceptions.ArgumentNull("action");
			}
			Contract.EndContractBlock();
			for (uint i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void Times(this uint source, Action<uint> action)
		{
			if (action == null)
			{
				throw CommonExceptions.ArgumentNull("action");
			}
			Contract.EndContractBlock();
			for (uint i = 0; i < source; i++)
			{
				action(i);
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
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
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
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this uint source, Func<T> value)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
			for (uint i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <typeparam name="T">要重复的值的类型。</typeparam>
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">返回要重复的值的函数，参数为当前执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this uint source, Func<uint, T> value)
		{
			if (value == null)
			{
				throw CommonExceptions.ArgumentNull("value");
			}
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
			for (uint i = 0; i < source; i++)
			{
				yield return value(i);
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
		[CLSCompliant(false)]
		public static IEnumerable<uint> To(this uint source, uint destination)
		{
			Contract.Ensures(Contract.Result<IEnumerable<uint>>() != null);
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
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		[CLSCompliant(false)]
		public static void To(this uint source, uint destination, Action<uint> action)
		{
			if (action == null)
			{
				throw CommonExceptions.ArgumentNull("action");
			}
			Contract.EndContractBlock();
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
		/// 判断指定整数是否是 <c>2</c> 的幂。
		/// </summary>
		/// <param name="value">要判断是否是 <c>2</c> 的幂的整数。</param>
		/// <returns>如果指定整数是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>对于 <c>0</c>，结果为 <c>false</c>。</remarks>
		[CLSCompliant(false)]
		public static bool IsPowerOf2(this uint value)
		{
			return value != 0U && (value & (value - 1U)) == 0U;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中 <c>1</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountBits(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			value -= (value >> 1) & 0x55555555U;
			value = (value & 0x33333333U) + ((value >> 2) & 0x33333333U);
			value = (value + (value >> 4)) & 0x0F0F0F0FU;
			value = (value * 0x01010101U) >> 24;
			Contract.Assume(value <= 32);
			return (int)value;
		}
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的整数。</param>
		/// <returns>如果指定整数的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；
		/// 否则为 <c>0</c>。</returns>
		[CLSCompliant(false)]
		public static int Parity(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 1);
			value ^= value >> 16;
			value ^= value >> 8;
			value ^= value >> 4;
			value &= 0xFU;
			return (0x6996 >> (int)value) & 1;
		}
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的整数。</param>
		/// <returns>指定整数二进制位逆序的结果。</returns>
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
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>2</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>0</c>，则结果未知。</returns>
		[CLSCompliant(false)]
		public static int LogBase2(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return logBase2_32[(value * 0x07C4ACDDU) >> 27];
		}
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>10</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 等于 <c>10</c>，则结果未知。</returns>
		[CLSCompliant(false)]
		public static int LogBase10(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 9);
			if (value >= 1000000000U) { return 9; }
			if (value >= 100000000U) { return 8; }
			if (value >= 10000000U) { return 7; }
			if (value >= 1000000U) { return 6; }
			if (value >= 100000U) { return 5; }
			if (value >= 10000U) { return 4; }
			if (value >= 1000U) { return 3; }
			if (value >= 100U) { return 2; }
			return (value >= 10U) ? 1 : 0;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountTrailingZeroBits(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			return CountTrailingZeroBits((int)value);
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
		[CLSCompliant(false)]
		public static int CountTrailingBits(this uint value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			return ((value ^ (value + 1U)) >> 1).CountBits();
		}
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于指定整数的最小的 <c>2</c> 的幂次。</returns>
		[CLSCompliant(false)]
		public static uint CeilingPowerOf2(this uint value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value == 0U ? 1U : value;
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
		[CLSCompliant(false)]
		public static uint NextBitPermutation(this uint value)
		{
			uint t = value | (value - 1);
			return (t + 1U) | (((~t & (uint)(-(int)~t)) - 1U) >> (value.CountTrailingZeroBits() + 1));
		}

		#endregion // 位运算

	}
}
