using System;
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
}
