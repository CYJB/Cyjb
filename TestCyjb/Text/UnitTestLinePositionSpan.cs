using System;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="LinePositionSpan"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestLinePositionSpan
{
	/// <summary>
	/// 对 <see cref="LinePositionSpan"/> 的构造函数进行测试。
	/// </summary>
	[TestMethod]
	public void TestConstructor()
	{
		LinePosition pos1 = new(10, 20);
		LinePosition pos2 = new(10, 22);
		LinePositionSpan span = new(pos1, pos2);
		Assert.AreEqual(new LinePosition(10, 20), span.Start);
		Assert.AreEqual(new LinePosition(10, 22), span.End);
		Assert.AreEqual(new LinePositionSpan(new LinePosition(10, 20), new LinePosition(10, 22)), span);

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinePositionSpan(pos2, pos1));
	}
}
