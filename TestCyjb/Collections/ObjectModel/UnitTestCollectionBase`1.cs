using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="CollectionBase{T}"/> ��ĵ�Ԫ���ԡ�
	/// </summary>
	[TestClass]
	public class UnitTestCollectionBase
	{
		private class TestCollection : CollectionBase<int>
		{
			private readonly List<int> items = new();
			public override int Count => items.Count;

			public override void Clear()
			{
				items.Clear();
			}

			public override bool Contains(int item)
			{
				return items.Contains(item);
			}

			public override IEnumerator<int> GetEnumerator()
			{
				return items.GetEnumerator();
			}

			public override bool Remove(int item)
			{
				return items.Remove(item);
			}

			protected override void AddItem(int item)
			{
				items.Add(item);
			}
		}

		/// <summary>
		/// �� <see cref="CollectionBase{T}"/> ����в��ԡ�
		/// </summary>
		[TestMethod]
		public void TestCollectionBase()
		{
			TestCollection collection = new();
			Assert.AreEqual(0, collection.Count);
			Assert.IsFalse(collection.Contains(1));

			collection.Add(1);
			Assert.AreEqual(1, collection.Count);
			Assert.IsTrue(collection.Contains(1));
			CollectionAssert.AreEqual(new int[] { 1 }, collection.ToArray());

			collection.Add(2);
			Assert.AreEqual(2, collection.Count);
			Assert.IsTrue(collection.Contains(1));
			Assert.IsTrue(collection.Contains(2));
			CollectionAssert.AreEqual(new int[] { 1, 2 }, collection.ToArray());

			Assert.IsTrue(collection.Remove(1));
			Assert.AreEqual(1, collection.Count);
			Assert.IsFalse(collection.Contains(1));
			Assert.IsTrue(collection.Contains(2));
			CollectionAssert.AreEqual(new int[] { 2 }, collection.ToArray());

			collection.Clear();
			Assert.AreEqual(0, collection.Count);
			Assert.IsFalse(collection.Contains(1));
			Assert.IsFalse(collection.Contains(2));
			CollectionAssert.AreEqual(Array.Empty<int>(), collection.ToArray());
		}
	}
}
