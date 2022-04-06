using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="ReadOnlyListBase{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestReadOnlyListBase
	{
		private class TestList : ReadOnlyListBase<int>
		{
			private readonly List<int> items = new();
			public override int Count => items.Count;
			public override int IndexOf(int item)
			{
				return items.IndexOf(item);
			}
			protected override int GetItemAt(int index)
			{
				return items[index];
			}
			public override IEnumerator<int> GetEnumerator()
			{
				return items.GetEnumerator();
			}
			public void TestAdd(int item)
			{
				items.Add(item);
			}
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyListBase{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestReadOnlyListBase()
		{
			TestList list = new();
			Assert.IsTrue(((ICollection<int>)list).IsReadOnly);
			Assert.AreEqual(0, list.Count);
			Assert.IsFalse(list.Contains(1));

			list.TestAdd(1);
			Assert.AreEqual(1, list.Count);
			Assert.AreEqual(1, list[0]);
			Assert.IsTrue(list.Contains(1));
			CollectionAssert.AreEqual(new int[] { 1 }, list.ToArray());

			list.TestAdd(2);
			Assert.AreEqual(2, list.Count);
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
			Assert.IsTrue(list.Contains(1));
			Assert.IsTrue(list.Contains(2));
			CollectionAssert.AreEqual(new int[] { 1, 2 }, list.ToArray());

			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)list).Add(3));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)list).Remove(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)list).Clear());
			Assert.ThrowsException<NotSupportedException>(() => ((IList<int>)list).Insert(3, 3));
			Assert.ThrowsException<NotSupportedException>(() => ((IList<int>)list)[4] = 3);
			Assert.ThrowsException<NotSupportedException>(() => ((IList<int>)list).RemoveAt(3));
		}
	}
}
