using System.Globalization;

namespace Cyjb.Conversions;

/// <summary>
/// 提供进制转换的相关能力。
/// </summary>
public static class BaseConvert
{
	/// <summary>
	/// 所有用于不同进制的字符。
	/// </summary>
	private static readonly char[] BaseDigits =
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
		'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
	};

	#region 字符串转换为数字

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位有符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位有符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="sbyte.MinValue"/> 或大于 
	/// <see cref="sbyte.MaxValue"/> 的数字。</exception>
	[CLSCompliant(false)]
	public static sbyte ToSByte(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return sbyte.Parse(value, NumberStyles.AllowLeadingSign);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return sbyte.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		uint uValue = StringToUInt32(value, fromBase);
		// fromBase 总是不为 10。
		if (uValue <= byte.MaxValue)
		{
			return unchecked((sbyte)uValue);
		}
		throw CommonExceptions.OverflowSByte();
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>8</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>8</c> 位无符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="byte.MinValue"/> 或大于 
	/// <see cref="byte.MaxValue"/> 的数字。</exception>
	public static byte ToByte(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return byte.Parse(value, NumberStyles.None);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return byte.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		uint uValue = StringToUInt32(value, fromBase);
		if (uValue > byte.MaxValue)
		{
			throw CommonExceptions.OverflowByte();
		}
		return (byte)uValue;
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位有符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位有符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="short.MinValue"/> 或大于 
	/// <see cref="short.MaxValue"/> 的数字。</exception>
	public static short ToInt16(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return short.Parse(value, NumberStyles.AllowLeadingSign);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return short.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		uint uValue = StringToUInt32(value, fromBase);
		// fromBase 总是不为 10。
		if (uValue <= ushort.MaxValue)
		{
			return unchecked((short)uValue);
		}
		throw CommonExceptions.OverflowInt16();
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>16</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>16</c> 位无符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="ushort.MinValue"/> 或大于 
	/// <see cref="ushort.MaxValue"/> 的数字。</exception>
	[CLSCompliant(false)]
	public static ushort ToUInt16(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return ushort.Parse(value, NumberStyles.None);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return ushort.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		uint uValue = StringToUInt32(value, fromBase);
		if (uValue > ushort.MaxValue)
		{
			throw CommonExceptions.OverflowUInt16();
		}
		return (ushort)uValue;
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位有符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位有符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="int.MinValue"/> 或大于 
	/// <see cref="int.MaxValue"/> 的数字。</exception>
	public static int ToInt32(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return int.Parse(value, NumberStyles.AllowLeadingSign);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return int.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		return unchecked((int)StringToUInt32(value, fromBase));
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位无符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="int.MinValue"/> 或大于 
	/// <see cref="int.MaxValue"/> 的数字。</exception>
	[CLSCompliant(false)]
	public static uint ToUInt32(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return uint.Parse(value, NumberStyles.None);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return uint.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		return StringToUInt32(value, fromBase);
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位有符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位有符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="long.MinValue"/> 或大于 
	/// <see cref="long.MaxValue"/> 的数字。</exception>
	public static long ToInt64(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return long.Parse(value, NumberStyles.AllowLeadingSign);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return long.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		return unchecked((long)StringToUInt64(value, fromBase));
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位无符号整数，
	/// 如果 <paramref name="value"/> 为 <c>null</c>，则为 <c>0</c>（零）。</returns>
	/// <exception cref="ArgumentException"><paramref name="fromBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <exception cref="ArgumentException"><paramref name="value"/> 表示一个非 <c>10</c> 为基的有符号数，
	/// 但前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示小于 <see cref="ulong.MinValue"/> 或大于 
	/// <see cref="ulong.MaxValue"/> 的数字。</exception>
	[CLSCompliant(false)]
	public static ulong ToUInt64(ReadOnlySpan<char> value, int fromBase)
	{
		if (fromBase < 2 || fromBase > 36)
		{
			throw CommonExceptions.InvalidBase(fromBase);
		}
		// 使用内置方法，会快一些。
		if (fromBase == 10)
		{
			return ulong.Parse(value, NumberStyles.None);
		}
		else if (fromBase == 16)
		{
			// 16 进制允许使用 0x 或 0X 前缀。
			if (value.Length > 1 && value[0] == '0' && (value[1] == 'x' || value[1] == 'X'))
			{
				value = value[2..];
			}
			return ulong.Parse(value, NumberStyles.AllowHexSpecifier);
		}
		return StringToUInt64(value, fromBase);
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>32</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>32</c> 位无符号整数）。</returns>
	/// <exception cref="OverflowException"><paramref name="value"/> 前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示大于 <see cref="uint.MaxValue"/> 
	/// 的数字。</exception>
	private static uint StringToUInt32(ReadOnlySpan<char> value, int fromBase)
	{
		if (value.IsEmpty)
		{
			throw CommonExceptions.NoParsibleDigits();
		}
		else if (value[0] == '-')
		{
			throw CommonExceptions.NegativeUnsigned();
		}
		uint result = 0;
		uint uBase = (uint)fromBase;
		for (int i = 0; i < value.Length; i++)
		{
			int t = value[i].GetBaseValue(fromBase);
			if (t < 0)
			{
				if (i == 0)
				{
					throw CommonExceptions.NoParsibleDigits();
				}
				throw CommonExceptions.ExtraJunkAtEnd();
			}
			uint next = unchecked(result * uBase + (uint)t);
			// 判断是否超出 UInt32 的范围。
			if (next < result)
			{
				throw CommonExceptions.OverflowUInt32();
			}
			result = next;
		}
		return result;
	}

	/// <summary>
	/// 将指定基的数字的字符串表示形式转换为等效的 <c>64</c> 位无符号整数。
	/// </summary>
	/// <param name="value">包含要转换的数字的字符串，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="value"/> 中数字的基数。</param>
	/// <returns>与 <paramref name="value"/> 中数字等效的 <c>64</c> 位无符号整数 。</returns>
	/// <exception cref="OverflowException"><paramref name="value"/> 前面带一个负号。</exception>
	/// <exception cref="FormatException"><paramref name="value"/> 包含的一个字符不是 <paramref name="fromBase"/> 
	/// 指定的基中的有效数字。如果 <paramref name="value"/> 中的第一个字符无效，异常消息则指示没有可转换的数字；
	/// 否则，该消息将指示 <paramref name="value"/> 包含无效的尾随字符。</exception>
	/// <exception cref="OverflowException"><paramref name="value"/> 表示大于 <see cref="ulong.MaxValue"/> 
	/// 的数字。</exception>
	private static ulong StringToUInt64(ReadOnlySpan<char> value, int fromBase)
	{
		if (value.IsEmpty)
		{
			throw CommonExceptions.NoParsibleDigits();
		}
		if (value[0] == '-')
		{
			throw CommonExceptions.NegativeUnsigned();
		}
		ulong result = 0;
		ulong ulBase = (ulong)fromBase;
		for (int i = 0; i < value.Length; i++)
		{
			int t = value[i].GetBaseValue(fromBase);
			if (t < 0)
			{
				if (i == 0)
				{
					throw CommonExceptions.NoParsibleDigits();
				}
				throw CommonExceptions.ExtraJunkAtEnd();
			}
			ulong next = unchecked(result * ulBase + (uint)t);
			// 判断是否超出 UInt64 的范围。
			if (next < result)
			{
				throw CommonExceptions.OverflowUInt64();
			}
			result = next;
		}
		return result;
	}

	#endregion // 字符串转换为数字

	#region 数字转换为字符串

	/// <summary>
	/// 将 <c>8</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>8</c> 位有符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	/// <overloads>
	/// <summary>
	/// 将给定的整数值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// </overloads>
	[CLSCompliant(false)]
	public static string ToString(this sbyte value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		bool neg = false;
		uint uValue;
		if (value < 0 && toBase == 10)
		{
			// 仅 10 进制支持负数。
			neg = true;
			uValue = (uint)-value;
		}
		else
		{
			// 这里必须保证位数相同。
			uValue = (byte)value;
		}
		char[] buffer = new char[8];
		int idx = ConvertBase(buffer, uValue, toBase);
		if (neg)
		{
			buffer[--idx] = '-';
		}
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>16</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>16</c> 位有符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	public static string ToString(this short value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		bool neg = false;
		uint uValue;
		if (value < 0 && toBase == 10)
		{
			// 仅 10 进制支持负数。
			neg = true;
			uValue = (uint)-value;
		}
		else
		{
			// 这里必须保证位数相同。
			uValue = (ushort)value;
		}
		char[] buffer = new char[16];
		int idx = ConvertBase(buffer, uValue, toBase);
		if (neg)
		{
			buffer[--idx] = '-';
		}
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>32</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>32</c> 位有符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	public static string ToString(this int value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		bool neg = false;
		uint uValue;
		if (value < 0 && toBase == 10)
		{
			// 仅 10 进制支持负数。
			neg = true;
			uValue = (uint)-value;
		}
		else
		{
			// 这里必须保证位数相同。
			uValue = (uint)value;
		}
		char[] buffer = new char[32];
		int idx = ConvertBase(buffer, uValue, toBase);
		if (neg)
		{
			buffer[--idx] = '-';
		}
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>64</c> 位有符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>64</c> 位有符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	public static string ToString(this long value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		bool neg = false;
		ulong ulValue;
		if (value < 0 && toBase == 10)
		{
			// 仅 10 进制支持负数。
			neg = true;
			ulValue = (ulong)-value;
		}
		else
		{
			ulValue = (ulong)value;
		}
		char[] buffer = new char[64];
		int idx = ConvertBase(buffer, ulValue, toBase);
		if (neg)
		{
			buffer[--idx] = '-';
		}
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>8</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>8</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	public static string ToString(this byte value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		char[] buffer = new char[8];
		int idx = ConvertBase(buffer, value, toBase);
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>16</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>16</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	[CLSCompliant(false)]
	public static string ToString(this ushort value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		char[] buffer = new char[16];
		int idx = ConvertBase(buffer, value, toBase);
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>32</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>32</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException">
	/// <paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	[CLSCompliant(false)]
	public static string ToString(this uint value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		char[] buffer = new char[32];
		int idx = ConvertBase(buffer, value, toBase);
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>以 <paramref name="toBase"/> 为基的 <paramref name="value"/> 
	/// 的字符串表示形式。</returns>
	/// <exception cref="ArgumentException"><paramref name="toBase"/> 不是 <c>2</c> 到 <c>36</c> 之间的数字。</exception>
	[CLSCompliant(false)]
	public static string ToString(this ulong value, int toBase)
	{
		if (toBase < 2 || toBase > 36)
		{
			throw CommonExceptions.InvalidBase(toBase);
		}
		char[] buffer = new char[64];
		int idx = ConvertBase(buffer, value, toBase);
		return new string(buffer, idx, buffer.Length - idx);
	}

	/// <summary>
	/// 将 <c>32</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="buffer">字符串的缓冲区。</param>
	/// <param name="value">要转换的 <c>32</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>转换后字符串的起始索引。</returns>
	private static int ConvertBase(char[] buffer, uint value, int toBase)
	{
		// 从后向前转换，不必反转字符串。
		uint uBase = (uint)toBase;
		int idx = buffer.Length - 1;
		do
		{
			uint quot = value / uBase;
			int rem = (int)(value - quot * uBase);
			buffer[idx--] = BaseDigits[rem];
			value = quot;
		} while (value > 0);
		return idx + 1;
	}

	/// <summary>
	/// 将 <c>64</c> 位无符号整数的值转换为其指定基的等效字符串表示形式。
	/// </summary>
	/// <param name="buffer">字符串的缓冲区。</param>
	/// <param name="value">要转换的 <c>64</c> 位无符号整数。</param>
	/// <param name="toBase">返回值的基数，必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>转换后字符串的起始索引。</returns>
	private static int ConvertBase(char[] buffer, ulong value, int toBase)
	{
		// 从后向前转换，不必反转字符串。
		ulong ulBase = (ulong)toBase;
		int idx = buffer.Length - 1;
		do
		{
			ulong quot = value / ulBase;
			int rem = (int)(value - quot * ulBase);
			buffer[idx--] = BaseDigits[rem];
			value = quot;
		} while (value > 0);
		return idx + 1;
	}

	#endregion // 数字转换为字符串

}
