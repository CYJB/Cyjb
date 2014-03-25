using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.StringExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestStringExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.UnescapeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestUnescapeUnicode()
		{
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.UnescapeUnicode("English or 中文 or \\u0061\\u0308 or \\uD834\\uDD60"));
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.UnescapeUnicode("English or 中文 or \\u0061\\u0308 or \\U0001D160"));
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.UnescapeUnicode("English or 中文 or \\u0061\\u0308 or \\uD834\\uDD60"));
			Assert.AreEqual("\x25 \u0061\u0308 or \uD834\uDD60\\", StringExt.UnescapeUnicode("\x25 \\u0061\\u0308 or \\uD834\\uDD60\\"));
			Assert.AreEqual("\x25\\x\x2\x25 \x25\x25 \u0061\u0308 or \uD834\uDD60\\", StringExt.UnescapeUnicode("\x25\\x\\x02\\x25 \\x25\\x25 \\u0061\\u0308 or \\uD834\\uDD60\\"));
			Assert.AreEqual(null, StringExt.UnescapeUnicode(null));
			Assert.AreEqual("", StringExt.UnescapeUnicode(""));
			Assert.AreEqual("\\", StringExt.UnescapeUnicode("\\"));
			Assert.AreEqual("\\\\", StringExt.UnescapeUnicode("\\\\"));
			Assert.AreEqual("\\\x1", StringExt.UnescapeUnicode("\\\\x01"));
			Assert.AreEqual("\\\\\\", StringExt.UnescapeUnicode("\\\\\\"));
			Assert.AreEqual("\\\\\x1", StringExt.UnescapeUnicode("\\\\\\x01"));
			Assert.AreEqual("\\\\\\x1\\x2", StringExt.UnescapeUnicode("\\\\\\x1\\x2"));
			Assert.AreEqual("\\ab", StringExt.UnescapeUnicode("\\ab"));
			Assert.AreEqual("\\a\\b#556", StringExt.UnescapeUnicode("\\a\\b\\x23556"));
			Assert.AreEqual("\\a\\b\u23556", StringExt.UnescapeUnicode("\\a\\b\\u23556"));
			Assert.AreEqual("\\a\\b\\U23556", StringExt.UnescapeUnicode("\\a\\b\\U23556"));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.EscapeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestEscapeUnicode()
		{
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", StringExt.EscapeUnicode("English or 中文 or \u0061\u0308 or \uD834\uDD60"));
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", StringExt.EscapeUnicode("English or 中文 or \u0061\u0308 or \U0001D160"));
			Assert.AreEqual("% a\\u0308 or \\uD834\\uDD60\\", StringExt.EscapeUnicode("\x25 \u0061\u0308 or \uD834\uDD60\\"));
			Assert.AreEqual(null, StringExt.EscapeUnicode(null));
			Assert.AreEqual("", StringExt.EscapeUnicode(""));
			Assert.AreEqual("\\", StringExt.EscapeUnicode("\\"));
			Assert.AreEqual("\\\\", StringExt.EscapeUnicode("\\\\"));
			Assert.AreEqual("\\\\u0001", StringExt.EscapeUnicode("\\\x1"));
			Assert.AreEqual("\\\\\\", StringExt.EscapeUnicode("\\\\\\"));
			Assert.AreEqual("\\\\\\u0001", StringExt.EscapeUnicode("\\\\\x1"));
			Assert.AreEqual("\\ab", StringExt.EscapeUnicode("\\ab"));
			Assert.AreEqual("\\a\\b\\u23556", StringExt.EscapeUnicode("\\a\\b\u23556"));
			Assert.AreEqual("\\a\\b\\U23556", StringExt.EscapeUnicode("\\a\\b\\U23556"));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.Reverse"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestReverse()
		{
			Assert.AreEqual("hsilgnE", StringExt.Reverse("English"));
			Assert.AreEqual("文中hsilgnE", StringExt.Reverse("English中文"));
			Assert.AreEqual("\u0061\u0308文中hsilgnE", StringExt.Reverse("English中文\u0061\u0308"));
			Assert.AreEqual("\u0061\u0308文中hsilgnE\U0001D160", StringExt.Reverse("\U0001D160English中文\u0061\u0308"));
		}
	}
}
