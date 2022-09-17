using System.Collections.Generic;
using Cyjb.Collections.ObjectModel;
using Cyjb.Test.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="CollectionBase{T}"/> 类的单元测试。
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
		/// 对 <see cref="CollectionBase{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestCollectionBase()
		{
			TestCollection collection = new();
			CollectionTest.Test(collection, CollectionTestType.Unordered, 0, 1, 2, 3, 4, 5, 6);
		}
	}
}
