using System;
using System.Collections.Generic;

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
		private static readonly int[] MultiplyDeBruijnBitPosition_32 = new int[] {
			  0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
			  31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9 };
		/// <summary>
		/// 用于计算末尾连续零的个数的数组。
		/// </summary>
		private static readonly int[] MultiplyDeBruijnBitPosition_64 = new int[] {
			 0, 1, 2, 56, 3, 32, 57, 46, 29, 4, 20, 33, 7, 58, 11, 47, 
			 62, 30, 18, 5, 16, 21, 34, 23, 53, 8, 59, 36, 25, 12, 48, 39,
			 63, 55, 31, 45, 28, 19, 6, 10, 61, 17, 15, 22, 52, 35, 24, 38, 
			 54, 44, 27, 9, 60, 14, 51, 37, 43, 26, 13, 50, 42, 49, 41, 40 };
		/// <summary>
		/// 用于计算以 2 为底的对数值的数组。
		/// </summary>
		private static readonly int[] LogBase2_32 = new int[] { 
			0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30, 
			8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31 };

		#endregion // 常量定义

		#region Int32 操作

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
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		public static void Times(this int source, Action<int> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (int i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 返回将指定值重复多次的序列。
		/// </summary>
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
		/// <param name="source">要重复的次数。只有大于 <c>0</c> 时才有效。</param>
		/// <param name="value">返回要重复的值的函数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		public static IEnumerable<T> Times<T>(this int source, Func<T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (int i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。只有大于 <c>0</c> 时才有效。</param>
		/// <param name="value">返回要重复的值的函数，参数为已执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		public static IEnumerable<T> Times<T>(this int source, Func<int, T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (int i = 0; i < source; i++)
			{
				yield return value(i);
			}
		}
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
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续零的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续零的值。</param>
		/// <returns>当前值的二进制表示中末尾连续零的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续零的个数。
		/// </summary>
		/// </overloads>
		public static int BinTrailingZeroCount(this int value)
		{
			return MultiplyDeBruijnBitPosition_32[((uint)((value & -value) * 0x077CB531U)) >> 27];
		}
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续一的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续一的值。</param>
		/// <returns>当前值的二进制表示中末尾连续一的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续一的个数。
		/// </summary>
		/// </overloads>
		public static int BinTrailingOneCount(this int value)
		{
			return ((value ^ (value + 1)) >> 1).BinOneCnt();
		}
		/// <summary>
		/// 计算当前值的二进制表示中 1 的个数。
		/// </summary>
		/// <param name="value">要计算的值。</param>
		/// <returns>当前值的二进制表示中 1 的个数。</returns>
		/// <overloads>
		/// <summary>
		/// 计算当前值的二进制表示中 1 的个数。
		/// </summary>
		/// </overloads>
		public static int BinOneCnt(this int value)
		{
			value -= (value >> 1) & 0x55555555;
			value = (value & 0x33333333) + ((value >> 2) & 0x33333333);
			value = (value + (value >> 4)) & 0x0F0F0F0F;
			return (value * 0x01010101) >> 24;
		}
		/// <summary>
		/// 计算当前值以 2 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的值。</param>
		/// <returns>当前值以 2 为底的对数值。</returns>
		/// <overloads>
		/// <summary>
		/// 计算当前值以 2 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// </overloads>
		public static int LogBase2(this int value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return LogBase2_32[(uint)(value * 0x07C4ACDDU) >> 27];
		}

		#endregion // Int32 操作

		#region Int64 操作

		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于<c>0</c>时才有效。</param>
		/// <param name="action">要执行的操作。</param>
		public static void Times(this long source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (long i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。只有大于<c>0</c>时才有效。</param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		public static void Times(this long source, Action<long> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (long i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 返回将指定值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。只有大于<c>0</c>时才有效。</param>
		/// <param name="value">要重复的值。</param>
		/// <returns>将指定值重复多次的序列。</returns>
		public static IEnumerable<T> Times<T>(this long source, T value)
		{
			for (long i = 0; i < source; i++)
			{
				yield return value;
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。只有大于<c>0</c>时才有效。</param>
		/// <param name="value">返回要重复的值的函数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		public static IEnumerable<T> Times<T>(this long source, Func<T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (long i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。只有大于<c>0</c>时才有效。</param>
		/// <param name="value">返回要重复的值的函数，参数为当前执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		public static IEnumerable<T> Times<T>(this long source, Func<long, T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (long i = 0; i < source; i++)
			{
				yield return value(i);
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到指定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值递增（递减）的序列。</returns>
		public static IEnumerable<long> To(this long source, long destination)
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
		/// 从当前值递增（递减）到指定值并执行操作。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <param name="action">要执行的操作，参数为当前的值。</param>
		public static void To(this long source, long destination, Action<long> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续零的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续零的值。</param>
		/// <returns>当前值的二进制表示中末尾连续零的个数。</returns>
		public static int BinTrailingZeroCount(this long value)
		{
			return MultiplyDeBruijnBitPosition_64[((ulong)((value & -value) * 0x26752B916FC7B0DL)) >> 58];
		}
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续一的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续一的值。</param>
		/// <returns>当前值的二进制表示中末尾连续一的个数。</returns>
		public static int BinTrailingOneCount(this long value)
		{
			return ((value ^ (value + 1)) >> 1).BinOneCnt();
		}
		/// <summary>
		/// 计算当前值的二进制表示中 1 的个数。
		/// </summary>
		/// <param name="value">要计算的值。</param>
		/// <returns>当前值的二进制表示中 1 的个数。</returns>
		public static int BinOneCnt(this long value)
		{
			value -= (value >> 1) & 0x5555555555555555L;
			value = (value & 0x3333333333333333L) + ((value >> 2) & 0x3333333333333333L);
			value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FL;
			return (int)((value * 0x0101010101010101L) >> 56);
		}

		#endregion // Int64 操作

		#region UInt32 操作

		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作。</param>
		[CLSCompliant(false)]
		public static void Times(this uint source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		[CLSCompliant(false)]
		public static void Times(this uint source, Action<uint> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (uint i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 返回将指定值重复多次的序列。
		/// </summary>
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
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">返回要重复的值的函数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this uint source, Func<T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (uint i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">返回要重复的值的函数，参数为当前执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this uint source, Func<uint, T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (uint i = 0; i < source; i++)
			{
				yield return value(i);
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到指定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值递增（递减）的序列。</returns>
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
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续零的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续零的值。</param>
		/// <returns>当前值的二进制表示中末尾连续零的个数。</returns>
		[CLSCompliant(false)]
		public static int BinTrailingZeroCount(this uint value)
		{
			return MultiplyDeBruijnBitPosition_32[(uint)((value & -value) * 0x077CB531U) >> 27];
		}
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续一的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续一的值。</param>
		/// <returns>当前值的二进制表示中末尾连续一的个数。</returns>
		[CLSCompliant(false)]
		public static int BinTrailingOneCount(this uint value)
		{
			return ((value ^ (value + 1U)) >> 1).BinOneCnt();
		}
		/// <summary>
		/// 计算当前值的二进制表示中 1 的个数。
		/// </summary>
		/// <param name="value">要计算的值。</param>
		/// <returns>当前值的二进制表示中 1 的个数。</returns>
		[CLSCompliant(false)]
		public static int BinOneCnt(this uint value)
		{
			value -= (value >> 1) & 0x55555555U;
			value = (value & 0x33333333U) + ((value >> 2) & 0x33333333U);
			value = (value + (value >> 4)) & 0x0F0F0F0FU;
			return (int)((value * 0x01010101U) >> 24);
		}
		/// <summary>
		/// 计算当前值以 2 为底的对数值，得到的结果是大于等于当前值的最小对数值。
		/// </summary>
		/// <param name="value">要计算对数的值。</param>
		/// <returns>当前值以 2 为底的对数值。</returns>
		[CLSCompliant(false)]
		public static int LogBase2(this uint value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return LogBase2_32[(value * 0x07C4ACDDU) >> 27];
		}

		#endregion // UInt32 操作

		#region UInt64 操作

		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Action action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (ulong i = 0; i < source; i++)
			{
				action();
			}
		}
		/// <summary>
		/// 将指定操作执行多次。
		/// </summary>
		/// <param name="source">要执行操作的次数。</param>
		/// <param name="action">要执行的操作，参数为当前执行的次数。</param>
		[CLSCompliant(false)]
		public static void Times(this ulong source, Action<ulong> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
			for (ulong i = 0; i < source; i++)
			{
				action(i);
			}
		}
		/// <summary>
		/// 返回将指定值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">要重复的值。</param>
		/// <returns>将指定值重复多次的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this ulong source, T value)
		{
			for (ulong i = 0; i < source; i++)
			{
				yield return value;
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">返回要重复的值的函数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this ulong source, Func<T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (ulong i = 0; i < source; i++)
			{
				yield return value();
			}
		}
		/// <summary>
		/// 返回将指定函数的返回值重复多次的序列。
		/// </summary>
		/// <param name="source">要重复的次数。</param>
		/// <param name="value">返回要重复的值的函数，参数为当前执行的次数。</param>
		/// <returns>将指定函数的返回值重复多次的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<T> Times<T>(this uint source, Func<ulong, T> value)
		{
			ExceptionHelper.CheckArgumentNull(value, "value");
			for (ulong i = 0; i < source; i++)
			{
				yield return value(i);
			}
		}
		/// <summary>
		/// 返回从当前值递增（递减）到指定值的序列。
		/// </summary>
		/// <param name="source">要执行操作的起始值。</param>
		/// <param name="destination">要执行操作的目标值。</param>
		/// <returns>数值递增（递减）的序列。</returns>
		[CLSCompliant(false)]
		public static IEnumerable<ulong> To(this ulong source, ulong destination)
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
		public static void To(this ulong source, ulong destination, Action<ulong> action)
		{
			ExceptionHelper.CheckArgumentNull(action, "action");
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
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续零的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续零的值。</param>
		/// <returns>当前值的二进制表示中末尾连续零的个数。</returns>
		[CLSCompliant(false)]
		public static int BinTrailingZeroCount(this ulong value)
		{
			return MultiplyDeBruijnBitPosition_64[((ulong)((value & (~value + 1UL)) * 0x26752B916FC7B0DUL)) >> 58];
		}
		/// <summary>
		/// 计算当前值的二进制表示中末尾连续一的个数。
		/// </summary>
		/// <param name="value">要计算二进制表示中末尾连续一的值。</param>
		/// <returns>当前值的二进制表示中末尾连续一的个数。</returns>
		[CLSCompliant(false)]
		public static int BinTrailingOneCount(this ulong value)
		{
			return ((value ^ (value + 1)) >> 1).BinOneCnt();
		}
		/// <summary>
		/// 计算当前值的二进制表示中 1 的个数。
		/// </summary>
		/// <param name="value">要计算的值。</param>
		/// <returns>当前值的二进制表示中 1 的个数。</returns>
		[CLSCompliant(false)]
		public static int BinOneCnt(this ulong value)
		{
			value -= (value >> 1) & 0x5555555555555555UL;
			value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
			value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
			return (int)((value * 0x0101010101010101UL) >> 56);
		}

		#endregion // UInt64 操作

	}
}
