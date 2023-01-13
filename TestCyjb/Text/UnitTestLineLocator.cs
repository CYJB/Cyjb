using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="LineLocator"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestLineLocator
{
	/// <summary>
	/// 对 <see cref="LineLocator.GetPosition"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestGetPosition()
	{
		LineLocator locator = new();
		Assert.AreEqual(new LinePosition(1, 0, 1), locator.GetPosition(0));
		Assert.AreEqual(new LinePosition(1, 1, 2), locator.GetPosition(1));
		Assert.AreEqual(new LinePosition(1, 10, 11), locator.GetPosition(10));

		locator.Read("abcd一二三四\t1\t2\t3\t中文\t123\t1234\n\t1\r");
		/*
abcd一二三四	1	2	1	中文	123	1234\n
	1\r");
		*/
		Assert.AreEqual(new LinePosition(1, 0, 1), locator.GetPosition(0));
		Assert.AreEqual(new LinePosition(1, 1, 2), locator.GetPosition(1));
		Assert.AreEqual(new LinePosition(1, 2, 3), locator.GetPosition(2));
		Assert.AreEqual(new LinePosition(1, 3, 4), locator.GetPosition(3));
		Assert.AreEqual(new LinePosition(1, 4, 5), locator.GetPosition(4));
		Assert.AreEqual(new LinePosition(1, 5, 7), locator.GetPosition(5));
		Assert.AreEqual(new LinePosition(1, 6, 9), locator.GetPosition(6));
		Assert.AreEqual(new LinePosition(1, 7, 11), locator.GetPosition(7));
		Assert.AreEqual(new LinePosition(1, 8, 13), locator.GetPosition(8));
		Assert.AreEqual(new LinePosition(1, 9, 17), locator.GetPosition(9));
		Assert.AreEqual(new LinePosition(1, 10, 18), locator.GetPosition(10));
		Assert.AreEqual(new LinePosition(1, 11, 21), locator.GetPosition(11));
		Assert.AreEqual(new LinePosition(1, 12, 22), locator.GetPosition(12));
		Assert.AreEqual(new LinePosition(1, 13, 25), locator.GetPosition(13));
		Assert.AreEqual(new LinePosition(1, 14, 26), locator.GetPosition(14));
		Assert.AreEqual(new LinePosition(1, 15, 29), locator.GetPosition(15));
		Assert.AreEqual(new LinePosition(1, 16, 31), locator.GetPosition(16));
		Assert.AreEqual(new LinePosition(1, 17, 33), locator.GetPosition(17));
		Assert.AreEqual(new LinePosition(1, 18, 37), locator.GetPosition(18));
		Assert.AreEqual(new LinePosition(1, 19, 38), locator.GetPosition(19));
		Assert.AreEqual(new LinePosition(1, 20, 39), locator.GetPosition(20));
		Assert.AreEqual(new LinePosition(1, 21, 40), locator.GetPosition(21));
		Assert.AreEqual(new LinePosition(1, 22, 41), locator.GetPosition(22));
		Assert.AreEqual(new LinePosition(1, 23, 42), locator.GetPosition(23));
		Assert.AreEqual(new LinePosition(1, 24, 43), locator.GetPosition(24));
		Assert.AreEqual(new LinePosition(1, 25, 44), locator.GetPosition(25));
		Assert.AreEqual(new LinePosition(1, 26, 45), locator.GetPosition(26));
		Assert.AreEqual(new LinePosition(2, 0, 1), locator.GetPosition(27));
		Assert.AreEqual(new LinePosition(2, 1, 5), locator.GetPosition(28));
		Assert.AreEqual(new LinePosition(2, 2, 6), locator.GetPosition(29));
		Assert.AreEqual(new LinePosition(2, 3, 6), locator.GetPosition(30));
		Assert.AreEqual(new LinePosition(2, 4, 6), locator.GetPosition(31));

		locator.Read("1234\r");
		Assert.AreEqual(new LinePosition(3, 0, 1), locator.GetPosition(30));
		Assert.AreEqual(new LinePosition(3, 1, 2), locator.GetPosition(31));
		Assert.AreEqual(new LinePosition(3, 2, 3), locator.GetPosition(32));
		Assert.AreEqual(new LinePosition(3, 3, 4), locator.GetPosition(33));
		Assert.AreEqual(new LinePosition(3, 4, 5), locator.GetPosition(34));
		Assert.AreEqual(new LinePosition(3, 5, 5), locator.GetPosition(35));

		locator.Read("1234\n");
		Assert.AreEqual(new LinePosition(4, 0, 1), locator.GetPosition(35));
		Assert.AreEqual(new LinePosition(4, 1, 2), locator.GetPosition(36));
		Assert.AreEqual(new LinePosition(4, 2, 3), locator.GetPosition(37));
		Assert.AreEqual(new LinePosition(4, 3, 4), locator.GetPosition(38));
		Assert.AreEqual(new LinePosition(4, 4, 5), locator.GetPosition(39));
		Assert.AreEqual(new LinePosition(5, 0, 1), locator.GetPosition(40));

		locator.Read("1234");
		Assert.AreEqual(new LinePosition(5, 0, 1), locator.GetPosition(40));
		Assert.AreEqual(new LinePosition(5, 1, 2), locator.GetPosition(41));
		Assert.AreEqual(new LinePosition(5, 2, 3), locator.GetPosition(42));
		Assert.AreEqual(new LinePosition(5, 3, 4), locator.GetPosition(43));
		Assert.AreEqual(new LinePosition(5, 4, 5), locator.GetPosition(44));
	}
}
