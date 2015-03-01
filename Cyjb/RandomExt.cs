using System;
using System.Diagnostics.Contracts;

namespace Cyjb
{
	/// <summary>
	/// 表示一个全局的线程安全伪随机数生成器，并提供关于随机数的扩展方法。
	/// </summary>
	public static class RandomExt
	{
		/// <summary>
		/// 全局伪随机数生成器。
		/// </summary>
		private static readonly Random globalRandom = new Random();
		/// <summary>
		/// 线程相关的伪随机数生成器。
		/// </summary>
		[ThreadStatic]
		private static Random random;
		/// <summary>
		/// 获取线程相关的伪随机数生成器。
		/// </summary>
		/// <value>线程相关的伪随机数生成器。</value>
		private static Random Random
		{
			get { return random ?? (random = new Random(globalRandom.Next())); }
		}

		#region Int32

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="Int32.MaxValue"/> 的 <c>32</c> 位有符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static int Next()
		{
			Contract.Ensures(Contract.Result<int>() >= 0);
			return Random.Next();
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>32</c> 位有符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> 小于零。</exception>
		public static int Next(int maxValue)
		{
			if (maxValue < 0)
			{
				throw CommonExceptions.ArgumentNegative("maxValue", maxValue);
			}
			Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= maxValue);
			return Random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>32</c> 位有符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="ArgumentException"><paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ReversedArgument("minValue", "maxValue");
			}
			Contract.Ensures(Contract.Result<int>() >= minValue && Contract.Result<int>() <= maxValue);
			return Random.Next(minValue, maxValue);
		}

		#endregion // Int32

		#region UInt32

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="UInt32.MaxValue"/> 的 <c>32</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static uint NextUInt32()
		{
			return (uint)((Random.Next() << 16) | (Random.Next() & 0xFFFF));
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>32</c> 位无符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		[CLSCompliant(false)]
		public static uint NextUInt32(uint maxValue)
		{
			Contract.Ensures(Contract.Result<uint>() <= maxValue);
			return (uint)(Random.NextDouble() * maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>32</c> 位无符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="ArgumentException"><paramref name="minValue"/> 大于 
		/// <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static uint NextUInt32(uint minValue, uint maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ReversedArgument("minValue", "maxValue");
			}
			Contract.Ensures(Contract.Result<uint>() >= minValue && Contract.Result<uint>() <= maxValue);
			return minValue + NextUInt32(maxValue - minValue);
		}

		#endregion // UInt32

		#region Int64

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="Int64.MaxValue"/> 的 <c>64</c> 位有符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static long NextInt64()
		{
			Contract.Ensures(Contract.Result<long>() >= 0L);
			long value = Random.Next();
			value <<= 31;
			value |= (uint)Random.Next();
			value <<= 1;
			value |= (uint)Random.Next() & 1;
			return value;
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>64</c> 位有符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> 小于零。</exception>
		public static long NextInt64(long maxValue)
		{
			if (maxValue < 0)
			{
				throw CommonExceptions.ArgumentMustBePositive("maxValue", maxValue);
			}
			Contract.Ensures(Contract.Result<long>() >= 0L && Contract.Result<long>() <= maxValue);
			return (long)(Random.NextDouble() * maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>64</c> 位有符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="ArgumentException"><paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static long NextInt64(long minValue, long maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ReversedArgument("minValue", "maxValue");
			}
			Contract.Ensures(Contract.Result<long>() >= minValue && Contract.Result<long>() <= maxValue);
			long num = unchecked(maxValue - minValue);
			// num <= long.MaxValue
			if (num >= 0)
			{
				return minValue + NextInt64(num);
			}
			// minValue < 0 && maxValue >= 0
			ulong uNum = (ulong)num;
			ulong value = NextUInt64(uNum);
			if (value > 0x7FFFFFFFFFFFFFFFUL)
			{
				return (minValue & 0x7FFFFFFFFFFFFFFFL) + (long)(value & 0x7FFFFFFFFFFFFFFFUL);
			}
			return minValue + (long)value;
		}

		#endregion // Int64

		#region UInt64

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="UInt32.MaxValue"/> 的 <c>64</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static ulong NextUInt64()
		{
			ulong value = (ulong)Random.Next();
			value <<= 31;
			value |= (uint)Random.Next();
			value <<= 31;
			value |= ((uint)Random.Next() & 3);
			return value;
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>64</c> 位无符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。</returns>
		[CLSCompliant(false)]
		public static ulong NextUInt64(ulong maxValue)
		{
			Contract.Ensures(Contract.Result<ulong>() <= maxValue);
			return (ulong)(Random.NextDouble() * maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>64</c> 位无符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="ArgumentException"><paramref name="minValue"/> 大于 
		/// <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static ulong NextUInt64(ulong minValue, ulong maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ReversedArgument("minValue", "maxValue");
			}
			Contract.Ensures(Contract.Result<ulong>() >= minValue && Contract.Result<ulong>() <= maxValue);
			return minValue + NextUInt64(maxValue - minValue);
		}

		#endregion // UInt64

		/// <summary>
		/// 返回随机布尔值。
		/// </summary>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean()
		{
			return (Random.Next() & 1) == 1;
		}
		/// <summary>
		/// 用随机数填充指定字节数组的元素。
		/// </summary>
		/// <param name="buffer">包含随机数的字节数组。</param>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> 为 <c>null</c>。</exception>
		public static void NextBytes(byte[] buffer)
		{
			CommonExceptions.CheckArgumentNull(buffer, "buffer");
			Contract.EndContractBlock();
			Random.NextBytes(buffer);
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的单精度浮点数。</returns>
		public static float NextSingle()
		{
			Contract.Ensures(Contract.Result<float>() >= 0F && Contract.Result<float>() < 1F);
			return (float)Random.NextDouble();
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的双精度浮点数。</returns>
		public static double NextDouble()
		{
			Contract.Ensures(Contract.Result<double>() >= 0.0 && Contract.Result<double>() < 1.0);
			return Random.NextDouble();
		}

		#region Choose

		/// <summary>
		/// 从指定的两个值中，随机挑选一个返回。
		/// </summary>
		/// <typeparam name="T">要随机挑选的值的类型。</typeparam>
		/// <param name="item1">第一个可能的值。</param>
		/// <param name="item2">第二个可能的值。</param>
		/// <returns>随机挑选的值。</returns>
		/// <overloads>
		/// <summary>
		/// 从指定的多个值中，随机挑选一个返回。
		/// </summary>
		/// </overloads>
		public static T Choose<T>(T item1, T item2)
		{
			return NextBoolean() ? item1 : item2;
		}
		/// <summary>
		/// 从指定的三个值中，随机挑选一个返回。
		/// </summary>
		/// <typeparam name="T">要随机挑选的值的类型。</typeparam>
		/// <param name="item1">第一个可能的值。</param>
		/// <param name="item2">第二个可能的值。</param>
		/// <param name="item3">第三个可能的值。</param>
		/// <returns>随机挑选的值。</returns>
		public static T Choose<T>(T item1, T item2, T item3)
		{
			int value = Random.Next(3);
			if (value == 0)
			{
				return item1;
			}
			if (value == 1)
			{
				return item2;
			}
			return item3;
		}
		/// <summary>
		/// 从指定的多个值中，随机挑选一个返回。
		/// </summary>
		/// <typeparam name="T">要随机挑选的值的类型。</typeparam>
		/// <param name="items">要随机挑选的值。</param>
		/// <returns>随机挑选的值。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="items"/> 为 <c>null</c>。</exception>
		/// <exception cref="ArgumentException"><paramref name="items"/> 为空数组。</exception>
		public static T Choose<T>(params T[] items)
		{
			CommonExceptions.CheckArgumentNull(items, "items");
			if (items.Length == 0)
			{
				throw CommonExceptions.CollectionEmpty("items");
			}
			Contract.EndContractBlock();
			if (items.Length == 1)
			{
				return items[0];
			}
			int index = Random.Next(items.Length);
			return items[index];
		}

		#endregion // Choose

	}
}
