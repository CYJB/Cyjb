using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="CollectionUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCollectionUtil
	{
		/// <summary>
		/// 对 <see cref="CollectionUtil.AddRange"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestAddRange()
		{
			LinkedList<int> list = new();
			list.AddRange(new int[] { 1, 2, 3 });

			int[] expected = { 1, 2, 3 };
			CollectionAssert.AreEqual(expected, list.ToArray());
		}
	}
}
