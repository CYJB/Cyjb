using System;

namespace Cyjb
{
	/// <summary>
	/// 表示一个全局的伪随机数生成器，并提供关于随机数的扩展方法。
	/// </summary>
	public static class RandomExt
	{
		/// <summary>
		/// 全局伪随机数生成器。
		/// </summary>
		private static Random random = new Random();
		/// <summary>
		/// 更新随机数的种子值为与时间相关的值。
		/// </summary>
		/// <overloads>
		/// <summary>
		/// 更新随机数的种子值。
		/// </summary>
		/// </overloads>
		public static void UpdateSeed()
		{
			random = new Random();
		}
		/// <summary>
		/// 更新随机数的种子值为与指定的值。
		/// </summary>
		/// <param name="seed">用来计算伪随机数序列起始值的数字。
		/// 如果指定的是负数，则使用其绝对值。</param>
		public static void UpdateSeed(int seed)
		{
			random = new Random(seed);
		}

		#region SByte

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.SByte.MaxValue"/> 
		/// 的 <c>16</c> 位带符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static sbyte NextSByte()
		{
			return (sbyte)random.Next(sbyte.MaxValue);
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>8</c> 位带符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		[CLSCompliant(false)]
		public static sbyte NextSByte(sbyte maxValue)
		{
			return (sbyte)random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>16</c> 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static sbyte NextSByte(sbyte minValue, sbyte maxValue)
		{
			return (sbyte)random.Next(minValue, maxValue);
		}

		#endregion // SByte

		#region Byte

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.Byte.MaxValue"/> 
		/// 的 <c>8</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static byte NextByte()
		{
			return (byte)random.Next(byte.MaxValue);
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>8</c> 位无符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		public static byte NextByte(byte maxValue)
		{
			return (byte)random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>8</c> 位无符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static byte NextByte(byte minValue, byte maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ArgumentMinMaxValue("minValue", "maxValue");
			}
			return (byte)random.Next(minValue, maxValue);
		}

		#endregion // Byte

		#region Int16

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.Int16.MaxValue"/> 
		/// 的 <c>16</c> 位带符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static short NextInt16()
		{
			return (short)random.Next(short.MaxValue);
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>16</c> 位带符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		public static short NextInt16(short maxValue)
		{
			return (short)random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>16</c> 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static short NextInt16(short minValue, short maxValue)
		{
			return (short)random.Next(minValue, maxValue);
		}

		#endregion // Int16

		#region UInt16

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.UInt16.MaxValue"/> 
		/// 的 <c>16</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static ushort NextUInt16()
		{
			return (ushort)random.Next(ushort.MaxValue);
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>16</c> 位无符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		[CLSCompliant(false)]
		public static ushort NextUInt16(ushort maxValue)
		{
			return (ushort)random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>16</c> 位无符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static ushort NextUInt16(ushort minValue, ushort maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ArgumentMinMaxValue("minValue", "maxValue");
			}
			return (ushort)random.Next(minValue, maxValue);
		}

		#endregion // UInt16

		#region Int32

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.Int32.MaxValue"/> 
		/// 的 <c>32</c> 位带符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static int Next()
		{
			return random.Next();
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>32</c> 位带符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		public static int Next(int maxValue)
		{
			return random.Next(maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>32</c> 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static int Next(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}

		#endregion // Int32

		#region UInt32

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.UInt32.MaxValue"/> 
		/// 的 <c>32</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static uint NextUInt32()
		{
			return (uint)((random.Next() << 1) | (random.Next() & 1));
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
			return (uint)(random.NextDouble() * maxValue);
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
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static uint NextUInt32(uint minValue, uint maxValue)
		{
			if (minValue > maxValue)
			{
				throw CommonExceptions.ArgumentMinMaxValue("minValue", "maxValue");
			}
			return minValue + NextUInt32(maxValue - minValue);
		}

		#endregion // UInt32

		#region Int64

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.Int64.MaxValue"/> 
		/// 的 <c>64</c> 位带符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		public static long NextInt64()
		{
			long value = random.Next();
			value <<= 31;
			value |= (uint)random.Next();
			value <<= 1;
			value |= (uint)random.Next() & 1;
			return value;
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>64</c> 位带符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		public static long NextInt64(long maxValue)
		{
			if (maxValue < 0)
			{
				throw CommonExceptions.ArgumentMustBePositive("maxValue", maxValue);
			}
			return (long)(random.NextDouble() * maxValue);
		}
		/// <summary>
		/// 返回一个指定范围内的随机数。
		/// </summary>
		/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
		/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。
		/// <paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
		/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 
		/// 的 <c>64</c> 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 
		/// 但不包括 <paramref name="maxValue"/>。
		/// 如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 
		/// <paramref name="minValue"/>。</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		public static long NextInt64(long minValue, long maxValue)
		{
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
			else
			{
				return minValue + (long)value;
			}
		}

		#endregion // Int64

		#region UInt64

		/// <summary>
		/// 返回非负随机数。
		/// </summary>
		/// <returns>大于等于零且小于 <see cref="System.UInt32.MaxValue"/> 
		/// 的 <c>64</c> 位无符号整数。</returns>
		/// <overloads>
		/// <summary>
		/// 返回随机数。
		/// </summary>
		/// </overloads>
		[CLSCompliant(false)]
		public static ulong NextUInt64()
		{
			ulong value = (ulong)random.Next();
			value <<= 31;
			value |= (ulong)(uint)random.Next();
			value <<= 31;
			value |= (ulong)((uint)random.Next() & 3);
			return value;
		}
		/// <summary>
		/// 返回一个小于所指定最大值的非负随机数。
		/// </summary>
		/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。
		/// <paramref name="maxValue"/> 必须大于或等于零。</param>
		/// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 <c>64</c> 位无符号整数，
		/// 即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。
		/// 不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="maxValue"/> 小于零。</exception>
		[CLSCompliant(false)]
		public static ulong NextUInt64(ulong maxValue)
		{
			return (ulong)(random.NextDouble() * maxValue);
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
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="minValue"/> 大于 <paramref name="maxValue"/>。</exception>
		[CLSCompliant(false)]
		public static ulong NextUInt64(ulong minValue, ulong maxValue)
		{
			return minValue + NextUInt64(maxValue - minValue);
		}

		#endregion // UInt64

		/// <summary>
		/// 返回随机布尔值。
		/// </summary>
		/// <returns>随机布尔值。</returns>
		public static bool NextBoolean()
		{
			return (random.Next() & 1) == 1;
		}
		/// <summary>
		/// 用随机数填充指定字节数组的元素。
		/// </summary>
		/// <param name="buffer">包含随机数的字节数组。</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="buffer"/> 为 <c>null</c>。</exception>
		public static void NextBytes(byte[] buffer)
		{
			random.NextBytes(buffer);
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的单精度浮点数。</returns>
		public static float NextSingle()
		{
			return (float)random.NextDouble();
		}
		/// <summary>
		/// 返回一个介于 <c>0.0</c> 和 <c>1.0</c> 之间的随机数。
		/// </summary>
		/// <returns>大于等于 <c>0.0</c> 并且小于 <c>1.0</c> 的双精度浮点数。</returns>
		public static double NextDouble()
		{
			return random.NextDouble();
		}
	}
}
