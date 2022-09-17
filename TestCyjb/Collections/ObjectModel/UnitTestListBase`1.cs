using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="ListBase{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestListBase
	{
		private class TestList : ListBase<int>
		{
			private readonly List<int> items = new();
			public override int Count => items.Count;
			public override int IndexOf(int item)
			{
				return items.IndexOf(item);
			}
			protected override void InsertItem(int index, int item)
			{
				items.Insert(index, item);
			}
			protected override void RemoveItem(int index)
			{
				items.RemoveAt(index);
			}
			protected override int GetItemAt(int index)
			{
				return items[index];
			}
			protected override void SetItemAt(int index, int item)
			{
				items[index] = item;
			}
			public override void Clear()
			{
				items.Clear();
			}
			public override IEnumerator<int> GetEnumerator()
			{
				return items.GetEnumerator();
			}
		}

		/// <summary>
		/// 对 <see cref="ListBase{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestListBase()
		{
			TestList list = new();
			Assert.AreEqual(0, list.Count);
			Assert.IsFalse(list.Contains(1));

			list.Add(1);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(1, list[0]);
			Assert.IsTrue(list.Contains(1));
			CollectionAssert.AreEqual(new int[] { 1 }, list.ToArray());

			list.Add(2);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
			Assert.IsTrue(list.Contains(1));
			Assert.IsTrue(list.Contains(2));
			CollectionAssert.AreEqual(new int[] { 1, 2 }, list.ToArray());

			list.Insert(0, 3);
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(3, list[0]);
			Assert.AreEqual(1, list[1]);
			Assert.AreEqual(2, list[2]);
			Assert.IsTrue(list.Contains(1));
			Assert.IsTrue(list.Contains(2));
			Assert.AreEqual(0, list.IndexOf(3));
			CollectionAssert.AreEqual(new int[] { 3, 1, 2 }, list.ToArray());

			Assert.IsTrue(list.Remove(1));
			Assert.AreEqual(2, list.Count);
			Assert.IsFalse(list.Contains(1));
			Assert.IsTrue(list.Contains(2));
			Assert.IsTrue(list.Contains(3));
			CollectionAssert.AreEqual(new int[] { 3, 2 }, list.ToArray());

			list.Clear();
			Assert.AreEqual(0, list.Count);
			Assert.IsFalse(list.Contains(1));
			Assert.IsFalse(list.Contains(2));
			CollectionAssert.AreEqual(Array.Empty<int>(), list.ToArray());
		}
	}
}
