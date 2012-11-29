using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyjb;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.TypeExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsConvertableFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsConvertableFrom()
		{
			// Trues
			Assert.IsTrue(typeof(object).IsConvertableFrom(typeof(object)));
			Assert.IsTrue(typeof(object).IsConvertableFrom(typeof(string)));
			Assert.IsTrue(typeof(object).IsConvertableFrom(typeof(uint)));
			Assert.IsTrue(typeof(object).IsConvertableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(TestClass).IsConvertableFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(int).IsConvertableFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsConvertableFrom(typeof(ushort)));
			Assert.IsTrue(typeof(int).IsConvertableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(long).IsConvertableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsConvertableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsConvertableFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsConvertableFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Delegate).IsConvertableFrom(typeof(Func<int>)));
			Assert.IsTrue(typeof(int?).IsConvertableFrom(typeof(int)));
			// Falses
			Assert.IsFalse(typeof(TestClass).IsConvertableFrom(typeof(object)));
			Assert.IsFalse(typeof(TestClass2).IsConvertableFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(TestClass).IsConvertableFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(int).IsConvertableFrom(typeof(long)));
			Assert.IsFalse(typeof(uint).IsConvertableFrom(typeof(short)));
			Assert.IsFalse(typeof(short).IsConvertableFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(string).IsConvertableFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(int).IsConvertableFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(Tristate).IsConvertableFrom(typeof(Enum)));
			Assert.IsFalse(typeof(Func<int>).IsConvertableFrom(typeof(Delegate)));
			Assert.IsFalse(typeof(int).IsConvertableFrom(typeof(int?)));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsCastableFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsCastableFrom()
		{
			// Trues
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(object)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(string)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(uint)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(TestClass).IsCastableFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsCastableFrom(typeof(ushort)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(long).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsCastableFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsCastableFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Delegate).IsCastableFrom(typeof(Func<int>)));
			Assert.IsTrue(typeof(int?).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(TestClass).IsCastableFrom(typeof(object)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(long)));
			Assert.IsTrue(typeof(uint).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(short).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass3)));
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(Enum)));
			Assert.IsTrue(typeof(Func<int>).IsCastableFrom(typeof(Delegate)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(int?)));
			Assert.IsTrue(typeof(TestClass2).IsCastableFrom(typeof(TestClass)));
			// Falses
			Assert.IsFalse(typeof(TestClass).IsConvertableFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(string).IsCastableFrom(typeof(TestClass)));
		}
		private class TestClass
		{
			public static implicit operator int(TestClass tc)
			{
				return 0;
			}
		}
		private class TestClass2 : TestClass
		{
			public static implicit operator bool(TestClass2 tc)
			{
				return false;
			}
		}
		private class TestClass3
		{
			public static explicit operator int(TestClass3 tc)
			{
				return 0;
			}
		}
	}
}
