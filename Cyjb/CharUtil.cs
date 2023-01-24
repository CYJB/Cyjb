using System.Globalization;
using Cyjb.Collections;
using Cyjb.Conversions;

namespace Cyjb
{
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
				_ => escapeVisibleUnicode || !IsVisibleUnicode(ch) ? $"\\u{((uint)ch).ToString(16).PadLeft(4, '0')}" : ch.ToString(),
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
			return CharUnicodeInfo.GetUnicodeCategory(ch) switch
			{
				UnicodeCategory.Control or
				UnicodeCategory.NonSpacingMark or
				UnicodeCategory.EnclosingMark or
				UnicodeCategory.Format => 0,
				_ => WidthChars.Contains(ch) ? 2 : 1,
			};
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
}
