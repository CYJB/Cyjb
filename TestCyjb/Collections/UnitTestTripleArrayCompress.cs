using System;
using System.Collections.Generic;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;

/// <summary>
/// <see cref="TripleArrayCompress{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestTripleArrayCompress
{
	/// <summary>
	/// 表示无效的节点。
	/// </summary>
	private const int I = -1;

	[TestMethod]
	public void TestCompress1()
	{
		TripleArrayCompress<int> compress = new(I);
		// 测试压缩 DFA 的状态转移
		//     | 0 | 1 |
		// | 0 | 0 | 1 |
		// | 1 | 2 | 1 |
		// | 2 | 3 | 1 |
		// | 3 | 0 | 1 |
		Assert.AreEqual(0, compress.AddNode(0, new KeyValuePair<int, int>[]
		{
			new(0, 0),
			new(1, 1),
		}));
		Assert.AreEqual(2, compress.AddNode(1, new KeyValuePair<int, int>[]
		{
			new(0, 2),
			new(1, 1),
		}));
		Assert.AreEqual(4, compress.AddNode(2, new KeyValuePair<int, int>[]
		{
			new(0, 3),
			new(1, 1),
		}));
		Assert.AreEqual(6, compress.AddNode(3, new KeyValuePair<int, int>[]
		{
			new(0, 0),
			new(1, 1),
		}));

		//                                    0  1  2  3  4  5  6  7
		CollectionAssert.AreEqual(new int[] { 0, 1, 2, 1, 3, 1, 0, 1 }, compress.Next);
		CollectionAssert.AreEqual(new int[] { 0, 0, 1, 1, 2, 2, 3, 3 }, compress.Check);
	}

	[TestMethod]
	public void TestCompress2()
	{
		TripleArrayCompress<int> compress = new(I);
		// 测试压缩 DFA 的状态转移
		//     | 0 | 1 | 2 | 3 | 4 | 5 |
		// | 0 | 1 |   |   |   | 2 |   |
		// | 1 | 5 | 1 | 6 |   | 1 |   |
		// | 2 | 3 |   |   |   |   |   |
		// | 3 | 4 | 3 | 3 | 3 | 3 | 3 |
		// | 4 | 3 |   |   |   |   |   |
		// | 5 |   |   |   |   |   |   |
		// | 6 | 1 | 1 | 1 | 1 | 1 |   |
		Assert.AreEqual(0, compress.AddNode(0, new KeyValuePair<int, int>[]
		{
			new(0, 1),
			new(4, 2),
		}));
		Assert.AreEqual(1, compress.AddNode(1, new KeyValuePair<int, int>[]
		{
			new(0, 5),
			new(1, 1),
			new(2, 6),
			new(4, 1),
		}));
		Assert.AreEqual(6, compress.AddNode(2, new KeyValuePair<int, int>[]
		{
			new(0, 3),
		}));
		Assert.AreEqual(7, compress.AddNode(3, new KeyValuePair<int, int>[]
		{
			new(0, 4),
			new(1, 3),
			new(2, 3),
			new(3, 3),
			new(4, 3),
			new(5, 3),
		}));
		Assert.AreEqual(13, compress.AddNode(4, new KeyValuePair<int, int>[]
		{
			new(0, 3),
		}));
		Assert.AreEqual(int.MinValue, compress.AddNode(5, Array.Empty<KeyValuePair<int, int>>()));
		Assert.AreEqual(14, compress.AddNode(6, new KeyValuePair<int, int>[]
		{
			new(0, 1),
			new(1, 1),
			new(2, 1),
			new(3, 1),
			new(4, 1),
		}));

		//                                    0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18
		CollectionAssert.AreEqual(new int[] { 1, 5, 1, 6, 2, 1, 3, 4, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 1 }, compress.Next);
		CollectionAssert.AreEqual(new int[] { 0, 1, 1, 1, 0, 1, 2, 3, 3, 3, 3, 3, 3, 4, 6, 6, 6, 6, 6 }, compress.Check);
	}

	[TestMethod]
	public void TestCompress3()
	{
		TripleArrayCompress<int> compress = new(I);
		// 测试压缩 DFA 的状态转移
		//     | 0 | 1 | 2 | 3 | 4 | 5 |
		// | 0 | 1 | 1 |   |   | 2 |   |
		// | 1 | 5 | 1 | 6 |   | 1 |   |
		// | 2 | 3 |   |   |   |   |   |
		Assert.AreEqual(0, compress.AddNode(0, new KeyValuePair<int, int>[]
		{
			new(0, 1),
			new(1, 1),
			new(4, 2),
		}));
		Assert.AreEqual(5, compress.AddNode(1, new KeyValuePair<int, int>[]
		{
			new(0, 5),
			new(1, 1),
			new(2, 6),
			new(4, 1),
		}));
		Assert.AreEqual(2, compress.AddNode(2, new KeyValuePair<int, int>[]
		{
			new(0, 3),
		}));

		//                                    0  1  2  3  4  5  6  7  8  9
		CollectionAssert.AreEqual(new int[] { 1, 1, 3, I, 2, 5, 1, 6, I, 1 }, compress.Next);
		CollectionAssert.AreEqual(new int[] { 0, 0, 2, I, 0, 1, 1, 1, I, 1 }, compress.Check);
	}
}
