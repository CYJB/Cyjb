using System.Globalization;
using Cyjb.Collections;
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
	public static string UnicodeEscape(this char ch, bool escapeVisibleUnicode = true)
	{
		if (ch == '\\')
		{
			return @"\\";
		}
		else if (ch >= ' ' && ch <= '~')
		{
			return ch.ToString(CultureInfo.InvariantCulture);
		}
		return ch switch
		{
			'\0' => @"\0",
			'\a' => @"\a",
			'\b' => @"\b",
			'\f' => @"\f",
			'\n' => @"\n",
			'\r' => @"\r",
			'\t' => @"\t",
			'\v' => @"\v",
			_ => escapeVisibleUnicode || !IsVisibleUnicode(ch) ? $"\\u{((uint)ch).ToString(16).PadLeft(4, '0')}" : ch.ToString(CultureInfo.InvariantCulture),
		};
	}

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
	public static bool NeedUnicodeEscape(this char ch, bool escapeVisibleUnicode = true)
	{
		if (ch == '\\')
		{
			return true;
		}
		else if (ch >= ' ' && ch <= '~')
		{
			return false;
		}
		return ch switch
		{
			'\0' => true,
			'\a' => true,
			'\b' => true,
			'\f' => true,
			'\n' => true,
			'\r' => true,
			'\t' => true,
			'\v' => true,
			_ => escapeVisibleUnicode || !IsVisibleUnicode(ch),
		};
	}

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
		UnicodeCategory category = char.GetUnicodeCategory(ch);
		if (category <= UnicodeCategory.NonSpacingMark ||
			category == UnicodeCategory.DecimalDigitNumber ||
			category == UnicodeCategory.ConnectorPunctuation)
		{
			return true;
		}
		// U+200C ZERO WIDTH NON-JOINER and U+200D ZERO WIDTH JOINER.
		return ch == '\u200C' || ch == '\u200D';
	}

	#region Width

	/// <summary>
	/// 零宽字符范围。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版，具体包括 <see cref="UnicodeCategory.Control"/>、
	/// <see cref="UnicodeCategory.NonSpacingMark"/>、<see cref="UnicodeCategory.EnclosingMark"/> 和
	/// <see cref="UnicodeCategory.Format"/>。</remarks>
	/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
	private static readonly CharSet ZeroWidthChars = CharSet.FromRange(
		"\u0000\u001F" +
		"\u007F\u009F" +
		"\u00AD\u00AD" +
		"\u0300\u036F" +
		"\u0483\u0489" +
		"\u0591\u05BD" +
		"\u05BF\u05BF" +
		"\u05C1\u05C2" +
		"\u05C4\u05C5" +
		"\u05C7\u05C7" +
		"\u0600\u0605" +
		"\u0610\u061A" +
		"\u061C\u061C" +
		"\u064B\u065F" +
		"\u0670\u0670" +
		"\u06D6\u06DD" +
		"\u06DF\u06E4" +
		"\u06E7\u06E8" +
		"\u06EA\u06ED" +
		"\u070F\u070F" +
		"\u0711\u0711" +
		"\u0730\u074A" +
		"\u07A6\u07B0" +
		"\u07EB\u07F3" +
		"\u07FD\u07FD" +
		"\u0816\u0819" +
		"\u081B\u0823" +
		"\u0825\u0827" +
		"\u0829\u082D" +
		"\u0859\u085B" +
		"\u08D3\u0902" +
		"\u093A\u093A" +
		"\u093C\u093C" +
		"\u0941\u0948" +
		"\u094D\u094D" +
		"\u0951\u0957" +
		"\u0962\u0963" +
		"\u0981\u0981" +
		"\u09BC\u09BC" +
		"\u09C1\u09C4" +
		"\u09CD\u09CD" +
		"\u09E2\u09E3" +
		"\u09FE\u09FE" +
		"\u0A01\u0A02" +
		"\u0A3C\u0A3C" +
		"\u0A41\u0A42" +
		"\u0A47\u0A48" +
		"\u0A4B\u0A4D" +
		"\u0A51\u0A51" +
		"\u0A70\u0A71" +
		"\u0A75\u0A75" +
		"\u0A81\u0A82" +
		"\u0ABC\u0ABC" +
		"\u0AC1\u0AC5" +
		"\u0AC7\u0AC8" +
		"\u0ACD\u0ACD" +
		"\u0AE2\u0AE3" +
		"\u0AFA\u0AFF" +
		"\u0B01\u0B01" +
		"\u0B3C\u0B3C" +
		"\u0B3F\u0B3F" +
		"\u0B41\u0B44" +
		"\u0B4D\u0B4D" +
		"\u0B55\u0B56" +
		"\u0B62\u0B63" +
		"\u0B82\u0B82" +
		"\u0BC0\u0BC0" +
		"\u0BCD\u0BCD" +
		"\u0C00\u0C00" +
		"\u0C04\u0C04" +
		"\u0C3E\u0C40" +
		"\u0C46\u0C48" +
		"\u0C4A\u0C4D" +
		"\u0C55\u0C56" +
		"\u0C62\u0C63" +
		"\u0C81\u0C81" +
		"\u0CBC\u0CBC" +
		"\u0CBF\u0CBF" +
		"\u0CC6\u0CC6" +
		"\u0CCC\u0CCD" +
		"\u0CE2\u0CE3" +
		"\u0D00\u0D01" +
		"\u0D3B\u0D3C" +
		"\u0D41\u0D44" +
		"\u0D4D\u0D4D" +
		"\u0D62\u0D63" +
		"\u0D81\u0D81" +
		"\u0DCA\u0DCA" +
		"\u0DD2\u0DD4" +
		"\u0DD6\u0DD6" +
		"\u0E31\u0E31" +
		"\u0E34\u0E3A" +
		"\u0E47\u0E4E" +
		"\u0EB1\u0EB1" +
		"\u0EB4\u0EBC" +
		"\u0EC8\u0ECD" +
		"\u0F18\u0F19" +
		"\u0F35\u0F35" +
		"\u0F37\u0F37" +
		"\u0F39\u0F39" +
		"\u0F71\u0F7E" +
		"\u0F80\u0F84" +
		"\u0F86\u0F87" +
		"\u0F8D\u0F97" +
		"\u0F99\u0FBC" +
		"\u0FC6\u0FC6" +
		"\u102D\u1030" +
		"\u1032\u1037" +
		"\u1039\u103A" +
		"\u103D\u103E" +
		"\u1058\u1059" +
		"\u105E\u1060" +
		"\u1071\u1074" +
		"\u1082\u1082" +
		"\u1085\u1086" +
		"\u108D\u108D" +
		"\u109D\u109D" +
		"\u135D\u135F" +
		"\u1712\u1714" +
		"\u1732\u1734" +
		"\u1752\u1753" +
		"\u1772\u1773" +
		"\u17B4\u17B5" +
		"\u17B7\u17BD" +
		"\u17C6\u17C6" +
		"\u17C9\u17D3" +
		"\u17DD\u17DD" +
		"\u180B\u180E" +
		"\u1885\u1886" +
		"\u18A9\u18A9" +
		"\u1920\u1922" +
		"\u1927\u1928" +
		"\u1932\u1932" +
		"\u1939\u193B" +
		"\u1A17\u1A18" +
		"\u1A1B\u1A1B" +
		"\u1A56\u1A56" +
		"\u1A58\u1A5E" +
		"\u1A60\u1A60" +
		"\u1A62\u1A62" +
		"\u1A65\u1A6C" +
		"\u1A73\u1A7C" +
		"\u1A7F\u1A7F" +
		"\u1AB0\u1AC0" +
		"\u1B00\u1B03" +
		"\u1B34\u1B34" +
		"\u1B36\u1B3A" +
		"\u1B3C\u1B3C" +
		"\u1B42\u1B42" +
		"\u1B6B\u1B73" +
		"\u1B80\u1B81" +
		"\u1BA2\u1BA5" +
		"\u1BA8\u1BA9" +
		"\u1BAB\u1BAD" +
		"\u1BE6\u1BE6" +
		"\u1BE8\u1BE9" +
		"\u1BED\u1BED" +
		"\u1BEF\u1BF1" +
		"\u1C2C\u1C33" +
		"\u1C36\u1C37" +
		"\u1CD0\u1CD2" +
		"\u1CD4\u1CE0" +
		"\u1CE2\u1CE8" +
		"\u1CED\u1CED" +
		"\u1CF4\u1CF4" +
		"\u1CF8\u1CF9" +
		"\u1DC0\u1DF9" +
		"\u1DFB\u1DFF" +
		"\u200B\u200F" +
		"\u202A\u202E" +
		"\u2060\u2064" +
		"\u2066\u206F" +
		"\u20D0\u20F0" +
		"\u2CEF\u2CF1" +
		"\u2D7F\u2D7F" +
		"\u2DE0\u2DFF" +
		"\u302A\u302D" +
		"\u3099\u309A" +
		"\uA66F\uA672" +
		"\uA674\uA67D" +
		"\uA69E\uA69F" +
		"\uA6F0\uA6F1" +
		"\uA802\uA802" +
		"\uA806\uA806" +
		"\uA80B\uA80B" +
		"\uA825\uA826" +
		"\uA82C\uA82C" +
		"\uA8C4\uA8C5" +
		"\uA8E0\uA8F1" +
		"\uA8FF\uA8FF" +
		"\uA926\uA92D" +
		"\uA947\uA951" +
		"\uA980\uA982" +
		"\uA9B3\uA9B3" +
		"\uA9B6\uA9B9" +
		"\uA9BC\uA9BD" +
		"\uA9E5\uA9E5" +
		"\uAA29\uAA2E" +
		"\uAA31\uAA32" +
		"\uAA35\uAA36" +
		"\uAA43\uAA43" +
		"\uAA4C\uAA4C" +
		"\uAA7C\uAA7C" +
		"\uAAB0\uAAB0" +
		"\uAAB2\uAAB4" +
		"\uAAB7\uAAB8" +
		"\uAABE\uAABF" +
		"\uAAC1\uAAC1" +
		"\uAAEC\uAAED" +
		"\uAAF6\uAAF6" +
		"\uABE5\uABE5" +
		"\uABE8\uABE8" +
		"\uABED\uABED" +
		"\uFB1E\uFB1E" +
		"\uFE00\uFE0F" +
		"\uFE20\uFE2F" +
		"\uFEFF\uFEFF" +
		"\uFFF9\uFFFB");

	/// <summary>
	/// 宽字符范围。
	/// </summary>
	/// <remarks>此字段基于 Unicode 标准 13.0.0 版。详情请参见 
	/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。</remarks>
	/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
	private static readonly CharSet WidthChars = CharSet.FromRange(
		// 1100 ~ 115F Hangul Jamo
		"\u1100\u115F" +
		// User interface symbols
		"\u231A\u231B" +
		"\u2329\u232A" +
		"\u23E9\u23EC" +
		"\u23F0\u23F0" +
		"\u23F3\u23F3" +
		"\u25FD\u25FE" +
		"\u2614\u2615" +
		"\u2648\u2653" +
		"\u267F\u267F" +
		"\u2693\u2693" +
		"\u26A1\u26A1" +
		"\u26AA\u26AB" +
		"\u26BD\u26BE" +
		"\u26C4\u26C5" +
		"\u26CE\u26CE" +
		"\u26D4\u26D4" +
		"\u26EA\u26EA" +
		"\u26F2\u26F3" +
		"\u26F5\u26F5" +
		"\u26FA\u26FA" +
		"\u26FD\u26FD" +
		"\u2705\u2705" +
		"\u270A\u270B" +
		"\u2728\u2728" +
		"\u274C\u274C" +
		"\u274E\u274E" +
		"\u2753\u2755" +
		"\u2757\u2757" +
		"\u2795\u2797" +
		"\u27B0\u27B0" +
		"\u27BF\u27BF" +
		"\u2B1B\u2B1C" +
		"\u2B50\u2B50" +
		"\u2B55\u2B55" +
		// 2E80 ~ 2EFF CJK Radicals Supplement
		// 2F00 ~ 2FDF Kangxi Radicals
		// 2FF0 ~ 2FFF Ideographic Description Characters
		// 3000 ~ 303E CJK Symbols and Punctuation
		"\u2E80\u303E" +
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
		"\u3040\u4DBF" +
		// 4E00 ~ 9FFC CJK Unified Ideographs
		// A000 ~ A48F Yi Syllables
		// A490 ~ A4CF Yi Radicals
		"\u4E00\uA4CF" +
		// A960 ~ A97F Hangul Jamo Extended-A
		"\uA960\uA97F" +
		// AC00 ~ D7A3 Hangul Syllables
		"\uAC00\uD7A3" +
		// F900 ~ FAFF CJK Compatibility Ideographs
		"\uF900\uFAFF" +
		// FE10 ~ FE19 Vertical forms
		"\uFE10\uFE19" +
		// FE30 ~ FE4F CJK Compatibility Forms
		// FE50 ~ FE6F Small Form Variants
		"\uFE30\uFE6F" +
		// FF00 ~ FF60 Fullwidth ASCII variants
		"\uFF01\uFF60" +
		// FFE0 ~ FFE6 Fullwidth symbol variants
		"\uFFE0\uFFE6");

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
		if (ZeroWidthChars.Contains(ch))
		{
			return 0;
		}
		else if (WidthChars.Contains(ch))
		{
			return 2;
		}
		else
		{
			return 1;
		}
	}

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
