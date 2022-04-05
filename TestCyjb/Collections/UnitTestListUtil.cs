using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="ListUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestListUtil
	{
		/// <summary>
		/// 对 <see cref="ListUtil.InsertRange"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestInsertRange()
		{
			IList<int> list = new List<int>();

			list.InsertRange(0, new int[] { 1, 2, 3 });
			int[] expected = { 1, 2, 3 };
			CollectionAssert.AreEqual(expected, list.ToArray());

			list.InsertRange(0, new int[] { 4, 5 });
			expected = new int[] { 4, 5, 1, 2, 3 };
			CollectionAssert.AreEqual(expected, list.ToArray());

			list.InsertRange(5, new int[] { 6, 7 });
			expected = new int[] { 4, 5, 1, 2, 3, 6, 7 };
			CollectionAssert.AreEqual(expected, list.ToArray());
		}

		/// <summary>
		/// 对 <see cref="ListUtil.Suffle"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestSuffle()
		{
			int size = 10;
			IList<int> list = new List<int>();
			int[,] cnt = new int[size, size];
			int loop = 10000;
			for (int i = 0; i < loop; i++)
			{
				list.Clear();
				for (int j = 0; j < size; j++)
				{
					list.Add(j);
				}
				list.Suffle();
				for (int j = 0; j < size; j++)
				{
					cnt[j, list[j]]++;
				}
			}
			double min = loop / size * 0.8;
			double max = loop / size * 1.2;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					Assert.IsTrue(cnt[i, j] > min && cnt[i, j] < max);
					Console.Write("{0} ", cnt[i, j]);
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// 对 <see cref="ListUtil.Reverse"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestReverse()
		{
			IList<int> list = new List<int>();

			CollectionAssert.AreEqual(Array.Empty<int>(), list.Reverse().ToArray());

			list.Add(1);
			CollectionAssert.AreEqual(new int[] { 1 }, list.Reverse().ToArray());
			CollectionAssert.AreEqual(new int[] { 1 }, list.Reverse(0, 0).ToArray());

			list.AddRange(new int[] { 2, 3, 4 });
			CollectionAssert.AreEqual(new int[] { 4, 3, 2, 1 }, list.Reverse().ToArray());
			CollectionAssert.AreEqual(new int[] { 4, 2, 3, 1 }, list.Reverse(1, 2).ToArray());
			CollectionAssert.AreEqual(new int[] { 3, 2, 4, 1 }, list.Reverse(0, 3).ToArray());
		}

		/// <summary>
		/// 对 <see cref="ListUtil.BinarySearch"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestBinarySearch()
		{
			IList<int> list = new List<int>(new int[] { 1, 1, 2, 3, 10, 30, 30, 30, 30, 50, 90 });
			Assert.AreEqual(-1, list.BinarySearch(0));
			Assert.IsTrue(list.BinarySearch(1) >= 0);
			Assert.IsTrue(list.BinarySearch(1) <= 1);
			Assert.AreEqual(2, list.BinarySearch(2));
			Assert.AreEqual(3, list.BinarySearch(3));
			Assert.AreEqual(4, list.BinarySearch(10));
			Assert.IsTrue(list.BinarySearch(30) >= 5);
			Assert.IsTrue(list.BinarySearch(30) <= 8);
			Assert.AreEqual(9, list.BinarySearch(50));
			Assert.AreEqual(10, list.BinarySearch(90));
			Assert.AreEqual(-12, list.BinarySearch(100));

			Assert.AreEqual(-2, list.BinarySearch(1, 8, 0));
			Assert.AreEqual(1, list.BinarySearch(1, 8, 1));
			Assert.AreEqual(2, list.BinarySearch(1, 8, 2));
			Assert.AreEqual(3, list.BinarySearch(1, 8, 3));
			Assert.AreEqual(4, list.BinarySearch(1, 8, 10));
			Assert.IsTrue(list.BinarySearch(30) >= 5);
			Assert.IsTrue(list.BinarySearch(30) <= 8);
			Assert.AreEqual(-10, list.BinarySearch(1, 8, 50));
			Assert.AreEqual(-10, list.BinarySearch(1, 8, 90));
			Assert.AreEqual(-10, list.BinarySearch(1, 8, 100));

			Assert.AreEqual(-3, list.BinarySearch(2, 2, 1));
			Assert.AreEqual(2, list.BinarySearch(2, 2, 2));
			Assert.AreEqual(3, list.BinarySearch(2, 2, 3));
			Assert.AreEqual(-5, list.BinarySearch(2, 2, 4));

			Assert.AreEqual(-3, list.BinarySearch(2, 1, 1));
			Assert.AreEqual(2, list.BinarySearch(2, 1, 2));
			Assert.AreEqual(-4, list.BinarySearch(2, 1, 3));
			Assert.AreEqual(-4, list.BinarySearch(2, 1, 4));

			Assert.AreEqual(-3, list.BinarySearch(2, 0, 1));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, 2));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, 3));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, 4));

			static string selector(int i) => "T_" + i.ToString().PadLeft(2, '0');

			Assert.AreEqual(-1, list.BinarySearch("T_00", selector));
			Assert.IsTrue(list.BinarySearch("T_01", selector) >= 0);
			Assert.IsTrue(list.BinarySearch("T_01", selector) <= 1);
			Assert.AreEqual(2, list.BinarySearch("T_02", selector));
			Assert.AreEqual(3, list.BinarySearch("T_03", selector));
			Assert.AreEqual(4, list.BinarySearch("T_10", selector));
			Assert.IsTrue(list.BinarySearch("T_30", selector) >= 5);
			Assert.IsTrue(list.BinarySearch("T_30", selector) <= 8);
			Assert.AreEqual(9, list.BinarySearch("T_50", selector));
			Assert.AreEqual(10, list.BinarySearch("T_90", selector));
			Assert.AreEqual(-12, list.BinarySearch("T_99", selector));

			Assert.AreEqual(-2, list.BinarySearch(1, 8, "T_00", selector));
			Assert.AreEqual(1, list.BinarySearch(1, 8, "T_01", selector));
			Assert.AreEqual(2, list.BinarySearch(1, 8, "T_02", selector));
			Assert.AreEqual(3, list.BinarySearch(1, 8, "T_03", selector));
			Assert.AreEqual(4, list.BinarySearch(1, 8, "T_10", selector));
			Assert.IsTrue(list.BinarySearch("T_30", selector) >= 5);
			Assert.IsTrue(list.BinarySearch("T_30", selector) <= 8);
			Assert.AreEqual(-10, list.BinarySearch(1, 8, "T_50", selector));
			Assert.AreEqual(-10, list.BinarySearch(1, 8, "T_90", selector));
			Assert.AreEqual(-10, list.BinarySearch(1, 8, "T_99", selector));

			Assert.AreEqual(-3, list.BinarySearch(2, 2, "T_01", selector));
			Assert.AreEqual(2, list.BinarySearch(2, 2, "T_02", selector));
			Assert.AreEqual(3, list.BinarySearch(2, 2, "T_03", selector));
			Assert.AreEqual(-5, list.BinarySearch(2, 2, "T_04", selector));

			Assert.AreEqual(-3, list.BinarySearch(2, 1, "T_01", selector));
			Assert.AreEqual(2, list.BinarySearch(2, 1, "T_02", selector));
			Assert.AreEqual(-4, list.BinarySearch(2, 1, "T_03", selector));
			Assert.AreEqual(-4, list.BinarySearch(2, 1, "T_04", selector));

			Assert.AreEqual(-3, list.BinarySearch(2, 0, "T_01", selector));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, "T_02", selector));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, "T_03", selector));
			Assert.AreEqual(-3, list.BinarySearch(2, 0, "T_04", selector));
		}
	}
}
