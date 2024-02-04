using System.Globalization;
using System.Text;
using Cyjb;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="CharUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCharUtil
	{
		/// <summary>
		/// 对 <see cref="CharUtil.IsAnyLineSeparator"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		public void TestIsAnyLineSeparator()
		{
			for (int i = 0; i <= char.MaxValue; i++)
			{
				char ch = (char)i;
				bool expected = ch is '\n' or '\r' or '\u0085' or '\u2028' or '\u2029';
				Assert.AreEqual(expected, ch.IsAnyLineSeparator());
			}
		}

		/// <summary>
		/// 对 <see cref="CharUtil.UnicodeEscape"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow('a', true, @"a")]
		[DataRow('z', true, @"z")]
		[DataRow('\0', true, @"\0")]
		[DataRow('\\', true, @"\\")]
		[DataRow('\a', true, @"\a")]
		[DataRow('\b', true, @"\b")]
		[DataRow('\f', true, @"\f")]
		[DataRow('\n', true, @"\n")]
		[DataRow('\r', true, @"\r")]
		[DataRow('\t', true, @"\t")]
		[DataRow('\v', true, @"\v")]
		[DataRow('中', true, @"\u4E2D")]
		[DataRow('\x1', true, @"\u0001")]
		[DataRow('a', false, @"a")]
		[DataRow('z', false, @"z")]
		[DataRow('\0', false, @"\0")]
		[DataRow('\\', false, @"\\")]
		[DataRow('\a', false, @"\a")]
		[DataRow('\b', false, @"\b")]
		[DataRow('\f', false, @"\f")]
		[DataRow('\n', false, @"\n")]
		[DataRow('\r', false, @"\r")]
		[DataRow('\t', false, @"\t")]
		[DataRow('\v', false, @"\v")]
		[DataRow('中', false, @"中")]
		[DataRow('\x1', false, @"\u0001")]
		public void TestUnicodeEscape(char ch, bool escapeVisibleUnicode, string expected)
		{
			Assert.AreEqual(expected, ch.UnicodeEscape(escapeVisibleUnicode));
		}

		/// <summary>
		/// 对 <see cref="CharUtil.GetBaseValue"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow('/', 2, -1)]
		[DataRow('0', 2, 0)]
		[DataRow('1', 2, 1)]
		[DataRow('2', 2, -1)]
		[DataRow('2', 3, 2)]
		[DataRow('9', 10, 9)]
		[DataRow('a', 16, 10)]
		[DataRow('A', 36, 10)]
		[DataRow('z', 35, -1)]
		[DataRow('Z', 35, -1)]
		[DataRow('z', 36, 35)]
		[DataRow('Z', 36, 35)]
		public void TestGetBaseValue(char ch, int fromBase, int expected)
		{
			Assert.AreEqual(expected, ch.GetBaseValue(fromBase));
		}

		/// <summary>
		/// 对 <see cref="CharUtil.IsWord"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsWord()
		{
			for (int i = 0; i < char.MaxValue; i++)
			{
				char ch = (char)i;
				UnicodeCategory category = char.GetUnicodeCategory(ch);
				bool expected = (category == UnicodeCategory.LowercaseLetter ||
					category == UnicodeCategory.UppercaseLetter ||
					category == UnicodeCategory.TitlecaseLetter ||
					category == UnicodeCategory.OtherLetter ||
					category == UnicodeCategory.ModifierLetter ||
					category == UnicodeCategory.NonSpacingMark ||
					category == UnicodeCategory.DecimalDigitNumber ||
					category == UnicodeCategory.ConnectorPunctuation ||
					ch == '\u200C' || ch == '\u200D');
				Assert.AreEqual(expected, ch.IsWord());
			}
		}

		/// <summary>
		/// 对 <see cref="CharUtil.Width"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestWidth()
		{
			// 宽字符范围：
			// 0x1100 ~ 0x115F：Hangul Jamo init. consonants
			Assert.AreEqual(1, '\u10FF'.Width());
			Assert.AreEqual(2, '\u1100'.Width());
			Assert.AreEqual(2, '\u1101'.Width());
			Assert.AreEqual(2, '\u115E'.Width());
			Assert.AreEqual(2, '\u115F'.Width());
			Assert.AreEqual(1, '\u1160'.Width());
			// 0x2329, 0x232A：左右尖括号〈〉
			Assert.AreEqual(1, '\u2328'.Width());
			Assert.AreEqual(2, '\u2329'.Width());
			Assert.AreEqual(2, '\u232A'.Width());
			Assert.AreEqual(1, '\u232B'.Width());
			// 0x2E80 ~ 0xA4CF 除了 0x303F：CJK ... YI
			Assert.AreEqual(1, '\u2E7F'.Width());
			Assert.AreEqual(2, '\u2E80'.Width());
			Assert.AreEqual(2, '\u2E81'.Width());
			Assert.AreEqual(2, '\u303E'.Width());
			Assert.AreEqual(1, '\u303F'.Width());
			Assert.AreEqual(2, '\u3040'.Width());
			Assert.AreEqual(2, '\uA4CE'.Width());
			Assert.AreEqual(2, '\uA4CF'.Width());
			Assert.AreEqual(1, '\uA4D0'.Width());
			// 0xAC00 ~ 0xD7A3：Hangul Syllables
			Assert.AreEqual(1, '\uABFF'.Width());
			Assert.AreEqual(2, '\uAC00'.Width());
			Assert.AreEqual(2, '\uD7A3'.Width());
			Assert.AreEqual(1, '\uD7A4'.Width());
			// 0xF900 ~ 0xFAFF：CJK Compatibility Ideographs
			Assert.AreEqual(1, '\uF8FF'.Width());
			Assert.AreEqual(2, '\uF900'.Width());
			Assert.AreEqual(2, '\uFAFF'.Width());
			Assert.AreEqual(1, '\uFB00'.Width());
			// 0xFE10 ~ 0xFE19：Vertical forms
			Assert.AreEqual(0, '\uFE0F'.Width()); // '\uFE0F' 是 NonSpacingMark
			Assert.AreEqual(2, '\uFE10'.Width());
			Assert.AreEqual(2, '\uFE19'.Width());
			Assert.AreEqual(1, '\uFE1A'.Width());
			// 0xFE30 ~ 0xFE6F：CJK Compatibility Forms
			Assert.AreEqual(0, '\uFE2F'.Width()); // '\uFE0F' 是 Format
			Assert.AreEqual(2, '\uFE30'.Width());
			Assert.AreEqual(2, '\uFE6F'.Width());
			Assert.AreEqual(1, '\uFE70'.Width());
			// 0xFF00 ~ 0xFF60：Fullwidth Forms
			Assert.AreEqual(0, '\uFEFF'.Width()); // '\uFEFF' 是 Format
			Assert.AreEqual(2, '\uFF00'.Width());
			Assert.AreEqual(2, '\uFF60'.Width());
			Assert.AreEqual(1, '\uFF61'.Width());
			// 0xFFE0 ~ 0xFFE6
			Assert.AreEqual(1, '\uFFDF'.Width());
			Assert.AreEqual(2, '\uFFE0'.Width());
			Assert.AreEqual(2, '\uFFE6'.Width());
			Assert.AreEqual(1, '\uFFE7'.Width());
			StringBuilder text = new("\uFFDF\uFFE0\uFFE6\uFFE7");
			text.AppendUTF32Char(0x10000)
				.AppendUTF32Char(0x16FDF)
				.AppendUTF32Char(0x16FE0)
				.AppendUTF32Char(0x16FE1)
				.AppendUTF32Char(0x1B2FE)
				.AppendUTF32Char(0x1B2FF)
				.AppendUTF32Char(0x1B300)
				.AppendUTF32Char(0x1F5A3)
				.AppendUTF32Char(0x1F5A4)
				.AppendUTF32Char(0x1F5A5)
				.AppendUTF32Char(0x1FAFE)
				.AppendUTF32Char(0x1FAFF)
				.AppendUTF32Char(0x1FB00)
				.AppendUTF32Char(0x1FFFF)
				.AppendUTF32Char(0x20000)
				.AppendUTF32Char(0x2FFFD)
				.AppendUTF32Char(0x2FFFE)
				.AppendUTF32Char(0x2FFFF)
				.AppendUTF32Char(0x30000)
				.AppendUTF32Char(0x3FFFD)
				.AppendUTF32Char(0xE0020)
				.AppendUTF32Char(0x3FFFE)
				;
			string target = text.ToString();
			int index = 0;
			// 0xFFDF
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0xFFE0
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0xFFE6
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0xFFE7"
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x10000
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x16FDF
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x16FE0
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x16FE1
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1B2FE
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1B2FF
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1B300
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x1F5A3
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x1F5A4
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1F5A5
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x1FAFE
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1FAFF
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x1FB00
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x1FFFF
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x20000
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x2FFFD
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x2FFFE
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x2FFFF
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x30000
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0x3FFFD
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			// 0xE0020
			Assert.AreEqual(0, CharUtil.Width(target, ref index));
			// 0x3FFFE
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
		}

		/// <summary>
		/// 对 <see cref="CharUtil.Width"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestWidth2()
		{
			// 零宽字符范围。
			CharSet ZeroWidthChars = CharSet.FromRange(
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

			// 宽字符范围。
			CharSet WidthChars = CharSet.FromRange(
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
				"\uFF00\uFF60" +
				// FFE0 ~ FFE6 Fullwidth symbol variants
				"\uFFE0\uFFE6");
			for (int i = 0; i <= char.MaxValue; i++)
			{
				char ch = (char)i;
				int targetWidth = ZeroWidthChars.Contains(ch)
					? 0
					: WidthChars.Contains(ch) ? 2 : 1;
				Assert.AreEqual(targetWidth, ch.Width(), ch.UnicodeEscape());
			}
		}

		/// <summary>
		/// 对 <see cref="CharUtil.ToHalfWidth"/> 和 <see cref="CharUtil.ToFullWidth"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestChangeWidth()
		{
			string halfWidth = "ƒ“ !\"#$%&'()*+,-./0123456789:;<=>?@" +
				"ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~”中";
			string fullWidth = "ƒ“　！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠" +
				"ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀" +
				"ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～”中";
			for (int i = 0; i < halfWidth.Length; i++)
			{
				Assert.AreEqual(halfWidth[i], fullWidth[i].ToHalfWidth());
				Assert.AreEqual(fullWidth[i], halfWidth[i].ToFullWidth());
			}
		}
	}
	static class StringBuilderTestUtil
	{
		/// <summary>
		/// 附加指定的 UTF-32 字符。
		/// </summary>
		public static StringBuilder AppendUTF32Char(this StringBuilder text, int ch)
		{
			return text.Append(char.ConvertFromUtf32(ch));
		}
	}
}
