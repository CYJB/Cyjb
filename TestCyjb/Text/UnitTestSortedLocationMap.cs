using System;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="SortedLocationMap"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestSortedLocationMap
{
	/// <summary>
	/// 对 <see cref="SortedLocationMap.MapLocation"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestMapLocation()
	{
		SortedLocationMap map = new(Array.Empty<Tuple<int, int>>());
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(1, map.MapLocation(1));
		Assert.AreEqual(3, map.MapLocation(3));

		map = new(new Tuple<int, int>[]
		{
			new Tuple<int, int>(1, 10),
			new Tuple<int, int>(5, 100),
			new Tuple<int, int>(8, 101),
			new Tuple<int, int>(10, 103),
			new Tuple<int, int>(15, 108),
		});
		Assert.AreEqual(0, map.MapLocation(0));
		Assert.AreEqual(10, map.MapLocation(1));
		Assert.AreEqual(12, map.MapLocation(3));
		Assert.AreEqual(100, map.MapLocation(7));
		Assert.AreEqual(101, map.MapLocation(8));
		Assert.AreEqual(113, map.MapLocation(20));

		map = new(new Tuple<int, int>[]
		{
			new Tuple<int, int>(0, 34),
			new Tuple<int, int>(2, 34)
		});
		Assert.AreEqual(34, map.MapLocation(0));
		Assert.AreEqual(34, map.MapLocation(2));
		Assert.AreEqual(37, map.MapLocation(5));
	}
}
