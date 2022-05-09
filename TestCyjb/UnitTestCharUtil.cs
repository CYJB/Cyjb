using System.Globalization;
using System.Text;
using Cyjb;
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
		/// 对 <see cref="CharUtil.UnicodeEscape"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow('a', @"a")]
		[DataRow('z', @"z")]
		[DataRow('\0', @"\0")]
		[DataRow('\\', @"\\")]
		[DataRow('\a', @"\a")]
		[DataRow('\b', @"\b")]
		[DataRow('\f', @"\f")]
		[DataRow('\n', @"\n")]
		[DataRow('\r', @"\r")]
		[DataRow('\t', @"\t")]
		[DataRow('\v', @"\v")]
		[DataRow('中', @"\u4E2D")]
		[DataRow('\x1', @"\u0001")]
		public void TestUnicodeEscape(char ch, string expected)
		{
			Assert.AreEqual(expected, ch.UnicodeEscape());
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
			Assert.AreEqual(1, '\uFF00'.Width());
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
			// 0x3FFFE
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
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
