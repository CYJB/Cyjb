using System.Globalization;
using BenchmarkDotNet.Attributes;
using Cyjb.Collections;

namespace Cyjb.Benchmark;

/// <summary>
/// | Method | Mean     | Error   | StdDev  |<br/>
/// |------- |---------:|--------:|--------:|<br/>
/// | Impl1  | 333.3 us | 2.55 us | 2.26 us |<br/>
/// | Impl2  | 196.5 us | 0.28 us | 0.25 us |<br/>
/// | Impl3  | 141.3 us | 1.14 us | 1.07 us |<br/>
/// </summary>
public class CharWidthBenchmark
{
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

	[Benchmark]
	public int Impl1()
	{
		int width = 0;
		for (int i = 0; i < 65536; i++)
		{
			width = Width1((char)i);
		}
		return width;
	}

	private static int Width1(char ch)
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

	[Benchmark]
	public int Impl2()
	{
		int width = 0;
		for (int i = 0; i < 65536; i++)
		{
			width = Width2((char)i);
		}
		return width;
	}

	private static int Width2(char ch)
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

	[Benchmark]
	public int Impl3()
	{
		int width = 0;
		for (int i = 0; i < 65536; i++)
		{
			width = ((char)i).Width();
		}
		return width;
	}
}

