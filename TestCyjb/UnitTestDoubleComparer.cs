using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="DoubleComparer"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestDoubleComparer
	{
		/// <summary>
		/// 对 <see cref="DoubleComparer.Comparer"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(1.0D, 1.0D, 0)]
		[DataRow(1.1D, 1.0D, 1)]
		[DataRow(double.NaN, double.NaN, 0)]
		[DataRow(1.0D, double.NaN, 1)]
		[DataRow(double.PositiveInfinity, double.PositiveInfinity, 0)]
		[DataRow(double.PositiveInfinity, 1.0D, 1)]
		[DataRow(double.NegativeInfinity, double.NegativeInfinity, 0)]
		[DataRow(1.0D, double.NegativeInfinity, 1)]
		[DataRow(double.PositiveInfinity, double.NaN, 1)]
		[DataRow(double.NegativeInfinity, double.NaN, 1)]
		[DataRow(double.PositiveInfinity, double.NegativeInfinity, 1)]
		[DataRow(1.0D / 3.0D, 0.3333333333333333333333D, 0)]
		public void TestComparer(double x, double y, int ret)
		{
			Assert.AreEqual(ret, DoubleComparer.Default.Compare(x, y));
			Assert.AreEqual(-ret, DoubleComparer.Default.Compare(y, x));
		}

		/// <summary>
		/// 对 <see cref="DoubleComparer"/> 的 <c>epsilon</c> 属性进行测试。
		/// </summary>
		[TestMethod]
		public void TestEpsilon()
		{
			DoubleComparer comparer = new(0.01D);
			Assert.AreEqual(0, comparer.Compare(100D, 100D));
			Assert.AreEqual(0, comparer.Compare(99.1D, 100D));
			Assert.AreEqual(0, comparer.Compare(100D, 99.1D));
			Assert.AreEqual(-1, comparer.Compare(98D, 100D));
			Assert.AreEqual(1, comparer.Compare(100D, 98D));

			Assert.AreEqual(0, comparer.Compare(-100D, -100D));
			Assert.AreEqual(0, comparer.Compare(-99.1D, -100D));
			Assert.AreEqual(0, comparer.Compare(-100D, -99.1D));
			Assert.AreEqual(1, comparer.Compare(-98D, -100D));
			Assert.AreEqual(-1, comparer.Compare(-100D, -98D));

			Assert.AreEqual(-1, comparer.Compare(-200D, 100D));
			Assert.AreEqual(1, comparer.Compare(100D, -200D));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				new DoubleComparer(-1);
			});
		}
	}
}
