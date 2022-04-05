using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="TypeCodeUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeCodeUtil
	{
		/// <summary>
		/// 对 <see cref="TypeCodeUtil.IsNumeric"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsNumeric()
		{
			Assert.IsTrue(TypeCode.Char.IsNumeric());
			Assert.IsTrue(TypeCode.SByte.IsNumeric());
			Assert.IsTrue(TypeCode.Byte.IsNumeric());
			Assert.IsTrue(TypeCode.Int16.IsNumeric());
			Assert.IsTrue(TypeCode.UInt16.IsNumeric());
			Assert.IsTrue(TypeCode.Int32.IsNumeric());
			Assert.IsTrue(TypeCode.UInt32.IsNumeric());
			Assert.IsTrue(TypeCode.Int64.IsNumeric());
			Assert.IsTrue(TypeCode.UInt64.IsNumeric());
			Assert.IsTrue(TypeCode.Single.IsNumeric());
			Assert.IsTrue(TypeCode.Double.IsNumeric());
			Assert.IsTrue(TypeCode.Decimal.IsNumeric());

			Assert.IsFalse(TypeCode.Boolean.IsNumeric());
			Assert.IsFalse(TypeCode.DateTime.IsNumeric());
			Assert.IsFalse(TypeCode.DBNull.IsNumeric());
			Assert.IsFalse(TypeCode.Empty.IsNumeric());
			Assert.IsFalse(TypeCode.Object.IsNumeric());
			Assert.IsFalse(TypeCode.String.IsNumeric());
		}

		/// <summary>
		/// 对 <see cref="TypeCodeUtil.IsUnsigned"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsUnsigned()
		{
			Assert.IsTrue(TypeCode.Char.IsUnsigned());
			Assert.IsTrue(TypeCode.Byte.IsUnsigned());
			Assert.IsTrue(TypeCode.UInt16.IsUnsigned());
			Assert.IsTrue(TypeCode.UInt32.IsUnsigned());
			Assert.IsTrue(TypeCode.UInt64.IsUnsigned());

			Assert.IsFalse(TypeCode.SByte.IsUnsigned());
			Assert.IsFalse(TypeCode.Int16.IsUnsigned());
			Assert.IsFalse(TypeCode.Int32.IsUnsigned());
			Assert.IsFalse(TypeCode.Int64.IsUnsigned());
			Assert.IsFalse(TypeCode.Single.IsUnsigned());
			Assert.IsFalse(TypeCode.Double.IsUnsigned());
			Assert.IsFalse(TypeCode.Decimal.IsUnsigned());

			Assert.IsFalse(TypeCode.Boolean.IsUnsigned());
			Assert.IsFalse(TypeCode.DateTime.IsUnsigned());
			Assert.IsFalse(TypeCode.DBNull.IsUnsigned());
			Assert.IsFalse(TypeCode.Empty.IsUnsigned());
			Assert.IsFalse(TypeCode.Object.IsUnsigned());
			Assert.IsFalse(TypeCode.String.IsUnsigned());
		}

		/// <summary>
		/// 对 <see cref="TypeCodeUtil.IsSigned"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsSigned()
		{
			Assert.IsTrue(TypeCode.SByte.IsSigned());
			Assert.IsTrue(TypeCode.Int16.IsSigned());
			Assert.IsTrue(TypeCode.Int32.IsSigned());
			Assert.IsTrue(TypeCode.Int64.IsSigned());

			Assert.IsFalse(TypeCode.Char.IsSigned());
			Assert.IsFalse(TypeCode.Byte.IsSigned());
			Assert.IsFalse(TypeCode.UInt16.IsSigned());
			Assert.IsFalse(TypeCode.UInt32.IsSigned());
			Assert.IsFalse(TypeCode.UInt64.IsSigned());
			Assert.IsFalse(TypeCode.Single.IsSigned());
			Assert.IsFalse(TypeCode.Double.IsSigned());
			Assert.IsFalse(TypeCode.Decimal.IsSigned());

			Assert.IsFalse(TypeCode.Boolean.IsSigned());
			Assert.IsFalse(TypeCode.DateTime.IsSigned());
			Assert.IsFalse(TypeCode.DBNull.IsSigned());
			Assert.IsFalse(TypeCode.Empty.IsSigned());
			Assert.IsFalse(TypeCode.Object.IsSigned());
			Assert.IsFalse(TypeCode.String.IsSigned());
		}
	}
}

