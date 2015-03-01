using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	public static partial class IntegerExt
	{

		#region 常量定义

		/// <summary>
		/// 用于计算末尾连续零的个数的数组。
		/// </summary>
		private static readonly int[] multiplyDeBruijnBitPosition32 =
		{
			0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
			31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
		};

		#endregion // 常量定义

		#region Times

		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 <c>0</c> 时才有效。</param>
		/// <param name="action">要执行的操作。</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		/// <overloads>
		/// <summary>
		/// 将指定操作执行多次，或者得到指定值重复多次的序列。
		/// </summary>
		/// </overloads>
		public static void Times(this int source, Action action)
		{
			CommonExceptions.CheckArgumentNull(action, "action");
			Contract.EndContractBlock();
			for (int i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于 <c>0</c> 时才有效。</param>
		/// <param name="action">要执行的操作，参数为已执行的次数。</param>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		public static void Times(this int source, Action<int> action)
		{
			CommonExceptions.CheckArgumentNull(action, "action");
			Contract.EndContractBlock();
			for (int i = 0; i < source; i++)
			{
				action(i);
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
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
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
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static IEnumerable<T> Times<T>(this int source, Func<T> value)
		{
			CommonExceptions.CheckArgumentNull(value, "value");
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
			for (int i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <typeparam name="T">要重复的值的类型。</typeparam>
		/// <param name="source">要重复的次数。只有大于 <c>0</c> 时才有效。</param>
		/// <param name="value">返回要重复的值的函数，参数为已执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <c>null</c>。</exception>
		public static IEnumerable<T> Times<T>(this int source, Func<int, T> value)
		{
			CommonExceptions.CheckArgumentNull(value, "value");
			Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
			for (int i = 0; i < source; i++)
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
		/// <overloads>
		/// <summary>
		/// 返回从当前值递增（递减）到指定值的序列。
		/// </summary>
		/// </overloads>
		public static IEnumerable<int> To(this int source, int destination)
		{
			Contract.Ensures(Contract.Result<IEnumerable<int>>() != null);
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
		/// <exception cref="ArgumentNullException"><paramref name="action"/> 为 <c>null</c>。</exception>
		public static void To(this int source, int destination, Action<int> action)
		{
			CommonExceptions.CheckArgumentNull(action, "action");
			Contract.EndContractBlock();
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
		/// 判断指定整数是否是 <c>2</c> 的幂。
		/// </summary>
		/// <param name="value">要判断是否是 <c>2</c> 的幂的整数。</param>
		/// <returns>如果指定整数是 <c>2</c> 的幂，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		/// <remarks>对于负数和 <c>0</c>，结果为 <c>false</c>。</remarks>
		/// <overloads>
		/// <summary>
		/// 判断指定整数是否是 2 的幂。
		/// </summary>
		/// </overloads>
		public static bool IsPowerOf2(this int value)
		{
			return value > 0 && (value & (value - 1)) == 0;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中 <c>1</c> 的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定整数的二进制表示中 <c>1</c> 的个数。
		/// </summary>
		/// </overloads>
		public static int CountBits(this int value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			value -= (value >> 1) & 0x55555555;
			value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
			value = (value + (value >> 4)) & 0x0F0F0F0F;
			value = (value * 0x01010101) >> 24;
			Contract.Assume(value >= 0 && value <= 32);
			return value;
		}
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// <param name="value">要计算偶校验位的整数。</param>
		/// <returns>如果指定整数的二进制表示中包含奇数个 <c>1</c>，则为 <c>1</c>；
		/// 否则为 <c>0</c>。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定整数的偶校验位。
		/// </summary>
		/// </overloads>
		public static int Parity(this int value)
		{
			Contract.Ensures(Contract.Result<int>() == 0 || Contract.Result<int>() == 1);
			value ^= value >> 1;
			value ^= value >> 2;
			value = (value & 0x11111111) * 0x11111111;
			return (value >> 28) & 1;
		}
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// <param name="value">要二进制位逆序的整数。</param>
		/// <returns>指定整数二进制位逆序的结果。</returns>
		/// <overloads>
		/// <summary>
		/// 将指定整数的二进制位逆序。
		/// </summary>
		/// </overloads>
		public static int ReverseBits(this int value)
		{
			value = ((value >> 1) & 0x55555555) | ((value & 0x55555555) << 1);
			value = ((value >> 2) & 0x33333333) | ((value & 0x33333333) << 2);
			value = ((value >> 4) & 0x0F0F0F0F) | ((value & 0x0F0F0F0F) << 4);
			value = ((value >> 8) & 0x00FF00FF) | ((value & 0x00FF00FF) << 8);
			value = (value >> 16) | (value << 16);
			return value;
		}
		/// <summary>
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>2</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 小于等于 <c>0</c>，则结果未知。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定正整数以 <c>2</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// </overloads>
		public static int LogBase2(this int value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			return LogBase2((uint)value);
		}
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的整数。</param>
		/// <returns>指定正整数以 <c>10</c> 为底的对数值，
		/// 如果 <paramref name="value"/> 小于等于 <c>10</c>，则结果未知。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定正整数以 <c>10</c> 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// </overloads>
		public static int LogBase10(this int value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 9);
			if (value >= 1000000000) { return 9; }
			if (value >= 100000000) { return 8; }
			if (value >= 10000000) { return 7; }
			if (value >= 1000000) { return 6; }
			if (value >= 100000) { return 5; }
			if (value >= 10000) { return 4; }
			if (value >= 1000) { return 3; }
			if (value >= 100) { return 2; }
			return (value >= 10) ? 1 : 0;
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>0</c> 的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>0</c> 的个数。
		/// </summary>
		/// </overloads>
		public static int CountTrailingZeroBits(this int value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 31);
			return multiplyDeBruijnBitPosition32[((uint)((value & -value) * 0x077CB531U)) >> 27];
		}
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// <param name="value">要计算的整数。</param>
		/// <returns>指定整数的二进制表示中末尾连续 <c>1</c> 的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算指定整数的二进制表示中末尾连续 <c>1</c> 的个数。
		/// </summary>
		/// </overloads>
		public static int CountTrailingBits(this int value)
		{
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= 32);
			return ((value ^ (value + 1)) >> 1).CountBits();
		}
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// <param name="value"><c>2</c> 的幂次要大于等于的值。</param>
		/// <returns>大于或等于指定整数的最小的 <c>2</c> 的幂次。</returns>
		/// <overloads>
		/// <summary>
		/// 返回大于或等于指定整数的最小的 <c>2</c> 的幂次。
		/// </summary>
		/// </overloads>
		public static int CeilingPowerOf2(this int value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return value <= 0 ? 1 : value + 1;
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
		/// <overloads>
		/// <summary>
		/// 返回指定整数二进制表示的下一字典序排列。
		/// </summary>
		/// </overloads>
		public static int NextBitPermutation(this int value)
		{
			int t = value | (value - 1);
			return (t + 1) | (((~t & -~t) - 1) >> (value.CountTrailingZeroBits() + 1));
		}

		#endregion // 位运算

	}
}
