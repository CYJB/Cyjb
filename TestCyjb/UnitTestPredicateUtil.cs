using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="PredicateUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestPredicateUtil
	{
		/// <summary>
		/// 对 <see cref="PredicateUtil"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestPredicateUtil()
		{
			Predicate<int> isOdd = value => (value & 0x1) == 1;
			Predicate<int> isGt10 = value => value > 10;
			Predicate<int> isLt20 = value => value < 20;

			Predicate<int> predicate = isOdd.And(isGt10);
			Assert.IsFalse(predicate(1));
			Assert.IsFalse(predicate(2));
			Assert.IsFalse(predicate(12));
			Assert.IsTrue(predicate(13));

			predicate = isOdd.And(isGt10, isLt20);
			Assert.IsFalse(predicate(1));
			Assert.IsFalse(predicate(2));
			Assert.IsFalse(predicate(12));
			Assert.IsTrue(predicate(13));
			Assert.IsFalse(predicate(21));
			Assert.IsFalse(predicate(22));

			predicate = isOdd.Or(isGt10);
			Assert.IsTrue(predicate(1));
			Assert.IsFalse(predicate(2));
			Assert.IsTrue(predicate(12));
			Assert.IsTrue(predicate(13));

			predicate = isOdd.Or(isGt10, isLt20);
			Assert.IsTrue(predicate(1));
			Assert.IsTrue(predicate(2));
			Assert.IsTrue(predicate(12));
			Assert.IsTrue(predicate(13));

			predicate = isOdd.Not();
			Assert.IsFalse(predicate(1));
			Assert.IsTrue(predicate(2));

			Assert.IsTrue(PredicateUtil.True<int>()(12));
			Assert.IsFalse(PredicateUtil.False<int>()(12));
		}
	}
}
