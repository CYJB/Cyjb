using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="ReadOnlyCollectionBase{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestReadOnlyCollectionBase
	{
		private class TestCollection : ReadOnlyCollectionBase<int>
		{
			private readonly List<int> items = new();
			public override int Count => items.Count;
			public override bool Contains(int item)
			{
				return items.Contains(item);
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
		/// 对 <see cref="ReadOnlyCollectionBase{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestReadOnlyCollectionBase()
		{
			TestCollection collection = new();
			Assert.IsTrue(((ICollection<int>)collection).IsReadOnly);
			Assert.AreEqual(0, collection.Count);
			Assert.IsFalse(collection.Contains(1));

			collection.TestAdd(1);
			Assert.AreEqual(1, collection.Count);
			Assert.IsTrue(collection.Contains(1));
			CollectionAssert.AreEqual(new int[] { 1 }, collection.ToArray());

			collection.TestAdd(2);
			Assert.AreEqual(2, collection.Count);
			Assert.IsTrue(collection.Contains(1));
			Assert.IsTrue(collection.Contains(2));
			CollectionAssert.AreEqual(new int[] { 1, 2 }, collection.ToArray());

			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Add(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Remove(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Clear());
		}
	}
}
