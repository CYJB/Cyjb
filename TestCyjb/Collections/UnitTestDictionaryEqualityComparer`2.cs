using System.Collections.Generic;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="DictionaryEqualityComparer{TKey, TValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestDictionaryEqualityComparer
	{
		/// <summary>
		/// 对 <see cref="DictionaryEqualityComparer{TKey, TValue}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestDictionaryEqualityComparer()
		{
			IEqualityComparer<IReadOnlyDictionary<int, string>> comparer =
				DictionaryEqualityComparer<int, string>.Default;
			SortedDictionary<int, string> a = new()
			{
				{ 3, "A" },
				{ 2, "B" },
				{ 1, "C" },
			};
			Dictionary<int, string> b = new() {
				{ 3, "A" },
				{ 2, "B" },
				{ 1, "C" },
			};
			Assert.IsTrue(comparer.Equals(a, b));
			Assert.IsTrue(comparer.GetHashCode(a) == comparer.GetHashCode(b));

			b.Add(0, "D");
			Assert.IsFalse(comparer.Equals(a, b));
			Assert.IsFalse(comparer.GetHashCode(a) == comparer.GetHashCode(b));
		}
	}
}
