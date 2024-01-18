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
		Assert.IsFalse(tree.TryGetValue("", out int value));
		Assert.IsFalse(tree.TryGetValue("foo", out value));
		Assert.IsFalse(tree.TryMatchLongest("", out var pair));
		Assert.IsFalse(tree.TryMatchLongest("foo", out pair));
		Assert.IsFalse(tree.TryMatchShortest("", out pair));
		Assert.IsFalse(tree.TryMatchShortest("foo", out pair));

		var data = tree.GetData();
		Assert.AreEqual(0, data.Items.Length);
		CollectionAssert.AreEqual(new ulong[] { 0xFFFFFFFF80000000 }, data.Nodes);
		Assert.AreEqual(0, data.Trans.Length);
	}

	/// <summary>
	/// 测试前缀树。
	/// </summary>
	[TestMethod]
	public void TestPrefixTree()
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
		Assert.IsFalse(tree.TryGetValue("foo", out value));
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
		CollectionAssert.AreEqual(new ulong[] {
			0xFFFFFF9F,
			0x1FFFFFFA0,
			0xFFFFFFFFFFFFFFA3,
			0xFFFFFFFFFFFFFFA0,
			0xFFFFFFFFFFFFFFA0,
			0x280000000,
			0xFFFFFFFFFFFFFFA0,
			0x380000000,
			0xFFFFFFFFFFFFFFA3,
			0xFFFFFFFFFFFFFFA3,
			0x480000000,
		}, data.Nodes);
		CollectionAssert.AreEqual(new ulong[] {
			0x100000000,
			0x200000000,
			0x300000001,
			0x500000003,
			0x400000003,
			0x600000004,
			0x700000006,
			0x800000002,
			0x900000008,
			0xa00000009,
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
			new ulong[] {
				0xFFFFFF9F,
				0x1FFFFFFA0,
				0xFFFFFFFFFFFFFFA3,
				0xFFFFFFFFFFFFFFA0,
				0xFFFFFFFFFFFFFFA0,
				0x280000000,
				0xFFFFFFFFFFFFFFA0,
				0x380000000,
				0xFFFFFFFFFFFFFFA3,
				0xFFFFFFFFFFFFFFA3,
				0x480000000,
			},
			new ulong[] {
				0x100000000,
				0x200000000,
				0x300000001,
				0x500000003,
				0x400000003,
				0x600000004,
				0x700000006,
				0x800000002,
				0x900000008,
				0xa00000009,
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
		Assert.IsFalse(tree.TryGetValue("foo", out value));
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
