using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Cyjb;
using Cyjb.Collections;
using Cyjb.Globalization;
using Cyjb.Test.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="CharSet"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public partial class UnitTestCharSet
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
		/// 对 <see cref="CharSet"/> 的 <see cref="ICollection{T}"/> 接口进行测试。
		/// </summary>
		[TestMethod]
		public void TestCollection()
		{
			CharSet set = new();
			CollectionTest.Test(set, CollectionTestType.Sorted | CollectionTestType.Unique,
				"dlgozuqhzlbiuoapzoijkokljtkpuiwoeu2839ythiz7tyb6中文测试86r329rujweguizlob8ya9yh12huihijkl".ToCharArray());
		}

		/// <summary>
		/// 对 <see cref="CharSet"/> 的添加和删除进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddRemove()
		{
			CharSet set = new();
			HashSet<char> expected = new();
			Assert.AreEqual(0, set.Count);
			Assert.AreEqual("[]", set.ToString());
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('a', 'c'));
			expected.UnionWith("abc");
			Assert.AreEqual("[a-c]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('e'));
			expected.UnionWith("e");
			Assert.AreEqual("[a-ce]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsFalse(set.Add('b', 'c'));
			Assert.AreEqual("[a-ce]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.SetEquals(expected));
			Assert.IsTrue(set.IsSubsetOf(expected));
			Assert.IsTrue(set.IsSupersetOf(expected));
			Assert.IsFalse(set.IsProperSubsetOf(expected));
			Assert.IsFalse(set.IsProperSupersetOf(expected));

			Assert.IsTrue(set.Add('\0'));
			expected.UnionWith("\0");
			Assert.AreEqual(@"[\0a-ce]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('d', 'g'));
			expected.UnionWith("dfg");
			Assert.AreEqual(@"[\0a-g]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Remove('d', 'f'));
			expected.ExceptWith("def");
			Assert.AreEqual(@"[\0a-cg]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('h', 'l'));
			expected.UnionWith("hijkl");
			Assert.AreEqual(@"[\0a-cg-l]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('o', 'q'));
			expected.UnionWith("opq");
			Assert.AreEqual(@"[\0a-cg-lo-q]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Remove('k', 'p'));
			expected.ExceptWith("klmnop");
			Assert.AreEqual(@"[\0a-cg-jq]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.SetEquals(expected));
			Assert.IsTrue(set.IsSubsetOf(expected));
			Assert.IsTrue(set.IsSupersetOf(expected));
			Assert.IsFalse(set.IsProperSubsetOf(expected));
			Assert.IsFalse(set.IsProperSupersetOf(expected));

			Assert.IsFalse(set.Remove('d', 'f'));
			Assert.AreEqual(@"[\0a-cg-jq]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add(char.MaxValue));
			expected.Add(char.MaxValue);
			Assert.AreEqual(@"[\0a-cg-jq\uFFFF]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			// 随机添加或删除范围
			for (int i = 0; i < 200; i++)
			{
				string range = Random.Shared.Choose(TestRanges);
				if (Random.Shared.NextBoolean())
				{
					set.Add(range[0], range[^1]);
					expected.AddRange(range);
				}
				else
				{
					set.Remove(range[0], range[^1]);
					expected.ExceptWith(range);
				}
				Assert.AreEqual(expected.Count, set.Count);
				Assert.IsTrue(expected.SetEquals(set));
			}

			set.Remove('\0', char.MaxValue);
			expected.Clear();
			Assert.AreEqual("[]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));
		}

		/// <summary>
		/// 对 <see cref="CharSet.AddIgnoreCase"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddIgnoreCase()
		{
			CharSet set = new();
			HashSet<char> expected = new();

			Assert.IsTrue(set.AddIgnoreCase('a'));
			expected.AddRange("aA");
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.AddIgnoreCase('Z'));
			expected.AddRange("zZ");
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.AddIgnoreCase('0', '3'));
			expected.AddRange("0123");
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.AddIgnoreCase('c', 'f'));
			expected.AddRange("cdefCDEF");
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.AddIgnoreCase('X', 'b'));
			expected.AddRange(@"XYZ[\]^_`abxyzAB");
			Assert.IsTrue(expected.SetEquals(set));
		}

		/// <summary>
		/// 对 <see cref="CharSet"/> 的集合方法进行测试。
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
					CharSet set = new(first);
					HashSet<char> expected = new(first);

					Assert.AreEqual(expected.IsSubsetOf(second), set.IsSubsetOf(second));
					Assert.AreEqual(expected.IsSubsetOf(second), set.IsSubsetOf(new CharSet(second)));

					Assert.AreEqual(expected.IsProperSubsetOf(second), set.IsProperSubsetOf(second));
					Assert.AreEqual(expected.IsProperSubsetOf(second), set.IsProperSubsetOf(new CharSet(second)));

					Assert.AreEqual(expected.IsSupersetOf(second), set.IsSupersetOf(second));
					Assert.AreEqual(expected.IsSupersetOf(second), set.IsSupersetOf(new CharSet(second)));

					Assert.AreEqual(expected.IsProperSupersetOf(second), set.IsProperSupersetOf(second));
					Assert.AreEqual(expected.IsProperSupersetOf(second), set.IsProperSupersetOf(new CharSet(second)));

					Assert.AreEqual(expected.Overlaps(second), set.Overlaps(second));
					Assert.AreEqual(expected.Overlaps(second), set.Overlaps(new CharSet(second)));

					Assert.AreEqual(expected.SetEquals(second), set.SetEquals(second));
					Assert.AreEqual(expected.SetEquals(second), set.SetEquals(new CharSet(second)));

					set = new CharSet(first);
					set.ExceptWith(second);
					expected = new HashSet<char>(first);
					expected.ExceptWith(second);
					Assert.IsTrue(expected.SetEquals(set));
					set = new CharSet(first);
					set.ExceptWith(new CharSet(second));
					Assert.IsTrue(expected.SetEquals(set));

					set = new CharSet(first);
					set.ExceptWith(second);
					expected = new HashSet<char>(first);
					expected.ExceptWith(second);
					Assert.IsTrue(expected.SetEquals(set));
					set = new CharSet(first);
					set.ExceptWith(new CharSet(second));
					Assert.IsTrue(expected.SetEquals(set));

					set = new CharSet(first);
					set.SymmetricExceptWith(second);
					expected = new HashSet<char>(first);
					expected.SymmetricExceptWith(second);
					Assert.IsTrue(expected.SetEquals(set));
					set = new CharSet(first);
					set.SymmetricExceptWith(new CharSet(second));
					Assert.IsTrue(expected.SetEquals(set));

					set = new CharSet(first);
					set.UnionWith(second);
					expected = new HashSet<char>(first);
					expected.UnionWith(second);
					Assert.IsTrue(expected.SetEquals(set));
					set = new CharSet(first);
					set.UnionWith(new CharSet(second));
					Assert.IsTrue(expected.SetEquals(set));
				}
			}
		}

		/// <summary>
		/// 对 <see cref="CharSet.IsSubsetOf"/> 方法进行测试。
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
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.IsSubsetOf(other));
			Assert.AreEqual(expected, set.IsSubsetOf(new CharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="CharSet.IsSupersetOf"/> 方法进行测试。
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
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.IsSupersetOf(other));
			Assert.AreEqual(expected, set.IsSupersetOf(new CharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="CharSet.Overlaps"/> 方法进行测试。
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
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.Overlaps(other));
			Assert.AreEqual(expected, set.Overlaps(new CharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="CharSet.SetEquals"/> 方法进行测试。
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
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.SetEquals(other));
			Assert.AreEqual(expected, set.SetEquals(new CharSet(other)));
		}

		/// <summary>
		/// 对 <see cref="CharSet.SymmetricExceptWith"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", "[]", 0)]
		[DataRow("", "abc", "[a-c]", 3)]
		[DataRow("abc", "", "[a-c]", 3)]
		[DataRow("abc", "a", "[bc]", 2)]
		[DataRow("abc", "ac", "[b]", 1)]
		[DataRow("abc", "abc", "[]", 0)]
		[DataRow("a", "bc", "[a-c]", 3)]
		[DataRow("ab", "bc", "[ac]", 2)]
		[DataRow("abcd", "cdef", "[abef]", 4)]
		[DataRow("acdgkz04", "befhijsxy29", "[0249a-ksx-z]", 19)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", "[04acdgkz]", 8)]
		[DataRow("abcdefghjkz", "bcdghijklmnxyz", "[aefil-nxy]", 9)]
		public void TestSymmetricExceptWith(string initial, string other, string expected, int expectedCount)
		{
			CharSet set = new(initial);
			set.SymmetricExceptWith(other);
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);

			set = new(initial);
			set.SymmetricExceptWith(new CharSet(other));
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);
		}

		/// <summary>
		/// 对 <see cref="CharSet.UnionWith"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", "[]", 0)]
		[DataRow("", "abc", "[a-c]", 3)]
		[DataRow("abc", "", "[a-c]", 3)]
		[DataRow("abc", "a", "[a-c]", 3)]
		[DataRow("abc", "ac", "[a-c]", 3)]
		[DataRow("abc", "abc", "[a-c]", 3)]
		[DataRow("acdgkz04", "befhijsxy29", "[0249a-ksx-z]", 19)]
		public void TestUnionWith(string initial, string other, string expected, int expectedCount)
		{
			CharSet set = new(initial);
			set.UnionWith(other);
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);

			set = new(initial);
			set.UnionWith(new CharSet(other));
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);
		}

		/// <summary>
		/// 对 <see cref="CharSet.Negated"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestNegated()
		{
			CharSet set = new();
			set.Negated();
			HashSet<char> expected = Negated(new HashSet<char>());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			set.Negated();
			expected = Negated(expected);
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			set = new("acdefghiz");
			set.Negated();
			expected = Negated(new HashSet<char>("acdefghiz"));
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			set.Negated();
			expected = Negated(expected);
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));
		}

		private static HashSet<char> Negated(HashSet<char> set)
		{
			HashSet<char> result = new();
			for (int i = 0; i <= char.MaxValue; i++)
			{
				result.Add((char)i);
			}
			result.ExceptWith(set);
			return result;
		}

		/// <summary>
		/// 对 <see cref="CharSet.MarkReadOnly"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestMarkReadOnly()
		{
			CharSet set = new("abc");
			set.MarkReadOnly();
			Assert.IsTrue(set.Contains('b'));
			Assert.ThrowsException<NotSupportedException>(() => set.Add('a'));
			Assert.ThrowsException<NotSupportedException>(() => set.Remove('a'));
			Assert.ThrowsException<NotSupportedException>(() => set.UnionWith("b"));
		}

		/// <summary>
		/// 对 <see cref="CharSet.ToString"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToString()
		{
			CharSet set = new();
			set.UnionWith(UnicodeCategory.UppercaseLetter.GetChars());
			Assert.AreEqual(@"[\p{Lu}]", set.ToString());
			set.UnionWith(UnicodeCategory.Control.GetChars());
			Assert.AreEqual(@"[\p{Lu}\p{Cc}]", set.ToString());
			set.UnionWith("123");
			Assert.AreEqual(@"[1-3\p{Lu}\p{Cc}]", set.ToString());
		}

		/// <summary>
		/// 对 <see cref="CharSet.AddLowercase"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddLowercase()
		{
			Stopwatch generateMappingWatch = new();
			Stopwatch charSetWatch = new();
			Stopwatch hashSetWatch = new();
			CultureInfo[] cultures = new[] {
				CultureInfo.InvariantCulture,
				CultureInfo.CurrentCulture,
			};
			foreach (CultureInfo culture in cultures)
			{
				CharSet warmup = new("0123456789");
				generateMappingWatch.Start();
				warmup.AddLowercase(culture);
				generateMappingWatch.Stop();
				for (int i = 0; i < 10; i++)
				{
					CharSet set = new();
					for (int j = 0; j < char.MaxValue; j++)
					{
						char ch = (char)j;
						if (char.GetUnicodeCategory(ch) == UnicodeCategory.UppercaseLetter)
						{
							if (Random.Shared.NextDouble() > 0.3)
							{
								set.Add(ch);
							}
						}
						else
						{
							if (Random.Shared.NextDouble() > 0.92)
							{
								set.Add(ch);
							}
						}
					}
					HashSet<char> expected = new(set);
					hashSetWatch.Start();
					foreach (char ch in set)
					{
						expected.Add(culture.TextInfo.ToLower(ch));
					}
					hashSetWatch.Stop();
					charSetWatch.Start();
					set.AddLowercase(culture);
					charSetWatch.Stop();
					if (!set.SetEquals(expected))
					{
						HashSet<char> surplusChars = new(set);
						surplusChars.ExceptWith(expected);
						HashSet<char> lackChars = new(expected);
						lackChars.ExceptWith(set);
						Assert.Fail("Set not equals, surplus [{0}], lack [{1}]",
							new string(surplusChars.ToArray()),
							new string(lackChars.ToArray())
						);
					}
					Assert.IsTrue(set.SetEquals(expected));
				}
			}
			Console.WriteLine("Warmup {0}\n CharSet {1}\n HashSet {2}",
				generateMappingWatch.ElapsedMilliseconds,
				charSetWatch.ElapsedMilliseconds,
				hashSetWatch.ElapsedMilliseconds);
		}

		/// <summary>
		/// 对 <see cref="CharSet.AddUppercase"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddUppercase()
		{
			Stopwatch generateMappingWatch = new();
			Stopwatch charSetWatch = new();
			Stopwatch hashSetWatch = new();
			CultureInfo[] cultures = new[] {
				CultureInfo.InvariantCulture,
				CultureInfo.CurrentCulture,
			};
			foreach (CultureInfo culture in cultures)
			{
				CharSet warmup = new("0123456789");
				generateMappingWatch.Start();
				warmup.AddUppercase(culture);
				generateMappingWatch.Stop();
				for (int i = 0; i < 10; i++)
				{
					CharSet set = new();
					for (int j = 0; j < char.MaxValue; j++)
					{
						char ch = (char)j;
						if (char.GetUnicodeCategory(ch) == UnicodeCategory.LowercaseLetter)
						{
							if (Random.Shared.NextDouble() > 0.3)
							{
								set.Add(ch);
							}
						}
						else
						{
							if (Random.Shared.NextDouble() > 0.92)
							{
								set.Add(ch);
							}
						}
					}
					HashSet<char> expected = new(set);
					hashSetWatch.Start();
					foreach (char ch in set)
					{
						expected.Add(culture.TextInfo.ToUpper(ch));
					}
					hashSetWatch.Stop();
					charSetWatch.Start();
					set.AddUppercase(culture);
					charSetWatch.Stop();
					if (!set.SetEquals(expected))
					{
						HashSet<char> surplusChars = new(set);
						surplusChars.ExceptWith(expected);
						HashSet<char> lackChars = new(expected);
						lackChars.ExceptWith(set);
						Assert.Fail("Set not equals, surplus [{0}], lack [{1}]",
							new string(surplusChars.ToArray()),
							new string(lackChars.ToArray())
						);
					}
					Assert.IsTrue(set.SetEquals(expected));
				}
			}
			Console.WriteLine("Warmup {0}\n CharSet {1}\n HashSet {2}",
				generateMappingWatch.ElapsedMilliseconds,
				charSetWatch.ElapsedMilliseconds,
				hashSetWatch.ElapsedMilliseconds);
		}
	}
}
