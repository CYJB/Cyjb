using System;
using System.Collections.Generic;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="ReadOnlySetBase{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestReadOnlySetBase
	{
		private class TestSet : ReadOnlySetBase<int>
		{
			private readonly HashSet<int> items = new();
			public override int Count => items.Count;
			public override bool Contains(int item)
			{
				return items.Contains(item);
			}
			public override IEnumerator<int> GetEnumerator()
			{
				return items.GetEnumerator();
			}
			public bool TestAdd(int item)
			{
				return items.Add(item);
			}
		}

		/// <summary>
		/// 对 <see cref="ReadOnlySetBase{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestReadOnlySetBase()
		{
			TestSet set = new();
			Assert.AreEqual(0, set.Count);
			Assert.IsFalse(set.Contains(1));

			Assert.IsTrue(set.TestAdd(1));
			Assert.AreEqual(1, set.Count);
			Assert.IsTrue(set.Contains(1));
			Assert.IsTrue(set.SetEquals(new int[] { 1 }));

			Assert.IsTrue(set.TestAdd(3));
			Assert.AreEqual(2, set.Count);
			Assert.IsTrue(set.Contains(1));
			Assert.IsTrue(set.Contains(3));
			Assert.IsTrue(set.SetEquals(new int[] { 1, 3 }));

			Assert.IsFalse(set.IsProperSubsetOf(new int[] { 1 }));
			Assert.IsFalse(set.IsProperSubsetOf(new int[] { 1, 1 }));
			Assert.IsFalse(set.IsProperSubsetOf(new int[] { 1, 3 }));
			Assert.IsFalse(set.IsProperSubsetOf(new int[] { 1, 3, 3 }));
			Assert.IsTrue(set.IsProperSubsetOf(new int[] { 1, 2, 3 }));
			Assert.IsTrue(set.IsProperSubsetOf(new int[] { 1, 2, 2, 3 }));
			Assert.IsFalse(set.IsSubsetOf(new int[] { 1 }));
			Assert.IsFalse(set.IsSubsetOf(new int[] { 1, 1 }));
			Assert.IsTrue(set.IsSubsetOf(new int[] { 1, 3 }));
			Assert.IsTrue(set.IsSubsetOf(new int[] { 1, 3, 3 }));
			Assert.IsTrue(set.IsSubsetOf(new int[] { 1, 2, 3 }));
			Assert.IsTrue(set.IsSubsetOf(new int[] { 1, 2, 2, 3 }));

			Assert.IsTrue(set.IsProperSupersetOf(new int[] { 1 }));
			Assert.IsTrue(set.IsProperSupersetOf(new int[] { 1, 1 }));
			Assert.IsFalse(set.IsProperSupersetOf(new int[] { 1, 3 }));
			Assert.IsFalse(set.IsProperSupersetOf(new int[] { 1, 3, 3 }));
			Assert.IsFalse(set.IsProperSupersetOf(new int[] { 1, 2, 3 }));
			Assert.IsFalse(set.IsProperSupersetOf(new int[] { 1, 2, 2, 3 }));
			Assert.IsTrue(set.IsSupersetOf(new int[] { 1 }));
			Assert.IsTrue(set.IsSupersetOf(new int[] { 1, 1 }));
			Assert.IsTrue(set.IsSupersetOf(new int[] { 1, 3 }));
			Assert.IsTrue(set.IsSupersetOf(new int[] { 1, 3, 3 }));
			Assert.IsFalse(set.IsSupersetOf(new int[] { 1, 2, 3 }));
			Assert.IsFalse(set.IsSupersetOf(new int[] { 1, 2, 2, 3 }));

			Assert.IsTrue(set.Overlaps(new int[] { 1 }));
			Assert.IsTrue(set.Overlaps(new int[] { 1, 1 }));
			Assert.IsTrue(set.Overlaps(new int[] { 1, 2 }));
			Assert.IsTrue(set.Overlaps(new int[] { 1, 2, 2 }));
			Assert.IsTrue(set.Overlaps(new int[] { 1, 2, 3 }));
			Assert.IsTrue(set.Overlaps(new int[] { 1, 2, 3, 3 }));
			Assert.IsFalse(set.Overlaps(new int[] { 2 }));
			Assert.IsFalse(set.Overlaps(new int[] { 2, 2 }));

			Assert.IsFalse(set.SetEquals(new int[] { 1 }));
			Assert.IsFalse(set.SetEquals(new int[] { 1, 1 }));
			Assert.IsTrue(set.SetEquals(new int[] { 1, 3 }));
			Assert.IsTrue(set.SetEquals(new int[] { 1, 3, 3 }));
			Assert.IsTrue(set.SetEquals(new int[] { 1, 3, 3, 1 }));
			Assert.IsFalse(set.SetEquals(new int[] { 1, 2, 3 }));

			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)set).Add(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)set).Remove(1));
			Assert.ThrowsException<NotSupportedException>(() => ((ICollection<int>)set).Clear());
			Assert.ThrowsException<NotSupportedException>(() => ((ISet<int>)set).UnionWith(new int[] { 1 }));
			Assert.ThrowsException<NotSupportedException>(() => ((ISet<int>)set).IntersectWith(new int[] { 1 }));
			Assert.ThrowsException<NotSupportedException>(() => ((ISet<int>)set).SymmetricExceptWith(new int[] { 1 }));
		}
	}
}
