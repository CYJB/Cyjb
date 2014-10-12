using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="CharExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCharExt
	{
		/// <summary>
		/// 对 <see cref="CharExt.IsHex(char)"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsHex()
		{
			for (char c = '\x00'; c < '\xFF'; c++)
			{
				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						Assert.IsTrue(c.IsHex(), "{0} 应当不是 Hex。", c);
						break;
					default:
						Assert.IsFalse(c.IsHex(), "{0} 应当是 Hex。", c);
						break;
				}
			}
		}
		/// <summary>
		/// 对 <see cref="CharExt.Width(char)"/> 方法进行测试。
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
			Assert.AreEqual(1, '\uFE2F'.Width());
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
			// 0x20000 ~ 0x2FFFD
			Assert.AreEqual(1, CharExt.Width(char.ConvertFromUtf32(0x1FFFF), 0));
			Assert.AreEqual(2, CharExt.Width(char.ConvertFromUtf32(0x20000), 0));
			Assert.AreEqual(2, CharExt.Width(char.ConvertFromUtf32(0x2FFFD), 0));
			Assert.AreEqual(1, CharExt.Width(char.ConvertFromUtf32(0x2FFFE), 0));
			// 0x30000 ~ 0x3FFFD
			Assert.AreEqual(1, CharExt.Width(char.ConvertFromUtf32(0x2FFFF), 0));
			Assert.AreEqual(2, CharExt.Width(char.ConvertFromUtf32(0x30000), 0));
			Assert.AreEqual(2, CharExt.Width(char.ConvertFromUtf32(0x3FFFD), 0));
			Assert.AreEqual(1, CharExt.Width(char.ConvertFromUtf32(0x3FFFE), 0));
		}
	}
}
