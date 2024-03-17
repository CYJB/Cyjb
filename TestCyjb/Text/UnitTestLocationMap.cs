using System;
using System.Collections.Generic;
using System.Reflection;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="LocationMap"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestLocationMap
{
	/// <summary>
	/// <see cref="LocationMap"/> 的 <c>indexMap</c> 字段。
	/// </summary>
	private static readonly FieldInfo IndexMapField = typeof(LocationMap)
		.GetField("indexMap", BindingFlags.Instance | BindingFlags.NonPublic)!;

	/// <summary>
	/// 对 <see cref="LocationMap.MapLocation"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestMapLocationIndex()
	{
		LocationMap map = new(Array.Empty<Tuple<int, int>>(), LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(3, map.MapLocation(3));
		Assert.AreEqual(1, map.MapLocation(1));

		map = new(new Tuple<int, int>[]
		{
			new(1, 10),
			new(5, 100),
			new(8, 101),
			new(10, 103),
			new(15, 108),
		}, LocationMapType.Index);
		Assert.AreEqual(10, map.MapLocation(1));
		Assert.AreEqual(100, map.MapLocation(7));
		Assert.AreEqual(101, map.MapLocation(8));
		Assert.AreEqual(113, map.MapLocation(20));
		Assert.AreEqual(12, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));

		map = new(new Tuple<int, int>[]
		{
			new(0, 34),
			new(2, 34)
		}, LocationMapType.Index);
		Assert.AreEqual(34, map.MapLocation(0));
		Assert.AreEqual(37, map.MapLocation(5));
		Assert.AreEqual(34, map.MapLocation(2));

		map = new(new Tuple<int, int>[]
		{
			new(10, 34),
			new(18, 42),
			new(20, 44),
			new(21, 47),
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(9, map.MapLocation(9));
		Assert.AreEqual(34, map.MapLocation(10));
		Assert.AreEqual(41, map.MapLocation(17));
		Assert.AreEqual(42, map.MapLocation(18));
		Assert.AreEqual(43, map.MapLocation(19));
		Assert.AreEqual(44, map.MapLocation(20));
		Assert.AreEqual(47, map.MapLocation(21));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.MapLocation"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestMapLocationOffset()
	{
		LocationMap map = new(Array.Empty<Tuple<int, int>>(), LocationMapType.Offset);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(3, map.MapLocation(3));
		Assert.AreEqual(1, map.MapLocation(1));

		map = new(new Tuple<int, int>[]
		{
			new(1, 10),
			new(4, 90),
			new(3, 1),
			new(2, 2),
			new(5, 5),
		}, LocationMapType.Offset);
		Assert.AreEqual(10, map.MapLocation(1));
		Assert.AreEqual(100, map.MapLocation(7));
		Assert.AreEqual(101, map.MapLocation(8));
		Assert.AreEqual(113, map.MapLocation(20));
		Assert.AreEqual(12, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));

		map = new(new Tuple<int, int>[]
		{
			new(0, 34),
			new(2, 0)
		}, LocationMapType.Offset);
		Assert.AreEqual(34, map.MapLocation(0));
		Assert.AreEqual(37, map.MapLocation(5));
		Assert.AreEqual(34, map.MapLocation(2));

		map = new(new Tuple<int, int>[]
		{
			new(10, 34),
			new(8, 8),
			new(2, 2),
			new(1, 3),
		}, LocationMapType.Offset);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(9, map.MapLocation(9));
		Assert.AreEqual(34, map.MapLocation(10));
		Assert.AreEqual(41, map.MapLocation(17));
		Assert.AreEqual(42, map.MapLocation(18));
		Assert.AreEqual(43, map.MapLocation(19));
		Assert.AreEqual(44, map.MapLocation(20));
		Assert.AreEqual(47, map.MapLocation(21));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到空映射。</remarks>
	[TestMethod]
	public void TestAddToEmpty()
	{
		LocationMap map = new();
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(3, map.MapLocation(3));

		map.Add(2, 10);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(1, map.MapLocation(1));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(1, map.MapLocation(1));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到首个映射之前。</remarks>
	[TestMethod]
	public void TestAddBeforeFirst()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(1, 3);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(3, map.MapLocation(1));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(3, map.MapLocation(1));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到首个映射之前。</remarks>
	[TestMethod]
	public void TestAddBeforeFirst_2()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(1, 30);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(30, map.MapLocation(1));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(30, map.MapLocation(1));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换首个映射，变小。</remarks>
	[TestMethod]
	public void TestAddReplaceFirstLess()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(2, 3);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(4, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(4, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换首个映射，不变。</remarks>
	[TestMethod]
	public void TestAddReplaceFirstEqual()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(2, 10);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换首个映射，变大。</remarks>
	[TestMethod]
	public void TestAddReplaceFirstGreater()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(2, 16);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(17, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(17, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到首个映射之后。</remarks>
	[TestMethod]
	public void TestAddAfterFirst()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(3, 14);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(14, map.MapLocation(3));
		Assert.AreEqual(15, map.MapLocation(4));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(15, map.MapLocation(4));
		Assert.AreEqual(14, map.MapLocation(3));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到首个映射之后。</remarks>
	[TestMethod]
	public void TestAddAfterFirst_2()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(3, 30);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(30, map.MapLocation(3));
		Assert.AreEqual(30, map.MapLocation(4));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(30, map.MapLocation(4));
		Assert.AreEqual(30, map.MapLocation(3));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换第二个映射，变小。</remarks>
	[TestMethod]
	public void TestAddReplaceSecondLess()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(6, 3);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(10, map.MapLocation(5));
		Assert.AreEqual(3, map.MapLocation(6));
		Assert.AreEqual(4, map.MapLocation(7));
		Assert.AreEqual(3, map.MapLocation(6));
		Assert.AreEqual(10, map.MapLocation(5));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换第二个映射，不变。</remarks>
	[TestMethod]
	public void TestAddReplaceSecondEqual()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(6, 20);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>替换第二个映射，变大。</remarks>
	[TestMethod]
	public void TestAddReplaceSecondGreater()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(6, 30);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(30, map.MapLocation(6));
		Assert.AreEqual(33, map.MapLocation(9));
		Assert.AreEqual(30, map.MapLocation(6));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到第二个映射之后。</remarks>
	[TestMethod]
	public void TestAddAfterSecond()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(10, 21);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(20, map.MapLocation(9));
		Assert.AreEqual(21, map.MapLocation(10));
		Assert.AreEqual(28, map.MapLocation(17));
		Assert.AreEqual(21, map.MapLocation(10));
		Assert.AreEqual(20, map.MapLocation(9));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.Add"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加连续的映射。</remarks>
	[TestMethod]
	public void TestAddContinuous()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.Add(3, 11);
		map.Add(4, 12);
		map.Add(5, 13);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(12, map.MapLocation(4));
		Assert.AreEqual(13, map.MapLocation(5));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(21, map.MapLocation(7));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(13, map.MapLocation(5));
		Assert.AreEqual(12, map.MapLocation(4));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));

		// 连续映射不会添加新索引。
		Assert.AreEqual(2, ((List<int>)IndexMapField.GetValue(map)!).Count);
	}


	/// <summary>
	/// 对 <see cref="LocationMap.AddOffset"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加到空映射。</remarks>
	[TestMethod]
	public void TestAddOffsetToEmpty()
	{
		LocationMap map = new();
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(3, map.MapLocation(3));

		map.AddOffset(2, 10);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(1, map.MapLocation(1));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(11, map.MapLocation(3));
		Assert.AreEqual(1, map.MapLocation(1));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.AddOffset"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAddOffset()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.AddOffset(4, 1);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(20, map.MapLocation(9));
		Assert.AreEqual(21, map.MapLocation(10));
		Assert.AreEqual(28, map.MapLocation(17));
		Assert.AreEqual(21, map.MapLocation(10));
		Assert.AreEqual(20, map.MapLocation(9));
		Assert.AreEqual(10, map.MapLocation(2));
		Assert.AreEqual(0, map.MapLocation(0));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.AddOffset"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAddOffsetZero()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.AddOffset(0, 2);
		Assert.AreEqual(13, map.MapLocation(5));
		Assert.AreEqual(22, map.MapLocation(6));
		Assert.AreEqual(23, map.MapLocation(7));
		Assert.AreEqual(22, map.MapLocation(6));
		Assert.AreEqual(13, map.MapLocation(5));
	}

	/// <summary>
	/// 对 <see cref="LocationMap.AddOffset"/> 方法进行测试。
	/// </summary>
	/// <remarks>添加连续的映射。</remarks>
	[TestMethod]
	public void TestAddOffsetContinuous()
	{
		LocationMap map = new(new Tuple<int, int>[]
		{
			new(2, 10),
			new(6, 20)
		}, LocationMapType.Index);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(11, map.MapLocation(3));

		map.AddOffset(0, 0);
		map.AddOffset(1, 1);
		map.AddOffset(2, 2);
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(21, map.MapLocation(7));
		Assert.AreEqual(20, map.MapLocation(6));
		Assert.AreEqual(0, map.MapLocation(0));

		// 连续映射不会添加新索引。
		Assert.AreEqual(2, ((List<int>)IndexMapField.GetValue(map)!).Count);
	}

}
