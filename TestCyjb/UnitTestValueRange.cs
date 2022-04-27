using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="ValueRange{T}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestValueRange
	{
		/// <summary>
		/// 对 <see cref="ValueRange{T}"/> 的构造函数进行测试。
		/// </summary>
		[TestMethod]
		public void TestConstructor()
		{
			ValueRange<int> range = new(10, 10);
			Assert.AreEqual(10, range.Start);
			Assert.AreEqual(10, range.End);
			{
				var (start, end) = range;
				Assert.AreEqual(10, start);
				Assert.AreEqual(10, end);
			}

			range = new(10, 20);
			Assert.AreEqual(10, range.Start);
			Assert.AreEqual(20, range.End);
			{
				var (start, end) = range;
				Assert.AreEqual(10, start);
				Assert.AreEqual(20, end);
			}

			range = new(-10, 20);
			Assert.AreEqual(-10, range.Start);
			Assert.AreEqual(20, range.End);
			Assert.AreEqual(20, range.End);
			{
				var (start, end) = range;
				Assert.AreEqual(-10, start);
				Assert.AreEqual(20, end);
			}

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ValueRange<int>(1, -1));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ValueRange<int>(10, 1));
		}

		/// <summary>
		/// 对 <see cref="ValueRange<int>.Contains(int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(10, 20, 9, false)]
		[DataRow(10, 20, 10, true)]
		[DataRow(10, 20, 19, true)]
		[DataRow(10, 20, 20, true)]
		[DataRow(10, 20, 21, false)]
		[DataRow(10, 10, 9, false)]
		[DataRow(10, 10, 10, true)]
		[DataRow(10, 10, 11, false)]
		public void TestContainsInt(int start, int end, int position, bool expected)
		{

			Assert.AreEqual(expected, new ValueRange<int>(start, end).Contains(position));
		}

		/// <summary>
		/// 对 <see cref="ValueRange<int>.Contains(ValueRange<int>)"/> 方法进行测试。
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
		[DataRow(20, 20, true)]
		[DataRow(20, 21, false)]
		[DataRow(21, 21, false)]
		[DataRow(21, 22, false)]
		public void TestContainsRange(int otherStart, int otherEnd, bool expected)
		{
			ValueRange<int> target = new(10, 20);
			Assert.AreEqual(expected, target.Contains(new ValueRange<int>(otherStart, otherEnd)));
		}

		/// <summary>
		/// 对 <see cref="ValueRange<int>.OverlapsWith"/> 方法进行测试。
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
		public void TestOverlapsWith(int otherStart, int otherEnd, bool expected)
		{
			ValueRange<int> target = new(10, 20);
			Assert.AreEqual(expected, target.OverlapsWith(new ValueRange<int>(otherStart, otherEnd)));
		}

		/// <summary>
		/// 对 <see cref="ValueRange<int>.Overlap"/> 方法进行测试。
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
		public void TestOverlap(int otherStart, int otherEnd, int expectedStart, int expectedEnd)
		{
			ValueRange<int> target = new(10, 20);
			ValueRange<int>? result = target.Overlap(new ValueRange<int>(otherStart, otherEnd));
			if (expectedStart < 0)
			{
				Assert.IsNull(result);
			}
			else
			{
				Assert.AreEqual(new ValueRange<int>(expectedStart, expectedEnd), result);
			}
		}

		/// <summary>
		/// 对 <see cref="ValueRange{T}"/> 的比较方法进行测试。
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
			ValueRange<int> left = new(leftStart, leftEnd);
			ValueRange<int> right = new(rightStart, rightEnd);
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
