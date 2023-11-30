using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Cyjb.Conversions;

namespace Cyjb;

/// <summary>
/// 提供 <see cref="string"/> 类的扩展方法。
/// </summary>
public static class StringUtil
{

	#region Escape

	/// <summary>
	/// 返回当前字符串的 Unicode 转义字符串。
	/// </summary>
	/// <param name="str">要进行转义的字符串。</param>
	/// <param name="escapeVisibleUnicode">是否转义可见的 Unicode 字符，默认为 <c>true</c>。</param>
	/// <returns>转义后的字符串。</returns>
	/// <remarks>
	/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
	/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会替换为其转义形式。</para>
	/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
	/// </remarks>
	[return: NotNullIfNotNull("str")]
	public static string? UnicodeEscape(this string? str, bool escapeVisibleUnicode = true)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		return str.AsSpan().UnicodeEscape(escapeVisibleUnicode);
	}

	/// <summary>
	/// 返回当前字符串的 Unicode 转义字符串。
	/// </summary>
	/// <param name="str">要进行转义的字符串。</param>
	/// <param name="escapeVisibleUnicode">是否转义可见的 Unicode 字符，默认为 <c>true</c>。</param>
	/// <returns>转义后的字符串。</returns>
	/// <remarks>
	/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），不会发生改变。</para>
	/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v），会替换为其转义形式。</para>
	/// <para>对于其它字符，会替换为其十六进制转义形式（\u0000）。</para>
	/// </remarks>
	public static string UnicodeEscape(this ReadOnlySpan<char> str, bool escapeVisibleUnicode = true)
	{
		if (str.IsEmpty)
		{
			return string.Empty;
		}
		StringBuilder builder = new(str.Length * 2);
		for (int i = 0; i < str.Length; i++)
		{
			builder.Append(str[i].UnicodeEscape(escapeVisibleUnicode));
		}
		return builder.ToString();
	}

	/// <summary>
	/// 将字符串中的 \u,\U 和 \x 转义转换为对应的字符。
	/// </summary>
	/// <param name="str">要转换的字符串。</param>
	/// <returns>转换后的字符串。</returns>
	/// <remarks>
	/// <para>解码方法支持 \x00，\u0000 和 \U00000000 转义。使用 \U 转义时，大于 0x10FFFF 的值不被支持，会被舍弃。</para>
	/// <para>如果不满足上面的情况，则不会进行转义，也不会报错。</para></remarks>
	[return: NotNullIfNotNull("str")]
	public static string? UnicodeUnescape(this string? str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		if (str.IndexOf('\\') < 0)
		{
			return str.ToString();
		}
		return str.AsSpan().UnicodeUnescape();
	}

	/// <summary>
	/// 将字符串中的 \u,\U 和 \x 转义转换为对应的字符。
	/// </summary>
	/// <param name="str">要转换的字符串。</param>
	/// <returns>转换后的字符串。</returns>
	/// <remarks>
	/// <para>解码方法支持 \x00，\u0000 和 \U00000000 转义。使用 \U 转义时，大于 0x10FFFF 的值不被支持，会被舍弃。</para>
	/// <para>如果不满足上面的情况，则不会进行转义，也不会报错。</para></remarks>
	public static string UnicodeUnescape(this ReadOnlySpan<char> str)
	{
		if (str.IsEmpty)
		{
			return string.Empty;
		}
		int idx = str.IndexOf('\\');
		if (idx < 0)
		{
			return str.ToString();
		}
		StringBuilder builder = new(str.Length);
		while (idx >= 0)
		{
			// 添加当前 '\' 之前的字符串。
			if (idx > 0)
			{
				builder.Append(str[..idx]);
				str = str[idx..];
			}
			// '\' 后没有其它字符，不是有效的转义。
			if (str.Length <= 1)
			{
				break;
			}
			// Unicode 转义需要的十六进制字符的长度。
			int hexLen = 0;
			switch (str[1])
			{
				case 'x':
					// \x 后面必须是 2 位。
					hexLen = 2;
					break;
				case 'u':
					// \u 后面必须是 4 位。
					hexLen = 4;
					break;
				case 'U':
					// \U 后面必须是 8 位。
					hexLen = 8;
					break;
				case '0':
					builder.Append('\0');
					break;
				case '\\':
					builder.Append('\\');
					break;
				case 'a':
					builder.Append('\a');
					break;
				case 'b':
					builder.Append('\b');
					break;
				case 'f':
					builder.Append('\f');
					break;
				case 'n':
					builder.Append('\n');
					break;
				case 'r':
					builder.Append('\r');
					break;
				case 't':
					builder.Append('\t');
					break;
				case 'v':
					builder.Append('\v');
					break;
				default:
					// 其它未支持的转义，添加未被解析的字符。
					builder.Append(str[..2]);
					break;
			}
			if (hexLen > 0)
			{
				// Unicode 反转义。
				if (CheckHexLength(str[2..], hexLen))
				{
					int charNum = BaseConvert.ToInt32(str[2..(2 + hexLen)], 16);
					if (charNum < 0xFFFF)
					{
						// 单个字符。
						builder.Append((char)charNum);
					}
					else
					{
						// 代理项对的字符。
						builder.Append(char.ConvertFromUtf32(charNum & 0x1FFFFF));
					}
					// 后面会统一跳过两个字符，只要跳过 hexLen 即可。
					str = str[hexLen..];
				}
				else
				{
					builder.Append(str[..2]);
				}
			}
			// 跳过已被解析或添加的字符。
			str = str[2..];
			idx = str.IndexOf('\\');
		}
		// 添加剩余的字符串。
		builder.Append(str);
		return builder.ToString();
	}

	/// <summary>
	/// 检查字符串指定索引位置之后是否包含指定个数的十六进制字符。
	/// </summary>
	/// <param name="str">要检查十六进制字符个数的字符串。</param>
	/// <param name="maxLength">需要的十六进制字符个数。</param>
	/// <returns>如果包含指定个数的十六进制字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private static bool CheckHexLength(ReadOnlySpan<char> str, int maxLength)
	{
		if (maxLength > str.Length)
		{
			return false;
		}
		for (int i = 0; i < maxLength; i++)
		{
			if (!Uri.IsHexDigit(str[i]))
			{
				return false;
			}
		}
		return true;
	}

	#endregion // Escape

	#region NaturalCompare

	/// <summary>
	/// 比较两个字符串，字符串中的数字部分会按照数字顺序比较。
	/// </summary>
	/// <param name="strA">要比较的第一个字符串。</param>
	/// <param name="strB">要比较的第二个字符串。</param>
	/// <param name="comparisonType">比较中要使用的规则。</param>
	/// <returns>一个 32 位带符号整数，指示两个字符串之间的顺序。
	/// <list type="table">
	/// <listheader>
	/// <term>值</term>
	/// <description>条件</description>
	/// </listheader>
	/// <item>
	/// <term>小于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之前。</description>
	/// </item>
	/// <item>
	/// <term>零</term>
	/// <description><paramref name="strA"/> 和 <paramref name="strB"/> 在排序顺序中的位置相同。</description>
	/// </item>
	/// <item>
	/// <term>大于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之后。</description>
	/// </item>
	/// </list>
	/// </returns>
	public static int NaturalCompare(string? strA, string? strB, StringComparison comparisonType = StringComparison.CurrentCulture)
	{
		if (strA == strB)
		{
			return 0;
		}
		if (strA == null)
		{
			return -1;
		}
		else if (strB == null)
		{
			return 1;
		}
		CultureInfo? culture = null;
		CompareOptions options = CompareOptions.None;
		switch (comparisonType)
		{
			case StringComparison.CurrentCulture:
				culture = CultureInfo.CurrentCulture;
				break;
			case StringComparison.CurrentCultureIgnoreCase:
				culture = CultureInfo.CurrentCulture;
				options = CompareOptions.IgnoreCase;
				break;
			case StringComparison.InvariantCulture:
				culture = CultureInfo.InvariantCulture;
				break;
			case StringComparison.InvariantCultureIgnoreCase:
				culture = CultureInfo.InvariantCulture;
				options = CompareOptions.IgnoreCase;
				break;
			case StringComparison.Ordinal:
				options = CompareOptions.Ordinal;
				break;
			case StringComparison.OrdinalIgnoreCase:
				options = CompareOptions.OrdinalIgnoreCase;
				break;
		}
		return NaturalCompareInternal(strA, strB, culture, options);
	}

	/// <summary>
	/// 比较两个字符串，字符串中的数字部分会按照数字顺序比较。
	/// </summary>
	/// <param name="strA">要比较的第一个字符串。</param>
	/// <param name="strB">要比较的第二个字符串。</param>
	/// <param name="ignoreCase">是否在比较过程中忽略大小写。</param>
	/// <param name="culture">区域性特定的比较信息。如果 <paramref name="culture"/> 为 <c>null</c>，
	/// 则使用当前区域性。</param>
	/// <returns>一个 32 位带符号整数，指示两个字符串之间的顺序。
	/// <list type="table">
	/// <listheader>
	/// <term>值</term>
	/// <description>条件</description>
	/// </listheader>
	/// <item>
	/// <term>小于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之前。</description>
	/// </item>
	/// <item>
	/// <term>零</term>
	/// <description><paramref name="strA"/> 和 <paramref name="strB"/> 在排序顺序中的位置相同。</description>
	/// </item>
	/// <item>
	/// <term>大于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之后。</description>
	/// </item>
	/// </list>
	/// </returns>
	public static int NaturalCompare(string? strA, string? strB, bool ignoreCase, CultureInfo? culture)
	{
		if (strA == strB)
		{
			return 0;
		}
		if (strA == null)
		{
			return -1;
		}
		else if (strB == null)
		{
			return 1;
		}
		return NaturalCompareInternal(strA, strB, culture, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
	}

	/// <summary>
	/// 比较两个字符串，字符串中的数字部分会按照数字顺序比较。
	/// </summary>
	/// <param name="strA">要比较的第一个字符串。</param>
	/// <param name="strB">要比较的第二个字符串。</param>
	/// <param name="culture">区域性特定的比较信息。如果 <paramref name="culture"/> 为 <c>null</c>，
	/// 则使用当前区域性。</param>
	/// <param name="options">要在执行比较时使用的选项（如忽略大小写或符号）。</param>
	/// <returns>一个 32 位带符号整数，指示两个字符串之间的顺序。
	/// <list type="table">
	/// <listheader>
	/// <term>值</term>
	/// <description>条件</description>
	/// </listheader>
	/// <item>
	/// <term>小于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之前。</description>
	/// </item>
	/// <item>
	/// <term>零</term>
	/// <description><paramref name="strA"/> 和 <paramref name="strB"/> 在排序顺序中的位置相同。</description>
	/// </item>
	/// <item>
	/// <term>大于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之后。</description>
	/// </item>
	/// </list>
	/// </returns>
	public static int NaturalCompare(string? strA, string? strB, CultureInfo? culture, CompareOptions options)
	{
		if (strA == strB)
		{
			return 0;
		}
		if (strA == null)
		{
			return -1;
		}
		else if (strB == null)
		{
			return 1;
		}
		return NaturalCompareInternal(strA, strB, culture, options);
	}

	/// <summary>
	/// 比较两个字符串，字符串中的数字部分会按照数字顺序比较。
	/// </summary>
	/// <param name="strA">要比较的第一个字符串。</param>
	/// <param name="strB">要比较的第二个字符串。</param>
	/// <param name="culture">区域性特定的比较信息。如果 <paramref name="culture"/> 为 <c>null</c>，
	/// 则使用当前区域性。</param>
	/// <param name="options">要在执行比较时使用的选项（如忽略大小写或符号）。</param>
	/// <returns>一个 32 位带符号整数，指示两个字符串之间的顺序。
	/// <list type="table">
	/// <listheader>
	/// <term>值</term>
	/// <description>条件</description>
	/// </listheader>
	/// <item>
	/// <term>小于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之前。</description>
	/// </item>
	/// <item>
	/// <term>零</term>
	/// <description><paramref name="strA"/> 和 <paramref name="strB"/> 在排序顺序中的位置相同。</description>
	/// </item>
	/// <item>
	/// <term>大于零</term>
	/// <description><paramref name="strA"/> 在排序顺序中位于 <paramref name="strB"/> 之后。</description>
	/// </item>
	/// </list>
	/// </returns>
	private static int NaturalCompareInternal(string strA, string strB, CultureInfo? culture, CompareOptions options)
	{
		IEnumerator<NaturalPart> secondEnum = SplitNaturalPart(strB).GetEnumerator();
		foreach (NaturalPart part in SplitNaturalPart(strA))
		{
			if (secondEnum.MoveNext())
			{
				int result = part.Compare(strA, secondEnum.Current, strB, culture, options);
				if (result != 0)
				{
					return result;
				}
			}
			else
			{
				// 第一个字符串更长。
				return 1;
			}
		}
		if (secondEnum.MoveNext())
		{
			// 第二个字符串更长。
			return -1;
		}
		else
		{
			// 两个字符串等长。
			return 0;
		}
	}

	/// <summary>
	/// ASCII 数字字符数组。
	/// </summary>
	private static readonly char[] AsciiDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

	/// <summary>
	/// 返回指定字符是否是 ASCII 的 0-9。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns><paramref name="ch"/> 是否是 ASCII 的 0-9。</returns>
	private static bool IsAsciiDigit(char ch)
	{
		return ch >= '0' && ch <= '9';
	}

	/// <summary>
	/// 自然比较的部分。
	/// </summary>
	private readonly struct NaturalPart
	{
		/// <summary>
		/// 部分的起始位置。
		/// </summary>
		private readonly int start;
		/// <summary>
		/// 部分的结束位置。
		/// </summary>
		private readonly int end;
		/// <summary>
		/// 部分是否是数字。
		/// </summary>
		private readonly bool isNumber;

		/// <summary>
		/// 初始化自然比较的部分。
		/// </summary>
		/// <param name="start">的起始位置。</param>
		/// <param name="end">的结束位置。</param>
		/// <param name="isNumber">是否是数字。</param>
		public NaturalPart(int start, int end, bool isNumber)
		{
			this.start = start;
			this.end = end;
			this.isNumber = isNumber;
		}

		/// <summary>
		/// 当前部分的长度。
		/// </summary>
		private int Length => end - start;

		/// <summary>
		/// 与另一个部分比较。
		/// </summary>比
		/// <param name="str">当前部分对应的字符串。</param>
		/// <param name="otherPart">要比较的部分。</param>
		/// <param name="otherStr">要比较的部分对应的字符串。</param>
		/// <param name="culture">区域性特定的比较信息。如果 <paramref name="culture"/> 为 <c>null</c>，
		/// 则使用当前区域性。</param>
		/// <param name="options">要在执行比较时使用的选项（如忽略大小写或符号）。</param>
		/// <returns>一个 32 位带符号整数，指示两个字符串之间的顺序。</returns>
		public int Compare(string str, NaturalPart otherPart, string otherStr, CultureInfo? culture, CompareOptions options)
		{
			if (isNumber)
			{
				if (otherPart.isNumber)
				{
					// 比较数字大小。
					ReadOnlySpan<char> spanA = str.AsSpan(start, Length);
					int prefixZeroA = TrimPrefixZero(ref spanA);
					ReadOnlySpan<char> spanB = otherStr.AsSpan(otherPart.start, otherPart.Length);
					int prefixZeroB = TrimPrefixZero(ref spanB);
					int result = spanA.Length - spanB.Length;
					if (result == 0)
					{
						// 数字直接按 Unicode 比较即可。
						result = spanA.CompareTo(spanB, StringComparison.Ordinal);
						if (result == 0)
						{
							// 数字相同时，比较前导 0 的个数。
							result = prefixZeroA - prefixZeroB;
						}
					}
					return result;
				}
				else
				{
					// 另一个部分的首字符一定不是数字，直接与 '0' 比较大小即可。
					return '0'.CompareTo(otherStr[otherPart.start]);
				}
			}
			else if (otherPart.isNumber)
			{
				// 当前部分的首字符一定不是数字，直接与 '0' 比较大小即可。
				return str[start].CompareTo('0');
			}
			else
			{
				// 比较字符串顺序。
				return string.Compare(str, start, otherStr, otherPart.start, Math.Max(Length, otherPart.Length),
					culture, options);
			}
		}
	}

	/// <summary>
	/// 按照文本和数字分割字符串。
	/// </summary>
	private static IEnumerable<NaturalPart> SplitNaturalPart(string fileName)
	{
		int len = fileName.Length;
		for (int i = 0; i < len;)
		{
			int idx = fileName.IndexOfAny(AsciiDigits, i);
			if (idx >= 0)
			{
				if (idx > i)
				{
					yield return new NaturalPart(i, idx, false);
				}
				for (i = idx + 1; i < len && IsAsciiDigit(fileName[i]); i++) ;
				yield return new NaturalPart(idx, i, true);
			}
			else
			{
				// 剩余部分全部是普通字符串。
				yield return new NaturalPart(i, len, false);
				break;
			}
		}
	}

	/// <summary>
	/// 移除指定字符串的前导 0。
	/// </summary>
	/// <param name="str">要移除前导 0 的字符串。</param>
	/// <returns>已移除的前导 0 个数。</returns>
	private static int TrimPrefixZero(ref ReadOnlySpan<char> str)
	{
		int i = 0;
		for (; i < str.Length && str[i] == '0'; i++) ;
		str = str[i..];
		return i;
	}

	#endregion // NaturalCompare

	/// <summary>
	/// 返回指定字符串的字符顺序是相反的字符串。
	/// </summary>
	/// <param name="str">字符反转的字符串。</param>
	/// <returns>字符反转后的字符串。</returns>
	/// <remarks>参考了 Microsoft.VisualBasic.Strings.StrReverse 方法的实现。</remarks>
	[return: NotNullIfNotNull("str")]
	public static string? Reverse(this string? str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		return string.Create(str.Length, str.Length, (Span<char> span, int len) =>
		{
			int end = len - 1;
			int i = 0;
			while (end >= 0)
			{
				switch (CharUnicodeInfo.GetUnicodeCategory(str[i]))
				{
					case UnicodeCategory.Surrogate:
					case UnicodeCategory.NonSpacingMark:
					case UnicodeCategory.SpacingCombiningMark:
					case UnicodeCategory.EnclosingMark:
						// 字符串中包含组合字符，翻转时需要保证组合字符的顺序。
						// 为了能够包含基字符，回退一位。
						if (i > 0)
						{
							i--;
							end++;
						}
						int start = i;
						TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(str, start);
						textElementEnumerator.MoveNext();
						int idx = start + textElementEnumerator.ElementIndex;
						while (end >= 0)
						{
							i = idx;
							idx = textElementEnumerator.MoveNext() ? start + textElementEnumerator.ElementIndex : len;
							for (int j = idx - 1; j >= i; span[end--] = str[j--]) ;
						}
						return;
				}
				// 直接复制。
				span[end--] = str[i++];
			}
		});
	}

	/// <summary>
	/// 使用指定的字符串替换与指定正则表达式模式匹配的所有字符串。
	/// </summary>
	/// <param name="text">要搜索匹配项的字符串。</param>
	/// <param name="pattern">要匹配的正则表达式模式。</param>
	/// <param name="replacement">替换字符串。</param>
	/// <returns>被替换后的新字符串。</returns>
	/// <overloads>
	/// <summary>
	/// 使用指定的字符串替换与指定正则表达式模式匹配的所有字符串。
	/// </summary>
	/// </overloads>
	[return: NotNullIfNotNull("text")]
	public static string? ReplacePattern(this string? text, string pattern, string replacement)
	{
		if (text == null)
		{
			return null;
		}
		return Regex.Replace(text, pattern, replacement);
	}

	/// <summary>
	/// 使用指定的字符串替换与指定正则表达式模式匹配的所有字符串。
	/// </summary>
	/// <param name="text">要搜索匹配项的字符串。</param>
	/// <param name="pattern">要匹配的正则表达式模式。</param>
	/// <param name="replacement">替换字符串。</param>
	/// <param name="options">正则表达式的匹配选项。</param>
	/// <returns>被替换后的新字符串。</returns>
	[return: NotNullIfNotNull("text")]
	public static string? ReplacePattern(this string? text, string pattern, string replacement, RegexOptions options)
	{
		if (text == null)
		{
			return null;
		}
		return Regex.Replace(text, pattern, replacement, options);
	}

	/// <summary>
	/// 判断指定的字符串是否是 <c>null</c> 或者 <see cref="string.Empty"/>。
	/// </summary>
	/// <param name="value">要测试的字符串。</param>
	/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c> 或者 <see cref="string.Empty"/>，
	/// 则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
	{
		return string.IsNullOrEmpty(value);
	}

	/// <summary>
	/// 判断指定的字符串是否是 <c>null</c>、<see cref="string.Empty"/> 或者仅由空白字符组成。
	/// </summary>
	/// <param name="value">要测试的字符串。</param>
	/// <returns>如果 <paramref name="value"/> 参数是 <c>null</c>、<see cref="string.Empty"/> 
	/// 或者仅由空白字符组成，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
	{
		return string.IsNullOrWhiteSpace(value);
	}
}
