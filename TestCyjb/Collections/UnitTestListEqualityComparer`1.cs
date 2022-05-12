using System.Collections.Generic;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="ListEqualityComparer{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestListEqualityComparer
	{
		/// <summary>
		/// 对 <see cref="ListEqualityComparer{T}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestListEqualityComparer()
		{
			IEqualityComparer<IList<int>> comparer = ListEqualityComparer<int>.Default;
			int[] a = { 1, 2, 3 };
			List<int> b = new() { 1, 2, 3 };
			Assert.IsTrue(comparer.Equals(a, b));
			Assert.IsTrue(comparer.GetHashCode(a) == comparer.GetHashCode(b));

			(b[0], b[1]) = (b[1], b[0]);
			Assert.IsFalse(comparer.Equals(a, b));
			Assert.IsFalse(comparer.GetHashCode(a) == comparer.GetHashCode(b));
		}
	}
}
