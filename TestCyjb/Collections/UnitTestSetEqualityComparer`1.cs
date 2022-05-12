using System.Collections.Generic;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="SetEqualityComparer{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestSetEqualityComparer
	{
		/// <summary>
		/// 对 <see cref="SetEqualityComparer{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestSetEqualityComparer()
		{
			IEqualityComparer<ISet<int>> comparer = SetEqualityComparer<int>.Default;
			SortedSet<int> a = new() { 1, 2, 3 };
			HashSet<int> b = new() { 3, 2, 1 };
			Assert.IsTrue(comparer.Equals(a, b));
			Assert.IsTrue(comparer.GetHashCode(a) == comparer.GetHashCode(b));

			b.Add(0);
			Assert.IsFalse(comparer.Equals(a, b));
			Assert.IsFalse(comparer.GetHashCode(a) == comparer.GetHashCode(b));
		}
	}
}
