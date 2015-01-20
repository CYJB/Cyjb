using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Convert = Cyjb.Convert;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.Convert"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestConvert
	{

		#region 测试相等转换

		/// <summary>
		/// 测试相等转换。
		/// </summary>
		[TestMethod]
		public void TestIdentityConversion()
		{
			TestChangeType(-42341, -42341);
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(int), typeof(int)));
			int[] arr = { 10, 20, 30 };
			TestChangeType(arr, arr);
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(int[]), typeof(int[])));
			List<int> list = new List<int>();
			TestChangeType(list, list);
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(List<int>), typeof(List<int>)));
		}
		private static void TestChangeType<TInput, TOutput>(TInput value, TOutput expected)
		{
			Assert.AreEqual(expected, Convert.ChangeType<TInput, TOutput>(value));
			Assert.AreEqual(expected, Convert.GetConverter(typeof(TInput), typeof(TOutput))(value));
		}

		#endregion // 测试相等转换

		#region 测试数值转换

		private static readonly sbyte[] sbyteValues = { -128, 0, 127 };
		private static readonly byte[] byteValues = { 0, 127, 255 };
		private static readonly short[] shortValues = { -32768, -128, 0, 127, 255, 32767 };
		private static readonly ushort[] ushortValues = { 0, 127, 255, 32767, 65535 };
		private static readonly int[] intValues = { -2147483648, -32768, -128, 0, 127, 255, 32767, 65535, 2147483647 };
		private static readonly uint[] uintValues = { 0, 127, 255, 32767, 65535, 2147483647, 4294967295 };
		private static readonly long[] longValues = { -9223372036854775808, -2147483648, -32768, -128, 0, 127, 255, 32767, 
			65535, 2147483647, 4294967295, 9223372036854775807 };
		private static readonly ulong[] ulongValues = { 0, 127, 255, 32767, 65535, 2147483647, 4294967295, 9223372036854775807,
			18446744073709551615 };
		private static readonly char[] charValues = { (char)0, (char)127, (char)255, (char)32767, (char)65535 };
		private static readonly float[] floatValues = { float.NegativeInfinity, -3.402823e38F, -9223372036854775808, -2147483648,
			-32768, -128, 0, 127, 255, 32767, 65535, 2147483647, 4294967295, 9223372036854775807, 18446744073709551615,
			+3.402823e38F, float.PositiveInfinity, float.NaN };
		private static readonly double[] doubleValues = { double.NegativeInfinity, float.NegativeInfinity, 
			-1.7976931348623157E+308, -3.402823e38, -9223372036854775808, -2147483648, -32768, -128, 0, 127, 255, 32767, 
			65535, 2147483647, 4294967295, 9223372036854775807, 18446744073709551615, +3.402823e38, 1.7976931348623157E+308,
			float.PositiveInfinity, double.PositiveInfinity, double.NaN };
		private static readonly decimal[] decimalValues = { decimal.MinValue, -9223372036854775808, -2147483648, -32768, -128,
			0, 127, 255, 32767, 65535, 2147483647, 4294967295, 9223372036854775807, 18446744073709551615, decimal.MaxValue };
		/// <summary>
		/// 测试数值转换。
		/// </summary>
		[TestMethod]
		public void TestNumericConversion()
		{
			// SByte
			TestConverter(sbyteValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(sbyte), typeof(sbyte)));
			TestConverter(sbyteValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(byte)));
			TestConverter(sbyteValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(short)));
			TestConverter(sbyteValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(ushort)));
			TestConverter(sbyteValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(int)));
			TestConverter(sbyteValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(uint)));
			TestConverter(sbyteValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(long)));
			TestConverter(sbyteValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(ulong)));
			TestConverter(sbyteValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(char)));
			TestConverter(sbyteValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(float)));
			TestConverter(sbyteValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(double)));
			TestConverter(sbyteValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(sbyte), typeof(decimal)));

			// Byte
			TestConverter(byteValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(sbyte)));
			TestConverter(byteValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(byte), typeof(byte)));
			TestConverter(byteValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(short)));
			TestConverter(byteValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(ushort)));
			TestConverter(byteValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(int)));
			TestConverter(byteValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(uint)));
			TestConverter(byteValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(long)));
			TestConverter(byteValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(ulong)));
			TestConverter(byteValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(char)));
			TestConverter(byteValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(float)));
			TestConverter(byteValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(double)));
			TestConverter(byteValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(byte), typeof(decimal)));

			// Int16
			TestConverter(shortValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(sbyte)));
			TestConverter(shortValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(byte)));
			TestConverter(shortValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(short), typeof(short)));
			TestConverter(shortValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(ushort)));
			TestConverter(shortValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(int)));
			TestConverter(shortValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(uint)));
			TestConverter(shortValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(long)));
			TestConverter(shortValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(ulong)));
			TestConverter(shortValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(char)));
			TestConverter(shortValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(float)));
			TestConverter(shortValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(double)));
			TestConverter(shortValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(short), typeof(decimal)));

			// UInt16
			TestConverter(ushortValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(sbyte)));
			TestConverter(ushortValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(byte)));
			TestConverter(ushortValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(short)));
			TestConverter(ushortValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(ushort), typeof(ushort)));
			TestConverter(ushortValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(int)));
			TestConverter(ushortValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(uint)));
			TestConverter(ushortValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(long)));
			TestConverter(ushortValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(ulong)));
			TestConverter(ushortValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(char)));
			TestConverter(ushortValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(float)));
			TestConverter(ushortValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(double)));
			TestConverter(ushortValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ushort), typeof(decimal)));

			// Int32
			TestConverter(intValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(sbyte)));
			TestConverter(intValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(byte)));
			TestConverter(intValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(short)));
			TestConverter(intValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(ushort)));
			TestConverter(intValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(int), typeof(int)));
			TestConverter(intValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(uint)));
			TestConverter(intValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(long)));
			TestConverter(intValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(ulong)));
			TestConverter(intValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(char)));
			TestConverter(intValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(float)));
			TestConverter(intValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(double)));
			TestConverter(intValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(int), typeof(decimal)));

			// UInt32
			TestConverter(uintValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(sbyte)));
			TestConverter(uintValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(byte)));
			TestConverter(uintValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(short)));
			TestConverter(uintValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(ushort)));
			TestConverter(uintValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(int)));
			TestConverter(uintValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(uint), typeof(uint)));
			TestConverter(uintValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(long)));
			TestConverter(uintValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(ulong)));
			TestConverter(uintValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(char)));
			TestConverter(uintValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(float)));
			TestConverter(uintValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(double)));
			TestConverter(uintValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(uint), typeof(decimal)));

			// Int64
			TestConverter(longValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(sbyte)));
			TestConverter(longValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(byte)));
			TestConverter(longValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(short)));
			TestConverter(longValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(ushort)));
			TestConverter(longValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(int)));
			TestConverter(longValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(uint)));
			TestConverter(longValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(long), typeof(long)));
			TestConverter(longValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(ulong)));
			TestConverter(longValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(char)));
			TestConverter(longValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(float)));
			TestConverter(longValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(double)));
			TestConverter(longValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(long), typeof(decimal)));

			// UInt64
			TestConverter(ulongValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(sbyte)));
			TestConverter(ulongValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(byte)));
			TestConverter(ulongValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(short)));
			TestConverter(ulongValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(ushort)));
			TestConverter(ulongValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(int)));
			TestConverter(ulongValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(uint)));
			TestConverter(ulongValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(long)));
			TestConverter(ulongValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(ulong), typeof(ulong)));
			TestConverter(ulongValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(char)));
			TestConverter(ulongValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(float)));
			TestConverter(ulongValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(double)));
			TestConverter(ulongValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(ulong), typeof(decimal)));

			// Char
			TestConverter(charValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(sbyte)));
			TestConverter(charValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(byte)));
			TestConverter(charValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(short)));
			TestConverter(charValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(ushort)));
			TestConverter(charValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(int)));
			TestConverter(charValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(uint)));
			TestConverter(charValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(long)));
			TestConverter(charValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(ulong)));
			TestConverter(charValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(char), typeof(char)));
			TestConverter(charValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(float)));
			TestConverter(charValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(double)));
			TestConverter(charValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(char), typeof(decimal)));

			// Single
			TestConverter(floatValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(sbyte)));
			TestConverter(floatValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(byte)));
			TestConverter(floatValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(short)));
			TestConverter(floatValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(ushort)));
			TestConverter(floatValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(int)));
			TestConverter(floatValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(uint)));
			TestConverter(floatValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(long)));
			TestConverter(floatValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(ulong)));
			TestConverter(floatValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(char)));
			TestConverter(floatValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(float), typeof(float)));
			TestConverter(floatValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ImplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(double)));
			TestConverter(floatValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(decimal)));

			// Double
			TestConverter(doubleValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(sbyte)));
			TestConverter(doubleValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(byte)));
			TestConverter(doubleValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(short)));
			TestConverter(doubleValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(ushort)));
			TestConverter(doubleValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(int)));
			TestConverter(doubleValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(uint)));
			TestConverter(doubleValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(long)));
			TestConverter(doubleValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(ulong)));
			TestConverter(doubleValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(char)));
			TestConverter(doubleValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(float)));
			TestConverter(doubleValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(double), typeof(double)));
			TestConverter(doubleValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(double), typeof(decimal)));

			// Decimal
			TestConverter(decimalValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(sbyte)));
			TestConverter(decimalValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(byte)));
			TestConverter(decimalValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(short)));
			TestConverter(decimalValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(ushort)));
			TestConverter(decimalValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(int)));
			TestConverter(decimalValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(uint)));
			TestConverter(decimalValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(long)));
			TestConverter(decimalValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(ulong)));
			TestConverter(decimalValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(float), typeof(char)));
			TestConverter(decimalValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(float)));
			TestConverter(decimalValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.ExplicitNumericConversion, Convert.GetConvertType(typeof(decimal), typeof(double)));
			TestConverter(decimalValues, v => checked(v), v => unchecked(v));
			Assert.AreEqual(ConversionType.IdentityConversion, Convert.GetConvertType(typeof(decimal), typeof(decimal)));
		}
		private static void TestConverter<TInput, TOutput>(TInput[] inputValues,
			Func<TInput, TOutput> checkedConverter, Func<TInput, TOutput> uncheckedConverter)
		{
			TestConverter(inputValues, true, checkedConverter);
			TestConverter(inputValues, false, uncheckedConverter);
		}
		private static readonly Func<Type, Type, object> getConversion = Type.GetType(
			"Cyjb.Conversions.ConversionFactory, Cyjb, Version=0.1.0.0, Culture=neutral, PublicKeyToken=40880b37bef542a8")
			.CreateDelegate<Func<Type, Type, object>>("GetConversion");
		private static readonly Func<object, Type, Type, bool, bool, Delegate> buildConverter =
			typeof(Convert).CreateDelegate<Func<object, Type, Type, bool, bool, Delegate>>("BuildConverter");
		private static void TestConverter<TInput, TOutput>(TInput[] inputValues, bool isChecked,
			Func<TInput, TOutput> standardConverter)
		{
			object conversion = getConversion(typeof(TInput), typeof(TOutput));
			Converter<TInput, TOutput> genericConverter = (Converter<TInput, TOutput>)buildConverter(conversion,
				typeof(TInput), typeof(TOutput), isChecked, true);
			Converter<object, object> objectConverter = (Converter<object, object>)buildConverter(conversion,
				typeof(TInput), typeof(TOutput), isChecked, false);
			for (int i = 0; i < inputValues.Length; i++)
			{
				TInput input = inputValues[i];
				TOutput expected = default(TOutput);
				try
				{
					expected = standardConverter(input);
				}
				catch (Exception expectedEx)
				{
					AssertExt.ThrowsException(() => genericConverter(input), expectedEx.GetType());
					AssertExt.ThrowsException(() => objectConverter(input), expectedEx.GetType());
					continue;
				}
				Assert.AreEqual(expected, genericConverter(input));
				Assert.AreEqual(expected, objectConverter(input));
			}
		}

		#endregion // 测试数值转换

		#region 测试枚举转换

		private static readonly BindingFlags[] bindingFlagsValues = { BindingFlags.Default, BindingFlags.IgnoreCase,
			BindingFlags.DeclaredOnly, BindingFlags.Instance, BindingFlags.Public, BindingFlags.NonPublic, 
			BindingFlags.FlattenHierarchy, BindingFlags.InvokeMethod, BindingFlags.CreateInstance, BindingFlags.GetField, 
			BindingFlags.SetField, BindingFlags.GetProperty, BindingFlags.SetProperty, BindingFlags.PutDispProperty, 
			BindingFlags.ExactBinding, BindingFlags.SuppressChangeType, BindingFlags.OptionalParamBinding, 
			BindingFlags.IgnoreReturn };

		/// <summary>
		/// 测试枚举转换。
		/// </summary>
		[TestMethod]
		public void TestEnumConversion()
		{
			TestConverter(sbyteValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(sbyte), typeof(BindingFlags)));
			TestConverter(byteValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(byte), typeof(BindingFlags)));
			TestConverter(shortValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(short), typeof(BindingFlags)));
			TestConverter(ushortValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(ushort), typeof(BindingFlags)));
			TestConverter(intValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(int), typeof(BindingFlags)));
			TestConverter(uintValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(uint), typeof(BindingFlags)));
			TestConverter(longValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(long), typeof(BindingFlags)));
			TestConverter(ulongValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(ulong), typeof(BindingFlags)));
			TestConverter(charValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(char), typeof(BindingFlags)));
			TestConverter(floatValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(float), typeof(BindingFlags)));
			TestConverter(doubleValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(double), typeof(BindingFlags)));
			TestConverter(decimalValues, v => checked((BindingFlags)v), v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(decimal), typeof(BindingFlags)));

			TestConverter(bindingFlagsValues, v => checked((sbyte)v), v => unchecked((sbyte)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(sbyte)));
			TestConverter(bindingFlagsValues, v => checked((byte)v), v => unchecked((byte)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(byte)));
			TestConverter(bindingFlagsValues, v => checked((short)v), v => unchecked((short)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(short)));
			TestConverter(bindingFlagsValues, v => checked((ushort)v), v => unchecked((ushort)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(ushort)));
			TestConverter(bindingFlagsValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(int)));
			TestConverter(bindingFlagsValues, v => checked((uint)v), v => unchecked((uint)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(uint)));
			TestConverter(bindingFlagsValues, v => checked((long)v), v => unchecked((long)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(long)));
			TestConverter(bindingFlagsValues, v => checked((ulong)v), v => unchecked((ulong)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(ulong)));
			TestConverter(bindingFlagsValues, v => checked((char)v), v => unchecked((char)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(char)));
			TestConverter(bindingFlagsValues, v => checked((float)v), v => unchecked((float)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(float)));
			TestConverter(bindingFlagsValues, v => checked((double)v), v => unchecked((double)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(double)));
			TestConverter(bindingFlagsValues, v => checked((decimal)v), v => unchecked((decimal)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(decimal)));

			TestConverter(bindingFlagsValues, v => checked((Tristate)v), v => unchecked((Tristate)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(Tristate), typeof(BindingFlags)));
			TestConverter(new[] { Tristate.False, Tristate.True, Tristate.NotSure }, v => checked((BindingFlags)v),
				v => unchecked((BindingFlags)v));
			Assert.AreEqual(ConversionType.EnumConversion, Convert.GetConvertType(typeof(BindingFlags), typeof(Tristate)));
		}

		#endregion // 测试枚举转换

		#region 测试可空类型转换

		/// <summary>
		/// 测试可空类型转换。
		/// </summary>
		[TestMethod]
		public void TestNullableConversion()
		{
			// 从 S 到 T? 的隐式转换。
			TestConverter(intValues, v => checked((int?)v), v => unchecked((int?)v));
			Assert.AreEqual(ConversionType.ImplicitNullableConversion, Convert.GetConvertType(typeof(int), typeof(int?)));
			TestConverter(intValues, v => checked((long?)v), v => unchecked((long?)v));
			Assert.AreEqual(ConversionType.ImplicitNullableConversion, Convert.GetConvertType(typeof(int), typeof(long?)));
			// 从 S? 到 T? 的隐式转换。
			int?[] intNValues = { -2147483648, -32768, -128, 0, 127, 255, 32767, 65535, 2147483647, null };
			TestConverter(intNValues, v => checked((long?)v), v => unchecked((long?)v));
			Assert.AreEqual(ConversionType.ImplicitNullableConversion, Convert.GetConvertType(typeof(int?), typeof(long?)));
			// 从 S 到 T? 的显式转换。
			TestConverter(longValues, v => checked((int?)v), v => unchecked((int?)v));
			Assert.AreEqual(ConversionType.ExplicitNullableConversion, Convert.GetConvertType(typeof(long), typeof(int?)));
			// 从 S? 到 T? 的显式转换。
			long?[] longNValues = { -9223372036854775808, -2147483648, -32768, -128, 0, 127, 255, 32767, 65535, 2147483647, 
										4294967295, 9223372036854775807, null };
			TestConverter(longNValues, v => checked((int?)v), v => unchecked((int?)v));
			Assert.AreEqual(ConversionType.ExplicitNullableConversion, Convert.GetConvertType(typeof(long?), typeof(int?)));
			// 从 S? 到 T 的显式转换。
			TestConverter(longNValues, v => checked((int)v), v => unchecked((int)v));
			Assert.AreEqual(ConversionType.ExplicitNullableConversion, Convert.GetConvertType(typeof(long?), typeof(int)));
		}

		#endregion // 测试可空类型转换

		#region 测试隐式引用类型转换

		/// <summary>
		/// 测试隐式引用类型转换。
		/// </summary>
		[TestMethod]
		public void TestImplicitReferenceConversion()
		{
			// 从任何 reference-type 到 object。
			string str = "abc";
			TestChangeType(str, (object)str);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(string), typeof(object)));
			AnyReferenceClass anyReferenceClass = new AnyReferenceClass();
			TestChangeType(anyReferenceClass, (object)anyReferenceClass);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(AnyReferenceClass), typeof(object)));

			// 从任何 class-type S 到任何 class-type T（前提是 S 是从 T 派生的）。
			AnySubClass anySubClass = new AnySubClass();
			TestChangeType(anySubClass, (AnyBaseClass)anySubClass);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(AnySubClass), typeof(AnyBaseClass)));

			// 从任何 class-type S 到任何 interface-type T（前提是 S 实现了 T）。
			List<int> intList = new List<int>();
			TestChangeType(intList, (IList<int>)intList);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(List<int>), typeof(IList<int>)));

			// 从任何 interface-type S 到任何 interface-type T（前提是 S 是从 T 派生的）。
			TestChangeType((IList<int>)intList, (ICollection<int>)intList);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(IList<int>), typeof(ICollection<int>)));

			// 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			//   o S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			//   o SE 和 TE 都是 reference-type。
			//   o 存在从 SE 到 TE 的隐式引用转换。
			string[] strArr = new string[0];
			TestChangeType(strArr, (object[])strArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(string[]), typeof(object[])));
			AnySubClass[] anySubArr = new AnySubClass[0];
			TestChangeType(anySubArr, (AnyBaseClass[])anySubArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(AnySubClass[]), typeof(AnyBaseClass[])));
			List<string>[] stringListArray = new List<string>[0];
			TestChangeType(stringListArray, (IList[])stringListArray);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(List<string>[]), typeof(IList[])));
			TestChangeType(stringListArray, (IEnumerable<object>[])stringListArray);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(List<string>[]), typeof(IEnumerable<object>[])));

			// 从任何 array-type 到 System.Array 及其实现的接口。
			int[] intArr = new int[0];
			TestChangeType(intArr, (Array)intArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(int[]), typeof(Array)));
			TestChangeType(intArr, (IStructuralComparable)intArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(int[]), typeof(IStructuralComparable)));

			// 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口（前提是存在从 S 到 T 的隐式标识或引用转换）。
			TestChangeType(strArr, (IList<object>)strArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(string[]), typeof(IList<object>)));

			// 从任何 delegate-type 到 System.Delegate 及其实现的接口。
			Func<int> intFunc = () => 0;
			TestChangeType(intFunc, (Delegate)intFunc);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(Func<int>), typeof(Delegate)));

			// 从任何 reference-type 到接口或委托类型 T
			// （前提是它具有到接口或委托类型 T0 的隐式标识或引用转换，且 T0 可变化转换为T）。
			// 这里的变化转换在规范的 13.1.3.2 节，就是泛型的协变和逆变。
			// 协变。
			TestChangeType(strArr, (IEnumerable<object>)strArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(string[]), typeof(IEnumerable<object>)));
			IEnumerable<string> strEnum = Enumerable.Empty<string>();
			TestChangeType(strEnum, (IEnumerable<object>)strEnum);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));

			IEqualityComparer<object>[][] nestArr = new IEqualityComparer<object>[0][];
			TestChangeType(nestArr, (IEnumerable<IEqualityComparer<string>[]>)nestArr);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(IEqualityComparer<object>[][]), typeof(IEnumerable<IEqualityComparer<string>[]>)));
			IEnumerable<IEqualityComparer<object>[]> nestArrEnum = Enumerable.Empty<IEqualityComparer<object>[]>();
			TestChangeType(nestArrEnum, (IEnumerable<IEqualityComparer<string>[]>)nestArrEnum);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable<IEqualityComparer<object>[]>), typeof(IEnumerable<IEqualityComparer<string>[]>)));

			object[][][] nestArr2 = new object[0][][];
			TestChangeType(nestArr2, (IEnumerable<IList<IList<object>>>)nestArr2);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(object[][][]), typeof(IEnumerable<IList<IList<object>>>)));
			IEnumerable<object[][]> nestArrEnum2 = Enumerable.Empty<object[][]>();
			TestChangeType(nestArrEnum2, (IEnumerable<IList<IList<object>>>)nestArrEnum2);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable<object[][]>), typeof(IEnumerable<IList<IList<object>>>)));
			// 逆变。
			EqualityComparer<object> objCmp = EqualityComparer<object>.Default;
			TestChangeType(objCmp, (IEqualityComparer<string>)objCmp);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(EqualityComparer<object>), typeof(IEqualityComparer<string>)));

			EqualityComparer<IEnumerable<object>[]> objEnumCmp = EqualityComparer<IEnumerable<object>[]>.Default;
			TestChangeType(objEnumCmp, (IEqualityComparer<IEnumerable<string>[]>)objEnumCmp);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(EqualityComparer<IEnumerable<object>[]>), typeof(IEqualityComparer<IEnumerable<string>[]>)));

			TestChangeType(objEnumCmp, (IEqualityComparer<string[][]>)objEnumCmp);
			Assert.AreEqual(ConversionType.ImplicitReferenceConversion, Convert.GetConvertType(typeof(EqualityComparer<IEnumerable<object>[]>), typeof(IEqualityComparer<string[][]>)));
		}
		private class AnyReferenceClass { }
		private class AnyBaseClass { }
		private class AnySubClass : AnyBaseClass { }

		#endregion // 测试隐式引用类型转换

		#region 测试装箱转换

		/// <summary>
		/// 测试装箱转换。
		/// </summary>
		[TestMethod]
		public void TestBoxConversion()
		{
			// 从 non-nullable-value-type 到 object。
			TestChangeType(10, (object)10);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int), typeof(object)));
			AnyValueType anyValueType = new AnyValueType();
			TestChangeType(anyValueType, (object)anyValueType);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(AnyValueType), typeof(object)));

			// 从 non-nullable-value-type 到 System.ValueType。
			TestChangeType(10, (ValueType)10);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int), typeof(ValueType)));
			TestChangeType(anyValueType, (ValueType)anyValueType);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(AnyValueType), typeof(ValueType)));

			// 从 non-nullable-value-type 到其实现的接口。
			TestChangeType(10, (IComparable<int>)10);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int), typeof(IComparable<int>)));

			// 从 enum-type 转换为 System.Enum 类型。
			TestChangeType(Tristate.True, (Enum)Tristate.True);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(Tristate), typeof(Enum)));

			// 从 nullable-type 到 non-nullable-value-type 到该引用类型的装箱转换。
			TestChangeType((int?)10, (object)(int?)10);
			TestChangeType((int?)null, (object)(int?)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int?), typeof(object)));
			TestChangeType((int?)10, (ValueType)(int?)10);
			TestChangeType((int?)null, (ValueType)(int?)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int?), typeof(ValueType)));
			TestChangeType((int?)10, (IComparable<int>)(int?)10);
			TestChangeType((int?)null, (IComparable<int>)(int?)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(int?), typeof(IComparable<int>)));
			TestChangeType((Tristate?)Tristate.True, (Enum)(Tristate?)Tristate.True);
			TestChangeType((Tristate?)null, (Enum)(Tristate?)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(Tristate?), typeof(Enum)));

			// 如果值类型具有到接口或委托类型 I0 的装箱转换，且 I0 变化转换为接口类型 I，则值类型具有到 I 的装箱转换。
			StringEnumStruct stringEnumStruct = new StringEnumStruct();
			TestChangeType(stringEnumStruct, (IEnumerable<object>)stringEnumStruct);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(StringEnumStruct), typeof(IEnumerable<object>)));
			TestChangeType((StringEnumStruct?)null, (IEnumerable<object>)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(StringEnumStruct?), typeof(IEnumerable<object>)));
		}
		private struct AnyValueType { }
		private struct StringEnumStruct : IEnumerable<string>
		{
			public IEnumerator<string> GetEnumerator() { return null; }
			IEnumerator IEnumerable.GetEnumerator() { return null; }
		}


		#endregion // 测试装箱转换

		#region 测试显式引用转换

		/// <summary>
		/// 测试显式引用类型转换。
		/// </summary>
		[TestMethod]
		public void TestExplicitReferenceConversion()
		{
			// 从 object 到任何其他 reference-type。
			string str = "abc";
			TestChangeType((object)str, str);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(object), typeof(string)));
			AnyReferenceClass anyReferenceClass = new AnyReferenceClass();
			TestChangeType((object)anyReferenceClass, anyReferenceClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(object), typeof(AnyReferenceClass)));

			// 从任何 class-type S 到任何 class-type T（前提是 S 为 T 的基类）。
			AnySubClass anySubClass = new AnySubClass();
			TestChangeType((AnyBaseClass)anySubClass, anySubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(AnyBaseClass), typeof(AnySubClass)));

			// 从任何 class-type S 到任何 interface-type T（前提是 S 未密封并且 S 未实现 T）。
			InterfaceSubClass interfaceSubClass = new InterfaceSubClass();
			TestChangeType((AnyBaseClass)interfaceSubClass, (IEnumerable)interfaceSubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(AnyBaseClass), typeof(IEnumerable)));

			// 从任何 interface-type S 到任何 class-type T（前提是 T 未密封或 T 实现 S）。
			TestChangeType((IEnumerable)interfaceSubClass, (AnyBaseClass)interfaceSubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable), typeof(AnyBaseClass)));
			TestChangeType((IEnumerable)interfaceSubClass, interfaceSubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable), typeof(InterfaceSubClass)));

			// 从任何 interface-type S 到任何 interface-type T（前提是 S 不是从 T 派生的）。
			TestChangeType((IEnumerable)interfaceSubClass, (IComparable)interfaceSubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable), typeof(IComparable)));
			TestChangeType((IComparable)interfaceSubClass, (IEnumerable)interfaceSubClass);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IComparable), typeof(IEnumerable)));

			// 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			//   o	S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			//   o	SE 和 TE 都是 reference-type。
			//   o	存在从 SE 到 TE 的显式引用转换。
			string[] strArr = new string[0];
			TestChangeType((object[])strArr, strArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(object[]), typeof(string[])));
			AnySubClass[] anySubArr = new AnySubClass[0];
			TestChangeType((AnyBaseClass[])anySubArr, anySubArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(AnyBaseClass[]), typeof(AnySubClass[])));
			List<string>[] stringListArray = new List<string>[0];
			TestChangeType((IList[])stringListArray, stringListArray);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IList[]), typeof(List<string>[])));
			TestChangeType((IEnumerable<object>[])stringListArray, stringListArray);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IEnumerable<object>[]), typeof(List<string>[])));
			IEnumerable[] enumArr = new InterfaceSubClass[0];
			TestChangeType((IComparable[])enumArr, enumArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IComparable[]), typeof(IEnumerable[])));

			// 从 System.Array 及其实现的接口到任何 array-type。
			int[] intArr = new int[0];
			TestChangeType((Array)intArr, intArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(Array), typeof(int[])));
			TestChangeType((IStructuralComparable)intArr, intArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IStructuralComparable), typeof(int[])));

			// 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口（前提是存在从 S 到 T 的显式标识或引用转换）。
			TestChangeType((object[])strArr, (IList<string>)strArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(object[]), typeof(IList<string>)));

			// 从 System.Collections.Generic.IList<S> 及其基接口到一维数组类型 T[]（前提是存在从 S 到 T 的显式标识或引用转换）。
			TestChangeType((IList<string>)strArr, (object[])strArr);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(IList<string>), typeof(object[])));

			// 从 System.Delegate 及其实现的接口到任何 delegate-type。
			Func<int> intFunc = () => 0;
			TestChangeType((Delegate)intFunc, intFunc);
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(Delegate), typeof(Func<int>)));

			// 委托类型间的协变和逆变。
			Assert.AreEqual(ConversionType.ExplicitReferenceConversion, Convert.GetConvertType(typeof(Converter<string, string>), typeof(Converter<object, object>)));
		}
		private class InterfaceSubClass : AnyBaseClass, IEnumerable, IComparable
		{
			public IEnumerator GetEnumerator() { return null; }
			public int CompareTo(object obj) { return 0; }
		}

		#endregion // 测试显式引用转换

		#region 测试拆箱转换

		/// <summary>
		/// 测试装箱转换。
		/// </summary>
		[TestMethod]
		public void TestUnboxConversion()
		{
			// 从 object 和 System.ValueType 到任何 non-nullable-value-type。
			TestChangeType((object)10, 10);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(object), typeof(int)));
			AnyValueType anyValueType = new AnyValueType();
			TestChangeType((object)anyValueType, anyValueType);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(object), typeof(AnyValueType)));

			TestChangeType((ValueType)10, 10);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(ValueType), typeof(int)));
			TestChangeType((ValueType)anyValueType, anyValueType);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(ValueType), typeof(AnyValueType)));

			// 从任何 interface-type 到实现 interface-type 的任何 non-nullable-value-type。
			TestChangeType((IComparable<int>)10, 10);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(IComparable<int>), typeof(int)));

			// 从 System.Enum 类型到任何 enum-type。
			TestChangeType((Enum)Tristate.True, Tristate.True);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(Enum), typeof(Tristate)));

			// 从引用类型到 non-nullable-value-type 到 nullable-type 的装箱转换。
			TestChangeType((object)10, (int?)10);
			TestChangeType((object)null, (int?)null);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(object), typeof(int?)));
			TestChangeType((ValueType)10, (int?)10);
			TestChangeType((ValueType)null, (int?)null);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(ValueType), typeof(int?)));
			TestChangeType((IComparable<int>)10, (int?)10);
			TestChangeType((IComparable<int>)null, (int?)null);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(IComparable<int>), typeof(int?)));
			TestChangeType((Enum)Tristate.True, (Tristate?)Tristate.True);
			TestChangeType((Enum)null, (Tristate?)null);
			Assert.AreEqual(ConversionType.UnboxConversion, Convert.GetConvertType(typeof(Enum), typeof(Tristate?)));

			// 如果值类型 S 具有来自接口或委托类型 I0 的取消装箱转换，且 I0 可变化转换为 I 或 I 可变化转换为 I0，
			// 则它具有来自 I 的取消装箱转换。。
			StringEnumStruct stringEnumStruct = new StringEnumStruct();
			TestChangeType(stringEnumStruct, (IEnumerable<object>)stringEnumStruct);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(StringEnumStruct), typeof(IEnumerable<object>)));
			TestChangeType((StringEnumStruct?)null, (IEnumerable<object>)null);
			Assert.AreEqual(ConversionType.BoxConversion, Convert.GetConvertType(typeof(StringEnumStruct?), typeof(IEnumerable<object>)));
		}

		#endregion // 测试拆箱转换

		#region 测试用户自定义转换

		/// <summary>
		/// 测试用户自定义转换。
		/// </summary>
		[TestMethod]
		public void TestUserDefinedConversion()
		{
			// 直接转换。
			TestChangeType(UserStruct.Default, (int)UserStruct.Default);
			TestChangeType(10, (UserStruct)10);
			TestChangeType(UserBaseClass.Default, (int)UserBaseClass.Default);
			TestChangeType(10, (UserBaseClass)10);
			TestChangeType(UserSubClass.Default, (int)UserSubClass.Default);
			// TestChangeType(10, (UserSubClass)10); // Exception

			// 多个转换。
			Assert.IsFalse(Convert.CanChangeType(typeof(UserMultiStruct), typeof(char)));
			TestChangeType(UserMultiStruct.Default, (byte)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (sbyte)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (short)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (ushort)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (int)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (uint)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (long)UserMultiStruct.Default);
			TestChangeType(UserMultiStruct.Default, (ulong)UserMultiStruct.Default);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserMultiStruct), typeof(float)));
			Assert.IsFalse(Convert.CanChangeType(typeof(UserMultiStruct), typeof(double)));
			Assert.IsFalse(Convert.CanChangeType(typeof(UserMultiStruct), typeof(decimal)));

			Assert.IsFalse(Convert.CanChangeType(typeof(char), typeof(UserMultiStruct)));
			TestChangeType((byte)0, (UserMultiStruct)(byte)0);
			TestChangeType((sbyte)0, (UserMultiStruct)(sbyte)0);
			TestChangeType((short)0, (UserMultiStruct)(short)0);
			Assert.IsFalse(Convert.CanChangeType(typeof(ushort), typeof(UserMultiStruct)));
			TestChangeType((int)0, (UserMultiStruct)(int)0);
			TestChangeType((uint)0, (UserMultiStruct)(uint)0);
			long longValue = 0;
			TestChangeType((long)0, (UserMultiStruct)longValue);
			TestChangeType((ulong)0, (UserMultiStruct)(ulong)0);
			Assert.IsFalse(Convert.CanChangeType(typeof(float), typeof(UserMultiStruct)));
			Assert.IsFalse(Convert.CanChangeType(typeof(double), typeof(UserMultiStruct)));
			Assert.IsFalse(Convert.CanChangeType(typeof(decimal), typeof(UserMultiStruct)));

			// 额外的转换。
			TestChangeType(UserStruct.Default, (char)UserStruct.Default);
			TestChangeType(UserStruct.Default, (byte)UserStruct.Default);
			TestChangeType(UserStruct.Default, (sbyte)UserStruct.Default);
			TestChangeType(UserStruct.Default, (short)UserStruct.Default);
			TestChangeType(UserStruct.Default, (ushort)UserStruct.Default);
			TestChangeType(UserStruct.Default, (int)UserStruct.Default);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct), typeof(uint)));
			TestChangeType(UserStruct.Default, (long)UserStruct.Default);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct), typeof(ulong)));
			TestChangeType(UserStruct.Default, (float)UserStruct.Default);
			TestChangeType(UserStruct.Default, (double)UserStruct.Default);
			TestChangeType(UserStruct.Default, (decimal)UserStruct.Default);

			TestChangeType((char)10, (UserStruct)(char)10);
			TestChangeType((byte)10, (UserStruct)(byte)10);
			TestChangeType((sbyte)10, (UserStruct)(sbyte)10);
			TestChangeType((short)10, (UserStruct)(short)10);
			TestChangeType((ushort)10, (UserStruct)(ushort)10);
			TestChangeType((int)10, (UserStruct)(int)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(uint), typeof(UserStruct)));
			TestChangeType((long)10, (UserStruct)(long)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(ulong), typeof(UserStruct)));
			TestChangeType((float)10, (UserStruct)(float)10);
			TestChangeType((double)10, (UserStruct)(double)10);
			TestChangeType((decimal)10, (UserStruct)(decimal)10);

			// 可空类型。
			TestChangeType(UserBaseClass.Default, (int?)UserBaseClass.Default);
			TestChangeType((int?)10, (UserBaseClass)(int?)10);
			TestChangeType((UserBaseClass)null, (int?)(UserBaseClass)null);
			TestChangeType((int?)null, (UserBaseClass)(int?)null);
			// 非可空类型转为可空类型。
			TestChangeType(UserStruct.Default, (char?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (byte?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (sbyte?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (short?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (ushort?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (int?)UserStruct.Default);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct), typeof(uint?)));
			TestChangeType(UserStruct.Default, (long?)UserStruct.Default);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct), typeof(ulong?)));
			TestChangeType(UserStruct.Default, (float?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (double?)UserStruct.Default);
			TestChangeType(UserStruct.Default, (decimal?)UserStruct.Default);

			// 可空类型转为可空类型。
			TestChangeType((char?)10, (UserStruct)(char?)10);
			TestChangeType((byte?)10, (UserStruct)(byte?)10);
			TestChangeType((sbyte?)10, (UserStruct)(sbyte?)10);
			TestChangeType((short?)10, (UserStruct)(short?)10);
			TestChangeType((ushort?)10, (UserStruct)(ushort?)10);
			TestChangeType((int?)10, (UserStruct)(int?)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(uint?), typeof(UserStruct)));
			TestChangeType((long?)10, (UserStruct)(long?)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(ulong?), typeof(UserStruct)));
			TestChangeType((float?)10, (UserStruct)(float?)10);
			TestChangeType((double?)10, (UserStruct)(double?)10);
			TestChangeType((decimal?)10, (UserStruct)(decimal?)10);

			UserStruct? value = UserStruct.Default;
			// 可空类型转为可空类型。
			TestChangeType(value, (char?)value);
			TestChangeType(value, (byte?)value);
			TestChangeType(value, (sbyte?)value);
			TestChangeType(value, (short?)value);
			TestChangeType(value, (ushort?)value);
			TestChangeType(value, (int?)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(uint?)));
			TestChangeType(value, (long?)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(ulong?)));
			TestChangeType(value, (float?)value);
			TestChangeType(value, (double?)value);
			TestChangeType(value, (decimal?)value);

			// 可空类型转为非可空类型。
			TestChangeType(value, (char)value);
			TestChangeType(value, (byte)value);
			TestChangeType(value, (sbyte)value);
			TestChangeType(value, (short)value);
			TestChangeType(value, (ushort)value);
			TestChangeType(value, (int?)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(uint)));
			TestChangeType(value, (long)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(ulong)));
			TestChangeType(value, (float)value);
			TestChangeType(value, (double)value);
			TestChangeType(value, (decimal)value);

			value = null;
			// 可空类型转为可空类型。
			TestChangeType(value, (char?)value);
			TestChangeType(value, (byte?)value);
			TestChangeType(value, (sbyte?)value);
			TestChangeType(value, (short?)value);
			TestChangeType(value, (ushort?)value);
			TestChangeType(value, (int?)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(uint?)));
			TestChangeType(value, (long?)value);
			Assert.IsFalse(Convert.CanChangeType(typeof(UserStruct?), typeof(ulong?)));
			TestChangeType(value, (float?)value);
			TestChangeType(value, (double?)value);
			TestChangeType(value, (decimal?)value);

			// 可空类型转为非可空类型。
			AssertExt.ThrowsException(() => Convert.ChangeType<UserStruct?, char>(value), typeof(InvalidOperationException));
			AssertExt.ThrowsException(() => Convert.ChangeType<UserStruct?, byte>(value), typeof(InvalidOperationException));

			// 可空类型转为可空类型。
			TestChangeType((char?)10, (UserStruct?)(char?)10);
			TestChangeType((byte?)10, (UserStruct?)(byte?)10);
			TestChangeType((sbyte?)10, (UserStruct?)(sbyte?)10);
			TestChangeType((short?)10, (UserStruct?)(short?)10);
			TestChangeType((ushort?)10, (UserStruct?)(ushort?)10);
			TestChangeType((int?)10, (UserStruct?)(int?)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(uint?), typeof(UserStruct?)));
			TestChangeType((long?)10, (UserStruct?)(long?)10);
			Assert.IsFalse(Convert.CanChangeType(typeof(ulong?), typeof(UserStruct?)));
			TestChangeType((float?)10, (UserStruct?)(float?)10);
			TestChangeType((double?)10, (UserStruct?)(double?)10);
			TestChangeType((decimal?)10, (UserStruct?)(decimal?)10);

			TestChangeType((char?)null, (UserStruct?)(char?)null);
			TestChangeType((byte?)null, (UserStruct?)(byte?)null);
			TestChangeType((sbyte?)null, (UserStruct?)(sbyte?)null);
			TestChangeType((short?)null, (UserStruct?)(short?)null);
			TestChangeType((ushort?)null, (UserStruct?)(ushort?)null);
			TestChangeType((int?)null, (UserStruct?)(int?)null);
			Assert.IsFalse(Convert.CanChangeType(typeof(uint?), typeof(UserStruct?)));
			TestChangeType((long?)null, (UserStruct?)(long?)null);
			Assert.IsFalse(Convert.CanChangeType(typeof(ulong?), typeof(UserStruct?)));
			TestChangeType((float?)null, (UserStruct?)(float?)null);
			TestChangeType((double?)null, (UserStruct?)(double?)null);
			TestChangeType((decimal?)null, (UserStruct?)(decimal?)null);

			// 可空类型的用户自定义类型转换。
			TestChangeType(UserNullableStruct.Default, (int)UserNullableStruct.Default);
			TestChangeType(10, (UserNullableStruct)10);
			TestChangeType(UserNullableStruct.Default, (int?)UserNullableStruct.Default);
			TestChangeType((int?)10, (UserNullableStruct)(int?)10);
			TestChangeType((UserNullableStruct?)UserNullableStruct.Default, (int)UserNullableStruct.Default);
			TestChangeType(10, (UserNullableStruct?)10);
			TestChangeType((UserNullableStruct?)UserNullableStruct.Default, (int?)UserNullableStruct.Default);
			TestChangeType((int?)10, (UserNullableStruct?)(int?)10);
			TestChangeType((UserNullableStruct?)null, (int?)(UserNullableStruct?)null);
			TestChangeType((int?)null, (UserNullableStruct)(int?)null);

			TestChangeType(UserNullableStruct.Default, (char)UserNullableStruct.Default);
			TestChangeType('\x0A', (UserNullableStruct)'\x0A');
			TestChangeType(UserNullableStruct.Default, (char?)UserNullableStruct.Default);
			TestChangeType((char?)10, (UserNullableStruct)(char?)10);
			TestChangeType((UserNullableStruct?)UserNullableStruct.Default, (char)UserNullableStruct.Default);
			TestChangeType((char)10, (UserNullableStruct?)(char)10);
			TestChangeType((UserNullableStruct?)UserNullableStruct.Default, (char?)UserNullableStruct.Default);
			TestChangeType((char?)10, (UserNullableStruct?)(char?)10);
			TestChangeType((UserNullableStruct?)null, (char?)(UserNullableStruct?)null);
			TestChangeType((char?)null, (UserNullableStruct)(char?)null);

			Assert.IsFalse(Convert.CanChangeType(typeof(UserNullableStruct), typeof(uint)));
			Assert.IsFalse(Convert.CanChangeType(typeof(uint), typeof(UserNullableStruct)));
			Assert.IsFalse(Convert.CanChangeType(typeof(UserNullableStruct), typeof(uint?)));
			Assert.IsFalse(Convert.CanChangeType(typeof(uint?), typeof(UserNullableStruct)));
			Assert.IsFalse(Convert.CanChangeType(typeof(UserNullableStruct?), typeof(uint)));
			Assert.IsFalse(Convert.CanChangeType(typeof(uint), typeof(UserNullableStruct?)));
			Assert.IsFalse(Convert.CanChangeType(typeof(UserNullableStruct?), typeof(uint?)));
			Assert.IsFalse(Convert.CanChangeType(typeof(uint?), typeof(UserNullableStruct?)));

			// 值类型与引用类型的用户自定义类型转换。
			TestChangeType(UserNullableStruct.Default, (string)UserNullableStruct.Default);
			TestChangeType("X", (UserNullableStruct)"X");
			TestChangeType((UserNullableStruct?)UserNullableStruct.Default, (string)UserNullableStruct.Default);
			TestChangeType("X", (UserNullableStruct?)"X");
			TestChangeType((UserNullableStruct?)null, (string)(UserNullableStruct?)null);
			TestChangeType((string)null, (UserNullableStruct?)(string)null);
		}
		private struct UserStruct
		{
			private int V;
			public static UserStruct Default = new UserStruct { V = 10 };
			public static explicit operator int(UserStruct s) { return 10; }
			public static explicit operator UserStruct(int s) { return Default; }
		}
		private class UserBaseClass
		{
			public static UserBaseClass Default = new UserBaseClass();
			public static explicit operator int(UserBaseClass s) { return 10; }
			public static explicit operator UserBaseClass(int s) { return Default; }
		}
		private class UserSubClass : UserBaseClass
		{
			public static UserSubClass Default = new UserSubClass();
		}
		private struct UserMultiStruct
		{
			private string Name;
			public static UserMultiStruct Default = new UserMultiStruct { Name = "Default" };
			public static UserMultiStruct Byte = new UserMultiStruct { Name = "Byte" };
			public static UserMultiStruct Int32 = new UserMultiStruct { Name = "Int32" };
			public static UserMultiStruct UInt64 = new UserMultiStruct { Name = "UInt64" };
			public static explicit operator byte(UserMultiStruct s) { return 1; }
			public static explicit operator UserMultiStruct(byte s) { return Byte; }
			public static explicit operator int(UserMultiStruct s) { return 2; }
			public static explicit operator UserMultiStruct(int s) { return Int32; }
			public static explicit operator ulong(UserMultiStruct s) { return 3; }
			public static explicit operator UserMultiStruct(ulong s) { return UInt64; }
			/// <summary>
			/// 返回当前对象的字符串表示形式。
			/// </summary>
			/// <returns>当前对象的字符串表示形式。</returns>
			public override string ToString()
			{
				return "[UserMultiStruct " + this.Name + "]";
			}
		}
		private struct UserNullableStruct
		{
			private int V;
			public static UserNullableStruct Default = new UserNullableStruct { V = 10 };
			public static explicit operator int?(UserNullableStruct s) { return 10; }
			public static explicit operator UserNullableStruct(int? s) { return Default; }
			public static explicit operator string(UserNullableStruct s) { return ""; }
			public static explicit operator UserNullableStruct(string s) { return Default; }
		}

		#endregion // 测试用户自定转换

		#region 测试进制转换

		/// <summary>
		/// 对 <see cref="Convert.ToSByte"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToSByte()
		{
			Assert.AreEqual(0, Convert.ToSByte("0", 13));
			Assert.AreEqual(1, Convert.ToSByte("1", 3));
			Assert.AreEqual(120, Convert.ToSByte("60", 20));
			Assert.AreEqual(127, Convert.ToSByte("9a", 13));
			Assert.AreEqual(-1, Convert.ToSByte("9c", 27));
			Assert.AreEqual(-2, Convert.ToSByte("9b", 27));
			Assert.AreEqual(-90, Convert.ToSByte("4m", 36));
			Assert.AreEqual(-128, Convert.ToSByte("4c", 29));
			AssertExt.ThrowsException(() => Convert.ToSByte("38o7", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToInt16"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt16()
		{
			Assert.AreEqual(0, Convert.ToInt16("0", 13));
			Assert.AreEqual(1, Convert.ToInt16("1", 3));
			Assert.AreEqual(14720, Convert.ToInt16("1Gg0", 20));
			Assert.AreEqual(32767, Convert.ToInt16("11bb7", 13));
			Assert.AreEqual(-1, Convert.ToInt16("38o6", 27));
			Assert.AreEqual(-2, Convert.ToInt16("38o5", 27));
			Assert.AreEqual(-21458, Convert.ToInt16("y0e", 36));
			Assert.AreEqual(-32768, Convert.ToInt16("19rr", 29));
			AssertExt.ThrowsException(() => Convert.ToInt16("38o7", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToInt32"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt32()
		{
			Assert.AreEqual(0, Convert.ToInt32("0", 13));
			Assert.AreEqual(1, Convert.ToInt32("1", 3));
			Assert.AreEqual(14720, Convert.ToInt32("1Gg0", 20));
			Assert.AreEqual(2147483647, Convert.ToInt32("282ba4aaa", 13));
			Assert.AreEqual(-1, Convert.ToInt32("b28jpdl", 27));
			Assert.AreEqual(-2, Convert.ToInt32("b28jpdk", 27));
			Assert.AreEqual(-1235678902, Convert.ToInt32("1elf616", 36));
			Assert.AreEqual(-2147483648, Convert.ToInt32("3hk7988", 29));
			AssertExt.ThrowsException(() => Convert.ToInt32("b28jpdm", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToInt64"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt64()
		{
			Assert.AreEqual(0L, Convert.ToInt64("0", 13));
			Assert.AreEqual(1L, Convert.ToInt64("1", 3));
			Assert.AreEqual(14720L, Convert.ToInt64("1Gg0", 20));
			Assert.AreEqual(9223372036854775807L, Convert.ToInt64("10B269549075433C37", 13));
			Assert.AreEqual(-1L, Convert.ToInt64("4Eo8hfam6fllmo", 27));
			Assert.AreEqual(-2L, Convert.ToInt64("4Eo8hfam6fllmn", 27));
			Assert.AreEqual(-8071017880399937603L, Convert.ToInt64("26tvjyybszf7h", 36));
			Assert.AreEqual(-9223372036854775808L, Convert.ToInt64("q1se8f0m04isc", 29));
			AssertExt.ThrowsException(() => Convert.ToInt64("4Eo8hfam6fllmp", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToByte"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToByte()
		{
			Assert.AreEqual(0, Convert.ToByte("0", 13));
			Assert.AreEqual(1, Convert.ToByte("1", 3));
			Assert.AreEqual(120, Convert.ToByte("60", 20));
			Assert.AreEqual(127, Convert.ToByte("9a", 13));
			Assert.AreEqual(128, Convert.ToByte("4c", 29));
			Assert.AreEqual(166, Convert.ToByte("4m", 36));
			Assert.AreEqual(254, Convert.ToByte("9b", 27));
			Assert.AreEqual(255, Convert.ToByte("9c", 27));
			AssertExt.ThrowsException(() => Convert.ToByte("38o7", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToUInt16"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt16()
		{
			Assert.AreEqual(0, Convert.ToUInt16("0", 13));
			Assert.AreEqual(1, Convert.ToUInt16("1", 3));
			Assert.AreEqual(14720, Convert.ToUInt16("1Gg0", 20));
			Assert.AreEqual(32767, Convert.ToUInt16("11bb7", 13));
			Assert.AreEqual(32768, Convert.ToUInt16("19rr", 29));
			Assert.AreEqual(44078, Convert.ToUInt16("y0e", 36));
			Assert.AreEqual(65534, Convert.ToUInt16("38o5", 27));
			Assert.AreEqual(65535, Convert.ToUInt16("38o6", 27));
			AssertExt.ThrowsException(() => Convert.ToUInt16("38o7", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToUInt32"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt32()
		{
			Assert.AreEqual(0U, Convert.ToUInt32("0", 13));
			Assert.AreEqual(1U, Convert.ToUInt32("1", 3));
			Assert.AreEqual(14720U, Convert.ToUInt32("1Gg0", 20));
			Assert.AreEqual(2147483647U, Convert.ToUInt32("282ba4aaa", 13));
			Assert.AreEqual(2147483648U, Convert.ToUInt32("3hk7988", 29));
			Assert.AreEqual(3059288394U, Convert.ToUInt32("1elf616", 36));
			Assert.AreEqual(4294967294U, Convert.ToUInt32("b28jpdk", 27));
			Assert.AreEqual(4294967295U, Convert.ToUInt32("b28jpdl", 27));
			AssertExt.ThrowsException(() => Convert.ToUInt32("b28jpdm", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <see cref="Convert.ToUInt64"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt64()
		{
			Assert.AreEqual(0UL, Convert.ToUInt64("0", 13));
			Assert.AreEqual(1UL, Convert.ToUInt64("1", 3));
			Assert.AreEqual(14720UL, Convert.ToUInt64("1Gg0", 20));
			Assert.AreEqual(9223372036854775807UL, Convert.ToUInt64("10B269549075433C37", 13));
			Assert.AreEqual(9223372036854775808UL, Convert.ToUInt64("q1se8f0m04isc", 29));
			Assert.AreEqual(10375726193309614013UL, Convert.ToUInt64("26tvjyybszf7h", 36));
			Assert.AreEqual(18446744073709551614UL, Convert.ToUInt64("4Eo8hfam6fllmn", 27));
			Assert.AreEqual(18446744073709551615UL, Convert.ToUInt64("4Eo8hfam6fllmo", 27));
			AssertExt.ThrowsException(() => Convert.ToUInt64("4Eo8hfam6fllmp", 27), typeof(OverflowException));
		}
		/// <summary>
		/// 对 <c>Convert.ToString()</c> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToString()
		{
			// 对转换 SByte 进行测试。
			Assert.AreEqual("0", Convert.ToString((sbyte)0, 13));
			Assert.AreEqual("1", Convert.ToString((sbyte)1, 3));
			Assert.AreEqual("60", Convert.ToString((sbyte)120, 20));
			Assert.AreEqual("9A", Convert.ToString((sbyte)127, 13));
			Assert.AreEqual("9C", Convert.ToString((sbyte)-1, 27));
			Assert.AreEqual("-2", Convert.ToString((sbyte)-2, 10));
			Assert.AreEqual("4M", Convert.ToString((sbyte)-90, 36));
			Assert.AreEqual("4C", Convert.ToString((sbyte)-128, 29));

			// 对转换 Int16 进行测试。
			Assert.AreEqual("0", Convert.ToString((short)0, 13));
			Assert.AreEqual("1", Convert.ToString((short)1, 3));
			Assert.AreEqual("1GG0", Convert.ToString((short)14720, 20));
			Assert.AreEqual("11BB7", Convert.ToString((short)32767, 13));
			Assert.AreEqual("38O6", Convert.ToString((short)-1, 27));
			Assert.AreEqual("-2", Convert.ToString((short)-2, 10));
			Assert.AreEqual("Y0E", Convert.ToString((short)-21458, 36));
			Assert.AreEqual("19RR", Convert.ToString((short)-32768, 29));

			// 对转换 Int32 进行测试。
			Assert.AreEqual("0", Convert.ToString(0, 13));
			Assert.AreEqual("1", Convert.ToString(1, 3));
			Assert.AreEqual("1GG0", Convert.ToString(14720, 20));
			Assert.AreEqual("282BA4AAA", Convert.ToString(2147483647, 13));
			Assert.AreEqual("B28JPDL", Convert.ToString(-1, 27));
			Assert.AreEqual("-2", Convert.ToString(-2, 10));
			Assert.AreEqual("1ELF616", Convert.ToString(-1235678902, 36));
			Assert.AreEqual("3HK7988", Convert.ToString(-2147483648, 29));

			// 对转换 Int64 进行测试。
			Assert.AreEqual("0", Convert.ToString(0L, 13));
			Assert.AreEqual("1", Convert.ToString(1L, 3));
			Assert.AreEqual("1GG0", Convert.ToString(14720L, 20));
			Assert.AreEqual("10B269549075433C37", Convert.ToString(9223372036854775807L, 13));
			Assert.AreEqual("4EO8HFAM6FLLMO", Convert.ToString(-1L, 27));
			Assert.AreEqual("-2", Convert.ToString(-2L, 10));
			Assert.AreEqual("26TVJYYBSZF7H", Convert.ToString(-8071017880399937603L, 36));
			Assert.AreEqual("Q1SE8F0M04ISC", Convert.ToString(-9223372036854775808L, 29));

			// 对转换 Byte 进行测试。
			Assert.AreEqual("0", Convert.ToString((byte)0, 13));
			Assert.AreEqual("1", Convert.ToString((byte)1, 3));
			Assert.AreEqual("60", Convert.ToString((byte)120, 20));
			Assert.AreEqual("9A", Convert.ToString((byte)127, 13));
			Assert.AreEqual("4C", Convert.ToString((byte)128, 29));
			Assert.AreEqual("4M", Convert.ToString((byte)166, 36));
			Assert.AreEqual("254", Convert.ToString((byte)254, 10));
			Assert.AreEqual("9C", Convert.ToString((byte)255, 27));

			// 对转换 UInt16 进行测试。
			Assert.AreEqual("0", Convert.ToString((ushort)0, 13));
			Assert.AreEqual("1", Convert.ToString((ushort)1, 3));
			Assert.AreEqual("1GG0", Convert.ToString((ushort)14720, 20));
			Assert.AreEqual("11BB7", Convert.ToString((ushort)32767, 13));
			Assert.AreEqual("19RR", Convert.ToString((ushort)32768, 29));
			Assert.AreEqual("Y0E", Convert.ToString((ushort)44078, 36));
			Assert.AreEqual("65534", Convert.ToString((ushort)65534, 10));
			Assert.AreEqual("38O6", Convert.ToString((ushort)65535, 27));

			// 对转换 UInt32 进行测试。
			Assert.AreEqual("0", Convert.ToString(0U, 13));
			Assert.AreEqual("1", Convert.ToString(1U, 3));
			Assert.AreEqual("1GG0", Convert.ToString(14720U, 20));
			Assert.AreEqual("282BA4AAA", Convert.ToString(2147483647U, 13));
			Assert.AreEqual("3HK7988", Convert.ToString(2147483648U, 29));
			Assert.AreEqual("1ELF616", Convert.ToString(3059288394U, 36));
			Assert.AreEqual("4294967294", Convert.ToString(4294967294U, 10));
			Assert.AreEqual("B28JPDL", Convert.ToString(4294967295U, 27));

			// 对转换 UInt64 进行测试。
			Assert.AreEqual("0", Convert.ToString(0U, 13));
			Assert.AreEqual("1", Convert.ToString(1U, 3));
			Assert.AreEqual("1GG0", Convert.ToString(14720U, 20));
			Assert.AreEqual("10B269549075433C37", Convert.ToString(9223372036854775807UL, 13));
			Assert.AreEqual("Q1SE8F0M04ISC", Convert.ToString(9223372036854775808UL, 29));
			Assert.AreEqual("26TVJYYBSZF7H", Convert.ToString(10375726193309614013UL, 36));
			Assert.AreEqual("18446744073709551614", Convert.ToString(18446744073709551614UL, 10));
			Assert.AreEqual("4EO8HFAM6FLLMO", Convert.ToString(18446744073709551615UL, 27));
		}

		#endregion // 测试进制转换

	}
}
