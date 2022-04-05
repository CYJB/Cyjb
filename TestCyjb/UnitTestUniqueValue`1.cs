using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="UniqueValue{TestValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestUniqueValue
	{
		/// <summary>
		/// 对 <see cref="UniqueValue{TestValue}"/> 的引用类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestClassType()
		{
			UniqueValue<string> value = new();
			Assert.IsNull(value.Value);
			Assert.IsFalse(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsTrue(value.IsEmpty);

			value.Value = "1";
			Assert.AreEqual("1", value.Value);
			Assert.IsTrue(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Value = "1";
			Assert.AreEqual("1", value.Value);
			Assert.IsTrue(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Value = "2";
			Assert.IsNull(value.Value);
			Assert.IsFalse(value.IsUnique);
			Assert.IsTrue(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Reset();
			Assert.IsNull(value.Value);
			Assert.IsFalse(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsTrue(value.IsEmpty);
		}

		/// <summary>
		/// 对 <see cref="UniqueValue{TestValue}"/> 的值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestValueType()
		{
			UniqueValue<int> value = new();
			Assert.AreEqual(0, value.Value);
			Assert.IsFalse(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsTrue(value.IsEmpty);

			value.Value = 1;
			Assert.AreEqual(1, value.Value);
			Assert.IsTrue(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Value = 1;
			Assert.AreEqual(1, value.Value);
			Assert.IsTrue(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Value = 2;
			Assert.AreEqual(0, value.Value);
			Assert.IsFalse(value.IsUnique);
			Assert.IsTrue(value.IsAmbig);
			Assert.IsFalse(value.IsEmpty);

			value.Reset();
			Assert.IsFalse(value.IsUnique);
			Assert.IsFalse(value.IsAmbig);
			Assert.IsTrue(value.IsEmpty);
		}
	}
}
