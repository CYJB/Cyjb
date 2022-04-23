using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Cyjb.Test.Collections;
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
			int[] values = new[] { 1, 2, 3 };
			TestCollection collection = new();
			List<int> expected = new();

			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Add(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Remove(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)collection).Clear());

			CollectionTest.Test(expected, collection, true, values);

			for (int i = 0; i < values.Length; i++)
			{
				collection.TestAdd(values[i]);
				expected.Add(values[i]);
				CollectionTest.Test(expected, collection, true, values);
			}
		}
	}
}
