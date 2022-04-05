using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections
{
	/// <summary>
	/// <see cref="EnumerableUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestEnumerableUtil
	{
		/// <summary>
		/// 对 <see cref="EnumerableUtil.IsDistinct"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsDistinct()
		{
			Assert.IsTrue(new int[] { }.IsDistinct());
			Assert.IsTrue(new int[] { 1, 2, 3 }.IsDistinct());
			Assert.IsFalse(new int[] { 1, 2, 3, 3 }.IsDistinct());
		}
	}
}
