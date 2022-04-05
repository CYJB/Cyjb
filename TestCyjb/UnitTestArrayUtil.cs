using System;
using Cyjb;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="ArrayUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestArrayUtil
	{
		/// <summary>
		/// 对 <see cref="ArrayUtil.Add"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestAdd()
		{
			int[] arr = new int[] { 1, 2 };
			CollectionAssert.AreEqual(new int[] { 1, 2 }, arr.Add());
			CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, arr.Add(3));
			CollectionAssert.AreEqual(new int[] { 1, 2, 4, 5 }, arr.Add(4, 5));
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Add());
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Insert"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestInsert()
		{
			int[] arr = new int[] { 1, 2 };
			CollectionAssert.AreEqual(new int[] { 1, 2 }, arr.Insert(0));
			CollectionAssert.AreEqual(new int[] { 3, 1, 2 }, arr.Insert(0, 3));
			CollectionAssert.AreEqual(new int[] { 1, 4, 5, 2 }, arr.Insert(1, 4, 5));
			CollectionAssert.AreEqual(new int[] { 1, 2, 6, 7 }, arr.Insert(2, 6, 7));
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Insert(0));
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Concat"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestConcat()
		{
			int[] arr = new int[] { 1, 2 };
			CollectionAssert.AreEqual(new int[] { 1, 2 }, arr.Concat());
			CollectionAssert.AreEqual(new int[] { 1, 2 }, arr.Concat(Array.Empty<int>()));
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, arr.Concat(new int[] { 3, 4 }));
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, arr.Concat(new int[] { 3, 4 },
					Array.Empty<int>(), new int[] { 5, 6 }));
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Concat());
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Resize"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestResize()
		{
			int[] arr = new int[] { 1, 2, 3, 4 };
			CollectionAssert.AreEqual(Array.Empty<int>(), arr.Resize(0));
			CollectionAssert.AreEqual(new int[] { 1, 2 }, arr.Resize(2));
			Assert.AreEqual(arr, arr.Resize(4));
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 0, 0 }, arr.Resize(6));
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 2, 2 }, arr.Resize(6, 2));
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Resize(1));
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Fill"/> 方法的一维数组进行测试。
		/// </summary>
		[TestMethod]
		public void TestFill_1()
		{
			// 一维数组
			int[] arr = new int[10];
			arr.Fill(3);
			CollectionAssert.AreEqual(new int[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 }, arr);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Fill(3));

			arr.Fill(4, 2, 4);
			CollectionAssert.AreEqual(new int[] { 3, 3, 4, 4, 4, 4, 3, 3, 3, 3 }, arr);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Fill(4, 2, 4));

			arr.Fill((int index) => index);
			CollectionAssert.AreEqual(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, arr);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Fill((int index) => index));

			arr.Fill((int index) => index * 2, 5, 3);
			CollectionAssert.AreEqual(new int[] { 0, 1, 2, 3, 4, 10, 12, 14, 8, 9 }, arr);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[]>().Fill((int index) => index * 2, 5, 3));
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Fill"/> 方法的二维数组进行测试。
		/// </summary>
		[TestMethod]
		public void TestFill_2()
		{
			int[,] arr2 = new int[2, 2];
			arr2.Fill(3);
			Assert.AreEqual(3, arr2[0, 0]);
			Assert.AreEqual(3, arr2[0, 1]);
			Assert.AreEqual(3, arr2[1, 0]);
			Assert.AreEqual(3, arr2[1, 1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[,]>().Fill(3));

			arr2.Fill((int x, int y) => x * 2 + y);
			Assert.AreEqual(0, arr2[0, 0]);
			Assert.AreEqual(1, arr2[0, 1]);
			Assert.AreEqual(2, arr2[1, 0]);
			Assert.AreEqual(3, arr2[1, 1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[,]>().Fill((int x, int y) => x * 2 + y));

			int[][] arr3 = new int[2][];
			arr3.Fill((int index) => new int[2]);
			arr3.Fill(3);
			Assert.AreEqual(3, arr3[0][0]);
			Assert.AreEqual(3, arr3[0][1]);
			Assert.AreEqual(3, arr3[1][0]);
			Assert.AreEqual(3, arr3[1][1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[][]>().Fill(3));

			arr3.Fill((int x, int y) => x * 2 + y);
			Assert.AreEqual(0, arr3[0][0]);
			Assert.AreEqual(1, arr3[0][1]);
			Assert.AreEqual(2, arr3[1][0]);
			Assert.AreEqual(3, arr3[1][1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[][]>().Fill((int x, int y) => x * 2 + y));
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Fill"/> 方法的三维数组进行测试。
		/// </summary>
		[TestMethod]
		public void TestFill_3()
		{
			// 三维数组
			int[,,] arr4 = new int[2, 2, 2];
			arr4.Fill(3);
			Assert.AreEqual(3, arr4[0, 0, 0]);
			Assert.AreEqual(3, arr4[0, 0, 1]);
			Assert.AreEqual(3, arr4[0, 1, 0]);
			Assert.AreEqual(3, arr4[0, 1, 1]);
			Assert.AreEqual(3, arr4[1, 0, 0]);
			Assert.AreEqual(3, arr4[1, 0, 1]);
			Assert.AreEqual(3, arr4[1, 1, 0]);
			Assert.AreEqual(3, arr4[1, 1, 1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[,,]>().Fill(3));

			arr4.Fill((int x, int y, int z) => x * 4 + y * 2 + z);
			Assert.AreEqual(0, arr4[0, 0, 0]);
			Assert.AreEqual(1, arr4[0, 0, 1]);
			Assert.AreEqual(2, arr4[0, 1, 0]);
			Assert.AreEqual(3, arr4[0, 1, 1]);
			Assert.AreEqual(4, arr4[1, 0, 0]);
			Assert.AreEqual(5, arr4[1, 0, 1]);
			Assert.AreEqual(6, arr4[1, 1, 0]);
			Assert.AreEqual(7, arr4[1, 1, 1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[,,]>().Fill((int x, int y, int z) => x * 4 + y * 2 + z));

			int[][][] arr5 = new int[2][][];
			arr5.Fill((int index) => (new int[2][]).Fill((int index) => new int[2]));
			arr5.Fill(3);
			Assert.AreEqual(3, arr5[0][0][0]);
			Assert.AreEqual(3, arr5[0][0][1]);
			Assert.AreEqual(3, arr5[0][1][0]);
			Assert.AreEqual(3, arr5[0][1][1]);
			Assert.AreEqual(3, arr5[1][0][0]);
			Assert.AreEqual(3, arr5[1][0][1]);
			Assert.AreEqual(3, arr5[1][1][0]);
			Assert.AreEqual(3, arr5[1][1][1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[][][]>().Fill(3));

			arr5.Fill((int x, int y, int z) => x * 4 + y * 2 + z);
			Assert.AreEqual(0, arr5[0][0][0]);
			Assert.AreEqual(1, arr5[0][0][1]);
			Assert.AreEqual(2, arr5[0][1][0]);
			Assert.AreEqual(3, arr5[0][1][1]);
			Assert.AreEqual(4, arr5[1][0][0]);
			Assert.AreEqual(5, arr5[1][0][1]);
			Assert.AreEqual(6, arr5[1][1][0]);
			Assert.AreEqual(7, arr5[1][1][1]);
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<int[][][]>().Fill((int x, int y, int z) => x * 4 + y * 2 + z));
		}

		/// <summary>
		/// 对 <see cref="ArrayUtil.Suffle{T}(T[])"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestSuffle_1()
		{
			int size = 10;
			int[] arr = new int[size];
			int[,] cnt = new int[size, size];
			int loop = 10000;
			for (int i = 0; i < loop; i++)
			{
				arr.Fill(n => n).Suffle();
				for (int j = 0; j < size; j++)
				{
					cnt[j, arr[j]]++;
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
		/// 对 <see cref="ArrayUtil.Suffle{T}(T[,])"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestSuffle_2()
		{
			int w = 4;
			int h = 3;
			int size = w * h;
			int[,] arr = new int[h, w];
			int[,] cnt = new int[size, size];
			int loop = 10000;
			for (int i = 0; i < loop; i++)
			{
				arr.Fill((y, x) => y * w + x).Suffle();
				for (int j = 0; j < size; j++)
				{
					cnt[j, arr[j / w, j % w]]++;
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
		/// 对 <see cref="ArrayUtil.Suffle{T}(T[,,])"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestSuffle_3()
		{
			int w = 2;
			int h = 2;
			int d = 3;
			int size = w * h * d;
			int[,,] arr = new int[d, h, w];
			int[,] cnt = new int[size, size];
			int loop = 30000;
			for (int i = 0; i < loop; i++)
			{
				arr.Fill((z, y, x) => z * w * h + y * w + x).Suffle();
				for (int j = 0; j < size; j++)
				{
					cnt[j, arr[j / (w * h), j / w % h, j % w]]++;
				}
			}
			double min = loop / size * 0.9;
			double max = loop / size * 1.1;
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
	}
}