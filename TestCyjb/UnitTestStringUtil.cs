using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="StringUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestStringUtil
	{
		/// <summary>
		/// 对 <see cref="StringUtil.UnicodeEscape"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, null)]
		[DataRow("", "")]
		[DataRow("English or 中文 or \u0061\u0308 or \uD834\uDD60", @"English or \u4E2D\u6587 or a\u0308 or \uD834\uDD60")]
		[DataRow("English or 中文 or \u0061\u0308 or \U0001D160", @"English or \u4E2D\u6587 or a\u0308 or \uD834\uDD60")]
		[DataRow("\x25 \u0061\u0308 or \uD834\uDD60\\", @"% a\u0308 or \uD834\uDD60\\")]
		[DataRow("\\", @"\\")]
		[DataRow("\\\\", @"\\\\")]
		[DataRow("\\\x1", @"\\\u0001")]
		[DataRow("\\\\\\", @"\\\\\\")]
		[DataRow("\\\\\x1", @"\\\\\u0001")]
		[DataRow("\\ab", @"\\ab")]
		[DataRow("\\a\\b\u23556", @"\\a\\b\u23556")]
		[DataRow("\\a\\b\\U23556", @"\\a\\b\\U23556")]
		public void TestUnicodeEscape(string? str, string? expected)
		{
			Assert.AreEqual(expected, str.UnicodeEscape());
		}

		/// <summary>
		/// 对 <see cref="StringUtil.UnicodeUnescape"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, null)]
		[DataRow("", "")]
		[DataRow(@"English or 中文 or \u0061\u0308 or \uD834\uDD60", "English or 中文 or \u0061\u0308 or \uD834\uDD60")]
		[DataRow(@"English or 中文 or \u0061\u0308 or \U0001D160", "English or 中文 or \u0061\u0308 or \uD834\uDD60")]
		[DataRow(@"English or 中文 or \u0061\u0308 or \uD834\uDD60", "English or 中文 or \u0061\u0308 or \uD834\uDD60")]
		[DataRow(@"\x25 \u0061\u0308 or \uD834\uDD60\", "\x25 \u0061\u0308 or \uD834\uDD60\\")]
		[DataRow(@"\x25\x\x02\x25 \x25\x25 \u0061\u0308 or \uD834\uDD60\", "\x25\\x\x2\x25 \x25\x25 \u0061\u0308 or \uD834\uDD60\\")]
		[DataRow(@"\\", "\\")]
		[DataRow(@"\\\\", "\\\\")]
		[DataRow(@"\\\x01", "\\\x1")]
		[DataRow(@"\\\\\\", "\\\\\\")]
		[DataRow(@"\\\\\x01", "\\\\\x1")]
		[DataRow(@"\\\\\\x1\\x2", "\\\\\\x1\\x2")]
		[DataRow(@"\\ab", "\\ab")]
		[DataRow(@"\\a\\b\x23556", "\\a\\b#556")]
		[DataRow(@"\\a\\b\u23556", "\\a\\b\u23556")]
		[DataRow(@"\\a\\b\\U23556", "\\a\\b\\U23556")]
		[DataRow(@"\0\\\a\b\f\n\r\t\v\s\x0\x01\u3\u4918\U84\U000AF02A", "\0\\\a\b\f\n\r\t\v\\s\\x0\x01\\u3\u4918\\U84\U000AF02A")]
		public void TestUnicodeUnescape(string? str, string? expected)
		{
			Assert.AreEqual(expected, str.UnicodeUnescape());
		}

		/// <summary>
		/// 对 <see cref="StringUtil.Reverse"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, null)]
		[DataRow("", "")]
		[DataRow("English", "hsilgnE")]
		[DataRow("English中文", "文中hsilgnE")]
		[DataRow("English中文\u0061\u0308", "\u0061\u0308文中hsilgnE")]
		[DataRow("\U0001D160English中文\u0061\u0308", "\u0061\u0308文中hsilgnE\U0001D160")]
		[DataRow("\U0001D160English中文\u0061\u0308ABC文章", "章文CBA\u0061\u0308文中hsilgnE\U0001D160")]
		public void TestReverse(string? str, string? expected)
		{
			Assert.AreEqual(expected, str.Reverse());
		}

		/// <summary>
		/// 对 <see cref="StringUtil.ReplacePattern"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, "", "", null)]
		[DataRow("abc", "", "", "abc")]
		[DataRow("abc    def   g h", @"\s+", " ", "abc def g h")]
		public void TestReplacePattern(string? text, string pattern, string replacement, string expected)
		{
			Assert.AreEqual(expected, text.ReplacePattern(pattern, replacement));
		}
	}
}
