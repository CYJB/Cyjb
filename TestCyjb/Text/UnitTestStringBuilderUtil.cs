using System;
using System.Collections.Generic;
using System.Text;
using Cyjb.Test;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="StringBuilderUtil"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestStringBuilderUtil
{
	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, char)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_1()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf('a'));
		Assert.AreEqual(-1, text.IndexOf('b'));
		Assert.AreEqual(-1, text.IndexOf('c'));

		text.Append('a');
		Assert.AreEqual(0, text.IndexOf('a'));
		Assert.AreEqual(-1, text.IndexOf('b'));
		Assert.AreEqual(-1, text.IndexOf('c'));

		text.Append('b');
		Assert.AreEqual(0, text.IndexOf('a'));
		Assert.AreEqual(1, text.IndexOf('b'));
		Assert.AreEqual(-1, text.IndexOf('c'));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, char, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_2()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf('a', 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', -1));

		text.Append('a');
		Assert.AreEqual(0, text.IndexOf('a', 0));
		Assert.AreEqual(-1, text.IndexOf('a', 1));
		Assert.AreEqual(-1, text.IndexOf('b', 0));
		Assert.AreEqual(-1, text.IndexOf('b', 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 2));

		text.Append('b');
		Assert.AreEqual(0, text.IndexOf('a', 0));
		Assert.AreEqual(-1, text.IndexOf('a', 1));
		Assert.AreEqual(-1, text.IndexOf('a', 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 3));
		Assert.AreEqual(1, text.IndexOf('b', 0));
		Assert.AreEqual(1, text.IndexOf('b', 1));
		Assert.AreEqual(-1, text.IndexOf('b', 2));
		Assert.AreEqual(-1, text.IndexOf('c', 0));
		Assert.AreEqual(-1, text.IndexOf('c', 1));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, char, int, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_3()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf('a', 0, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 1, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', -1, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 0, -1));

		text.Append('a');
		Assert.AreEqual(-1, text.IndexOf('a', 0, 0));
		Assert.AreEqual(0, text.IndexOf('a', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 0, 2));
		Assert.AreEqual(-1, text.IndexOf('a', 1, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 1, 1));
		Assert.AreEqual(-1, text.IndexOf('b', 0, 0));
		Assert.AreEqual(-1, text.IndexOf('b', 0, 1));

		text.Append('b');
		Assert.AreEqual(-1, text.IndexOf('a', 0, 0));
		Assert.AreEqual(0, text.IndexOf('a', 0, 1));
		Assert.AreEqual(0, text.IndexOf('a', 0, 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 0, 3));
		Assert.AreEqual(-1, text.IndexOf('a', 1, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf('a', 1, 2));
		Assert.AreEqual(-1, text.IndexOf('b', 0, 0));
		Assert.AreEqual(-1, text.IndexOf('b', 0, 1));
		Assert.AreEqual(1, text.IndexOf('b', 0, 2));
		Assert.AreEqual(-1, text.IndexOf('b', 1, 0));
		Assert.AreEqual(1, text.IndexOf('b', 1, 1));
		Assert.AreEqual(-1, text.IndexOf('c', 0, 0));
		Assert.AreEqual(-1, text.IndexOf('c', 0, 1));
		Assert.AreEqual(-1, text.IndexOf('c', 0, 2));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, string)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_4()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf("a"));
		Assert.AreEqual(-1, text.IndexOf("ab"));

		text.Append("abcabd");
		Assert.AreEqual(0, text.IndexOf("a"));
		Assert.AreEqual(1, text.IndexOf("b"));
		Assert.AreEqual(2, text.IndexOf("c"));
		Assert.AreEqual(5, text.IndexOf("d"));
		Assert.AreEqual(-1, text.IndexOf("e"));

		Assert.AreEqual(0, text.IndexOf("ab"));
		Assert.AreEqual(1, text.IndexOf("bc"));
		Assert.AreEqual(2, text.IndexOf("ca"));
		Assert.AreEqual(4, text.IndexOf("bd"));
		Assert.AreEqual(-1, text.IndexOf("de"));

		Assert.AreEqual(0, text.IndexOf("abc"));
		Assert.AreEqual(1, text.IndexOf("bcab"));
		Assert.AreEqual(3, text.IndexOf("abd"));
		Assert.AreEqual(-1, text.IndexOf("abda"));
		Assert.AreEqual(-1, text.IndexOf("abdaaliwh"));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, string, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_5()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf("a", 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("a", 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("a", -1));

		text.Append("abcabd");
		Assert.AreEqual(0, text.IndexOf("a", 0));
		Assert.AreEqual(3, text.IndexOf("a", 1));
		Assert.AreEqual(-1, text.IndexOf("a", 4));
		Assert.AreEqual(1, text.IndexOf("b", 0));
		Assert.AreEqual(1, text.IndexOf("b", 1));
		Assert.AreEqual(4, text.IndexOf("b", 2));
		Assert.AreEqual(-1, text.IndexOf("b", 5));
		Assert.AreEqual(2, text.IndexOf("c", 0));

		Assert.AreEqual(0, text.IndexOf("ab", 0));
		Assert.AreEqual(3, text.IndexOf("ab", 1));
		Assert.AreEqual(3, text.IndexOf("ab", 2));
		Assert.AreEqual(3, text.IndexOf("ab", 3));
		Assert.AreEqual(-1, text.IndexOf("ab", 4));
		Assert.AreEqual(1, text.IndexOf("bc", 1));
		Assert.AreEqual(-1, text.IndexOf("bc", 2));
		Assert.AreEqual(2, text.IndexOf("ca", 2));
		Assert.AreEqual(4, text.IndexOf("bd", 3));
		Assert.AreEqual(-1, text.IndexOf("de", 2));

		Assert.AreEqual(0, text.IndexOf("abc", 0));
		Assert.AreEqual(-1, text.IndexOf("abc", 1));
		Assert.AreEqual(1, text.IndexOf("bcab", 0));
		Assert.AreEqual(1, text.IndexOf("bcab", 1));
		Assert.AreEqual(-1, text.IndexOf("bcab", 2));
		Assert.AreEqual(3, text.IndexOf("abd", 0));
		Assert.AreEqual(-1, text.IndexOf("abda", 0));
		Assert.AreEqual(-1, text.IndexOf("abdaaliwh", 0));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, string, int, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf_6()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.IndexOf("a", 0, 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 0, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("a", 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("a", -1, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("a", 0, -1));

		text.Append("abcabd");
		Assert.AreEqual(0, text.IndexOf("a", 0, 1));
		Assert.AreEqual(0, text.IndexOf("a", 0, 2));
		Assert.AreEqual(-1, text.IndexOf("a", 3, 0));
		Assert.AreEqual(3, text.IndexOf("a", 3, 1));
		Assert.AreEqual(3, text.IndexOf("a", 3, 2));
		Assert.AreEqual(-1, text.IndexOf("a", 4, 1));
		Assert.AreEqual(-1, text.IndexOf("b", 0, 1));
		Assert.AreEqual(1, text.IndexOf("b", 1, 1));
		Assert.AreEqual(-1, text.IndexOf("b", 2, 1));
		Assert.AreEqual(-1, text.IndexOf("b", 2, 2));
		Assert.AreEqual(4, text.IndexOf("b", 2, 3));
		Assert.AreEqual(-1, text.IndexOf("b", 5, 1));
		Assert.AreEqual(2, text.IndexOf("c", 0, 4));

		Assert.AreEqual(-1, text.IndexOf("ab", 0, 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 0, 1));
		Assert.AreEqual(0, text.IndexOf("ab", 0, 2));
		Assert.AreEqual(0, text.IndexOf("ab", 0, 3));
		Assert.AreEqual(-1, text.IndexOf("ab", 1, 1));
		Assert.AreEqual(-1, text.IndexOf("ab", 1, 2));
		Assert.AreEqual(-1, text.IndexOf("ab", 1, 3));
		Assert.AreEqual(3, text.IndexOf("ab", 1, 4));
		Assert.AreEqual(-1, text.IndexOf("ab", 2, 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 2, 1));
		Assert.AreEqual(-1, text.IndexOf("ab", 2, 2));
		Assert.AreEqual(3, text.IndexOf("ab", 2, 3));
		Assert.AreEqual(3, text.IndexOf("ab", 2, 4));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("ab", 2, 5));
		Assert.AreEqual(-1, text.IndexOf("ab", 3, 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 3, 1));
		Assert.AreEqual(3, text.IndexOf("ab", 3, 2));
		Assert.AreEqual(3, text.IndexOf("ab", 3, 3));
		Assert.AreEqual(-1, text.IndexOf("ab", 4, 0));
		Assert.AreEqual(-1, text.IndexOf("ab", 4, 1));
		Assert.AreEqual(-1, text.IndexOf("ab", 4, 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.IndexOf("ab", 4, 3));
		Assert.AreEqual(1, text.IndexOf("bc", 1, 2));
		Assert.AreEqual(-1, text.IndexOf("bc", 2, 3));
		Assert.AreEqual(-1, text.IndexOf("ca", 2, 1));
		Assert.AreEqual(2, text.IndexOf("ca", 2, 2));
		Assert.AreEqual(2, text.IndexOf("ca", 2, 3));
		Assert.AreEqual(-1, text.IndexOf("bd", 3, 2));
		Assert.AreEqual(4, text.IndexOf("bd", 3, 3));
		Assert.AreEqual(4, text.IndexOf("bd", 4, 2));
		Assert.AreEqual(-1, text.IndexOf("de", 2, 3));

		Assert.AreEqual(0, text.IndexOf("abc", 0, 4));
		Assert.AreEqual(-1, text.IndexOf("abc", 1, 3));
		Assert.AreEqual(-1, text.IndexOf("bcab", 0, 3));
		Assert.AreEqual(-1, text.IndexOf("bcab", 0, 4));
		Assert.AreEqual(1, text.IndexOf("bcab", 0, 5));
		Assert.AreEqual(1, text.IndexOf("bcab", 1, 4));
		Assert.AreEqual(1, text.IndexOf("bcab", 1, 5));
		Assert.AreEqual(-1, text.IndexOf("bcab", 2, 4));
		Assert.AreEqual(-1, text.IndexOf("abd", 0, 3));
		Assert.AreEqual(3, text.IndexOf("abd", 2, 4));
		Assert.AreEqual(-1, text.IndexOf("abda", 0, 3));
		Assert.AreEqual(-1, text.IndexOf("abdaaliwh", 0, 3));

		text = new StringBuilder("dcbddbdbecbaceaecddbaeaecabde");
		Assert.AreEqual(9, text.IndexOf("cbac", 0, 13));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf(StringBuilder, string, int, int)"/> 方法进行随机测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOfRandomString()
	{
		Random random = Random.Shared;
		for (int i = 0; i < 100; i++)
		{
			// 随机字符串，长度 [10, 50]。
			StringBuilder builder = new();
			for (int j = random.Next(10, 50); j >= 0; j--)
			{
				builder.Append((char)random.Next('a', 'f'));
			}
			string text = builder.ToString();
			// 随机模式串，长度 [2, 5]。
			StringBuilder temp = new();
			for (int j = random.Next(2, 5); j >= 0; j--)
			{
				temp.Append((char)random.Next('a', 'f'));
			}
			string pattern = temp.ToString();
			for (int j = 0; j < text.Length; j++)
			{
				int end = text.Length - j;
				for (int k = 0; k <= end; k++)
				{
					Assert.AreEqual(text.IndexOf(pattern, j, k, StringComparison.Ordinal),
						builder.IndexOf(pattern, j, k),
						$"\"{text}\".IndexOf(\"{pattern}\", {j}, {k})");
				}
			}
		}
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, char)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_1()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf('a'));
		Assert.AreEqual(-1, text.LastIndexOf('b'));
		Assert.AreEqual(-1, text.LastIndexOf('c'));

		text.Append('a');
		Assert.AreEqual(0, text.LastIndexOf('a'));
		Assert.AreEqual(-1, text.LastIndexOf('b'));
		Assert.AreEqual(-1, text.LastIndexOf('c'));

		text.Append('b');
		Assert.AreEqual(0, text.LastIndexOf('a'));
		Assert.AreEqual(1, text.LastIndexOf('b'));
		Assert.AreEqual(-1, text.LastIndexOf('c'));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, char, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_2()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf('a', 0));
		Assert.AreEqual(-1, text.LastIndexOf('a', -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', -2));

		text.Append('a');
		Assert.AreEqual(0, text.LastIndexOf('a', 0));
		Assert.AreEqual(0, text.LastIndexOf('a', 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 2));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0));
		Assert.AreEqual(-1, text.LastIndexOf('b', 1));

		text.Append('b');
		Assert.AreEqual(0, text.LastIndexOf('a', 0));
		Assert.AreEqual(0, text.LastIndexOf('a', 1));
		Assert.AreEqual(0, text.LastIndexOf('a', 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 3));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0));
		Assert.AreEqual(1, text.LastIndexOf('b', 1));
		Assert.AreEqual(1, text.LastIndexOf('b', 2));
		Assert.AreEqual(-1, text.LastIndexOf('c', 0));
		Assert.AreEqual(-1, text.LastIndexOf('c', 1));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, char, int, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_3()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf('a', 0, 0));
		Assert.AreEqual(-1, text.LastIndexOf('a', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 1, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 0, 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', -1, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 0, -1));

		text.Append('a');
		Assert.AreEqual(-1, text.LastIndexOf('a', 0, 0));
		Assert.AreEqual(0, text.LastIndexOf('a', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 0, 2));
		Assert.AreEqual(-1, text.LastIndexOf('a', 1, 0));
		Assert.AreEqual(-1, text.LastIndexOf('a', 1, 1));
		Assert.AreEqual(0, text.LastIndexOf('a', 1, 2));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0, 0));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0, 1));

		text.Append('b');
		Assert.AreEqual(-1, text.LastIndexOf('a', 0, 0));
		Assert.AreEqual(0, text.LastIndexOf('a', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('a', 0, 2));
		Assert.AreEqual(-1, text.LastIndexOf('a', 1, 1));
		Assert.AreEqual(0, text.LastIndexOf('a', 1, 2));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0, 0));
		Assert.AreEqual(-1, text.LastIndexOf('b', 0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf('b', 0, 2));
		Assert.AreEqual(-1, text.LastIndexOf('b', 1, 0));
		Assert.AreEqual(1, text.LastIndexOf('b', 1, 1));
		Assert.AreEqual(-1, text.LastIndexOf('c', 0, 0));
		Assert.AreEqual(-1, text.LastIndexOf('c', 0, 1));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, string)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_4()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf("a"));
		Assert.AreEqual(-1, text.LastIndexOf("ab"));

		text.Append("abcabd");
		Assert.AreEqual(3, text.LastIndexOf("a"));
		Assert.AreEqual(4, text.LastIndexOf("b"));
		Assert.AreEqual(2, text.LastIndexOf("c"));
		Assert.AreEqual(5, text.LastIndexOf("d"));
		Assert.AreEqual(-1, text.LastIndexOf("e"));

		Assert.AreEqual(3, text.LastIndexOf("ab"));
		Assert.AreEqual(1, text.LastIndexOf("bc"));
		Assert.AreEqual(2, text.LastIndexOf("ca"));
		Assert.AreEqual(4, text.LastIndexOf("bd"));
		Assert.AreEqual(-1, text.LastIndexOf("de"));

		Assert.AreEqual(0, text.LastIndexOf("abc"));
		Assert.AreEqual(1, text.LastIndexOf("bcab"));
		Assert.AreEqual(3, text.LastIndexOf("abd"));
		Assert.AreEqual(-1, text.LastIndexOf("abda"));
		Assert.AreEqual(-1, text.LastIndexOf("abdaaliwh"));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, string, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_5()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf("a", 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", -2));

		text.Append("abcabd");
		Assert.AreEqual(0, text.LastIndexOf("a", 0));
		Assert.AreEqual(0, text.LastIndexOf("a", 1));
		Assert.AreEqual(3, text.LastIndexOf("a", 4));
		Assert.AreEqual(-1, text.LastIndexOf("b", 0));
		Assert.AreEqual(1, text.LastIndexOf("b", 1));
		Assert.AreEqual(1, text.LastIndexOf("b", 2));
		Assert.AreEqual(4, text.LastIndexOf("b", 5));
		Assert.AreEqual(-1, text.LastIndexOf("c", 0));
		Assert.AreEqual(2, text.LastIndexOf("c", 2));

		Assert.AreEqual(-1, text.LastIndexOf("ab", 0));
		Assert.AreEqual(0, text.LastIndexOf("ab", 1));
		Assert.AreEqual(0, text.LastIndexOf("ab", 2));
		Assert.AreEqual(0, text.LastIndexOf("ab", 3));
		Assert.AreEqual(3, text.LastIndexOf("ab", 4));
		Assert.AreEqual(-1, text.LastIndexOf("bc", 1));
		Assert.AreEqual(1, text.LastIndexOf("bc", 2));
		Assert.AreEqual(-1, text.LastIndexOf("ca", 2));
		Assert.AreEqual(2, text.LastIndexOf("ca", 3));
		Assert.AreEqual(2, text.LastIndexOf("ca", 4));
		Assert.AreEqual(4, text.LastIndexOf("bd", 5));
		Assert.AreEqual(4, text.LastIndexOf("bd", 6));
		Assert.AreEqual(-1, text.LastIndexOf("de", 2));

		Assert.AreEqual(-1, text.LastIndexOf("abc", 0));
		Assert.AreEqual(-1, text.LastIndexOf("abc", 1));
		Assert.AreEqual(0, text.LastIndexOf("abc", 2));
		Assert.AreEqual(1, text.LastIndexOf("bcab", 5));
		Assert.AreEqual(1, text.LastIndexOf("bcab", 4));
		Assert.AreEqual(-1, text.LastIndexOf("bcab", 3));
		Assert.AreEqual(3, text.LastIndexOf("abd", 5));
		Assert.AreEqual(-1, text.LastIndexOf("abda", 5));
		Assert.AreEqual(-1, text.LastIndexOf("abdaaliwh", 5));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, string, int, int)"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf_6()
	{
		StringBuilder text = new();

		Assert.AreEqual(-1, text.LastIndexOf("a", 0, 0));
		Assert.AreEqual(-1, text.LastIndexOf("a", 0, 1));
		Assert.AreEqual(-1, text.LastIndexOf("a", -1, 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 0, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", 0, 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", 0, -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", -1, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("a", -2, 0));

		text.Append("abcabd");
		Assert.AreEqual(-1, text.LastIndexOf("a", 6, 1));
		Assert.AreEqual(-1, text.LastIndexOf("a", 6, 2));
		Assert.AreEqual(-1, text.LastIndexOf("a", 6, 3));
		Assert.AreEqual(3, text.LastIndexOf("a", 6, 4));
		Assert.AreEqual(-1, text.LastIndexOf("a", 3, 0));
		Assert.AreEqual(3, text.LastIndexOf("a", 3, 1));
		Assert.AreEqual(3, text.LastIndexOf("a", 3, 2));
		Assert.AreEqual(-1, text.LastIndexOf("a", 2, 1));
		Assert.AreEqual(-1, text.LastIndexOf("b", 6, 1));
		Assert.AreEqual(-1, text.LastIndexOf("b", 5, 1));
		Assert.AreEqual(4, text.LastIndexOf("b", 5, 2));
		Assert.AreEqual(-1, text.LastIndexOf("b", 4, 0));
		Assert.AreEqual(4, text.LastIndexOf("b", 4, 1));
		Assert.AreEqual(4, text.LastIndexOf("b", 4, 2));
		Assert.AreEqual(4, text.LastIndexOf("b", 4, 3));
		Assert.AreEqual(-1, text.LastIndexOf("b", 2, 1));
		Assert.AreEqual(1, text.LastIndexOf("b", 1, 1));
		Assert.AreEqual(-1, text.LastIndexOf("c", 6, 4));
		Assert.AreEqual(2, text.LastIndexOf("c", 6, 5));

		Assert.AreEqual(-1, text.LastIndexOf("ab", 6, 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 6, 1));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 6, 2));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 6, 3));
		Assert.AreEqual(3, text.LastIndexOf("ab", 6, 4));
		Assert.AreEqual(3, text.LastIndexOf("ab", 6, 5));
		Assert.AreEqual(3, text.LastIndexOf("ab", 6, 6));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 5, 1));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 5, 2));
		Assert.AreEqual(3, text.LastIndexOf("ab", 5, 3));
		Assert.AreEqual(3, text.LastIndexOf("ab", 5, 4));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 4, 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 4, 1));
		Assert.AreEqual(3, text.LastIndexOf("ab", 4, 2));
		Assert.AreEqual(3, text.LastIndexOf("ab", 4, 3));
		Assert.AreEqual(3, text.LastIndexOf("ab", 4, 4));
		Assert.AreEqual(3, text.LastIndexOf("ab", 4, 5));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("ab", 4, 6));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 3, 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 3, 1));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 3, 2));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 3, 3));
		Assert.AreEqual(0, text.LastIndexOf("ab", 3, 4));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 2, 0));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 2, 1));
		Assert.AreEqual(-1, text.LastIndexOf("ab", 2, 2));
		Assert.AreEqual(0, text.LastIndexOf("ab", 2, 3));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => text.LastIndexOf("ab", 2, 4));
		Assert.AreEqual(-1, text.LastIndexOf("bc", 5, 2));
		Assert.AreEqual(-1, text.LastIndexOf("bc", 4, 3));
		Assert.AreEqual(1, text.LastIndexOf("bc", 2, 2));
		Assert.AreEqual(-1, text.LastIndexOf("ca", 4, 1));
		Assert.AreEqual(-1, text.LastIndexOf("ca", 4, 2));
		Assert.AreEqual(2, text.LastIndexOf("ca", 4, 3));
		Assert.AreEqual(-1, text.LastIndexOf("bd", 5, 1));
		Assert.AreEqual(4, text.LastIndexOf("bd", 5, 2));
		Assert.AreEqual(4, text.LastIndexOf("bd", 5, 3));
		Assert.AreEqual(-1, text.LastIndexOf("bd", 4, 3));
		Assert.AreEqual(-1, text.LastIndexOf("bd", 3, 3));
		Assert.AreEqual(-1, text.LastIndexOf("bd", 2, 2));
		Assert.AreEqual(-1, text.LastIndexOf("de", 4, 3));

		Assert.AreEqual(-1, text.LastIndexOf("abc", 6, 4));
		Assert.AreEqual(-1, text.LastIndexOf("abc", 6, 6));
		Assert.AreEqual(0, text.LastIndexOf("abc", 6, 7));
		Assert.AreEqual(-1, text.LastIndexOf("abc", 5, 5));
		Assert.AreEqual(0, text.LastIndexOf("abc", 5, 6));
		Assert.AreEqual(1, text.LastIndexOf("bcab", 6, 6));
		Assert.AreEqual(-1, text.LastIndexOf("bcab", 6, 5));
		Assert.AreEqual(1, text.LastIndexOf("bcab", 5, 5));
		Assert.AreEqual(-1, text.LastIndexOf("bcab", 5, 4));
		Assert.AreEqual(1, text.LastIndexOf("bcab", 4, 4));
		Assert.AreEqual(-1, text.LastIndexOf("abd", 6, 3));
		Assert.AreEqual(3, text.LastIndexOf("abd", 6, 4));
		Assert.AreEqual(-1, text.LastIndexOf("abda", 6, 3));
		Assert.AreEqual(-1, text.LastIndexOf("abdaaliwh", 6, 3));

		text = new StringBuilder("dcbddbdbecbaceaecddbaeaecabde");
		Assert.AreEqual(9, text.LastIndexOf("cbac", 17, 13));
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.LastIndexOf(StringBuilder, string, int, int)"/> 方法进行随机测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOfRandomString()
	{
		Random random = Random.Shared;
		for (int i = 0; i < 100; i++)
		{
			// 随机字符串，长度 [10, 50]。
			StringBuilder builder = new();
			for (int j = random.Next(10, 50); j >= 0; j--)
			{
				builder.Append((char)random.Next('a', 'f'));
			}
			string text = builder.ToString();
			// 随机模式串，长度 [2, 5]。
			StringBuilder temp = new();
			for (int j = random.Next(2, 5); j >= 0; j--)
			{
				temp.Append((char)random.Next('a', 'f'));
			}
			string pattern = temp.ToString();
			for (int j = 0; j <= text.Length; j++)
			{
				for (int k = 0; k <= j + 1; k++)
				{
					Assert.AreEqual(text.LastIndexOf(pattern, j, k, StringComparison.Ordinal),
						builder.LastIndexOf(pattern, j, k),
						$"\"{text}\".LastIndexOf(\"{pattern}\", {j}, {k})");
				}
			}
		}
	}

	/// <summary>
	/// 对 <see cref="StringBuilderUtil.IndexOf"/> 的性能进行测试。
	/// </summary>
	[TestMethod]
	public void TestPerformance()
	{
		Random random = Random.Shared;
		List<StringBuilder> builders = new();
		List<string> texts = new();
		List<string> patterns = new();
		for (int i = 0; i < 100; i++)
		{
			// 随机字符串，长度 [10, 50]。
			StringBuilder builder = new();
			for (int j = random.Next(10, 50); j >= 0; j--)
			{
				builder.Append((char)random.Next('a', 'f'));
			}
			builders.Add(builder);
			texts.Add(builder.ToString());
			// 随机模式串，长度 [2, 5]。
			StringBuilder temp = new();
			for (int j = random.Next(2, 5); j >= 0; j--)
			{
				temp.Append((char)random.Next('a', 'f'));
			}
			patterns.Add(temp.ToString());
		}

		Performance.Measure("string.IndexOf", 10, () =>
		{
			for (int i = 0; i < 100; i++)
			{
				string text = texts[i];
				string pattern = patterns[i];
				for (int j = 0; j < text.Length; j++)
				{
					int end = text.Length - j;
					for (int k = 0; k <= end; k++)
					{
						text.IndexOf(pattern, j, k, StringComparison.Ordinal);
					}
				}
			}
		}).Print();
		Performance.Measure("StringBuilder.IndexOf", 10, () =>
		{
			for (int i = 0; i < 100; i++)
			{
				StringBuilder builder = builders[i];
				string pattern = patterns[i];
				for (int j = 0; j < builder.Length; j++)
				{
					int end = builder.Length - j;
					for (int k = 0; k <= end; k++)
					{
						builder.IndexOf(pattern, j, k);
					}
				}
			}
		}).Print();
	}
}
