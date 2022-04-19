using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="CharSet"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCharSet
	{
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
			Assert.AreEqual("[\0a-ce]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('d', 'g'));
			expected.UnionWith("dfg");
			Assert.AreEqual("[\0a-g]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Remove('d', 'f'));
			expected.ExceptWith("def");
			Assert.AreEqual("[\0a-cg]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('h', 'l'));
			expected.UnionWith("hijkl");
			Assert.AreEqual("[\0a-cg-l]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add('o', 'q'));
			expected.UnionWith("opq");
			Assert.AreEqual("[\0a-cg-lo-q]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Remove('k', 'p'));
			expected.ExceptWith("klmnop");
			Assert.AreEqual("[\0a-cg-jq]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.SetEquals(expected));
			Assert.IsTrue(set.IsSubsetOf(expected));
			Assert.IsTrue(set.IsSupersetOf(expected));
			Assert.IsFalse(set.IsProperSubsetOf(expected));
			Assert.IsFalse(set.IsProperSupersetOf(expected));

			Assert.IsFalse(set.Remove('d', 'f'));
			Assert.AreEqual("[\0a-cg-jq]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Add(char.MaxValue));
			expected.Add(char.MaxValue);
			Assert.AreEqual("[\0a-cg-jq\uFFFF]", set.ToString());
			Assert.AreEqual(expected.Count, set.Count);
			Assert.IsTrue(expected.SetEquals(set));

			Assert.IsTrue(set.Remove('\0', char.MaxValue));
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
		/// 对 <see cref="CharSet.ReadOnlyClone"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestReadOnlyClone()
		{
			CharSet set = new();
			ReadOnlyCharSet set1 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('a', 'c'));
			ReadOnlyCharSet set2 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('e'));
			ReadOnlyCharSet set3 = set.ReadOnlyClone();
			Assert.IsFalse(set.Add('b', 'c'));
			ReadOnlyCharSet set4 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('\0'));
			ReadOnlyCharSet set5 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('d', 'g'));
			ReadOnlyCharSet set6 = set.ReadOnlyClone();
			Assert.IsTrue(set.Remove('d', 'f'));
			ReadOnlyCharSet set7 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('h', 'l'));
			ReadOnlyCharSet set8 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add('o', 'q'));
			ReadOnlyCharSet set9 = set.ReadOnlyClone();
			Assert.IsTrue(set.Remove('k', 'p'));
			ReadOnlyCharSet set10 = set.ReadOnlyClone();
			Assert.IsFalse(set.Remove('d', 'f'));
			ReadOnlyCharSet set11 = set.ReadOnlyClone();
			Assert.IsTrue(set.Add(char.MaxValue));
			ReadOnlyCharSet set12 = set.ReadOnlyClone();
			Assert.IsTrue(set.Remove('\0', char.MaxValue));
			ReadOnlyCharSet set13 = set.ReadOnlyClone();

			HashSet<char> expected = new();
			Assert.AreEqual(0, set1.Count);
			Assert.AreEqual("[]", set1.ToString());
			Assert.IsTrue(expected.SetEquals(set));

			expected.UnionWith("abc");
			Assert.AreEqual("[a-c]", set2.ToString());
			Assert.AreEqual(expected.Count, set2.Count);
			Assert.IsTrue(expected.SetEquals(set2));

			expected.UnionWith("e");
			Assert.AreEqual("[a-ce]", set3.ToString());
			Assert.AreEqual(expected.Count, set3.Count);
			Assert.IsTrue(expected.SetEquals(set3));

			Assert.AreEqual("[a-ce]", set4.ToString());
			Assert.AreEqual(expected.Count, set4.Count);
			Assert.IsTrue(expected.SetEquals(set4));
			Assert.IsTrue(set3.Equals(set4));

			Assert.IsTrue(set4.SetEquals(expected));
			Assert.IsTrue(set4.IsSubsetOf(expected));
			Assert.IsTrue(set4.IsSupersetOf(expected));
			Assert.IsFalse(set4.IsProperSubsetOf(expected));
			Assert.IsFalse(set4.IsProperSupersetOf(expected));

			expected.UnionWith("\0");
			Assert.AreEqual("[\0a-ce]", set5.ToString());
			Assert.AreEqual(expected.Count, set5.Count);
			Assert.IsTrue(expected.SetEquals(set5));

			expected.UnionWith("dfg");
			Assert.AreEqual("[\0a-g]", set6.ToString());
			Assert.AreEqual(expected.Count, set6.Count);
			Assert.IsTrue(expected.SetEquals(set6));

			expected.ExceptWith("def");
			Assert.AreEqual("[\0a-cg]", set7.ToString());
			Assert.AreEqual(expected.Count, set7.Count);
			Assert.IsTrue(expected.SetEquals(set7));

			expected.UnionWith("hijkl");
			Assert.AreEqual("[\0a-cg-l]", set8.ToString());
			Assert.AreEqual(expected.Count, set8.Count);
			Assert.IsTrue(expected.SetEquals(set8));

			expected.UnionWith("opq");
			Assert.AreEqual("[\0a-cg-lo-q]", set9.ToString());
			Assert.AreEqual(expected.Count, set9.Count);
			Assert.IsTrue(expected.SetEquals(set9));

			expected.ExceptWith("klmnop");
			Assert.AreEqual("[\0a-cg-jq]", set10.ToString());
			Assert.AreEqual(expected.Count, set10.Count);
			Assert.IsTrue(expected.SetEquals(set10));

			Assert.IsTrue(set10.SetEquals(expected));
			Assert.IsTrue(set10.IsSubsetOf(expected));
			Assert.IsTrue(set10.IsSupersetOf(expected));
			Assert.IsFalse(set10.IsProperSubsetOf(expected));
			Assert.IsFalse(set10.IsProperSupersetOf(expected));

			Assert.AreEqual("[\0a-cg-jq]", set11.ToString());
			Assert.AreEqual(expected.Count, set11.Count);
			Assert.IsTrue(expected.SetEquals(set11));

			expected.Add(char.MaxValue);
			Assert.AreEqual("[\0a-cg-jq\uFFFF]", set12.ToString());
			Assert.AreEqual(expected.Count, set12.Count);
			Assert.IsTrue(expected.SetEquals(set12));

			expected.Clear();
			Assert.AreEqual("[]", set13.ToString());
			Assert.AreEqual(expected.Count, set13.Count);
			Assert.IsTrue(expected.SetEquals(set13));
		}

		/// <summary>
		/// 对 <see cref="CharSet.ExceptWith"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", "[]", 0)]
		[DataRow("", "abc", "[]", 0)]
		[DataRow("abc", "", "[a-c]", 3)]
		[DataRow("abc", "a", "[bc]", 2)]
		[DataRow("abc", "ac", "[b]", 1)]
		[DataRow("abc", "abc", "[]", 0)]
		[DataRow("acdgkz04", "befhijsxy29", "[04acdgkz]", 8)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", "[04acdgkz]", 8)]
		public void TestExceptWith(string initial, string other, string expected, int expectedCount)
		{
			CharSet set = new(initial);
			set.ExceptWith(other);
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);

			set = new(initial);
			set.ExceptWith(new CharSet(other));
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);
		}

		/// <summary>
		/// 对 <see cref="CharSet.IntersectWith"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", "[]", 0)]
		[DataRow("", "abc", "[]", 0)]
		[DataRow("abc", "", "[]", 0)]
		[DataRow("abc", "a", "[a]", 1)]
		[DataRow("abc", "ac", "[ac]", 2)]
		[DataRow("abc", "abc", "[a-c]", 3)]
		[DataRow("acdgkz04", "befhijsxy29", "[]", 0)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", "[29befh-jsxy]", 11)]
		[DataRow("abcdefghjkz", "bcdghijklmnxyz", "[b-dghjkz]", 8)]
		public void TestIntersectWith(string initial, string other, string expected, int expectedCount)
		{
			CharSet set = new(initial);
			set.IntersectWith(other);
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);

			set = new(initial);
			set.IntersectWith(new CharSet(other));
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);
		}

		/// <summary>
		/// 对 <see cref="CharSet.IsProperSubsetOf"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", false)]
		[DataRow("", "abc", true)]
		[DataRow("abc", "", false)]
		[DataRow("abc", "a", false)]
		[DataRow("abc", "ac", false)]
		[DataRow("abc", "abc", false)]
		[DataRow("a", "abc", true)]
		[DataRow("ac", "abc", true)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", false)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", true)]
		public void TestIsProperSubsetOf(string initial, string other, bool expected)
		{
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.IsProperSubsetOf(other));
			Assert.AreEqual(expected, set.IsProperSubsetOf(new CharSet(other)));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsProperSubsetOf(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsProperSubsetOf(new CharSet(other).ReadOnlyClone()));
		}

		/// <summary>
		/// 对 <see cref="CharSet.IsProperSupersetOf"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("", "", false)]
		[DataRow("", "abc", false)]
		[DataRow("abc", "", true)]
		[DataRow("abc", "a", true)]
		[DataRow("abc", "ac", true)]
		[DataRow("abc", "abc", false)]
		[DataRow("a", "abc", false)]
		[DataRow("ac", "abc", false)]
		[DataRow("acdgkz04", "befhijsxy29", false)]
		[DataRow("cdh2esjzb0f9kiaxyg4", "befhijsxy29", true)]
		[DataRow("befhijsxy29", "cdh2esjzb0f9kiaxyg4", false)]
		public void TestIsProperSupersetOf(string initial, string other, bool expected)
		{
			CharSet set = new(initial);
			Assert.AreEqual(expected, set.IsProperSupersetOf(other));
			Assert.AreEqual(expected, set.IsProperSupersetOf(new CharSet(other)));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsProperSupersetOf(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsProperSupersetOf(new CharSet(other).ReadOnlyClone()));
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
			Assert.AreEqual(expected, set.ReadOnlyClone().IsSubsetOf(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsSubsetOf(new CharSet(other).ReadOnlyClone()));
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
			Assert.AreEqual(expected, set.ReadOnlyClone().IsSupersetOf(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().IsSupersetOf(new CharSet(other).ReadOnlyClone()));
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
			Assert.AreEqual(expected, set.ReadOnlyClone().Overlaps(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().Overlaps(new CharSet(other).ReadOnlyClone()));
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
			Assert.AreEqual(expected, set.ReadOnlyClone().SetEquals(other));
			Assert.AreEqual(expected, set.ReadOnlyClone().SetEquals(new CharSet(other).ReadOnlyClone()));
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

			set = new(initial);
			set.UnionWith(new CharSet(other).ReadOnlyClone());
			Assert.AreEqual(expected, set.ToString());
			Assert.AreEqual(expectedCount, set.Count);
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

		/// <summary>
		/// 对 <see cref="CharSet"/> 的添加性能进行测试。
		/// </summary>
		[TestMethod]
		public void TestPerformance()
		{
			List<char> charList = new();
			List<(char start, char end)> smallRangeList = new();
			List<(char start, char end)> largeRangeList = new();
			int loopCount = 100;
			for (int i = 0; i < 10000; i++)
			{
				charList.Add((char)Random.Shared.Next(0, char.MaxValue + 1));
			}
			for (int i = 0; i < 1000; i++)
			{
				int min = Random.Shared.Next(0, char.MaxValue + 1);
				int max = min + Random.Shared.Next(0, 10);
				if (max > char.MaxValue)
				{
					max = char.MaxValue;
				}
				smallRangeList.Add(((char)min, (char)max));

				max = min + Random.Shared.Next(0, 100);
				if (max > char.MaxValue)
				{
					max = char.MaxValue;
				}
				largeRangeList.Add(((char)min, (char)max));
			}

			Stopwatch hashSetCharWatch = new();
			hashSetCharWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				HashSet<char> hashSet = new();
				for (int j = 0; j < charList.Count; j++)
				{
					hashSet.Add(charList[j]);
				}
			}
			hashSetCharWatch.Stop();

			Stopwatch hashSetSmallRangeWatch = new();
			hashSetSmallRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				HashSet<char> hashSet = new();
				for (int j = 0; j < smallRangeList.Count; j++)
				{
					var (start, end) = smallRangeList[j];
					for (int k = start; k <= end; k++)
					{
						hashSet.Add((char)k);
					}
				}
			}
			hashSetSmallRangeWatch.Stop();

			Stopwatch hashSetLargeRangeWatch = new();
			hashSetLargeRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				HashSet<char> hashSet = new();
				for (int j = 0; j < largeRangeList.Count; j++)
				{
					var (start, end) = largeRangeList[j];
					for (int k = start; k <= end; k++)
					{
						hashSet.Add((char)k);
					}
				}
			}
			hashSetLargeRangeWatch.Stop();

			Stopwatch sortedSetCharWatch = new();
			sortedSetCharWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				SortedSet<char> sortedSet = new();
				for (int j = 0; j < charList.Count; j++)
				{
					sortedSet.Add(charList[j]);
				}
			}
			sortedSetCharWatch.Stop();

			Stopwatch sortedSetSmallRangeWatch = new();
			sortedSetSmallRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				SortedSet<char> sortedSet = new();
				for (int j = 0; j < largeRangeList.Count; j++)
				{
					var (start, end) = smallRangeList[j];
					for (int k = start; k <= end; k++)
					{
						sortedSet.Add((char)k);
					}
				}
			}
			sortedSetSmallRangeWatch.Stop();

			Stopwatch sortedSetLargeRangeWatch = new();
			sortedSetLargeRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				SortedSet<char> sortedSet = new();
				for (int j = 0; j < smallRangeList.Count; j++)
				{
					var (start, end) = largeRangeList[j];
					for (int k = start; k <= end; k++)
					{
						sortedSet.Add((char)k);
					}
				}
			}
			sortedSetLargeRangeWatch.Stop();

			Stopwatch charSetCharWatch = new();
			charSetCharWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				CharSet charSet = new();
				for (int j = 0; j < charList.Count; j++)
				{
					charSet.Add(charList[j]);
				}
			}
			charSetCharWatch.Stop();

			Stopwatch charSetSmallRangeWatch = new();
			charSetSmallRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				CharSet charSet = new();
				for (int j = 0; j < smallRangeList.Count; j++)
				{
					var (start, end) = smallRangeList[j];
					for (int k = start; k <= end; k++)
					{
						charSet.Add((char)k);
					}
				}
			}
			charSetSmallRangeWatch.Stop();

			Stopwatch charSetLargeRangeWatch = new();
			charSetLargeRangeWatch.Start();
			for (int i = 0; i < loopCount; i++)
			{
				CharSet charSet = new();
				for (int j = 0; j < largeRangeList.Count; j++)
				{
					var (start, end) = largeRangeList[j];
					for (int k = start; k <= end; k++)
					{
						charSet.Add((char)k);
					}
				}
			}
			charSetLargeRangeWatch.Stop();

			Console.WriteLine(@"CharPerformance
Char:
  CharSet {0}
  HashSet {1}
  SortedSet {2}
SmallRange:
  CharSet {3}
  HashSet {4}
  SortedSet {5}
LargeRange:
  CharSet {6}
  HashSet {7}
  SortedSet {8}
",
				 charSetCharWatch.ElapsedMilliseconds,
				 hashSetCharWatch.ElapsedMilliseconds,
				 sortedSetCharWatch.ElapsedMilliseconds,
				 charSetSmallRangeWatch.ElapsedMilliseconds,
				 hashSetSmallRangeWatch.ElapsedMilliseconds,
				 sortedSetSmallRangeWatch.ElapsedMilliseconds,
				 charSetLargeRangeWatch.ElapsedMilliseconds,
				 hashSetLargeRangeWatch.ElapsedMilliseconds,
				 sortedSetLargeRangeWatch.ElapsedMilliseconds);
		}
	}
}
