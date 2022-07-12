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
		[DataTestMethod]
		[DataRow(TypeCode.Char, true)]
		[DataRow(TypeCode.SByte, true)]
		[DataRow(TypeCode.Byte, true)]
		[DataRow(TypeCode.Int16, true)]
		[DataRow(TypeCode.UInt16, true)]
		[DataRow(TypeCode.Int32, true)]
		[DataRow(TypeCode.UInt32, true)]
		[DataRow(TypeCode.Int64, true)]
		[DataRow(TypeCode.UInt64, true)]
		[DataRow(TypeCode.Single, true)]
		[DataRow(TypeCode.Double, true)]
		[DataRow(TypeCode.Decimal, true)]
		[DataRow(TypeCode.Boolean, false)]
		[DataRow(TypeCode.DateTime, false)]
		[DataRow(TypeCode.DBNull, false)]
		[DataRow(TypeCode.Empty, false)]
		[DataRow(TypeCode.Object, false)]
		[DataRow(TypeCode.String, false)]
		public void TestIsNumeric(TypeCode typeCode, bool expected)
		{
			Assert.AreEqual(expected, typeCode.IsNumeric());
		}

		/// <summary>
		/// 对 <see cref="TypeCodeUtil.IsUnsigned"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(TypeCode.Char, true)]
		[DataRow(TypeCode.Byte, true)]
		[DataRow(TypeCode.UInt16, true)]
		[DataRow(TypeCode.UInt32, true)]
		[DataRow(TypeCode.UInt64, true)]
		[DataRow(TypeCode.SByte, false)]
		[DataRow(TypeCode.Int16, false)]
		[DataRow(TypeCode.Int32, false)]
		[DataRow(TypeCode.Int64, false)]
		[DataRow(TypeCode.Single, false)]
		[DataRow(TypeCode.Double, false)]
		[DataRow(TypeCode.Decimal, false)]
		[DataRow(TypeCode.Boolean, false)]
		[DataRow(TypeCode.DateTime, false)]
		[DataRow(TypeCode.DBNull, false)]
		[DataRow(TypeCode.Empty, false)]
		[DataRow(TypeCode.Object, false)]
		[DataRow(TypeCode.String, false)]
		public void TestIsUnsigned(TypeCode typeCode, bool expected)
		{
			Assert.AreEqual(expected, typeCode.IsUnsigned());
		}

		/// <summary>
		/// 对 <see cref="TypeCodeUtil.IsSigned"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(TypeCode.SByte, true)]
		[DataRow(TypeCode.Int16, true)]
		[DataRow(TypeCode.Int32, true)]
		[DataRow(TypeCode.Int64, true)]
		[DataRow(TypeCode.Char, false)]
		[DataRow(TypeCode.Byte, false)]
		[DataRow(TypeCode.UInt16, false)]
		[DataRow(TypeCode.UInt32, false)]
		[DataRow(TypeCode.UInt64, false)]
		[DataRow(TypeCode.Single, false)]
		[DataRow(TypeCode.Double, false)]
		[DataRow(TypeCode.Decimal, false)]
		[DataRow(TypeCode.Boolean, false)]
		[DataRow(TypeCode.DateTime, false)]
		[DataRow(TypeCode.DBNull, false)]
		[DataRow(TypeCode.Empty, false)]
		[DataRow(TypeCode.Object, false)]
		[DataRow(TypeCode.String, false)]
		public void TestIsSigned(TypeCode typeCode, bool expected)
		{
			Assert.AreEqual(expected, typeCode.IsSigned());
		}
	}
}

