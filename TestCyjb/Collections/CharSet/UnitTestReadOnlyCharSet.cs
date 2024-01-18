using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Cyjb;
using Cyjb.Collections;
using Cyjb.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="ReadOnlyCharSet"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public partial class UnitTestReadOnlyCharSet
	{
		/// <summary>
		/// 用于测试的字符范围。
		/// </summary>
		private readonly static string[] TestRanges = new string[]
		{
			// \u0000-\u080。
			new string (0.To(0x80).Select(i => (char)i).ToArray()),
			// \u0000-\u083。
			new string(0.To(0x83).Select(i => (char)i).ToArray()),
			// \u0000-\u0400。
			new string(0.To(0x400).Select(i => (char)i).ToArray()),
			// \u0000-\u0423。
			new string(0.To(0x423).Select(i => (char)i).ToArray()),
			// \u0000-\u0C00。
			new string(0.To(0xC00).Select(i => (char)i).ToArray()),
			// \u0000-\u0CF7。
			new string(0.To(0xCF7).Select(i => (char)i).ToArray()),
			// \u00F3-\u0100。
			new string(0xF3.To(0x100).Select(i => (char)i).ToArray()),
			// \u00F3-\u0200。
			new string(0xF3.To(0x200).Select(i => (char)i).ToArray()),
			// \u00F3-\u0249。
			new string(0xF3.To(0x249).Select(i => (char)i).ToArray()),
			// \u00F3-\u1660。
			new string(0xF3.To(0x1660).Select(i => (char)i).ToArray()),
			// \u04A2-\u04B0。
			new string(0x4A2.To(0x4B0).Select(i => (char)i).ToArray()),
			// \u04A2-\u04C0。
			new string(0x4A2.To(0x4C0).Select(i => (char)i).ToArray()),
			// \u04A2-\u0800。
			new string(0x4A2.To(0x800).Select(i => (char)i).ToArray()),
			// \u04A2-\u1812。
			new string(0x4A2.To(0x1812).Select(i => (char)i).ToArray()),
			// \u04A2-\uDA5E。
			new string(0x4A2.To(0xDA5E).Select(i => (char)i).ToArray()),
		 };

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet"/> 的集合方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestISetMethod()
		{
			for (int i = 0; i < TestRanges.Length; i++)
			{
				for (int j = 0; j < TestRanges.Length; j++)
				{
					string first = TestRanges[i];
					string second = TestRanges[j];
					ReadOnlyCharSet set = new(first);
					HashSet<char> expected = new(first);

					Assert.AreEqual(expected.IsSubsetOf(second), set.IsSubsetOf(second));
					Assert.AreEqual(expected.IsSubsetOf(second), set.IsSubsetOf(new ReadOnlyCharSet(second)));

					Assert.AreEqual(expected.IsProperSubsetOf(second), set.IsProperSubsetOf(second));
					Assert.AreEqual(expected.IsProperSubsetOf(second), set.IsProperSubsetOf(new ReadOnlyCharSet(second)));

					Assert.AreEqual(expected.IsSupersetOf(second), set.IsSupersetOf(second));
					Assert.AreEqual(expected.IsSupersetOf(second), set.IsSupersetOf(new ReadOnlyCharSet(second)));

					Assert.AreEqual(expected.IsProperSupersetOf(second), set.IsProperSupersetOf(second));
					Assert.AreEqual(expected.IsProperSupersetOf(second), set.IsProperSupersetOf(new ReadOnlyCharSet(second)));

					Assert.AreEqual(expected.Overlaps(second), set.Overlaps(second));
					Assert.AreEqual(expected.Overlaps(second), set.Overlaps(new ReadOnlyCharSet(second)));

					Assert.AreEqual(expected.SetEquals(second), set.SetEquals(second));
					Assert.AreEqual(expected.SetEquals(second), set.SetEquals(new ReadOnlyCharSet(second)));
				}
			}
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.IsSubsetOf"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", true)]
		[DataRow("", "abc", true)]
		[DataRow("abc", "", false)]
		[DataRow("abc", "a", false)]
		[DataRow("abc", "ac", false)]
		[DataRow("abc", "abc", true)]
		[DataRow("a", "abc", true)]
		[DataRow("ac", "abc", true)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", false)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", true)]
		public void TestIsSubsetOf(string initial, string other, bool expected)
		{
			ReadOnlyCharSet set = new(initial);
			Assert.AreEqual(expected, set.IsSubsetOf(other));
			Assert.AreEqual(expected, set.IsSubsetOf(new ReadOnlyCharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.IsSupersetOf"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", true)]
		[DataRow("", "abc", false)]
		[DataRow("abc", "", true)]
		[DataRow("abc", "a", true)]
		[DataRow("abc", "ac", true)]
		[DataRow("abc", "abc", true)]
		[DataRow("a", "abc", false)]
		[DataRow("ac", "abc", false)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", true)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", false)]
		public void TestIsSupersetOf(string initial, string other, bool expected)
		{
			ReadOnlyCharSet set = new(initial);
			Assert.AreEqual(expected, set.IsSupersetOf(other));
			Assert.AreEqual(expected, set.IsSupersetOf(new ReadOnlyCharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.Overlaps"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", false)]
		[DataRow("", "abc", false)]
		[DataRow("abc", "", false)]
		[DataRow("abc", "a", true)]
		[DataRow("abc", "ac", true)]
		[DataRow("abc", "abc", true)]
		[DataRow("a", "abc", true)]
		[DataRow("ac", "abc", true)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", true)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", true)]
		public void TestOverlaps(string initial, string other, bool expected)
		{
			ReadOnlyCharSet set = new(initial);
			Assert.AreEqual(expected, set.Overlaps(other));
			Assert.AreEqual(expected, set.Overlaps(new ReadOnlyCharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.SetEquals"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", true)]
		[DataRow("", "abc", false)]
		[DataRow("abc", "", false)]
		[DataRow("abc", "a", false)]
		[DataRow("abc", "ac", false)]
		[DataRow("abc", "abc", true)]
		[DataRow("a", "abc", false)]
		[DataRow("ac", "abc", false)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", false)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", false)]
		[DataRow("azigxhbyc2kdjs0fe49", "cdh2esjzb0f9kiaxyg4", true)]
		public void TestSetEquals(string initial, string other, bool expected)
		{
			ReadOnlyCharSet set = new(initial);
			Assert.AreEqual(expected, set.SetEquals(other));
			Assert.AreEqual(expected, set.SetEquals(new ReadOnlyCharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.ToString"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToString()
		{
			ReadOnlyCharSet set = new("abc");
			Assert.AreEqual(@"[a-c]", set.ToString());
			set += UnicodeCategory.UppercaseLetter.GetChars();
			Assert.AreEqual(@"[a-c\p{Lu}]", set.ToString());
			set += UnicodeCategory.LowercaseLetter.GetChars();
			Assert.AreEqual(@"[\p{Lu}\p{Ll}]", set.ToString());
			set += UnicodeCategory.Control.GetChars();
			Assert.AreEqual(@"[\p{Lu}\p{Ll}\p{Cc}]", set.ToString());
			set += "123";
			Assert.AreEqual(@"[1-3\p{Lu}\p{Ll}\p{Cc}]", set.ToString());
			set += UnicodeCategory.DecimalDigitNumber.GetChars();
			Assert.AreEqual(@"[\p{Lu}\p{Ll}\p{Nd}\p{Cc}]", set.ToString());
			set -= "abcdefhjk";
			Assert.AreEqual(@"[\p{Lu}\p{Ll}\p{Nd}\p{Cc}-[a-fhjk]]", set.ToString());
			set += new CharSet() { { '\0', char.MaxValue } };
			Assert.AreEqual(@"[\0-\uFFFF]", set.ToString());
			set -= new CharSet() { { '\u03AA', '\u03AD' } };
			Assert.AreEqual(@"[\0-Ωή-\uFFFF]", set.ToString());
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyCharSet.RangeCount"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestRangeCount()
		{
			ReadOnlyCharSet testSet = new();
			Assert.AreEqual(0, testSet.RangeCount());
			testSet += new CharSet() { { '\0', char.MaxValue } };
			Assert.AreEqual(1, testSet.RangeCount());

			StringBuilder text = new();
			for (int i = 0x40; i < 0x80; i++)
			{
				text.Append((char)i);
			}
			string str40 = text.ToString();
			List<string> testStrs = new() { "\0", "\x01", "\x02", "\x04", "\x05", "\x3E", "\x3F", str40 };
			testStrs.Sort();

			int count = testStrs.Count;
			while (testStrs.NextPermutation())
			{
				ReadOnlyCharSet set = new();
				for (int i = 0; i < count; i++)
				{
					set+= testStrs[i];
					Assert.AreEqual(set.Ranges().Count(), set.RangeCount(), set.ToString());
				}
			}

			testStrs = new List<string>() { "\x3E", "\x3F", str40, "\x80", "\x81", "\x13C", "\x13D" };
			testStrs.Sort();

			count = testStrs.Count;
			while (testStrs.NextPermutation())
			{
				ReadOnlyCharSet set = new();
				for (int i = 0; i < count; i++)
				{
					set += testStrs[i];
					Assert.AreEqual(set.Ranges().Count(), set.RangeCount(), set.ToString());
				}
			}
		}
	}
}
