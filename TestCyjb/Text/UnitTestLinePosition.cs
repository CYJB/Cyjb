using System;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text;

/// <summary>
/// <see cref="LinePosition"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestLinePosition
{
	/// <summary>
	/// 对 <see cref="LinePosition"/> 的构造函数进行测试。
	/// </summary>
	[TestMethod]
	public void TestConstructor()
	{
		LinePosition pos = new(10, 20);
		Assert.AreEqual(10, pos.Line);
		Assert.AreEqual(20, pos.Character);
		Assert.AreEqual(20, pos.Column);

		pos = new(10, 20, 24);
		Assert.AreEqual(10, pos.Line);
		Assert.AreEqual(20, pos.Character);
		Assert.AreEqual(24, pos.Column);

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinePosition(-1, 10));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinePosition(1, -1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinePosition(10, 1, -1));
	}

	/// <summary>
	/// 对 <see cref="LinePosition"/> 的比较方法进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow(10, 20, 9, 30, 1)]
	[DataRow(10, 20, 10, 10, 1)]
	[DataRow(10, 20, 10, 20, 0)]
	[DataRow(10, 20, 10, 30, -1)]
	public void TestCompare(int leftLine, int leftCharacter, int rightLine, int rightCharacter, int expected)
	{
		LinePosition left = new(leftLine, leftCharacter);
		LinePosition right = new(rightLine, rightCharacter);
		if (expected < 0)
		{
			Assert.IsTrue(left.CompareTo(right) < 0);
			Assert.IsTrue(left < right);
			Assert.IsTrue(left <= right);
			Assert.IsFalse(left > right);
			Assert.IsFalse(left >= right);

			Assert.IsFalse(left.Equals(right));
			Assert.IsFalse(left.GetHashCode() == right.GetHashCode());
			Assert.IsFalse(left == right);
			Assert.IsTrue(left != right);
		}
		else if (expected > 0)
		{
			Assert.IsTrue(left.CompareTo(right) > 0);
			Assert.IsFalse(left < right);
			Assert.IsFalse(left <= right);
			Assert.IsTrue(left > right);
			Assert.IsTrue(left >= right);

			Assert.IsFalse(left.Equals(right));
			Assert.IsFalse(left.GetHashCode() == right.GetHashCode());
			Assert.IsFalse(left == right);
			Assert.IsTrue(left != right);
		}
		else
		{
			Assert.IsTrue(left.CompareTo(right) == 0);
			Assert.IsFalse(left < right);
			Assert.IsTrue(left <= right);
			Assert.IsFalse(left > right);
			Assert.IsTrue(left >= right);

			Assert.IsTrue(left.Equals(right));
			Assert.IsTrue(left.GetHashCode() == right.GetHashCode());
			Assert.IsTrue(left == right);
			Assert.IsFalse(left != right);
		}
	}
}
