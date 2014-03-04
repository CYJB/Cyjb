using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.DoubleComparer"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestDoubleComparer
	{
		/// <summary>
		/// 对 <see cref="Cyjb.DoubleComparer.Comparer"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestComparer()
		{
			Assert.AreEqual((1.0D).CompareTo(1.0D), DoubleComparer.Default.Compare(1.0D, 1.0D));
			Assert.AreEqual((1.1D).CompareTo(1.0D), DoubleComparer.Default.Compare(1.1D, 1.0D));
			Assert.AreEqual((1.0D).CompareTo(1.1D), DoubleComparer.Default.Compare(1.0D, 1.1D));
			Assert.AreEqual((1.0D).CompareTo(double.NaN),
				DoubleComparer.Default.Compare(1.0D, double.NaN));
			Assert.AreEqual((double.NaN).CompareTo(1.0D),
				DoubleComparer.Default.Compare(double.NaN, 1.0D));
			Assert.AreEqual((double.NaN).CompareTo(double.NaN),
				DoubleComparer.Default.Compare(double.NaN, double.NaN));
			Assert.AreEqual((1.0D).CompareTo(double.PositiveInfinity),
				DoubleComparer.Default.Compare(1.0D, double.PositiveInfinity));
			Assert.AreEqual((double.PositiveInfinity).CompareTo(1.0D),
				DoubleComparer.Default.Compare(double.PositiveInfinity, 1.0D));
			Assert.AreEqual((double.PositiveInfinity).CompareTo(double.PositiveInfinity),
				DoubleComparer.Default.Compare(double.PositiveInfinity, double.PositiveInfinity));
			Assert.AreEqual((1.0D).CompareTo(double.NegativeInfinity),
				DoubleComparer.Default.Compare(1.0D, double.NegativeInfinity));
			Assert.AreEqual((double.NegativeInfinity).CompareTo(1.0D),
				DoubleComparer.Default.Compare(double.NegativeInfinity, 1.0D));
			Assert.AreEqual((double.NegativeInfinity).CompareTo(double.NegativeInfinity),
				DoubleComparer.Default.Compare(double.NegativeInfinity, double.NegativeInfinity));
			Assert.AreEqual((double.PositiveInfinity).CompareTo(double.NaN),
				DoubleComparer.Default.Compare(double.PositiveInfinity, double.NaN));
			Assert.AreEqual((double.NaN).CompareTo(double.PositiveInfinity),
				DoubleComparer.Default.Compare(double.NaN, double.PositiveInfinity));
			Assert.AreEqual((double.NegativeInfinity).CompareTo(double.NaN),
				DoubleComparer.Default.Compare(double.NegativeInfinity, double.NaN));
			Assert.AreEqual((double.NaN).CompareTo(double.NegativeInfinity),
				DoubleComparer.Default.Compare(double.NaN, double.NegativeInfinity));
			Assert.AreEqual((double.PositiveInfinity).CompareTo(double.NegativeInfinity),
				DoubleComparer.Default.Compare(double.PositiveInfinity, double.NegativeInfinity));
			Assert.AreEqual((double.NegativeInfinity).CompareTo(double.PositiveInfinity),
				DoubleComparer.Default.Compare(double.NegativeInfinity, double.PositiveInfinity));
			Assert.AreEqual((double.NegativeInfinity).CompareTo(double.PositiveInfinity),
				DoubleComparer.Default.Compare(double.NegativeInfinity, double.PositiveInfinity));
			Assert.AreEqual((double.PositiveInfinity).CompareTo(double.NegativeInfinity),
				DoubleComparer.Default.Compare(double.PositiveInfinity, double.NegativeInfinity));
			Assert.AreEqual(0, DoubleComparer.Default.Compare(1D / 3D, 0.3333333333333333333333D));
		}
	}
}
