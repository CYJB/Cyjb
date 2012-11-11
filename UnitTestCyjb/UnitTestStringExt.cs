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
		/// 对 <see cref="Cyjb.StringExt.DecodeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestDecodeUnicode()
		{
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.DecodeUnicode("English or 中文 or \u0061\u0308 or \uD834\uDD60"));
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.DecodeUnicode("English or 中文 or \u0061\u0308 or \U0001D160"));
			Assert.AreEqual("English or 中文 or \u0061\u0308 or \uD834\uDD60", StringExt.DecodeUnicode("English or 中文 or \\u0061\\u0308 or \\uD834\\uDD60"));
			Assert.AreEqual("\x25 \u0061\u0308 or \uD834\uDD60\\", StringExt.DecodeUnicode("\x25 \\u0061\\u0308 or \\uD834\\uDD60\\"));
			Assert.AreEqual("\x25\\x\x2\x25 \x25\x25 \u0061\u0308 or \uD834\uDD60\\", StringExt.DecodeUnicode("\x25\\x\\x2\\x25 \\x025\\x0025 \\u0061\\u0308 or \\uD834\\uDD60\\"));
			Assert.AreEqual(null, StringExt.DecodeUnicode(null));
			Assert.AreEqual("", StringExt.DecodeUnicode(""));
			Assert.AreEqual("\\", StringExt.DecodeUnicode("\\"));
			Assert.AreEqual("\\\\", StringExt.DecodeUnicode("\\\\"));
			Assert.AreEqual("\\\x1", StringExt.DecodeUnicode("\\\\x1"));
			Assert.AreEqual("\\\\\\", StringExt.DecodeUnicode("\\\\\\"));
			Assert.AreEqual("\\\\\x1", StringExt.DecodeUnicode("\\\\\\x1"));
			Assert.AreEqual("\\ab", StringExt.DecodeUnicode("\\ab"));
			Assert.AreEqual("\\a\\b\x23556", StringExt.DecodeUnicode("\\a\\b\\x23556"));
			Assert.AreEqual("\\a\\b\u23556", StringExt.DecodeUnicode("\\a\\b\\u23556"));
			Assert.AreEqual("\\a\\b\\U23556", StringExt.DecodeUnicode("\\a\\b\\U23556"));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.StringExt.EncodeUnicode"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestEncodeUnicode()
		{
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", StringExt.EncodeUnicode("English or 中文 or \u0061\u0308 or \uD834\uDD60"));
			Assert.AreEqual("English or \\u4E2D\\u6587 or a\\u0308 or \\uD834\\uDD60", StringExt.EncodeUnicode("English or 中文 or \u0061\u0308 or \U0001D160"));
			Assert.AreEqual("% a\\u0308 or \\uD834\\uDD60\\", StringExt.EncodeUnicode("\x25 \u0061\u0308 or \uD834\uDD60\\"));
			Assert.AreEqual(null, StringExt.EncodeUnicode(null));
			Assert.AreEqual("", StringExt.EncodeUnicode(""));
			Assert.AreEqual("\\", StringExt.EncodeUnicode("\\"));
			Assert.AreEqual("\\\\", StringExt.EncodeUnicode("\\\\"));
			Assert.AreEqual("\\\\u0001", StringExt.EncodeUnicode("\\\x1"));
			Assert.AreEqual("\\\\\\", StringExt.EncodeUnicode("\\\\\\"));
			Assert.AreEqual("\\\\\\u0001", StringExt.EncodeUnicode("\\\\\x1"));
			Assert.AreEqual("\\ab", StringExt.EncodeUnicode("\\ab"));
			Assert.AreEqual("\\a\\b\\u23556", StringExt.EncodeUnicode("\\a\\b\x23556"));
			Assert.AreEqual("\\a\\b\\u23556", StringExt.EncodeUnicode("\\a\\b\u23556"));
			Assert.AreEqual("\\a\\b\\U23556", StringExt.EncodeUnicode("\\a\\b\\U23556"));
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
