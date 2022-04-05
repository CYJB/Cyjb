using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="BitList"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestBitList
	{
		/// <summary>
		/// 对 <see cref="BitList"/> 的添加和删除进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddRemove()
		{
			BitList list = new();
			List<bool> expected = new();

			list.Add(true);
			expected.Add(true);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.Add(false);
			expected.Add(false);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.AddRange(new bool[] { false, true, false });
			expected.AddRange(new bool[] { false, true, false });
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.AddRange(new byte[] { 0x25 });
			expected.AddRange(new bool[] { true, false, true, false, false, true, false, false });
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);
			CollectionAssert.AreEqual(expected.ToArray(), list.ToArray());

			list.RemoveAt(11);
			expected.RemoveAt(11);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.InsertRange(10, new uint[] { 0x76D24266, 0x97FE3308 });
			expected.InsertRange(10, new bool[] {
				false, true, true, false, false, true, true, false,
				false, true, false, false, false, false, true, false,
				false, true, false, false, true, false, true, true,
				false, true, true, false, true, true, true, false,
				false, false, false, true, false, false, false, false,
				true, true, false, false, true, true, false, false,
				false, true, true, true, true, true, true, true,
				true, true, true, false, true, false, false, true,
			});
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.RemoveRange(37, 17);
			expected.RemoveRange(37, 17);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			list.Fill(11, 3, false);
			for (int i = 11; i < 14; i++)
			{
				expected[i] = false;
			}
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			bool[] insertValue = RandomArray(57);
			insertValue.Fill((idx) => Random.Shared.NextBoolean());
			list.InsertRange(17, insertValue);
			expected.InsertRange(17, insertValue);

			list.Insert(18, true);
			expected.Insert(18, true);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			// 移除单个位
			list.RemoveAt(18);
			expected.RemoveAt(18);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			// 普通范围移除
			list.RemoveRange(30, 18);
			expected.RemoveRange(30, 18);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			insertValue = RandomArray(18);
			list.InsertRange(30, insertValue);
			expected.InsertRange(30, insertValue);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			// 起始对齐到 32 的范围移除（highBits 为 0）
			list.RemoveRange(32, 13);
			expected.RemoveRange(32, 13);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			insertValue = RandomArray(13);
			list.InsertRange(32, insertValue);
			expected.InsertRange(32, insertValue);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			// 结束对齐到 32 的范围移除（highBits 和 lowBits 都为 0）
			list.RemoveRange(30, 34);
			expected.RemoveRange(30, 34);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			insertValue = RandomArray(34);
			list.InsertRange(30, insertValue);
			expected.InsertRange(30, insertValue);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			// 范围删除（lowBits 为 0）
			list.RemoveRange(13, 36);
			expected.RemoveRange(13, 36);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);

			insertValue = RandomArray(36);
			list.InsertRange(13, insertValue);
			expected.InsertRange(13, insertValue);
			Assert.AreEqual(expected.Count, list.Count);
			CollectionAssert.AreEqual(expected, list);
		}

		private static bool[] RandomArray(int count)
		{
			bool[] values = new bool[count];
			return values.Fill((idx) => Random.Shared.NextBoolean());
		}

		/// <summary>
		/// 对 <see cref="BitList"/> 的二进制操作进行测试。
		/// </summary>
		[TestMethod]
		public void TestBinary()
		{
			BitList list = new(new uint[] { 0x32A6A26, 0x352B });
			list.Resize(45);
			list.And(new BitList(new uint[] { 0xA21422, 0x1120D7 }));
			Assert.AreEqual(new BitList(new uint[] { 0x220022, 0xD44C003 }).Resize(45), list);

			list.Or(new BitList(new uint[] { 0xA25AB6E7 }));
			Assert.AreEqual(new BitList(new uint[] { 0xA27AB6E7, 0xDD44E003 }).Resize(45), list);

			list.Xor(new BitList(new uint[] { 0x522E8A25, 0x6A451D5 }));
			Assert.AreEqual(new BitList(new uint[] { 0xF0543CC2, 0xC286B1D6 }).Resize(45), list);

			list.Not();
			Assert.AreEqual(new BitList(new uint[] { 0xFABC33D, 0x3D794E29 }).Resize(45), list);

			list.LeftShift(1);
			Assert.AreEqual(new BitList(new uint[] { 0x1F57867A, 0x7AF29C52 }).Resize(45), list);

			list.LeftShift(20);
			Assert.AreEqual(new BitList(new uint[] { 0x67A00000, 0xC521F578 }).Resize(45), list);

			list.RightShift(17);
			Assert.AreEqual(new BitList(new uint[] { 0xABC33D0, 0x6000 }).Resize(45), list);
		}
	}
}
