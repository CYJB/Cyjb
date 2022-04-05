﻿using Cyjb;
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
			// 0x20000 ~ 0x2FFFD
			string target = char.ConvertFromUtf32(0x1FFFF) + char.ConvertFromUtf32(0x20000) +
				char.ConvertFromUtf32(0x2FFFD) + char.ConvertFromUtf32(0x2FFFE);
			int index = 0;
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			// 0x30000 ~ 0x3FFFD
			target = char.ConvertFromUtf32(0x2FFFF) + char.ConvertFromUtf32(0x30000) +
				char.ConvertFromUtf32(0x3FFFD) + char.ConvertFromUtf32(0x3FFFE);
			index = 0;
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			Assert.AreEqual(2, CharUtil.Width(target, ref index));
			Assert.AreEqual(1, CharUtil.Width(target, ref index));
		}
	}
}