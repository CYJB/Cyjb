using System.Globalization;
using Cyjb.Conversions;

namespace Cyjb;

/// <summary>
/// 提供 <see cref="char"/> 类的扩展方法。
/// </summary>
public static class CharUtil
{
	/// <summary>
	/// 返回当前字符是否是行分隔符，包含 <c>'\n'</c>, <c>'\r'</c>, <c>'\u0085'</c>,
	/// <c>'\u2028'</c> 和 <c>'\u2029'</c>。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>当前字符是否是行分隔符。</returns>
	public static bool IsAnyLineSeparator(this char ch)
	{
		if (ch > '\r')
		{
			if (ch < '\u0085')
			{
				return false;
			}
			else if (ch > '\u2029')
			{
				return false;
			}
			return ch is '\u0085' or '\u2028' or '\u2029';
		}
		else
		{
			return ch is '\n' or '\r';
		}
	}

	/// <summary>
	/// 返回当前字符的 Unicode 转义字符串。
	/// </summary>
	/// <param name="ch">要获取转义字符串的字符。</param>
	/// <param name="escapeVisibleUnicode">是否转义可见的 Unicode 字符，默认为 <c>true</c>。</param>
	/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
	/// <remarks>
	/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
	/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会返回其转义形式。</para>
	/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
	/// </remarks>
	public static string UnicodeEscape(this char ch, bool escapeVisibleUnicode = true) => ch switch
	{
		'\0' => @"\0",
		'\a' => @"\a",
		'\b' => @"\b",
		'\f' => @"\f",
		'\n' => @"\n",
		'\r' => @"\r",
		'\t' => @"\t",
		'\v' => @"\v",
		'\\' => @"\\",
		>= ' ' and <= '~' => ch.ToString(CultureInfo.InvariantCulture),
		_ => escapeVisibleUnicode || !IsVisibleUnicode(ch)
			? $"\\u{((uint)ch).ToString(16).PadLeft(4, '0')}"
			: ch.ToString(CultureInfo.InvariantCulture),
	};

	/// <summary>
	/// 返回当前字符的 Unicode 字符是否需要被转义。
	/// </summary>
	/// <param name="ch">要检查是否需要转义的字符。</param>
	/// <param name="escapeVisibleUnicode">是否转义可见的 Unicode 字符，默认为 <c>true</c>。</param>
	/// <returns>如果字符需要被转义，则返回 <c>true</c>；不需要被转义则返回 <c>false</c>。</returns>
	/// <remarks>
	/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
	/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会返回其转义形式。</para>
	/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
	/// </remarks>
	public static bool NeedUnicodeEscape(this char ch, bool escapeVisibleUnicode = true) => ch switch
	{
		'\0' => true,
		'\a' => true,
		'\b' => true,
		'\f' => true,
		'\n' => true,
		'\r' => true,
		'\t' => true,
		'\v' => true,
		'\\' => true,
		>= ' ' and <= '~' => false,
		_ => escapeVisibleUnicode || !IsVisibleUnicode(ch),
	};

	/// <summary>
	/// 返回指定的字符是否是可见的 Unicode 字符。
	/// </summary>
	/// <param name="ch">要检查的字符。</param>
	/// <returns>如果指定字符是可见的 Unicode 字符，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	private static bool IsVisibleUnicode(char ch)
	{
		UnicodeCategory category = char.GetUnicodeCategory(ch);
		return category switch
		{
			UnicodeCategory.Control
			or UnicodeCategory.SpaceSeparator
			or UnicodeCategory.Format
			or UnicodeCategory.NonSpacingMark
			or UnicodeCategory.OtherNotAssigned
			or UnicodeCategory.EnclosingMark
			or UnicodeCategory.SpacingCombiningMark
			or UnicodeCategory.LineSeparator
			or UnicodeCategory.ParagraphSeparator
			or UnicodeCategory.Surrogate => false,
			_ => true,
		};
	}

	/// <summary>
	/// 返回当前字符以指定的基表示的值。
	/// </summary>
	/// <param name="ch">要获取值的字符，使用不区分大小写的字母表示大于 <c>10</c> 的数。</param>
	/// <param name="fromBase"><paramref name="ch"/> 中数字的基数，它必须位于 <c>2</c> 到 <c>36</c> 之间。</param>
	/// <returns>如果字符有效，则返回字符对应的值。否则返回 <c>-1</c>。</returns>
	public static int GetBaseValue(this char ch, int fromBase)
	{
		int value = -1;
		if (ch < 'A')
		{
			if (ch >= '0' && ch <= '9')
			{
				value = ch - '0';
			}
		}
		else if (ch < 'a')
		{
			if (ch <= 'Z')
			{
				value = ch - 'A' + 10;
			}
		}
		else if (ch <= 'z')
		{
			value = ch - 'a' + 10;
		}
		if (value < fromBase)
		{
			return value;
		}
		return -1;
	}

	/// <summary>
	/// ASCII 部分的单词查找表。
	/// </summary>
	private static readonly byte[] WordASCIILookup = new byte[] {
		0, 0, 0, 0, 0, 0, 255, 3, 254, 255, 255, 135, 254, 255, 255, 7
	};

	/// <summary>
	/// 返回当前字符是否是单词字符。
	/// </summary>
	/// <param name="ch">要判断的字符。</param>
	/// <returns>如果当前字符是单词字符，则返回 <c>true</c>；否则返回 <c>false</c>。</returns>
	/// <remarks>
	/// 单词字符包括：
	/// <list type="table">
	/// <item><term>Ll</term><description>小写字母</description></item>
	/// <item><term>Lu</term><description>大写字母</description></item>
	/// <item><term>Lt</term><description>词首字母大写的字母</description></item>
	/// <item><term>Lo</term><description>其它字母</description></item>
	/// <item><term>Lm</term><description>修饰符字母</description></item>
	/// <item><term>Mn</term><description>非间距标记</description></item>
	/// <item><term>Nd</term><description>十进制数字</description></item>
	/// <item><term>Pc</term><description>连接符标点</description></item>
	/// <item><term>U+200C</term><description>ZERO WIDTH NON-JOINER</description></item>
	/// <item><term>U+200D</term><description>ZERO WIDTH JOINER</description></item>
	/// </list>
	/// </remarks>
	public static bool IsWord(this char ch)
	{
		if (ch <= '\x7F')
		{
			return (WordASCIILookup[ch >> 3] & (1 << (ch & 7))) != 0;
		}
		// U+200C ZERO WIDTH NON-JOINER and U+200D ZERO WIDTH JOINER.
		if (ch == '\u200C' || ch == '\u200D')
		{
			return true;
		}
		UnicodeCategory category = char.GetUnicodeCategory(ch);
		if (category <= UnicodeCategory.NonSpacingMark ||
			category == UnicodeCategory.DecimalDigitNumber ||
			category == UnicodeCategory.ConnectorPunctuation)
		{
			return true;
		}
		return false;
	}

	#region Width

	/// <summary>
	/// 零宽字符范围 0000 ~ 20FF。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版，具体包括 <see cref="UnicodeCategory.Control"/>、
	/// <see cref="UnicodeCategory.NonSpacingMark"/>、<see cref="UnicodeCategory.EnclosingMark"/> 和
	/// <see cref="UnicodeCategory.Format"/>。</remarks>
	private static readonly ulong[] ZeroWidthCharMap1 = new ulong[] {
		// 0000 ~ 001F
		0xFFFFFFFF, 
		// 007F ~ 009F
		// 00AD ~ 00AD
		0x8000000000000000, 0x2000FFFFFFFF, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		// 0300 ~ 036F
		ulong.MaxValue, 0xFFFFFFFFFFFF, 0, 0, 0, 0, 
		// 0483 ~ 0489
		0x3F8, 0, 0, 0, 
		// 0591 ~ 05BD
		// 05BF ~ 05BF
		0xBFFFFFFFFFFE0000, 
		// 05C1 ~ 05C2
		// 05C4 ~ 05C5
		// 05C7 ~ 05C7
		0xB6, 
		// 0600 ~ 0605
		// 0610 ~ 061A
		// 061C ~ 061C
		0x17FF003F,
		// 064B ~ 065F
		// 0670 ~ 0670
		0x10000FFFFF800, 0,
		// 06D6 ~ 06DD
		// 06DF ~ 06E4
		// 06E7 ~ 06E8
		// 06EA ~ 06ED
		0x3D9FBFC00000,
		// 070F ~ 070F
		// 0711 ~ 0711
		// 0730 ~ 074A
		0xFFFF000000028000, 0x7FF,
		// 07A6 ~ 07B0
		0x1FFC000000000,
		// 07EB ~ 07F3
		// 07FD ~ 07FD
		0x200FF80000000000,
		// 0816 ~ 0819
		// 081B ~ 0823
		// 0825 ~ 0827
		// 0829 ~ 082D
		0x3EEFFBC00000,
		// 0859 ~ 085B
		0xE000000, 0, 
		// 08D3 ~ 0902
		// 093A ~ 093A
		// 093C ~ 093C
		0xFFFFFFFFFFF80000, 0x1400000000000007,
		// 0941 ~ 0948
		// 094D ~ 094D
		// 0951 ~ 0957
		// 0962 ~ 0963
		0xC00FE21FE,
		// 0981 ~ 0981
		// 09BC ~ 09BC
		0x1000000000000002,
		// 09C1 ~ 09C4
		// 09CD ~ 09CD
		// 09E2 ~ 09E3
		// 09FE ~ 09FE
		0x4000000C0000201E,
		// 0A01 ~ 0A02
		// 0A3C ~ 0A3C
		0x1000000000000006,
		// 0A41 ~ 0A42
		// 0A47 ~ 0A48
		// 0A4B ~ 0A4D
		// 0A51 ~ 0A51
		// 0A70 ~ 0A71
		// 0A75 ~ 0A75
		0x23000000023986,
		// 0A81 ~ 0A82
		// 0ABC ~ 0ABC
		0x1000000000000006,
		// 0AC1 ~ 0AC5
		// 0AC7 ~ 0AC8
		// 0ACD ~ 0ACD
		// 0AE2 ~ 0AE3
		// 0AFA ~ 0AFF
		0xFC00000C000021BE,
		// 0B01 ~ 0B01
		// 0B3C ~ 0B3C
		// 0B3F ~ 0B3F
		0x9000000000000002,
		// 0B41 ~ 0B44
		// 0B4D ~ 0B4D
		// 0B55 ~ 0B56
		// 0B62 ~ 0B63
		0xC0060201E,
		// 0B82 ~ 0B82
		0x4,
		// 0BC0 ~ 0BC0
		// 0BCD ~ 0BCD
		0x2001,
		// 0C00 ~ 0C00
		// 0C04 ~ 0C04
		// 0C3E ~ 0C40
		// 0C46 ~ 0C48
		// 0C4A ~ 0C4D
		// 0C55 ~ 0C56
		// 0C62 ~ 0C63
		0xC000000000000011, 0xC00603DC1,
		// 0C81 ~ 0C81
		// 0CBC ~ 0CBC
		// 0CBF ~ 0CBF
		0x9000000000000002,
		// 0CC6 ~ 0CC6
		// 0CCC ~ 0CCD
		// 0CE2 ~ 0CE3
		0xC00003040,
		// 0D00 ~ 0D01
		// 0D3B ~ 0D3C
		0x1800000000000003,
		// 0D41 ~ 0D44
		// 0D4D ~ 0D4D
		// 0D62 ~ 0D63
		0xC0000201E,
		// 0D81 ~ 0D81
		0x2,
		// 0DCA ~ 0DCA
		// 0DD2 ~ 0DD4
		// 0DD6 ~ 0DD6
		0x5C0400,
		// 0E31 ~ 0E31
		// 0E34 ~ 0E3A
		0x7F2000000000000,
		// 0E47 ~ 0E4E
		0x7F80,
		// 0EB1 ~ 0EB1
		// 0EB4 ~ 0EBC
		0x1FF2000000000000,
		// 0EC8 ~ 0ECD
		0x3F00,
		// 0F18 ~ 0F19
		// 0F35 ~ 0F35
		// 0F37 ~ 0F37
		// 0F39 ~ 0F39
		0x2A0000003000000,
		// 0F71 ~ 0F7E
		0x7FFE000000000000,
		// 0F80 ~ 0F84
		// 0F86 ~ 0F87
		// 0F8D ~ 0F97
		// 0F99 ~ 0FBC
		0x1FFFFFFFFEFFE0DF,
		// 0FC6 ~ 0FC6
		0x40,
		// 102D ~ 1030
		// 1032 ~ 1037
		// 1039 ~ 103A
		// 103D ~ 103E
		0x66FDE00000000000,
		// 1058 ~ 1059
		// 105E ~ 1060
		// 1071 ~ 1074
		0x1E0001C3000000,
		// 1082 ~ 1082
		// 1085 ~ 1086
		// 108D ~ 108D
		// 109D ~ 109D
		0x20002064, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
		// 135D ~ 135F
		0xE0000000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
		// 1712 ~ 1714
		// 1732 ~ 1734
		0x1C0000001C0000,
		// 1752 ~ 1753
		// 1772 ~ 1773
		0xC0000000C0000,
		// 17B4 ~ 17B5
		// 17B7 ~ 17BD
		0x3FB0000000000000,
		// 17C6 ~ 17C6
		// 17C9 ~ 17D3
		// 17DD ~ 17DD
		0x200FFE40,
		// 180B ~ 180E
		0x7800, 0,
		// 1885 ~ 1886
		// 18A9 ~ 18A9
		0x20000000060, 0,
		// 1920 ~ 1922
		// 1927 ~ 1928
		// 1932 ~ 1932
		// 1939 ~ 193B
		0xE04018700000000, 0, 0, 0,
		// 1A17 ~ 1A18
		// 1A1B ~ 1A1B
		0x9800000,
		// 1A56 ~ 1A56
		// 1A58 ~ 1A5E
		// 1A60 ~ 1A60
		// 1A62 ~ 1A62
		// 1A65 ~ 1A6C
		// 1A73 ~ 1A7C
		// 1A7F ~ 1A7F
		0x9FF81FE57F400000,
		// 1AB0 ~ 1AC0
		0xFFFF000000000000, 0x1,
		// 1B00 ~ 1B03
		// 1B34 ~ 1B34
		// 1B36 ~ 1B3A
		// 1B3C ~ 1B3C
		0x17D000000000000F,
		// 1B42 ~ 1B42
		// 1B6B ~ 1B73
		0xFF80000000004,
		// 1B80 ~ 1B81
		// 1BA2 ~ 1BA5
		// 1BA8 ~ 1BA9
		// 1BAB ~ 1BAD
		0x3B3C00000003,
		// 1BE6 ~ 1BE6
		// 1BE8 ~ 1BE9
		// 1BED ~ 1BED
		// 1BEF ~ 1BF1
		0x3A34000000000,
		// 1C2C ~ 1C33
		// 1C36 ~ 1C37
		0xCFF00000000000, 0, 0,
		// 1CD0 ~ 1CD2
		// 1CD4 ~ 1CE0
		// 1CE2 ~ 1CE8
		// 1CED ~ 1CED
		// 1CF4 ~ 1CF4
		// 1CF8 ~ 1CF9
		0x31021FDFFF70000, 0, 0, 0,
		// 1DC0 ~ 1DF9
		// 1DFB ~ 1DFF
		0xFBFFFFFFFFFFFFFF,0, 0, 0, 0, 0, 0, 0, 0,
		// 200B ~ 200F
		// 202A ~ 202E
		0x7C000000F800,
		// 2060 ~ 2064
		// 2066 ~ 206F
		0xFFDF00000000, 0,
		// 20D0 ~ 20F0
		0x1FFFFFFFF0000,
	};

	/// <summary>
	/// 零宽字符范围 2CC0 ~ 2DFF。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版，具体包括 <see cref="UnicodeCategory.Control"/>、
	/// <see cref="UnicodeCategory.NonSpacingMark"/>、<see cref="UnicodeCategory.EnclosingMark"/> 和
	/// <see cref="UnicodeCategory.Format"/>。</remarks>
	private static readonly ulong[] ZeroWidthCharMap2 = new ulong[] {
		// 2CEF ~ 2CF1
		0x3800000000000, 0,
		// 2D7F ~ 2D7F
		0x8000000000000000, 0,
		// 2DE0 ~ 2DFF
		0xFFFFFFFF00000000,
	};

	/// <summary>
	/// 零宽字符范围 A640 ~ ABFF。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版，具体包括 <see cref="UnicodeCategory.Control"/>、
	/// <see cref="UnicodeCategory.NonSpacingMark"/>、<see cref="UnicodeCategory.EnclosingMark"/> 和
	/// <see cref="UnicodeCategory.Format"/>。</remarks>
	private static readonly ulong[] ZeroWidthCharMap3 = new ulong[] {
		// A66F ~ A672
		// A674 ~ A67D
		0x3FF7800000000000, 
		// A69E ~ A69F
		0xC0000000,
		// A6F0 ~ A6F1
		0x3000000000000,0, 0, 0, 0,
		// A802 ~ A802
		// A806 ~ A806
		// A80B ~ A80B
		// A825 ~ A826
		// A82C ~ A82C
		0x106000000844, 0, 0,
		// A8C4 ~ A8C5
		// A8E0 ~ A8F1
		// A8FF ~ A8FF
		0x8003FFFF00000030,
		// A926 ~ A92D
		0x3FC000000000,
		// A947 ~ A951
		0x3FF80,
		// A980 ~ A982
		// A9B3 ~ A9B3
		// A9B6 ~ A9B9
		// A9BC ~ A9BD
		0x33C8000000000007,
		// A9E5 ~ A9E5
		0x2000000000,
		// AA29 ~ AA2E
		// AA31 ~ AA32
		// AA35 ~ AA36
		0x667E0000000000,
		// AA43 ~ AA43
		// AA4C ~ AA4C
		// AA7C ~ AA7C
		0x1000000000001008,
		// AAB0 ~ AAB0
		// AAB2 ~ AAB4
		// AAB7 ~ AAB8
		// AABE ~ AABF
		0xC19D000000000000,
		// AAC1 ~ AAC1
		// AAEC ~ AAED
		// AAF6 ~ AAF6
		0x40300000000002, 0, 0, 0,
		// ABE5 ~ ABE5
		// ABE8 ~ ABE8
		// ABED ~ ABED
		0x212000000000,
	};

	/// <summary>
	/// 宽字符范围 2300 ~ 2B7F。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版。详情请参见 
	/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。</remarks>
	/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
	private static readonly ulong[] WidthCharMap1 = new ulong[] {
		// User interface symbols
		// 231A ~ 231B
		// 2329 ~ 232A
		0x6000C000000, 0, 0,
		// 23E9 ~ 23EC
		// 23F0 ~ 23F0
		// 23F3 ~ 23F3
		0x91E0000000000, 0, 0, 0, 0, 0, 0, 0, 
		// 25FD ~ 25FE
		0x6000000000000000,
		// 2614 ~ 2615
		0x300000,
		// 2648 ~ 2653
		// 267F ~ 267F
		0x80000000000FFF00,
		// 2693 ~ 2693
		// 26A1 ~ 26A1
		// 26AA ~ 26AB
		// 26BD ~ 26BE
		0x60000C0200080000,
		// 26C4 ~ 26C5
		// 26CE ~ 26CE
		// 26D4 ~ 26D4
		// 26EA ~ 26EA
		// 26F2 ~ 26F3
		// 26F5 ~ 26F5
		// 26FA ~ 26FA
		// 26FD ~ 26FD
		0x242C040000104030,
		// 2705 ~ 2705
		// 270A ~ 270B
		// 2728 ~ 2728
		0x10000000C20,
		// 274C ~ 274C
		// 274E ~ 274E
		// 2753 ~ 2755
		// 2757 ~ 2757
		0xB85000,
		// 2795 ~ 2797
		// 27B0 ~ 27B0
		// 27BF ~ 27BF
		0x8001000000E00000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		// 2B1B ~ 2B1C
		0x18000000,
		// 2B50 ~ 2B50
		// 2B55 ~ 2B55
		0x210000,
	};

	/// <summary>
	/// 返回指定字符的宽度。
	/// </summary>
	/// <param name="ch">要获取宽度的字符。</param>
	/// <returns>指定字符的宽度，结果可能是 <c>0</c>、<c>1</c> 或 <c>2</c>。</returns>
	/// <overloads>
	/// <summary>
	/// 返回指定字符的宽度。
	/// </summary>
	/// </overloads>
	public static int Width(this char ch)
	{
		if (ch < 0x7F)
		{
			if (ch <= 0x1F)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
		else if (ch <= 0x20FF)
		{
			if (ch >= 0x1100 && ch <= 0x115F)
			{
				// 宽字符 1100 ~ 115F Hangul Jamo
				return 2;
			}
			else if ((ZeroWidthCharMap1[ch >> 6] & (1UL << (ch & 0x3F))) > 0)
			{
				// 零宽字符。
				return 0;
			}
			else
			{
				// 普通字符。
				return 1;
			}
		}
		else if (ch <= 0xA4CF)
		{
			if (ch < 0x2E80)
			{
				if (ch < 0x2300)
				{
					return 1;
				}
				else if (ch <= 0x2B7F)
				{
					if ((WidthCharMap1[(ch - 0x2300) >> 6] & (1UL << (ch & 0x3F))) > 0)
					{
						return 2;
					}
					else
					{
						return 1;
					}
				}
				else if (ch >= 0x2CC0 && ch <= 0x2DFF)
				{
					if ((ZeroWidthCharMap2[(ch - 0x2CC0) >> 6] & (1UL << (ch & 0x3F))) > 0)
					{
						return 0;
					}
					else
					{
						return 1;
					}
				}
				else
				{
					return 1;
				}
			}
			else
			{
				// 宽字符
				// 2E80 ~ 2EFF CJK Radicals Supplement
				// 2F00 ~ 2FDF Kangxi Radicals
				// 2FF0 ~ 2FFF Ideographic Description Characters
				// 3000 ~ 303E CJK Symbols and Punctuation
				// 下面两个范围虽然属于 NonSpacingMark，但实际宽度貌似是按照 2 来计算的。
				// 302A ~ 302D
				// 3099 ~ 309A
				// 
				// 3040 ~ 309F Hiragana
				// 30A0 ~ 30FF Katakana
				// 3100 ~ 312F Bopomofo
				// 3030 ~ 318F Hangul Compatibility Jamo
				// 3190 ~ 319F Kanbun
				// 31A0 ~ 31BF Bopomofo Extended
				// 31C0 ~ 31EF CJK Strokes
				// 31F0 ~ 31FF Katakana Phonetic Extensions
				// 3200 ~ 32FF Enclosed CJK Letters and Months
				// 3300 ~ 33FF CJK Compatibility
				// 3400 ~ 4DBF CJK Unified Ideographs Extension A
				//
				// 4E00 ~ 9FFC CJK Unified Ideographs
				// A000 ~ A48F Yi Syllables
				// A490 ~ A4CF Yi Radicals
				if (ch == 0x303F || (ch >= 0x4DC0 && ch <= 0x4DFF))
				{
					return 1;
				}
				else
				{
					return 2;
				}
			}
		}
		else if (ch <= 0xD7A3)
		{
			if (ch >= 0xAC00)
			{
				// 宽字符 AC00 ~ D7A3 Hangul Syllables
				return 2;
			}
			else if (ch >= 0xA640 && ch <= 0xABFF)
			{
				if ((ZeroWidthCharMap3[(ch - 0xA640) >> 6] & (1UL << (ch & 0x3F))) > 0)
				{
					return 0;
				}
				else if (ch >= 0xA960 && ch <= 0xA97F)
				{
					// 宽字符 A960 ~ A97F Hangul Jamo Extended-A
					return 2;
				}
				else
				{
					return 1;
				}
			}
			else
			{
				return 1;
			}
		}
		else if (ch < 0xF900)
		{
			return 1;
		}
		else if (ch <= 0xFAFF)
		{
			// 宽字符 F900 ~ FAFF CJK Compatibility Ideographs
			return 2;
		}
		else if (ch < 0xFE00)
		{
			// 零宽字符 FB1E ~ FB1E
			return ch == 0xFB1E ? 0 : 1;
		}
		else if (ch < 0xFEFF)
		{
			if (ch > 0xFE6F)
			{
				return 1;
			}
			else if (ch > 0xFE2F)
			{
				// FE30 ~ FE4F CJK Compatibility Forms
				// FE50 ~ FE6F Small Form Variants
				return 2;
			}
			else if (ch <= 0xFE19)
			{
				if (ch <= 0xFE0F)
				{
					// 零宽字符 FE00 ~ FE0F
					return 0;
				}
				else
				{
					// 宽字符 FE10 ~ FE19 Vertical forms
					return 2;
				}
			}
			else if (ch < 0xFE20)
			{
				return 1;
			}
			else
			{
				// 零宽字符 FE20 ~ FE2F
				return 0;
			}
		}
		else if (ch <= 0xFF60)
		{
			if (ch == 0xFEFF)
			{
				// 零宽字符 FEFF ~ FEFF
				return 0;
			}
			else
			{
				// 宽字符 FF00 ~ FF60 Fullwidth ASCII variants
				return 2;
			}
		}
		else if (ch < 0xFFE0)
		{
			return 1;
		}
		else if (ch <= 0xFFE6)
		{
			// 宽字符 FFE0 ~ FFE6 Fullwidth symbol variants
			return 2;
		}
		else if (ch < 0xFFF9)
		{
			return 1;
		}
		else if (ch <= 0xFFFB)
		{
			// FFF9 ~ FFFB"
			return 0;
		}
		return 1;
	}

	/// <summary>
	/// UTF-32 的宽字符范围。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版。详情请参见 
	/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。</remarks>
	/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
	private static readonly int[] WidthUTF32Chars = new[] {
		// 16FE0 ~ 16FFF Ideographic Symbols and Punctuation
		// 17000 ~ 18AFF Tangut & Components
		// 18B00 ~ 18CFF Khitan Small Script
		// 18D00 ~ 18D08 Tangut Supplement
		// 1B000 ~ 1B12F Kana Supplement & Extended-A
		// 1B130 ~ 1B16F Small Kana Extension
		// 1B170 ~ 1B2FF Nushu
		0x16FE0, 0x1B2FF,
		// 1F004 MAHJONG TILE RED DRAGON
		0x1F004, 0x1F004,
		// 1F0CF PLAYING CARD BLACK JOKER
		0x1F0CF, 0x1F0CF,
		// 1F18E NEGATIVE SQUARED AB
		0x1F18E, 0x1F18E,
		// 1F191 ~ 1F19A SQUARED CL..SQUARED VS
		0x1F191, 0x1F19A,
		// 1F200 ~ 1F2FF Enclosed Ideographic Supplement
		// 1F300 ~ 1F320 CYCLONE..SHOOTING STAR
		0x1F200, 0x1F320,
		// 1F32D ~ 1F335 HOT DOG..CACTUS
		0x1F32D, 0x1F335,
		// 1F337 ~ 1F37C TULIP..BABY BOTTLE
		0x1F337, 0x1F37C,
		// 1F37E ~ 1F393 BOTTLE WITH POPPING CORK..GRADUATION CAP
		0x1F37E, 0x1F393,
		// 1F3A0 ~ 1F3CA CAROUSEL HORSE..SWIMMER
		0x1F3A0, 0x1F3CA,
		// 1F3CF ~ 1F3D3 CRICKET BAT AND BALL..TABLE TENNIS PADDLE AND BALL
		0x1F3CF, 0x1F3D3,
		// 1F3E0 ~ 1F3F0 HOUSE BUILDING..EUROPEAN CASTLE
		0x1F3E0, 0x1F3F0,
		// 1F3F4 WAVING BLACK FLAG
		0x1F3F4, 0x1F3F4,
		// 1F3F8 ~ 1F43E BADMINTON RACQUET AND SHUTTLECOCK..AMPHORA
		0x1F3F8, 0x1F43E,
		// 1F440 EYES
		0x1F440, 0x1F440,
		// 1F442 ~ 1F4FC EAR..VIDEOCASSETTE
		0x1F442, 0x1F4FC,
		// 1F4FF ~ 1F53D PRAYER BEADS..DOWN-POINTING SMALL RED TRIANGLE
		0x1F4FF, 0x1F53D,
		// 1F54B ~ 1F54E KAABA..MENORAH WITH NINE BRANCHES
		0x1F54B, 0x1F54E,
		// 1F550 ~ 1F567 CLOCK FACE ONE OCLOCK..CLOCK FACE TWELVE-THIRTY
		0x1F550, 0x1F567,
		// 1F57A MAN DANCING
		0x1F57A, 0x1F57A,
		// 1F595 ~ 1F596 REVERSED HAND WITH MIDDLE FINGER EXTENDED..RAISED HAND WITH PART BETWEEN MIDDLE AND RING FINGERS
		0x1F595, 0x1F596,
		// 1F5A4 BLACK HEART
		0x1F5A4, 0x1F5A4,
		// 1F5FB ~ 1F64F MOUNT FUJI..MOYAI
		0x1F5FB, 0x1F64F,
		// 1F680 ~ 1F6C5 ROCKET..LEFT LUGGAGE
		0x1F680, 0x1F6C5,
		// 1F6CC SLEEPING ACCOMMODATION
		0x1F6CC, 0x1F6CC,
		// 1F6D0 ~ 1F6D2 PLACE OF WORSHIP..SHOPPING TROLLEY
		0x1F6D0, 0x1F6D2,
		// 1F6D5 ~ 1F6D7 HINDU TEMPLE..ELEVATOR
		0x1F6D5, 0x1F6D7,
		// 1F6EB ~ 1F6EC AIRPLANE DEPARTURE..AIRPLANE ARRIVING
		0x1F6EB, 0x1F6EC,
		// 1F6F4 ~ 1F6FC SCOOTER..ROLLER SKATE
		0x1F6F4, 0x1F6FC,
		// 1F7E0 ~ 1F7EB LARGE ORANGE CIRCLE..LARGE BROWN SQUARE
		0x1F7E0, 0x1F7EB,
		// 1F90C ~ 1F93A PINCHED FINGERS..FENCER
		0x1F90C, 0x1F93A,
		// 1F93C ~ 1F945 WRESTLERS..GOAL NET
		0x1F93C, 0x1F945,
		// 1F947 ~ 1F9FF FIRST PLACE MEDAL..DISGUISED FACE
		0x1F947, 0x1F9FF,
		// 1FA70 ~ 1FAFF Symbols and Pictographs Extended-A
		0x1FA70, 0x1FAFF,
	};

	/// <summary>
	/// 返回指定字符串中位于指定位置的字符的宽度。
	/// </summary>
	/// <param name="str">要获取宽度的字符字符串。</param>
	/// <param name="index"><paramref name="str"/> 中的字符位置，并在方法调用之后设置为下一个字符位置，
	/// 会识别可能的 Unicode 代理项对。</param>
	/// <returns><paramref name="str"/> 中指定字符的宽度，结果可能是 <c>0</c>、<c>1</c> 或 <c>2</c>。</returns>
	/// <remarks>此方法基于 Unicode 标准 13.0.0 版。详情请参见 
	/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。 
	/// 返回 <c>0</c> 表示控制字符、非间距字符或格式字符，<c>1</c> 表示半角字符，
	/// <c>2</c> 表示全角字符。</remarks>
	/// <exception cref="ArgumentNullException"><paramref name="str"/> 为 <c>null</c>。</exception>
	/// <exception cref="IndexOutOfRangeException"><paramref name="index"/> 大于等于字符串的长度或小于零。</exception>
	/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
	public static int Width(string str, ref int index)
	{
		if (char.IsSurrogatePair(str, index))
		{
			UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(str, index);
			switch (category)
			{
				case UnicodeCategory.Control:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.EnclosingMark:
				case UnicodeCategory.Format:
					index += 2;
					return 0;
			}
			int ch = char.ConvertToUtf32(str, index);
			index += 2;
			// 其它宽字符范围
			if (ch < 0x20000)
			{
				int idx = Array.BinarySearch(WidthUTF32Chars, ch);
				if (idx >= 0 || (~idx & 1) == 1)
				{
					return 2;
				}
			}
			else if (ch <= 0x3FFFD)
			{
				// Plane 2: U+20000..U+2FFFD
				// Plane 3: U+30000..U+3FFFD
				return 2;
			}
			return 1;
		}
		else
		{
			return Width(str[index++]);
		}
	}

	#endregion // Width

	/// <summary>
	/// 返回当前字符对应的全角字符。
	/// </summary>
	/// <param name="ch">要转换的字符。</param>
	/// <returns>当前字符对应的全角字符，如果不存在则返回当前字符。</returns>
	public static char ToFullWidth(this char ch)
	{
		if (ch <= '~' && ch >= '!')
		{
			return (char)(ch + 0xFEE0);
		}
		else if (ch == ' ')
		{
			return '　';
		}
		else
		{
			return ch;
		}
	}

	/// <summary>
	/// 返回当前字符对应的半角字符。
	/// </summary>
	/// <param name="ch">要转换的字符。</param>
	/// <returns>当前字符对应的半角字符，如果不存在则返回当前字符。</returns>
	public static char ToHalfWidth(this char ch)
	{
		if (ch >= '！' && ch <= '～')
		{
			return (char)(ch - 0xFEE0);
		}
		else if (ch == '　')
		{
			return ' ';
		}
		else
		{
			return ch;
		}
	}
}
