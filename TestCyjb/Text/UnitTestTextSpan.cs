using System;
using Cyjb.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Text
{
	/// <summary>
	/// <see cref="TextSpan"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTextSpan
	{
		/// <summary>
		/// 对 <see cref="TextSpan"/> 的构造函数进行测试。
		/// </summary>
		[TestMethod]
		public void TestConstructor()
		{
			TextSpan span = new(10, 20);
			Assert.AreEqual(10, span.Start);
			Assert.AreEqual(10, span.Length);
			Assert.AreEqual(20, span.End);
			Assert.IsFalse(span.IsEmpty);

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TextSpan(-1, 10));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TextSpan(1, -1));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new TextSpan(10, 1));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.Contains(int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(10, 20, 9, false)]
		[DataRow(10, 20, 10, true)]
		[DataRow(10, 20, 19, true)]
		[DataRow(10, 20, 20, false)]
		[DataRow(10, 10, 9, false)]
		[DataRow(10, 10, 10, false)]
		[DataRow(10, 10, 11, false)]
		public void TestContainsInt(int start, int end, int position, bool expected)
		{

			Assert.AreEqual(expected, new TextSpan(start, end).Contains(position));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.Contains(TextSpan)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 90, false)]
		[DataRow(9, 10, false)]
		[DataRow(9, 11, false)]
		[DataRow(9, 20, false)]
		[DataRow(9, 21, false)]
		[DataRow(9, 29, false)]
		[DataRow(10, 10, true)]
		[DataRow(10, 20, true)]
		[DataRow(10, 21, false)]
		[DataRow(18, 18, true)]
		[DataRow(18, 19, true)]
		[DataRow(18, 20, true)]
		[DataRow(18, 21, false)]
		[DataRow(20, 20, false)]
		[DataRow(20, 21, false)]
		[DataRow(21, 21, false)]
		[DataRow(21, 22, false)]
		public void TestContainsSpan(int otherStart, int otherEnd, bool expected)
		{
			TextSpan target = new(10, 20);
			Assert.AreEqual(expected, target.Contains(new TextSpan(otherStart, otherEnd)));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.OverlapsWith"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 9, false)]
		[DataRow(9, 10, false)]
		[DataRow(9, 11, true)]
		[DataRow(9, 20, true)]
		[DataRow(9, 21, true)]
		[DataRow(9, 29, true)]
		[DataRow(10, 10, false)]
		[DataRow(10, 20, true)]
		[DataRow(10, 21, true)]
		[DataRow(18, 18, false)]
		[DataRow(18, 19, true)]
		[DataRow(18, 20, true)]
		[DataRow(18, 21, true)]
		[DataRow(20, 20, false)]
		[DataRow(20, 21, false)]
		[DataRow(21, 21, false)]
		[DataRow(21, 22, false)]
		public void TestOverlapsWith(int otherStart, int otherEnd, bool expected)
		{
			TextSpan target = new(10, 20);
			Assert.AreEqual(expected, target.OverlapsWith(new TextSpan(otherStart, otherEnd)));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.Overlap"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 9, -1, 0)]
		[DataRow(9, 10, -1, 0)]
		[DataRow(9, 11, 10, 11)]
		[DataRow(9, 20, 10, 20)]
		[DataRow(9, 21, 10, 20)]
		[DataRow(9, 29, 10, 20)]
		[DataRow(10, 10, -1, 0)]
		[DataRow(10, 11, 10, 11)]
		[DataRow(10, 20, 10, 20)]
		[DataRow(10, 30, 10, 20)]
		[DataRow(18, 18, -1, 0)]
		[DataRow(18, 19, 18, 19)]
		[DataRow(18, 20, 18, 20)]
		[DataRow(18, 21, 18, 20)]
		[DataRow(20, 20, -1, 0)]
		[DataRow(20, 21, -1, 0)]
		[DataRow(21, 21, -1, 0)]
		[DataRow(21, 22, -1, 0)]
		public void TestOverlap(int otherStart, int otherEnd, int expectedStart, int expectedEnd)
		{
			TextSpan target = new(10, 20);
			TextSpan? result = target.Overlap(new TextSpan(otherStart, otherEnd));
			if (expectedStart < 0)
			{
				Assert.IsNull(result);
			}
			else
			{
				Assert.AreEqual(new TextSpan(expectedStart, expectedEnd), result);
			}
		}

		/// <summary>
		/// 对 <see cref="TextSpan.IntersectsWith(int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(10, 20, 9, false)]
		[DataRow(10, 20, 10, true)]
		[DataRow(10, 20, 19, true)]
		[DataRow(10, 20, 20, true)]
		[DataRow(10, 10, 9, false)]
		[DataRow(10, 10, 10, true)]
		[DataRow(10, 10, 11, false)]
		public void TestIntersectsWithInt(int start, int end, int position, bool expected)
		{

			Assert.AreEqual(expected, new TextSpan(start, end).IntersectsWith(position));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.IntersectsWith(TextSpan)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 9, false)]
		[DataRow(9, 10, true)]
		[DataRow(9, 11, true)]
		[DataRow(9, 20, true)]
		[DataRow(9, 21, true)]
		[DataRow(9, 29, true)]
		[DataRow(10, 10, true)]
		[DataRow(10, 20, true)]
		[DataRow(10, 21, true)]
		[DataRow(18, 18, true)]
		[DataRow(18, 19, true)]
		[DataRow(18, 20, true)]
		[DataRow(18, 21, true)]
		[DataRow(20, 20, true)]
		[DataRow(20, 21, true)]
		[DataRow(21, 21, false)]
		[DataRow(21, 22, false)]
		public void TestIntersectsWithSpan(int otherStart, int otherEnd, bool expected)
		{
			TextSpan target = new(10, 20);
			Assert.AreEqual(expected, target.IntersectsWith(new TextSpan(otherStart, otherEnd)));
		}

		/// <summary>
		/// 对 <see cref="TextSpan.Intersection"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 9, -1, 0)]
		[DataRow(9, 10, 10, 10)]
		[DataRow(9, 11, 10, 11)]
		[DataRow(9, 20, 10, 20)]
		[DataRow(9, 21, 10, 20)]
		[DataRow(9, 29, 10, 20)]
		[DataRow(10, 10, 10, 10)]
		[DataRow(10, 11, 10, 11)]
		[DataRow(10, 20, 10, 20)]
		[DataRow(10, 30, 10, 20)]
		[DataRow(18, 18, 18, 18)]
		[DataRow(18, 19, 18, 19)]
		[DataRow(18, 20, 18, 20)]
		[DataRow(18, 21, 18, 20)]
		[DataRow(20, 20, 20, 20)]
		[DataRow(20, 21, 20, 20)]
		[DataRow(21, 21, -1, 0)]
		[DataRow(21, 22, -1, 0)]
		public void TestIntersection(int otherStart, int otherEnd, int expectedStart, int expectedEnd)
		{
			TextSpan target = new(10, 20);
			TextSpan? result = target.Intersection(new TextSpan(otherStart, otherEnd));
			if (expectedStart < 0)
			{
				Assert.IsNull(result);
			}
			else
			{
				Assert.AreEqual(new TextSpan(expectedStart, expectedEnd), result);
			}
		}

		/// <summary>
		/// 对 <see cref="TextSpan.Union"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(9, 9, -1, 0)]
		[DataRow(9, 10, 9, 20)]
		[DataRow(9, 11, 9, 20)]
		[DataRow(9, 20, 9, 20)]
		[DataRow(9, 21, 9, 21)]
		[DataRow(9, 29, 9, 29)]
		[DataRow(10, 10, 10, 20)]
		[DataRow(10, 11, 10, 20)]
		[DataRow(10, 20, 10, 20)]
		[DataRow(10, 30, 10, 30)]
		[DataRow(18, 18, 10, 20)]
		[DataRow(18, 19, 10, 20)]
		[DataRow(18, 20, 10, 20)]
		[DataRow(18, 21, 10, 21)]
		[DataRow(20, 20, 10, 20)]
		[DataRow(20, 21, 10, 21)]
		[DataRow(21, 21, -1, 0)]
		[DataRow(21, 22, -1, 0)]
		public void TestUnion(int otherStart, int otherEnd, int expectedStart, int expectedEnd)
		{
			TextSpan target = new(10, 20);
			TextSpan? result = target.Union(new TextSpan(otherStart, otherEnd));
			if (expectedStart < 0)
			{
				Assert.IsNull(result);
			}
			else
			{
				Assert.AreEqual(new TextSpan(expectedStart, expectedEnd), result);
			}
		}

		/// <summary>
		/// 对 <see cref="TextSpan"/> 的比较方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(10, 20, 9, 9, 1)]
		[DataRow(10, 20, 9, 10, 1)]
		[DataRow(10, 20, 9, 11, 1)]
		[DataRow(10, 20, 9, 20, 1)]
		[DataRow(10, 20, 9, 21, 1)]
		[DataRow(10, 20, 9, 29, 1)]
		[DataRow(10, 20, 10, 10, 1)]
		[DataRow(10, 20, 10, 11, 1)]
		[DataRow(10, 20, 10, 20, 0)]
		[DataRow(10, 20, 10, 30, -1)]
		[DataRow(10, 20, 18, 18, -1)]
		[DataRow(10, 20, 18, 19, -1)]
		[DataRow(10, 20, 18, 20, -1)]
		[DataRow(10, 20, 18, 21, -1)]
		[DataRow(10, 20, 20, 20, -1)]
		[DataRow(10, 20, 20, 21, -1)]
		[DataRow(10, 20, 21, 21, -1)]
		[DataRow(10, 20, 21, 22, -1)]
		[DataRow(10, 10, 10, 10, 0)]
		public void TestCompare(int leftStart, int leftEnd, int rightStart, int rightEnd, int expected)
		{
			TextSpan left = new(leftStart, leftEnd);
			TextSpan right = new(rightStart, rightEnd);
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
}
