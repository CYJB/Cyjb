using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="FloatComparer"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestFloatComparer
	{
		/// <summary>
		/// 对 <see cref="FloatComparer.Comparer"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(1.0F, 1.0F, 0)]
		[DataRow(1.1F, 1.0F, 1)]
		[DataRow(float.NaN, float.NaN, 0)]
		[DataRow(1.0F, float.NaN, 1)]
		[DataRow(float.PositiveInfinity, float.PositiveInfinity, 0)]
		[DataRow(float.PositiveInfinity, 1.0F, 1)]
		[DataRow(float.NegativeInfinity, float.NegativeInfinity, 0)]
		[DataRow(1.0F, float.NegativeInfinity, 1)]
		[DataRow(float.PositiveInfinity, float.NaN, 1)]
		[DataRow(float.NegativeInfinity, float.NaN, 1)]
		[DataRow(float.PositiveInfinity, float.NegativeInfinity, 1)]
		[DataRow(1.0F / 3.0F, 0.3333333333333333333333F, 0)]
		public void TestComparer(float x, float y, int ret)
		{
			Assert.AreEqual(ret, FloatComparer.Default.Compare(x, y));
			Assert.AreEqual(-ret, FloatComparer.Default.Compare(y, x));
		}

		/// <summary>
		/// 对 <see cref="FloatComparer"/> 的 <c>epsilon</c> 属性进行测试。
		/// </summary>
		[TestMethod]
		public void TestEpsilon()
		{
			FloatComparer comparer = new(0.01F);
			Assert.AreEqual(0, comparer.Compare(100F, 100F));
			Assert.AreEqual(0, comparer.Compare(99.1F, 100F));
			Assert.AreEqual(0, comparer.Compare(100F, 99.1F));
			Assert.AreEqual(-1, comparer.Compare(98F, 100F));
			Assert.AreEqual(1, comparer.Compare(100F, 98F));

			Assert.AreEqual(0, comparer.Compare(-100F, -100F));
			Assert.AreEqual(0, comparer.Compare(-99.1F, -100F));
			Assert.AreEqual(0, comparer.Compare(-100F, -99.1F));
			Assert.AreEqual(1, comparer.Compare(-98F, -100F));
			Assert.AreEqual(-1, comparer.Compare(-100F, -98F));

			Assert.AreEqual(-1, comparer.Compare(-200F, 100F));
			Assert.AreEqual(1, comparer.Compare(100F, -200F));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				new FloatComparer(-1);
			});
		}
	}
}
