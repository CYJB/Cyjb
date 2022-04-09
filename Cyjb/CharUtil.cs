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
		/// 返回当前字符的 Unicode 转义字符串。
		/// </summary>
		/// <param name="ch">要获取转义字符串的字符。</param>
		/// <returns>字符的转义字符串，如果无需转义则返回原始字符。</returns>
		/// <remarks>
		/// <para>对于 ASCII 可见字符（从 0x20 空格到 0x7E ~ 符号），会返回原始字符。</para>
		/// <para>对于某些特殊字符（\0, \, \a, \b, \f, \n, \r, \t, \v）以及自定义的字符，会返回其转义形式。</para>
		/// <para>对于其它字符，会返回其十六进制转义形式（\u0000）。</para>
		/// </remarks>
		public static string UnicodeEscape(this char ch)
		{
			if (ch == '\\')
			{
				return "\\\\";
			}
			else if (ch >= ' ' && ch <= '~')
			{
				return ch.ToString(CultureInfo.InvariantCulture);
			}
			return ch switch
			{
				'\0' => "\\0",
				'\a' => "\\a",
				'\b' => "\\b",
				'\f' => "\\f",
				'\n' => "\\n",
				'\r' => "\\r",
				'\t' => "\\t",
				'\v' => "\\v",
				_ => $"\\u{((uint)ch).ToString(16).PadLeft(4, '0')}",
			};
		}

		#region Width

		/// <summary>
		/// 宽字符范围。
		/// </summary>
		/// <remarks>此方法基于 Unicode 标准 13.0.0 版。详情请参见 
		/// <see href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</see>。</remarks>
		/// <seealso href="http://www.unicode.org/reports/tr11/">Unicode Standard Annex #11 EAST ASIAN WIDTH</seealso>。
		private static readonly ReadOnlyCharSet WidthChars = new("\uA522" +
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
		/// <remarks>此方法基于 Unicode 标准 6.3 版。详情请参见 
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
				// Plane 2: U+20000..U+2FFFD
				if (ch >= 0x20000 && ch <= 0x2FFFD)
				{
					return 2;
				}
				// Plane 3: U+30000..U+3FFFD
				if (ch >= 0x30000 && ch <= 0x3FFFD)
				{
					return 2;
				}
				return 1;
			}
			else
			{
				char ch = str[index];
				index++;
				return Width(ch);
			}
		}

		#endregion // Width

	}
}
