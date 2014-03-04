using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.FloatComparer"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestFloatComparer
	{
		/// <summary>
		/// 对 <see cref="Cyjb.FloatComparer.Comparer"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestComparer()
		{
			Assert.AreEqual((1.0F).CompareTo(1.0F), FloatComparer.Default.Compare(1.0F, 1.0F));
			Assert.AreEqual((1.1F).CompareTo(1.0F), FloatComparer.Default.Compare(1.1F, 1.0F));
			Assert.AreEqual((1.0F).CompareTo(1.1F), FloatComparer.Default.Compare(1.0F, 1.1F));
			Assert.AreEqual((1.0F).CompareTo(float.NaN),
				FloatComparer.Default.Compare(1.0F, float.NaN));
			Assert.AreEqual((float.NaN).CompareTo(1.0F),
				FloatComparer.Default.Compare(float.NaN, 1.0F));
			Assert.AreEqual((float.NaN).CompareTo(float.NaN),
				FloatComparer.Default.Compare(float.NaN, float.NaN));
			Assert.AreEqual((1.0F).CompareTo(float.PositiveInfinity),
				FloatComparer.Default.Compare(1.0F, float.PositiveInfinity));
			Assert.AreEqual((float.PositiveInfinity).CompareTo(1.0F),
				FloatComparer.Default.Compare(float.PositiveInfinity, 1.0F));
			Assert.AreEqual((float.PositiveInfinity).CompareTo(float.PositiveInfinity),
				FloatComparer.Default.Compare(float.PositiveInfinity, float.PositiveInfinity));
			Assert.AreEqual((1.0F).CompareTo(float.NegativeInfinity),
				FloatComparer.Default.Compare(1.0F, float.NegativeInfinity));
			Assert.AreEqual((float.NegativeInfinity).CompareTo(1.0F),
				FloatComparer.Default.Compare(float.NegativeInfinity, 1.0F));
			Assert.AreEqual((float.NegativeInfinity).CompareTo(float.NegativeInfinity),
				FloatComparer.Default.Compare(float.NegativeInfinity, float.NegativeInfinity));
			Assert.AreEqual((float.PositiveInfinity).CompareTo(float.NaN),
				FloatComparer.Default.Compare(float.PositiveInfinity, float.NaN));
			Assert.AreEqual((float.NaN).CompareTo(float.PositiveInfinity),
				FloatComparer.Default.Compare(float.NaN, float.PositiveInfinity));
			Assert.AreEqual((float.NegativeInfinity).CompareTo(float.NaN),
				FloatComparer.Default.Compare(float.NegativeInfinity, float.NaN));
			Assert.AreEqual((float.NaN).CompareTo(float.NegativeInfinity),
				FloatComparer.Default.Compare(float.NaN, float.NegativeInfinity));
			Assert.AreEqual((float.PositiveInfinity).CompareTo(float.NegativeInfinity),
				FloatComparer.Default.Compare(float.PositiveInfinity, float.NegativeInfinity));
			Assert.AreEqual((float.NegativeInfinity).CompareTo(float.PositiveInfinity),
				FloatComparer.Default.Compare(float.NegativeInfinity, float.PositiveInfinity));
			Assert.AreEqual((float.NegativeInfinity).CompareTo(float.PositiveInfinity),
				FloatComparer.Default.Compare(float.NegativeInfinity, float.PositiveInfinity));
			Assert.AreEqual((float.PositiveInfinity).CompareTo(float.NegativeInfinity),
				FloatComparer.Default.Compare(float.PositiveInfinity, float.NegativeInfinity));
			Assert.AreEqual(0, FloatComparer.Default.Compare(1F / 3F, 0.3333333333333333333333F));
		}
	}
}
