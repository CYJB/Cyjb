using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cyjb;

/// <summary>
/// MersenneTwister 伪随机数发生器，同一种子总是会生成相同的伪随机序列。
/// </summary>
public sealed class MersenneTwister : Random
{
	// MT19937-64 的参数列表
	/// <summary>
	/// 长度（bit）。
	/// </summary>
	private const int w = 64;
	/// <summary>
	/// 递归长度。
	/// </summary>
	private const ulong n = 312UL;
	/// <summary>
	/// 周期参数。
	/// </summary>
	private const ulong m = 156UL;
	/// <summary>
	/// 低位掩码要提取的位数。
	/// </summary>
	private const int r = 31;
	/// <summary>
	/// 旋转矩阵的参数。
	/// </summary>
	private const ulong a = 0xB5026F5AA96619E9UL;
	/// <summary>
	/// 初始化梅森旋转链的参数。
	/// </summary>
	private const ulong f = 6364136223846793005UL;
	/// <summary>
	/// 额外梅森旋转所需的掩码。
	/// </summary>
	private const int u = 29;
	private const ulong d = 0x5555555555555555UL;
	/// <summary>
	/// TGFSR 的掩码和位移量。
	/// </summary>
	private const int s = 17;
	private const ulong b = 0x71D67FFFEDA60000UL;
	private const int t = 37;
	private const ulong c = 0xFFF7EEE000000000UL;
	/// <summary>
	/// 额外梅森旋转所需的位移。
	/// </summary>
	private const int l = 43;
	/// <summary>
	/// 低位掩码。
	/// </summary>
	private const ulong lowerMask = unchecked((1 << r) - 1);
	/// <summary>
	/// 高位掩码。
	/// </summary>
	private const ulong upperMask = ~lowerMask;

	private readonly ulong[] MT = new ulong[n];
	private ulong index;

	/// <summary>
	/// 使用默认种子值初始化 <see cref="MersenneTwister"/> 类的新实例。
	/// </summary>
	public MersenneTwister() : this(Shared.Next())
	{ }

	/// <summary>
	/// 使用指定的种子值初始化 <see cref="MersenneTwister"/> 类的新实例。
	/// </summary>
	/// <param name="seed">用来计算伪随机序列起始值的数字。</param>
	public MersenneTwister(int seed)
	{
		index = n;
		MT[0] = (uint)seed;
		for (ulong i = 1; i < n; ++i)
		{
			MT[i] = (f * (MT[i - 1] ^ (MT[i - 1] >> (w - 2))) + i);
		}
	}

	#region Next

	/// <summary>
	/// 返回一个非负随机整数。
	/// </summary>
	/// <returns>大于或等于 <c>0</c> 且小于 <see cref="int.MaxValue"/> 的 32 位有符号整数。</returns>
	public override int Next()
	{
		ulong value;
		do
		{
			value = NextUInt64() >> 33;
		} while (value == int.MaxValue);
		return (int)value;
	}

	/// <summary>
	/// 返回一个小于所指定最大值的非负随机整数。
	/// </summary>
	/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。</param>
	/// <returns>大于或等于零且小于 <see cref="int.MaxValue"/> 的 32 位有符号整数。</returns>
	public override int Next(int maxValue)
	{
		if (maxValue < 0)
		{
			throw CommonExceptions.ArgumentNegative(maxValue);
		}
		if (maxValue > 1)
		{
			int log2 = Log2Ceiling((uint)maxValue);
			ulong result;
			do
			{
				result = NextUInt64() >> (64 - log2);
			}
			while (result >= (uint)maxValue);
			return (int)result;
		}
		return 0;
	}

	/// <summary>
	/// 返回在指定范围内的任意整数。
	/// </summary>
	/// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
	/// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。 maxValue 必须大于或等于 minValue。</param>
	/// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 32 位带符号整数。</returns>
	public override int Next(int minValue, int maxValue)
	{
		if (minValue > maxValue)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(minValue), nameof(maxValue));
		}
		ulong range = (ulong)maxValue - (ulong)minValue;
		if (range > 1)
		{
			int log2 = Log2Ceiling(range);
			ulong result;
			do
			{
				result = NextUInt64() >> (64 - log2);
			}
			while (result >= range);
			return (int)result + minValue;
		}
		return minValue;
	}

	#endregion // Next

	#region NextBytes

	/// <summary>
	/// 用随机数填充指定字节数组的元素。
	/// </summary>
	/// <param name="buffer">要用随机数填充的数组。</param>
	/// <exception cref="ArgumentNullException"><paramref name="buffer"/> 为 <c>null</c>。</exception>
	public override void NextBytes(byte[] buffer)
	{
		ArgumentNullException.ThrowIfNull(buffer);
		NextBytes((Span<byte>)buffer);
	}

	/// <summary>
	/// 用随机数填充指定字节范围的元素。
	/// </summary>
	/// <param name="buffer">要用随机数填充的连续内存。</param>
	public unsafe override void NextBytes(Span<byte> buffer)
	{
		while (buffer.Length >= 8)
		{
			Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer), NextUInt64());
			buffer = buffer[8..];
		}
		if (!buffer.IsEmpty)
		{
			ulong value = NextUInt64();
			byte* ptr = (byte*)(&value);
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = ptr[i];
			}
		}
	}

	#endregion // NextBytes

	#region NextInt64

	/// <summary>
	/// 返回一个非负随机整数。
	/// </summary>
	/// <returns>大于或等于 <c>0</c> 且小于 <see cref="long.MaxValue"/> 的 64 位带符号整数。</returns>
	public override long NextInt64()
	{
		ulong value;
		do
		{
			value = NextUInt64() >> 1;
		} while (value == long.MaxValue);
		return (long)value;
	}

	/// <summary>
	/// 返回一个小于所指定最大值的非负随机整数。
	/// </summary>
	/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。</param>
	/// <returns>大于或等于零且小于 <see cref="long.MaxValue"/> 的 64 位有符号整数。</returns>
	public override long NextInt64(long maxValue)
	{
		if (maxValue < 0)
		{
			throw CommonExceptions.ArgumentNegative(maxValue);
		}
		if (maxValue > 1)
		{
			int log2 = Log2Ceiling((ulong)maxValue);
			ulong result;
			do
			{
				result = NextUInt64() >> (64 - log2);
			}
			while (result >= (ulong)maxValue);
			return (long)result;
		}
		return 0L;
	}

	/// <summary>
	/// 返回在指定范围内的随机整数。
	/// </summary>
	/// <param name="minValue">要生成的随机数的下界（随机数可取该下界值）。</param>
	/// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。</param>
	/// <returns>大于或等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 64 位有符号整数。</returns>
	public override long NextInt64(long minValue, long maxValue)
	{
		if (minValue > maxValue)
		{
			throw CommonExceptions.ArgumentMinMaxValue(nameof(minValue), nameof(maxValue));
		}
		ulong range = (ulong)(maxValue - minValue);
		if (range > 1)
		{
			int log2 = Log2Ceiling(range);
			ulong result;
			do
			{
				result = NextUInt64() >> (64 - log2);
			}
			while (result >= range);
			return (long)result + minValue;
		}
		return minValue;
	}

	#endregion // NextInt64

	/// <summary>
	/// 返回一个大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的随机浮点数。
	/// </summary>
	/// <returns>大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的双精度浮点数。</returns>
	public override double NextDouble()
	{
		// double 精度为 52 位
		return (NextUInt64() >> 11) * 1.1102230246251565E-16;
	}

	/// <summary>
	/// 返回一个大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的随机浮点数。
	/// </summary>
	/// <returns>大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的单精度浮点数。</returns>
	public override float NextSingle()
	{
		// float 精度为 23 位
		return (NextUInt64() >> 40) * 5.96046448E-08F;
	}

	/// <summary>
	/// 返回一个大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的随机浮点数。
	/// </summary>
	/// <returns>大于或等于 <c>0.0</c> 且小于 <c>1.0</c> 的双精度浮点数。</returns>
	protected override double Sample()
	{
		return NextDouble();
	}

	/// <summary>
	/// 返回一个非负随机整数。
	/// </summary>
	/// <returns>大于或等于 <c>0</c> 且小于 <see cref="ulong.MaxValue"/> 的 64 位无符号整数。</returns>
	private ulong NextUInt64()
	{
		if (index >= n)
		{
			Twist();
		}
		ulong y = MT[index];
		y ^= ((y >> u) & d);
		y ^= ((y << s) & b);
		y ^= ((y << t) & c);
		y ^= (y >> l);
		index++;
		return y;
	}

	/// <summary>
	/// 返回指定值以 2 为底的对数向上取整的值。
	/// </summary>
	/// <param name="value">要获取对数的数字。</param>
	/// <returns>指定值的底数为 2 的对数。</returns>
	private static int Log2Ceiling(uint value)
	{
		int result = BitOperations.Log2(value);
		if (BitOperations.PopCount(value) != 1)
		{
			result++;
		}
		return result;
	}

	/// <summary>
	/// 返回指定值以 2 为底的对数向上取整的值。
	/// </summary>
	/// <param name="value">要获取对数的数字。</param>
	/// <returns>指定值的底数为 2 的对数。</returns>
	private static int Log2Ceiling(ulong value)
	{
		int result = BitOperations.Log2(value);
		if (BitOperations.PopCount(value) != 1)
		{
			result++;
		}
		return result;
	}

	/// <summary>
	/// 旋转算法。
	/// </summary>
	private void Twist()
	{
		for (ulong i = 0; i < n; ++i)
		{
			ulong x = (MT[i] & upperMask) + (MT[(i + 1) % n] & lowerMask);
			ulong xA = x >> 1;
			if (x % 2 != 0)
			{
				xA ^= a;
			}
			MT[i] = MT[(i + m) % n] ^ xA;
		}
		index = 0;
	}
}
