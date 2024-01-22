using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;

/// <summary>
/// <see cref="ReadOnlyPrefixTree{TValue}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestReadOnlyPrefixTree
{
	/// <summary>
	/// 测试空的前缀树。
	/// </summary>
	[TestMethod]
	public void TestEmptyPrefixTree()
	{
		ReadOnlyPrefixTree<int> tree = new(Array.Empty<KeyValuePair<string, int>>());
		Assert.AreEqual(0, tree.Count);
		Assert.AreEqual(0, tree.Keys.Count);
		Assert.AreEqual(0, tree.Values.Count);
		Assert.IsFalse(tree.Any());

		Assert.IsFalse(tree.ContainsKey(""));
		Assert.IsFalse(tree.ContainsKey("foo"));
		Assert.IsFalse(tree.TryGetValue("", out _));
		Assert.IsFalse(tree.TryGetValue("foo", out _));
		Assert.IsFalse(tree.TryMatchLongest("", out _));
		Assert.IsFalse(tree.TryMatchLongest("foo", out _));
		Assert.IsFalse(tree.TryMatchShortest("", out _));
		Assert.IsFalse(tree.TryMatchShortest("foo", out _));

		var data = tree.GetData();
		Assert.AreEqual(0, data.Items.Length);
		CollectionAssert.AreEqual(new int[] { 0 }, data.Nodes);
		Assert.AreEqual(0, data.Trans.Length);
	}

	/// <summary>
	/// 测试前缀树。
	/// </summary>
	[TestMethod]
	public void TestPrefixTree1()
	{
		ReadOnlyPrefixTree<int> tree = new(new KeyValuePair<string, int>[]
		{
			new("a", 2),
			new("abdef", 4),
			new("", 1),
			new("bdef", 5),
			new("abc", 3),
		});
		Assert.AreEqual(5, tree.Count);
		Assert.AreEqual(5, tree.Keys.Count);
		CollectionAssert.AreEqual(new string[] { "", "a", "abc", "abdef", "bdef" }, (ICollection)tree.Keys);
		Assert.AreEqual(5, tree.Values.Count);
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, (ICollection)tree.Values);

		Assert.IsTrue(tree.ContainsKey(""));
		Assert.IsFalse(tree.ContainsKey("foo"));
		Assert.IsTrue(tree.TryGetValue("", out int value));
		Assert.AreEqual(1, value);
		Assert.IsFalse(tree.TryGetValue("foo", out _));
		Assert.IsTrue(tree.ContainsKey("a"));
		Assert.IsTrue(tree.ContainsKey("abc"));
		Assert.IsTrue(tree.ContainsKey("abdef"));
		Assert.IsTrue(tree.ContainsKey("bdef"));
		Assert.IsFalse(tree.ContainsKey("bdefg"));
		Assert.IsFalse(tree.ContainsKey("ab"));
		Assert.IsFalse(tree.ContainsKey("bd"));

		Assert.IsTrue(tree.TryMatchLongest("", out var pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchLongest("foo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchLongest("abcdef", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("abc", 3), pair);

		Assert.IsTrue(tree.TryMatchShortest("", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchShortest("foo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);

		var data = tree.GetData();
		CollectionAssert.AreEqual(new KeyValuePair<string, int>[]
		{
			new("", 1),
			new("a", 2),
			new("abc", 3),
			new("abdef", 4),
			new("bdef", 5),
		}, data.Items);
		CollectionAssert.AreEqual(new int[] {
			-385,
			0,
			5,
			1,
			98,
			-386,
			12,
			6619236,
			102,
			1,
			4,
			1,
			2,
			8,
			6684773,
			1,
			3,
		}, data.Nodes);
		CollectionAssert.AreEqual(new int[] {
			0,
			2,
			0,
			6,
			5,
			11,
			5,
			13,
		}, data.Trans);
	}

	/// <summary>
	/// 测试前缀树。
	/// </summary>
	[TestMethod]
	public void TestPrefixTree2()
	{
		ReadOnlyPrefixTree<int> tree = new(new KeyValuePair<string, int>[]
		{
			new("foo", 3),
		});
		Assert.AreEqual(1, tree.Count);
		Assert.AreEqual(1, tree.Keys.Count);
		CollectionAssert.AreEqual(new string[] { "foo" }, (ICollection)tree.Keys);
		Assert.AreEqual(1, tree.Values.Count);
		CollectionAssert.AreEqual(new int[] { 3 }, (ICollection)tree.Values);

		Assert.IsFalse(tree.ContainsKey(""));
		Assert.IsFalse(tree.ContainsKey("f"));
		Assert.IsFalse(tree.ContainsKey("fo"));
		Assert.IsTrue(tree.ContainsKey("foo"));
		Assert.IsFalse(tree.ContainsKey("fooo"));
		Assert.IsTrue(tree.TryGetValue("foo", out int value));
		Assert.AreEqual(3, value);

		Assert.IsFalse(tree.TryMatchLongest("fo", out _));
		Assert.IsTrue(tree.TryMatchLongest("foo", out var pair));
		Assert.AreEqual(new KeyValuePair<string, int>("foo", 3), pair);
		Assert.IsTrue(tree.TryMatchLongest("foooo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("foo", 3), pair);

		Assert.IsFalse(tree.TryMatchShortest("fo", out _));
		Assert.IsTrue(tree.TryMatchShortest("foooo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("foo", 3), pair);

		var data = tree.GetData();
		CollectionAssert.AreEqual(new KeyValuePair<string, int>[]
		{
			new("foo", 3),
		}, data.Items);
		CollectionAssert.AreEqual(new int[] {
			12,
			7274598,
			111,
			1,
			0
		}, data.Nodes);
		CollectionAssert.AreEqual(Array.Empty<int>(), data.Trans);
	}

	/// <summary>
	/// 测试前缀树。
	/// </summary>
	[TestMethod]
	public void TestPrefixTree3()
	{
		ReadOnlyPrefixTree<int> tree = new(new KeyValuePair<string, int>[]
		{
			new("abab", 1),
			new("ababcd", 2),
			new("ababcdijk", 3),
			new("abcdef", 4),
			new("abcdefijkl", 5),
			new("abcdefijkm", 6),
			new("abcdefijkn", 7),
			new("abcdgh", 8),
		});
		Assert.AreEqual(8, tree.Count);
		Assert.AreEqual(8, tree.Keys.Count);
		CollectionAssert.AreEqual(new string[] {
			"abab",
			"ababcd",
			"ababcdijk",
			"abcdef",
			"abcdefijkl",
			"abcdefijkm",
			"abcdefijkn",
			"abcdgh",
		}, (ICollection)tree.Keys);
		Assert.AreEqual(8, tree.Values.Count);
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }, (ICollection)tree.Values);

		Assert.IsFalse(tree.ContainsKey(""));
		Assert.IsFalse(tree.ContainsKey("ab"));
		Assert.IsTrue(tree.ContainsKey("abab"));
		Assert.IsTrue(tree.ContainsKey("ababcd"));
		Assert.IsTrue(tree.ContainsKey("ababcdijk"));
		Assert.IsTrue(tree.ContainsKey("abcdefijkl"));
		Assert.IsTrue(tree.ContainsKey("abcdefijkm"));
		Assert.IsTrue(tree.ContainsKey("abcdgh"));
		Assert.IsTrue(tree.TryGetValue("abcdefijkn", out int value));
		Assert.AreEqual(7, value);
		Assert.IsTrue(tree.TryGetValue("abcdef", out value));
		Assert.AreEqual(4, value);

		Assert.IsFalse(tree.TryMatchLongest("a", out _));
		Assert.IsTrue(tree.TryMatchLongest("abcdefgh", out var pair));
		Assert.AreEqual(new KeyValuePair<string, int>("abcdef", 4), pair);
		Assert.IsTrue(tree.TryMatchLongest("abcdefijkopq", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("abcdef", 4), pair);

		Assert.IsFalse(tree.TryMatchShortest("a", out _));
		Assert.IsTrue(tree.TryMatchShortest("ababcdijkmn", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("abab", 1), pair);

		var data = tree.GetData();
		CollectionAssert.AreEqual(new KeyValuePair<string, int>[]
		{
			new("abab", 1),
			new("ababcd", 2),
			new("ababcdijk", 3),
			new("abcdef", 4),
			new("abcdefijkl", 5),
			new("abcdefijkm", 6),
			new("abcdefijkn", 7),
			new("abcdgh", 8),
		}, data.Items);
		CollectionAssert.AreEqual(new int[] {
			8,
			6422625,
			-386,
			4,
			98,
			9,
			0,
			6553699,
			13,
			1,
			6946921,
			107,
			1,
			2,
			4,
			100,
			-398,
			4,
			102,
			13,
			3,
			6946921,
			107,
			-414,
			4,
			104,
			1,
			7,
			1,
			4,
			1,
			5,
			1,
			6,
		}, data.Nodes);
		CollectionAssert.AreEqual(new int[]
		{
			2,
			3,
			16,
			17,
			2,
			14,
			16,
			24,
			23,
			28,
			23,
			30,
			23,
			32,
		}, data.Trans);
	}

	/// <summary>
	/// 测试使用预构建的数据初始化前缀树。
	/// </summary>
	[TestMethod]
	public void TestPrefixTreeWithData()
	{
		ReadOnlyPrefixTreeData<int> data = new(
			new KeyValuePair<string, int>[]
			{
				new("", 1),
				new("a", 2),
				new("abc", 3),
				new("abdef", 4),
				new("bdef", 5),
			},
			new int[] {
				-385,
				0,
				5,
				1,
				98,
				-386,
				12,
				6619236,
				102,
				1,
				4,
				1,
				2,
				8,
				6684773,
				1,
				3,
			},
			new int[] {
				0,
				2,
				0,
				6,
				5,
				11,
				5,
				13,
			}
		);
		ReadOnlyPrefixTree<int> tree = new(data);
		Assert.AreEqual(5, tree.Count);
		Assert.AreEqual(5, tree.Keys.Count);
		CollectionAssert.AreEqual(new string[] { "", "a", "abc", "abdef", "bdef" }, (ICollection)tree.Keys);
		Assert.AreEqual(5, tree.Values.Count);
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, (ICollection)tree.Values);

		Assert.IsTrue(tree.ContainsKey(""));
		Assert.IsFalse(tree.ContainsKey("foo"));
		Assert.IsTrue(tree.TryGetValue("", out int value));
		Assert.AreEqual(1, value);
		Assert.IsFalse(tree.TryGetValue("foo", out _));
		Assert.IsTrue(tree.ContainsKey("a"));
		Assert.IsTrue(tree.ContainsKey("abc"));
		Assert.IsTrue(tree.ContainsKey("abdef"));
		Assert.IsTrue(tree.ContainsKey("bdef"));
		Assert.IsFalse(tree.ContainsKey("bdefg"));
		Assert.IsFalse(tree.ContainsKey("ab"));
		Assert.IsFalse(tree.ContainsKey("bd"));

		Assert.IsTrue(tree.TryMatchLongest("", out var pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchLongest("foo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchLongest("abcdef", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("abc", 3), pair);

		Assert.IsTrue(tree.TryMatchShortest("", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);
		Assert.IsTrue(tree.TryMatchShortest("foo", out pair));
		Assert.AreEqual(new KeyValuePair<string, int>("", 1), pair);


	}
}
