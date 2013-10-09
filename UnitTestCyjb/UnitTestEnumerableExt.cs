using System.Linq;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.EnumerableExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestEnumerableExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.EnumerableExt.Iterative"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIterative()
		{
			AssertExt.AreEqual(new int[0], new int[0].Iterative().ToArray());
			AssertExt.AreEqual(new int[0], new int[] { 0, 1, 2, 3 }.Iterative().ToArray());
			AssertExt.AreEqual(new int[] { 0 }, new int[] { 0, 0, 0, 0 }.Iterative().ToArray());
			AssertExt.AreEqual(new int[] { 0, 1, 2 }, new int[] { 0, 0, 1, 1, 2, 2 }.Iterative().ToArray());
			AssertExt.AreEqual(new int[] { 1, 2, 0 }, new int[] { 0, 1, 1, 2, 2, 3, 4, 5, 6, 0 }.Iterative().ToArray());
		}
	}
}
