using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="GenericConvert"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestGenericConvert
	{

		#region 数值类型间转换

		private static readonly ulong[] NumericTestValues = {
			0UL, 1UL,
			126UL, 127UL, 128UL, 129UL,
			254UL, 255UL, 256UL, 257UL,
			32766UL, 32767UL, 32768UL, 32769UL,
			65534UL, 65535UL, 65536UL, 65537UL,
			2147483646UL, 2147483647UL, 2147483648UL, 2147483649UL,
			4294967294UL, 4294967295UL, 4294967296UL, 4294967297UL,
			9223372036854775806UL, 9223372036854775807UL, 9223372036854775808UL, 9223372036854775809UL,
			18446744073709551614UL, 18446744073709551615UL,
		};

		/// <summary>
		/// 对 <see cref="char"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestCharToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				char value = unchecked((char)originValue);
				TestConvert<char, char>(value, unchecked((char)value));
				TestConvert<char, sbyte>(value, unchecked((sbyte)value));
				TestConvert<char, byte>(value, unchecked((byte)value));
				TestConvert<char, short>(value, unchecked((short)value));
				TestConvert<char, ushort>(value, unchecked((ushort)value));
				TestConvert<char, int>(value, unchecked((int)value));
				TestConvert<char, uint>(value, unchecked((uint)value));
				TestConvert<char, long>(value, unchecked((long)value));
				TestConvert<char, ulong>(value, unchecked((ulong)value));
				TestConvert<char, float>(value, unchecked((float)value));
				TestConvert<char, double>(value, unchecked((double)value));
				TestConvert<char, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="sbyte"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestSByteToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				sbyte value = unchecked((sbyte)originValue);
				TestConvert<sbyte, char>(value, unchecked((char)value));
				TestConvert<sbyte, sbyte>(value, unchecked((sbyte)value));
				TestConvert<sbyte, byte>(value, unchecked((byte)value));
				TestConvert<sbyte, short>(value, unchecked((short)value));
				TestConvert<sbyte, ushort>(value, unchecked((ushort)value));
				TestConvert<sbyte, int>(value, unchecked((int)value));
				TestConvert<sbyte, uint>(value, unchecked((uint)value));
				TestConvert<sbyte, long>(value, unchecked((long)value));
				TestConvert<sbyte, ulong>(value, unchecked((ulong)value));
				TestConvert<sbyte, float>(value, unchecked((float)value));
				TestConvert<sbyte, double>(value, unchecked((double)value));
				TestConvert<sbyte, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="byte"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestByteToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				byte value = unchecked((byte)originValue);
				TestConvert<byte, char>(value, unchecked((char)value));
				TestConvert<byte, sbyte>(value, unchecked((sbyte)value));
				TestConvert<byte, byte>(value, unchecked((byte)value));
				TestConvert<byte, short>(value, unchecked((short)value));
				TestConvert<byte, ushort>(value, unchecked((ushort)value));
				TestConvert<byte, int>(value, unchecked((int)value));
				TestConvert<byte, uint>(value, unchecked((uint)value));
				TestConvert<byte, long>(value, unchecked((long)value));
				TestConvert<byte, ulong>(value, unchecked((ulong)value));
				TestConvert<byte, float>(value, unchecked((float)value));
				TestConvert<byte, double>(value, unchecked((double)value));
				TestConvert<byte, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="short"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestInt16ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				short value = unchecked((short)originValue);
				TestConvert<short, char>(value, unchecked((char)value));
				TestConvert<short, sbyte>(value, unchecked((sbyte)value));
				TestConvert<short, byte>(value, unchecked((byte)value));
				TestConvert<short, short>(value, unchecked((short)value));
				TestConvert<short, ushort>(value, unchecked((ushort)value));
				TestConvert<short, int>(value, unchecked((int)value));
				TestConvert<short, uint>(value, unchecked((uint)value));
				TestConvert<short, long>(value, unchecked((long)value));
				TestConvert<short, ulong>(value, unchecked((ulong)value));
				TestConvert<short, float>(value, unchecked((float)value));
				TestConvert<short, double>(value, unchecked((double)value));
				TestConvert<short, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="ushort"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestUInt16ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				ushort value = unchecked((ushort)originValue);
				TestConvert<ushort, char>(value, unchecked((char)value));
				TestConvert<ushort, sbyte>(value, unchecked((sbyte)value));
				TestConvert<ushort, byte>(value, unchecked((byte)value));
				TestConvert<ushort, short>(value, unchecked((short)value));
				TestConvert<ushort, ushort>(value, unchecked((ushort)value));
				TestConvert<ushort, int>(value, unchecked((int)value));
				TestConvert<ushort, uint>(value, unchecked((uint)value));
				TestConvert<ushort, long>(value, unchecked((long)value));
				TestConvert<ushort, ulong>(value, unchecked((ulong)value));
				TestConvert<ushort, float>(value, unchecked((float)value));
				TestConvert<ushort, double>(value, unchecked((double)value));
				TestConvert<ushort, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="int"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestInt32ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				int value = unchecked((int)originValue);
				TestConvert<int, char>(value, unchecked((char)value));
				TestConvert<int, sbyte>(value, unchecked((sbyte)value));
				TestConvert<int, byte>(value, unchecked((byte)value));
				TestConvert<int, short>(value, unchecked((short)value));
				TestConvert<int, ushort>(value, unchecked((ushort)value));
				TestConvert<int, int>(value, unchecked((int)value));
				TestConvert<int, uint>(value, unchecked((uint)value));
				TestConvert<int, long>(value, unchecked((long)value));
				TestConvert<int, ulong>(value, unchecked((ulong)value));
				TestConvert<int, float>(value, unchecked((float)value));
				TestConvert<int, double>(value, unchecked((double)value));
				TestConvert<int, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="uint"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestUInt32ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				uint value = unchecked((uint)originValue);
				TestConvert<uint, char>(value, unchecked((char)value));
				TestConvert<uint, sbyte>(value, unchecked((sbyte)value));
				TestConvert<uint, byte>(value, unchecked((byte)value));
				TestConvert<uint, short>(value, unchecked((short)value));
				TestConvert<uint, ushort>(value, unchecked((ushort)value));
				TestConvert<uint, int>(value, unchecked((int)value));
				TestConvert<uint, uint>(value, unchecked((uint)value));
				TestConvert<uint, long>(value, unchecked((long)value));
				TestConvert<uint, ulong>(value, unchecked((ulong)value));
				TestConvert<uint, float>(value, unchecked((float)value));
				TestConvert<uint, double>(value, unchecked((double)value));
				TestConvert<uint, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="long"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestInt64ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				long value = unchecked((long)originValue);
				TestConvert<long, char>(value, unchecked((char)value));
				TestConvert<long, sbyte>(value, unchecked((sbyte)value));
				TestConvert<long, byte>(value, unchecked((byte)value));
				TestConvert<long, short>(value, unchecked((short)value));
				TestConvert<long, ushort>(value, unchecked((ushort)value));
				TestConvert<long, int>(value, unchecked((int)value));
				TestConvert<long, uint>(value, unchecked((uint)value));
				TestConvert<long, long>(value, unchecked((long)value));
				TestConvert<long, ulong>(value, unchecked((ulong)value));
				TestConvert<long, float>(value, unchecked((float)value));
				TestConvert<long, double>(value, unchecked((double)value));
				TestConvert<long, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="ulong"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestUInt64ToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				ulong value = unchecked((ulong)originValue);
				TestConvert<ulong, char>(value, unchecked((char)value));
				TestConvert<ulong, sbyte>(value, unchecked((sbyte)value));
				TestConvert<ulong, byte>(value, unchecked((byte)value));
				TestConvert<ulong, short>(value, unchecked((short)value));
				TestConvert<ulong, ushort>(value, unchecked((ushort)value));
				TestConvert<ulong, int>(value, unchecked((int)value));
				TestConvert<ulong, uint>(value, unchecked((uint)value));
				TestConvert<ulong, long>(value, unchecked((long)value));
				TestConvert<ulong, ulong>(value, unchecked((ulong)value));
				TestConvert<ulong, float>(value, unchecked((float)value));
				TestConvert<ulong, double>(value, unchecked((double)value));
				TestConvert<ulong, decimal>(value, unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="float"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestSingleToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				float value = unchecked((float)originValue);
				TestConvert<float, char>(value, unchecked((char)value));
				TestConvert<float, sbyte>(value, unchecked((sbyte)value));
				TestConvert<float, byte>(value, unchecked((byte)value));
				TestConvert<float, short>(value, unchecked((short)value));
				TestConvert<float, ushort>(value, unchecked((ushort)value));
				TestConvert<float, int>(value, unchecked((int)value));
				TestConvert<float, uint>(value, unchecked((uint)value));
				TestConvert<float, long>(value, unchecked((long)value));
				TestConvert<float, ulong>(value, unchecked((ulong)value));
				TestConvert<float, float>(value, unchecked((float)value));
				TestConvert<float, double>(value, unchecked((double)value));
				TestConvert<float, decimal>(value, unchecked((decimal)value));
			}

			float[] TestValues = {
				float.NaN, float.Epsilon, float.PositiveInfinity, float.NegativeInfinity,
				float.MinValue, float.MaxValue, 2.3F, 126.88F, 127.1F, 32510.2F,
			};
			foreach (float value in TestValues)
			{
				TestConvert<float, char>(value, unchecked((char)value));
				TestConvert<float, sbyte>(value, unchecked((sbyte)value));
				TestConvert<float, byte>(value, unchecked((byte)value));
				TestConvert<float, short>(value, unchecked((short)value));
				TestConvert<float, ushort>(value, unchecked((ushort)value));
				TestConvert<float, int>(value, unchecked((int)value));
				TestConvert<float, uint>(value, unchecked((uint)value));
				TestConvert<float, long>(value, unchecked((long)value));
				TestConvert<float, ulong>(value, unchecked((ulong)value));
				TestConvert<float, float>(value, unchecked((float)value));
				TestConvert<float, double>(value, unchecked((double)value));
				TestConvertException<float, decimal, OverflowException>(value, () => unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="double"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestDoubleToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				double value = unchecked((double)originValue);
				TestConvert<double, char>(value, unchecked((char)value));
				TestConvert<double, sbyte>(value, unchecked((sbyte)value));
				TestConvert<double, byte>(value, unchecked((byte)value));
				TestConvert<double, short>(value, unchecked((short)value));
				TestConvert<double, ushort>(value, unchecked((ushort)value));
				TestConvert<double, int>(value, unchecked((int)value));
				TestConvert<double, uint>(value, unchecked((uint)value));
				TestConvert<double, long>(value, unchecked((long)value));
				TestConvert<double, ulong>(value, unchecked((ulong)value));
				TestConvert<double, float>(value, unchecked((float)value));
				TestConvert<double, double>(value, unchecked((double)value));
				TestConvert<double, decimal>(value, unchecked((decimal)value));
			}

			double[] TestValues = {
				double.NaN, double.Epsilon, double.PositiveInfinity, double.NegativeInfinity,
				double.MinValue, double.MaxValue, 2.3, 126.88, 127.1, 32510.2,
			};
			foreach (double value in TestValues)
			{
				TestConvert<double, char>(value, unchecked((char)value));
				TestConvert<double, sbyte>(value, unchecked((sbyte)value));
				TestConvert<double, byte>(value, unchecked((byte)value));
				TestConvert<double, short>(value, unchecked((short)value));
				TestConvert<double, ushort>(value, unchecked((ushort)value));
				TestConvert<double, int>(value, unchecked((int)value));
				TestConvert<double, uint>(value, unchecked((uint)value));
				TestConvert<double, long>(value, unchecked((long)value));
				TestConvert<double, ulong>(value, unchecked((ulong)value));
				TestConvert<double, float>(value, unchecked((float)value));
				TestConvert<double, double>(value, unchecked((double)value));
				TestConvertException<double, decimal, OverflowException>(value, () => unchecked((decimal)value));
			}
		}

		/// <summary>
		/// 对 <see cref="decimal"/> 转换到其它数值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestDecimalToNumeric()
		{
			foreach (ulong originValue in NumericTestValues)
			{
				decimal value = unchecked((decimal)originValue);
				TestConvertException<decimal, char, OverflowException>(value, () => unchecked((char)value));
				TestConvertException<decimal, sbyte, OverflowException>(value, () => unchecked((sbyte)value));
				TestConvertException<decimal, byte, OverflowException>(value, () => unchecked((byte)value));
				TestConvertException<decimal, short, OverflowException>(value, () => unchecked((short)value));
				TestConvertException<decimal, ushort, OverflowException>(value, () => unchecked((ushort)value));
				TestConvertException<decimal, int, OverflowException>(value, () => unchecked((int)value));
				TestConvertException<decimal, uint, OverflowException>(value, () => unchecked((uint)value));
				TestConvertException<decimal, long, OverflowException>(value, () => unchecked((long)value));
				TestConvertException<decimal, ulong, OverflowException>(value, () => unchecked((ulong)value));
				TestConvert<decimal, float>(value, unchecked((float)value));
				TestConvert<decimal, double>(value, unchecked((double)value));
			}
			decimal[] TestValues = {
				decimal.Zero, decimal.One, decimal.MinusOne, decimal.MinValue, decimal.MaxValue,
				(decimal)2.3, (decimal)126.88, (decimal)127.1, (decimal)32510.2,
			};
			foreach (decimal value in TestValues)
			{
				TestConvertException<decimal, char, OverflowException>(value, () => unchecked((char)value));
				TestConvertException<decimal, sbyte, OverflowException>(value, () => unchecked((sbyte)value));
				TestConvertException<decimal, byte, OverflowException>(value, () => unchecked((byte)value));
				TestConvertException<decimal, short, OverflowException>(value, () => unchecked((short)value));
				TestConvertException<decimal, ushort, OverflowException>(value, () => unchecked((ushort)value));
				TestConvertException<decimal, int, OverflowException>(value, () => unchecked((int)value));
				TestConvertException<decimal, uint, OverflowException>(value, () => unchecked((uint)value));
				TestConvertException<decimal, long, OverflowException>(value, () => unchecked((long)value));
				TestConvertException<decimal, ulong, OverflowException>(value, () => unchecked((ulong)value));
				TestConvert<decimal, float>(value, unchecked((float)value));
				TestConvert<decimal, double>(value, unchecked((double)value));
			}
		}

		#endregion // 数值类型间转换

		#region 值类型间转换

		private enum TestEnum1
		{
			A, B
		}
		private enum TestEnum2 : byte
		{
			A, B
		}
		private enum TestEnum3 : ulong
		{
			A, B
		}

		/// <summary>
		/// 对数值和枚举类型间的转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestNumericOrEnum()
		{
			int[] TestValues = { 0, 1, 2, 3 };
			foreach (int originValue in TestValues)
			{
				{
					int value = originValue;
					TestConvert<int, TestEnum1>(value, (TestEnum1)value);
					TestConvert<int, TestEnum2>(value, (TestEnum2)value);
					TestConvert<int, TestEnum3>(value, (TestEnum3)value);
				}
				{
					sbyte value = unchecked((sbyte)originValue);
					TestConvert<sbyte, TestEnum1>(value, (TestEnum1)value);
					TestConvert<sbyte, TestEnum2>(value, (TestEnum2)value);
					TestConvert<sbyte, TestEnum3>(value, (TestEnum3)value);
				}
				{
					decimal value = unchecked((decimal)originValue);
					TestConvert<decimal, TestEnum1>(value, (TestEnum1)value);
					TestConvert<decimal, TestEnum2>(value, (TestEnum2)value);
					TestConvert<decimal, TestEnum3>(value, (TestEnum3)value);
				}
				{
					TestEnum1 value = unchecked((TestEnum1)originValue);
					TestConvert<TestEnum1, int>(value, (int)value);
					TestConvert<TestEnum1, sbyte>(value, (sbyte)value);
					TestConvert<TestEnum1, decimal>(value, (decimal)value);
					TestConvert<TestEnum1, TestEnum1>(value, (TestEnum1)value);
					TestConvert<TestEnum1, TestEnum2>(value, (TestEnum2)value);
					TestConvert<TestEnum1, TestEnum3>(value, (TestEnum3)value);
				}
				{
					TestEnum2 value = unchecked((TestEnum2)originValue);
					TestConvert<TestEnum2, int>(value, (int)value);
					TestConvert<TestEnum2, sbyte>(value, (sbyte)value);
					TestConvert<TestEnum2, decimal>(value, (decimal)value);
					TestConvert<TestEnum2, TestEnum1>(value, (TestEnum1)value);
					TestConvert<TestEnum2, TestEnum2>(value, (TestEnum2)value);
					TestConvert<TestEnum2, TestEnum3>(value, (TestEnum3)value);
				}
				{
					TestEnum3 value = unchecked((TestEnum3)originValue);
					TestConvert<TestEnum3, int>(value, (int)value);
					TestConvert<TestEnum3, sbyte>(value, (sbyte)value);
					TestConvert<TestEnum3, decimal>(value, (decimal)value);
					TestConvert<TestEnum3, TestEnum1>(value, (TestEnum1)value);
					TestConvert<TestEnum3, TestEnum2>(value, (TestEnum2)value);
					TestConvert<TestEnum3, TestEnum3>(value, (TestEnum3)value);
				}
			}
		}

		/// <summary>
		/// 对可空类型转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestNullable()
		{
#pragma warning disable CS8629 // 可为 null 的值类型可为 null。
			int?[] TestValues = { null, 0, 1, 2, 3 };
			foreach (int? originValue in TestValues)
			{
				// S => T?
				try
				{
					int value = unchecked((int)originValue);
					TestConvert<int, int?>(value, (int?)value);
					TestConvert<int, byte?>(value, (byte?)value);
					TestConvert<int, ulong?>(value, (ulong?)value);
					TestConvert<int, decimal?>(value, (decimal?)value);
					TestConvert<int, TestEnum1?>(value, (TestEnum1?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					sbyte value = unchecked((sbyte)originValue);
					TestConvert<sbyte, int?>(value, (int?)value);
					TestConvert<sbyte, byte?>(value, (byte?)value);
					TestConvert<sbyte, ulong?>(value, (ulong?)value);
					TestConvert<sbyte, decimal?>(value, (decimal?)value);
					TestConvert<sbyte, TestEnum2?>(value, (TestEnum2?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					decimal value = unchecked((decimal)originValue);
					TestConvert<decimal, int?>(value, (int?)value);
					TestConvert<decimal, byte?>(value, (byte?)value);
					TestConvert<decimal, ulong?>(value, (ulong?)value);
					TestConvert<decimal, decimal?>(value, (decimal?)value);
					TestConvert<decimal, TestEnum3?>(value, (TestEnum3?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					TestEnum1 value = unchecked((TestEnum1)originValue);
					TestConvert<TestEnum1, int?>(value, (int?)value);
					TestConvert<TestEnum1, sbyte?>(value, (sbyte?)value);
					TestConvert<TestEnum1, decimal?>(value, (decimal?)value);
					TestConvert<TestEnum1, TestEnum1?>(value, (TestEnum1?)value);
					TestConvert<TestEnum1, TestEnum2?>(value, (TestEnum2?)value);
					TestConvert<TestEnum1, TestEnum3?>(value, (TestEnum3?)value);
				}
				catch (InvalidOperationException) { }
				// S? => T / S? => T?
				try
				{
					int? value = unchecked((int?)originValue);
					TestConvert<int?, int>(value, (int)value);
					TestConvert<int?, byte>(value, (byte)value);
					TestConvert<int?, ulong>(value, (ulong)value);
					TestConvert<int?, decimal>(value, (decimal)value);
					TestConvert<int?, TestEnum1>(value, (TestEnum1)value);
					TestConvert<int?, int?>(value, (int?)value);
					TestConvert<int?, byte?>(value, (byte?)value);
					TestConvert<int?, ulong?>(value, (ulong?)value);
					TestConvert<int?, decimal?>(value, (decimal?)value);
					TestConvert<int?, TestEnum1?>(value, (TestEnum1?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					sbyte? value = unchecked((sbyte?)originValue);
					TestConvert<sbyte?, int>(value, (int)value);
					TestConvert<sbyte?, byte>(value, (byte)value);
					TestConvert<sbyte?, ulong>(value, (ulong)value);
					TestConvert<sbyte?, decimal>(value, (decimal)value);
					TestConvert<sbyte?, TestEnum2>(value, (TestEnum2)value);
					TestConvert<sbyte?, int?>(value, (int?)value);
					TestConvert<sbyte?, byte?>(value, (byte?)value);
					TestConvert<sbyte?, ulong?>(value, (ulong?)value);
					TestConvert<sbyte?, decimal?>(value, (decimal?)value);
					TestConvert<sbyte?, TestEnum2?>(value, (TestEnum2?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					decimal? value = unchecked((decimal?)originValue);
					TestConvert<decimal?, int>(value, (int)value);
					TestConvert<decimal?, byte>(value, (byte)value);
					TestConvert<decimal?, ulong>(value, (ulong)value);
					TestConvert<decimal?, decimal>(value, (decimal)value);
					TestConvert<decimal?, TestEnum3>(value, (TestEnum3)value);
					TestConvert<decimal?, int?>(value, (int?)value);
					TestConvert<decimal?, byte?>(value, (byte?)value);
					TestConvert<decimal?, ulong?>(value, (ulong?)value);
					TestConvert<decimal?, decimal?>(value, (decimal?)value);
					TestConvert<decimal?, TestEnum3?>(value, (TestEnum3?)value);
				}
				catch (InvalidOperationException) { }
				try
				{
					TestEnum1? value = unchecked((TestEnum1?)originValue);
					TestConvert<TestEnum1?, int>(value, (int)value);
					TestConvert<TestEnum1?, sbyte>(value, (sbyte)value);
					TestConvert<TestEnum1?, decimal>(value, (decimal)value);
					TestConvert<TestEnum1?, TestEnum1>(value, (TestEnum1)value);
					TestConvert<TestEnum1?, TestEnum2>(value, (TestEnum2)value);
					TestConvert<TestEnum1?, TestEnum3>(value, (TestEnum3)value);
					TestConvert<TestEnum1?, int?>(value, (int?)value);
					TestConvert<TestEnum1?, sbyte?>(value, (sbyte?)value);
					TestConvert<TestEnum1?, decimal?>(value, (decimal?)value);
					TestConvert<TestEnum1?, TestEnum1?>(value, (TestEnum1?)value);
					TestConvert<TestEnum1?, TestEnum2?>(value, (TestEnum2?)value);
					TestConvert<TestEnum1?, TestEnum3?>(value, (TestEnum3?)value);
				}
				catch (InvalidOperationException) { }
			}
#pragma warning restore CS8629 // 可为 null 的值类型可为 null。
		}

		#endregion // 值类型间转换

		#region 装箱、拆箱转换

		private struct TestValueType { }
		private struct TestEnumStruct : IEnumerable<string>
		{
			public IEnumerator<string> GetEnumerator() { yield return "10"; }
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}

		/// <summary>
		/// 对装箱转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestBox()
		{
			// 从 non-nullable-value-type 到 object。
			{
				int value = 1;
				TestConvert<int, object>(value, value);
			}
			{
				TypeCode value = TypeCode.SByte;
				TestConvert<TypeCode, object>(value, value);
			}
			{
				TestValueType value = new();
				TestConvert<TestValueType, object>(value, value);
			}

			// 从 non-nullable-value-type 到 System.ValueType。
			{
				int value = 1;
				TestConvert<int, ValueType>(value, value);
			}
			{
				TestValueType value = new();
				TestConvert<TestValueType, ValueType>(value, value);
			}

			// 从 non-nullable-value-type 到其实现的接口。
			{
				int value = 1;
				TestConvert<int, IConvertible>(value, value);
			}
			{
				TypeCode value = TypeCode.SByte;
				TestConvert<TypeCode, IComparable>(value, value);
			}

			// 从 enum-type 转换为 System.Enum 类型。
			{
				TypeCode value = TypeCode.Int16;
				TestConvert<TypeCode, Enum>(value, value);
			}

			// 从 nullable-type 到 non-nullable-value-type 到该引用类型的装箱转换。
			{
				int? value = 1;
				TestConvert<int?, object>(value, value);
				TestConvert<int?, object>(value, value);
				value = null;
				TestConvert<int?, object>(value, value!);
				TestConvert<int?, IComparable<int>>(value, value!);
			}
			{
				TypeCode? value = TypeCode.SByte;
				TestConvert<TypeCode?, object>(value, value);
				TestConvert<TypeCode?, IConvertible>(value, value);
				TestConvert<TypeCode?, Enum>(value, value);
				value = null;
				TestConvert<TypeCode?, object>(value, value!);
				TestConvert<TypeCode?, IConvertible>(value, value!);
				TestConvert<TypeCode?, Enum>(value, value!);
			}

			// 如果值类型具有到接口或委托类型 I0 的装箱转换，且 I0 变化转换为接口类型 I，则值类型具有到 I 的装箱转换。
			{
				TestEnumStruct value = new();
				TestConvert<TestEnumStruct, IEnumerable<object>>(value, value);
			}
			{
				TestEnumStruct? value = new();
				TestConvert<TestEnumStruct?, IEnumerable<object>>(value, value);
				value = null;
				TestConvert<TestEnumStruct?, IEnumerable<object>>(value, value!);
			}
		}

		/// <summary>
		/// 对拆箱转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestUnbox()
		{
			// 从 object 和 System.ValueType 到任何 non-nullable-value-type。
			{
				int value = 1;
				TestConvert<object, int>(value, value);
				TestConvert<ValueType, int>(value, value);
			}
			{
				TypeCode value = TypeCode.DateTime;
				TestConvert<object, TypeCode>(value, value);
				TestConvert<ValueType, TypeCode>(value, value);
			}
			{
				TestValueType value = new();
				TestConvert<object, TestValueType>(value, value);
				TestConvert<ValueType, TestValueType>(value, value);
			}

			// 从任何 interface-type 到实现 interface-type 的任何 non-nullable-value-type。
			{
				int value = 1;
				TestConvert<IComparable<int>, int>(value, value);
			}
			{
				TypeCode value = TypeCode.DateTime;
				TestConvert<IConvertible, TypeCode>(value, value);
			}

			// 从 System.Enum 类型到任何 enum-type。
			{
				TypeCode value = TypeCode.DateTime;
				TestConvert<Enum, TypeCode>(value, value);
			}

			// 从引用类型到 non-nullable-value-type 到 nullable-type 的装箱转换。
			{
				int? value = 10;
				TestConvert<object, int?>(value, value);
				TestConvert<ValueType, int?>(value, value);
				TestConvert<IComparable<int>, int?>(value, value);
				value = null;
				TestConvert<object, int?>(value!, value);
				TestConvert<ValueType, int?>(value!, value);
				TestConvert<IComparable<int>, int?>(value!, value);
			}
			{
				TypeCode? value = TypeCode.Double;
				TestConvert<object, TypeCode?>(value, value);
				TestConvert<ValueType, TypeCode?>(value, value);
				TestConvert<Enum, TypeCode?>(value, value);
				TestConvert<IConvertible, TypeCode?>(value, value);
				value = null;
				TestConvert<object, TypeCode?>(value!, value);
				TestConvert<ValueType, TypeCode?>(value!, value);
				TestConvert<Enum, TypeCode?>(value!, value);
				TestConvert<IConvertible, TypeCode?>(value!, value);
			}

			// 如果值类型 S 具有来自接口或委托类型 I0 的取消装箱转换，且 I0 可变化转换为 I 或 I 可变化转换为 I0，
			// 则它具有来自 I 的取消装箱转换。
			{
				TestEnumStruct value = new();
				TestConvert<IEnumerable<object>, TestEnumStruct>(value, value);
			}
			{
				TestEnumStruct? value = new();
				TestConvert<IEnumerable<object>, TestEnumStruct?>(value, value);
				value = null;
				TestConvert<IEnumerable<object>, TestEnumStruct?>(value!, value);
			}
		}

		#endregion // 装箱、拆箱转换

		#region 引用类型转换

		private class TestClass { }
		private class TestBaseClass { }
		private class TestSubClass : TestBaseClass { }
		private class TestSubClass2 : TestBaseClass, IEnumerable, IComparable
		{
			public IEnumerator GetEnumerator() { yield return null; }
			public int CompareTo(object? obj) { return 0; }
		}

		/// <summary>
		/// 对隐式引用转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestImplicitReference()
		{
			// 从任何 reference-type 到 object。
			{
				string value = "abc";
				TestConvert<string, object>(value, value);
			}
			{
				TestClass value = new();
				TestConvert<TestClass, object>(value, value);
			}

			// 从任何 class-type S 到任何 class-type T（前提是 S 是从 T 派生的）。
			{
				TestSubClass value = new();
				TestConvert<TestSubClass, TestBaseClass>(value, value);
			}

			// 从任何 class-type S 到任何 interface-type T（前提是 S 实现了 T）。
			{
				List<int> value = new();
				TestConvert<List<int>, IList<int>>(value, value);
			}

			// 从任何 interface-type S 到任何 interface-type T（前提是 S 是从 T 派生的）。
			{
				List<int> value = new();
				TestConvert<IList<int>, ICollection<int>>(value, value);
			}

			// 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			//   o S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			//   o SE 和 TE 都是 reference-type。
			//   o 存在从 SE 到 TE 的隐式引用转换。
			{
				string[] value = Array.Empty<string>();
				TestConvert<string[], object[]>(value, value);
			}
			{
				TestSubClass[] value = Array.Empty<TestSubClass>();
				TestConvert<TestSubClass[], TestBaseClass[]>(value, value);
			}
			{
				List<string>[] value = Array.Empty<List<string>>();
				TestConvert<IList[], List<string>[]>(value, value);
				TestConvert<List<string>[], IEnumerable<object>[]>(value, value);
			}

			// 从任何 array-type 到 System.Array 及其实现的接口。
			{
				int[] value = Array.Empty<int>();
				TestConvert<int[], Array>(value, value);
				TestConvert<int[], IStructuralComparable>(value, value);
			}

			// 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口（前提是存在从 S 到 T 的隐式标识或引用转换）。
			{
				string[] value = Array.Empty<string>();
				TestConvert<string[], IList<object>>(value, value);
			}

			// 从任何 delegate-type 到 System.Delegate 及其实现的接口。
			{
				Func<int> value = () => 0;
				TestConvert<Func<int>, Delegate>(value, value);
			}

			// 从任何 reference-type 到接口或委托类型 T
			// （前提是它具有到接口或委托类型 T0 的隐式标识或引用转换，且 T0 可变化转换为T）。
			// 这里的变化转换在规范的 13.1.3.2 节，就是泛型的协变和逆变。
			// 协变。
			{
				string[] value = Array.Empty<string>();
				TestConvert<string[], IEnumerable<object>>(value, value);
			}
			{
				IEnumerable<string> value = Enumerable.Empty<string>();
				TestConvert<IEnumerable<string>, IEnumerable<object>>(value, value);
			}
			{
				IEqualityComparer<object>[][] value = new IEqualityComparer<object>[0][];
				TestConvert<IEqualityComparer<object>[][], IEnumerable<IEqualityComparer<string>[]>>(value, value);
			}
			{
				IEnumerable<IEqualityComparer<object>[]> value = Enumerable.Empty<IEqualityComparer<object>[]>();
				TestConvert<IEnumerable<IEqualityComparer<object>[]>, IEnumerable<IEqualityComparer<string>[]>>(value, value);
			}
			{
				object[][][] value = Array.Empty<object[][]>();
				TestConvert<object[][][], IEnumerable<IList<IList<object>>>>(value, value);
			}
			{
				IEnumerable<object[][]> value = Enumerable.Empty<object[][]>();
				TestConvert<IEnumerable<object[][]>, IEnumerable<IList<IList<object>>>>(value, value);
			}
			// 逆变。
			{
				EqualityComparer<object> value = EqualityComparer<object>.Default;
				TestConvert<EqualityComparer<object>, IEqualityComparer<string>>(value, value);
			}
			{
				EqualityComparer<IEnumerable<object>[]> value = EqualityComparer<IEnumerable<object>[]>.Default;
				TestConvert<EqualityComparer<IEnumerable<object>[]>, IEqualityComparer<IEnumerable<string>[]>>(value, value);
				TestConvert<EqualityComparer<IEnumerable<object>[]>, IEqualityComparer<string[][]>>(value, value);
			}
		}

		/// <summary>
		/// 对显式引用转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestExplicitReference()
		{
			// 从 object 到任何其他 reference-type。
			{
				string value = "abc";
				TestConvert<object, string>(value, value);
			}
			{
				TestClass value = new();
				TestConvert<object, TestClass>(value, value);
			}

			// 从任何 class-type S 到任何 class-type T（前提是 S 为 T 的基类）。
			{
				TestSubClass value = new();
				TestConvert<TestBaseClass, TestSubClass>(value, value);
			}

			{
				// 从任何 class-type S 到任何 interface-type T（前提是 S 未密封并且 S 未实现 T）。
				TestSubClass2 value = new();
				TestConvert<TestBaseClass, IEnumerable>(value, value);
				// 从任何 interface-type S 到任何 class-type T（前提是 T 未密封或 T 实现 S）。
				TestConvert<IEnumerable, TestBaseClass>(value, value);
				TestConvert<IEnumerable, TestSubClass2>(value, value);
				// 从任何 interface-type S 到任何 interface-type T（前提是 S 不是从 T 派生的）。
				TestConvert<IEnumerable, IComparable>(value, value);
				TestConvert<IComparable, IEnumerable>(value, value);
			}

			// 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			//   o	S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			//   o	SE 和 TE 都是 reference-type。
			//   o	存在从 SE 到 TE 的显式引用转换。
			{
				string[] value = Array.Empty<string>();
				TestConvert<object[], string[]>(value, value);
			}
			{
				TestSubClass[] value = Array.Empty<TestSubClass>();
				TestConvert<TestBaseClass[], TestSubClass[]>(value, value);
			}
			{
				List<string>[] value = Array.Empty<List<string>>();
				TestConvert<IList[], List<string>[]>(value, value);
				TestConvert<IEnumerable<object>[], List<string>[]>(value, value);
			}
			{
				TestSubClass2[] value = Array.Empty<TestSubClass2>();
				TestConvert<IComparable[], IEnumerable[]>(value, value);
			}

			// 从 System.Array 及其实现的接口到任何 array-type。
			{
				int[] value = Array.Empty<int>();
				TestConvert<Array, int[]>(value, value);
				TestConvert<IStructuralComparable, int[]>(value, value);
			}

			{
				// 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口（前提是存在从 S 到 T 的显式标识或引用转换）。
				string[] value = Array.Empty<string>();
				TestConvert<object[], IList<string>>(value, value);
				// 从 System.Collections.Generic.IList<S> 及其基接口到一维数组类型 T[]（前提是存在从 S 到 T 的显式标识或引用转换）。
				TestConvert<IList<string>, object[]>(value, value);
			}

			// 从 System.Delegate 及其实现的接口到任何 delegate-type。
			{
				Func<int> value = () => 0;
				TestConvert<Delegate, Func<int>>(value, value);
			}

			// 委托类型间的协变和逆变。
			{
				Converter<object, object> value = (v) => v;
				TestConvert<Converter<string, object>, Converter<object, object>>(value, value);
			}
			{
				Converter<string, string> value = (v) => v;
				TestConvertException<Converter<string, string>, Converter<object, object>, InvalidCastException>(
					value, () => (Converter<object, object>)(object)value);
				TestConvert<Converter<string, string>, Converter<object, object>>(null!, null!);
			}
		}

		#endregion // 引用类型转换

		#region 自定义类型转换

#pragma warning disable IDE0060 // 删除未使用的参数
		private struct UserStruct
		{
			private readonly int value;
			public UserStruct() : this(Random.Shared.Next(100)) { }
			public UserStruct(int value) { this.value = value; }
			public static explicit operator int(UserStruct value)
			{
				return value.value;
			}
			public static explicit operator UserStruct(int value)
			{
				return new(value);
			}
			public override string ToString()
			{
				return $"[UserStruct {value}]";
			}
		}
		private class UserBaseClass
		{
			protected readonly int value;
			public UserBaseClass() : this(Random.Shared.Next(100)) { }
			public UserBaseClass(int value) { this.value = value; }
			public static explicit operator int(UserBaseClass? value)
			{
				return value?.value ?? -1;
			}
			public static explicit operator UserBaseClass?(int value)
			{
				if (value < 0)
				{
					return null;
				}
				return new UserBaseClass(value);
			}
			public override bool Equals(object? obj)
			{
				if (obj is UserBaseClass other)
				{
					return value == other.value;
				}
				return false;
			}
			public override int GetHashCode()
			{
				return value.GetHashCode();
			}
		}
		private class UserSubClass : UserBaseClass
		{ }
		private struct UserMultiStruct
		{
			private readonly string name;
			private UserMultiStruct(string name) { this.name = name; }
			public static UserMultiStruct Default = new("Default");
			public static UserMultiStruct Byte = new("Byte");
			public static UserMultiStruct Int32 = new("Int32");
			public static UserMultiStruct UInt64 = new("UInt64");
			public static explicit operator byte(UserMultiStruct s) { return 11; }
			public static explicit operator UserMultiStruct(byte s) { return Byte; }
			public static explicit operator int(UserMultiStruct s) { return 13; }
			public static explicit operator UserMultiStruct(int s) { return Int32; }
			public static explicit operator ulong(UserMultiStruct s) { return 15; }
			public static explicit operator UserMultiStruct(ulong s) { return UInt64; }
			public override string ToString()
			{
				return $"[UserMultiStruct {name}]";
			}
		}
		private struct UserNullableStruct
		{
			private readonly int value;
			public UserNullableStruct() : this(Random.Shared.Next(100)) { }
			public UserNullableStruct(int value) { this.value = value; }
			public static explicit operator int?(UserNullableStruct s) { return s.value; }
			public static explicit operator UserNullableStruct(int? value)
			{
				return new UserNullableStruct(value.GetValueOrDefault(-1));
			}
			public static explicit operator string?(UserNullableStruct value)
			{
				return value.value < 0 ? null : value.value.ToString();
			}
			public static explicit operator UserNullableStruct(string value)
			{
				return new(value == null ? -1 : int.Parse(value));
			}
			public override string ToString()
			{
				return $"[UserNullableStruct {value}]";
			}
		}
		private class TestClass1
		{
			private readonly List<int> list = new() { 1 };
			private readonly TestSubClass sub = new();
			public static implicit operator ushort(TestClass1 value)
			{
				return 2;
			}
			public static implicit operator int(TestClass1 value)
			{
				return 1;
			}
			public static implicit operator List<int>(TestClass1 value)
			{
				return value.list;
			}
			public static explicit operator TestSubClass(TestClass1 value)
			{
				return value.sub;
			}
		}
		private class TestClass2
		{
			private static readonly TestClass2 ushortValue = new();
			private static readonly TestClass2 intValue = new();
			private static readonly TestClass2 listValue = new();
			private static readonly TestClass2 subValue = new();

			public static implicit operator TestClass2(ushort value)
			{
				return ushortValue;
			}
			public static implicit operator TestClass2(int value)
			{
				return intValue;
			}
			public static implicit operator TestClass2(List<int> value)
			{
				return listValue;
			}
			public static explicit operator TestClass2(TestBaseClass value)
			{
				return subValue;
			}
		}
#pragma warning restore IDE0060 // 删除未使用的参数

		/// <summary>
		/// 对自定义类型转换进行测试。
		/// </summary>
		[TestMethod]
		public void TestUserDefined()
		{
			// 直接转换。
			{
				UserStruct value = new();
				TestConvert<UserStruct, int>(value, (int)value);
				TestConvert<int, UserStruct>((int)value, value);
			}
			{
				UserBaseClass value = new();
				TestConvert<UserBaseClass, int>(value, (int)value);
				TestConvert<int, UserBaseClass>((int)value, value);
			}
			{
				UserSubClass value = new();
				TestConvert<UserSubClass, int>(value, (int)value);
				// Unable to cast object of type 'UserBaseClass' to type 'UserSubClass'.
				TestConvertException<int, UserSubClass, InvalidCastException>((int)value, () => (UserSubClass?)(int)value);
			}

			// 多个转换。
			{
				UserMultiStruct value = UserMultiStruct.Default;
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserMultiStruct), typeof(char)));
				TestConvert<UserMultiStruct, byte>(value, (byte)value);
				TestConvert<UserMultiStruct, sbyte>(value, (sbyte)value);
				TestConvert<UserMultiStruct, short>(value, (short)value);
				TestConvert<UserMultiStruct, ushort>(value, (ushort)value);
				TestConvert<UserMultiStruct, int>(value, (int)value);
				TestConvert<UserMultiStruct, uint>(value, (uint)value);
				TestConvert<UserMultiStruct, long>(value, (long)value);
				TestConvert<UserMultiStruct, ulong>(value, (ulong)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserMultiStruct), typeof(float)));
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserMultiStruct), typeof(double)));
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserMultiStruct), typeof(decimal)));

				Assert.IsFalse(GenericConvert.CanChangeType(typeof(char), typeof(UserMultiStruct)));
				TestConvert<byte, UserMultiStruct>((byte)1, (UserMultiStruct)(byte)1);
				TestConvert<sbyte, UserMultiStruct>((sbyte)1, (UserMultiStruct)(sbyte)1);
				TestConvert<short, UserMultiStruct>((short)1, (UserMultiStruct)(short)1);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(ushort), typeof(UserMultiStruct)));
				TestConvert<int, UserMultiStruct>((int)1, (UserMultiStruct)(int)1);
				TestConvert<uint, UserMultiStruct>((uint)1, (UserMultiStruct)(uint)1);
				// 这里需要使用 long 变量，否则由于 int/long 的常量表达式可以通过“隐式常量表达式转换”到 ulong 类型
				// 会导致 (UserMultiStruct)(long)1 会选择 ulong 来源而非 int 来源。
				long longValue = 1;
				TestConvert<long, UserMultiStruct>((long)longValue, (UserMultiStruct)(long)longValue);
				TestConvert<ulong, UserMultiStruct>((ulong)1, (UserMultiStruct)(ulong)1);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(float), typeof(UserMultiStruct)));
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(double), typeof(UserMultiStruct)));
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(decimal), typeof(UserMultiStruct)));
			}

			// 额外的转换。
			{
				UserStruct value = new();
				TestConvert<UserStruct, char>(value, (char)value);
				TestConvert<UserStruct, byte>(value, (byte)value);
				TestConvert<UserStruct, sbyte>(value, (sbyte)value);
				TestConvert<UserStruct, short>(value, (short)value);
				TestConvert<UserStruct, ushort>(value, (ushort)value);
				TestConvert<UserStruct, int>(value, (int)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct), typeof(uint)));
				TestConvert<UserStruct, long>(value, (long)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct), typeof(ulong)));
				TestConvert<UserStruct, float>(value, (float)value);
				TestConvert<UserStruct, double>(value, (double)value);
				TestConvert<UserStruct, decimal>(value, (decimal)value);
			}
			{
				int value = 10;
				TestConvert<char, UserStruct>((char)value, (UserStruct)(char)value);
				TestConvert<byte, UserStruct>((byte)value, (UserStruct)(byte)value);
				TestConvert<sbyte, UserStruct>((sbyte)value, (UserStruct)(sbyte)value);
				TestConvert<short, UserStruct>((short)value, (UserStruct)(short)value);
				TestConvert<ushort, UserStruct>((ushort)value, (UserStruct)(ushort)value);
				TestConvert<int, UserStruct>((int)value, (UserStruct)(int)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint), typeof(UserStruct)));
				TestConvert<long, UserStruct>((long)value, (UserStruct)(long)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(ulong), typeof(UserStruct)));
				TestConvert<float, UserStruct>((float)value, (UserStruct)(float)value);
				TestConvert<double, UserStruct>((double)value, (UserStruct)(double)value);
				TestConvert<float, UserStruct>((float)value, (UserStruct)(float)value);
			}

			// 可空类型。
			{
				UserBaseClass? value = new();
				TestConvert<UserBaseClass?, int?>(value, (int?)value);
				TestConvert<int?, UserBaseClass?>((int?)value, value);
				value = null;
				TestConvert<UserBaseClass?, int?>(value, (int?)value);
				TestConvert<int?, UserBaseClass?>((int?)value, value);
			}

			// 非可空类型转为可空类型。
			{
				UserStruct value = new();
				TestConvert<UserStruct, char?>(value, (char?)value);
				TestConvert<UserStruct, byte?>(value, (byte?)value);
				TestConvert<UserStruct, sbyte?>(value, (sbyte?)value);
				TestConvert<UserStruct, short?>(value, (short?)value);
				TestConvert<UserStruct, ushort?>(value, (ushort?)value);
				TestConvert<UserStruct, int?>(value, (int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct), typeof(uint?)));
				TestConvert<UserStruct, long?>(value, (long?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct), typeof(ulong?)));
				TestConvert<UserStruct, float?>(value, (float?)value);
				TestConvert<UserStruct, double?>(value, (double?)value);
				TestConvert<UserStruct, decimal?>(value, (decimal?)value);
			}

			// 可空类型转为非可空类型。
			{
				int value = 10;
				TestConvert<char?, UserStruct>((char?)value, (UserStruct)(char?)value);
				TestConvert<byte?, UserStruct>((byte?)value, (UserStruct)(byte?)value);
				TestConvert<sbyte?, UserStruct>((sbyte?)value, (UserStruct)(sbyte?)value);
				TestConvert<short?, UserStruct>((short?)value, (UserStruct)(short?)value);
				TestConvert<ushort?, UserStruct>((ushort?)value, (UserStruct)(ushort?)value);
				TestConvert<int?, UserStruct>((int?)value, (UserStruct)(int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint?), typeof(UserStruct)));
				TestConvert<long?, UserStruct>((long?)value, (UserStruct)((long?)value)!);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(ulong?), typeof(UserStruct)));
				TestConvert<float?, UserStruct>((float?)value, (UserStruct)((float?)value)!);
				TestConvert<double?, UserStruct>((double?)value, (UserStruct)((double?)value)!);
				TestConvert<decimal?, UserStruct>((decimal?)value, (UserStruct)((decimal?)value)!);
			}

			{
				// 可空类型转为可空类型。
				UserStruct? value = new();
				TestConvert<UserStruct?, char?>(value, (char?)value);
				TestConvert<UserStruct?, byte?>(value, (byte?)value);
				TestConvert<UserStruct?, sbyte?>(value, (sbyte?)value);
				TestConvert<UserStruct?, short?>(value, (short?)value);
				TestConvert<UserStruct?, ushort?>(value, (ushort?)value);
				TestConvert<UserStruct?, int?>(value, (int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(uint?)));
				TestConvert<UserStruct?, long?>(value, (long?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(ulong?)));
				TestConvert<UserStruct?, float?>(value, (float?)value);
				TestConvert<UserStruct?, double?>(value, (double?)value);
				TestConvert<UserStruct?, decimal?>(value, (decimal?)value);

				value = null;
				TestConvert<UserStruct?, char?>(value, (char?)value);
				TestConvert<UserStruct?, byte?>(value, (byte?)value);
				TestConvert<UserStruct?, sbyte?>(value, (sbyte?)value);
				TestConvert<UserStruct?, short?>(value, (short?)value);
				TestConvert<UserStruct?, ushort?>(value, (ushort?)value);
				TestConvert<UserStruct?, int?>(value, (int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(uint?)));
				TestConvert<UserStruct?, long?>(value, (long?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(ulong?)));
				TestConvert<UserStruct?, float?>(value, (float?)value);
				TestConvert<UserStruct?, double?>(value, (double?)value);
				TestConvert<UserStruct?, decimal?>(value, (decimal?)value);
			}
			{
				int? value = 13;
				TestConvert<char?, UserStruct?>((char?)value, (UserStruct?)(char?)value);
				TestConvert<byte?, UserStruct?>((byte?)value, (UserStruct?)(byte?)value);
				TestConvert<sbyte?, UserStruct?>((sbyte?)value, (UserStruct?)(sbyte?)value);
				TestConvert<short?, UserStruct?>((short?)value, (UserStruct?)(short?)value);
				TestConvert<ushort?, UserStruct?>((ushort?)value, (UserStruct?)(ushort?)value);
				TestConvert<int?, UserStruct?>((int?)value, (UserStruct?)(int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint?), typeof(UserStruct?)));
				TestConvert<long?, UserStruct?>((long?)value, (UserStruct?)((long?)value)!);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(ulong?), typeof(UserStruct?)));
				TestConvert<float?, UserStruct?>((float?)value, (UserStruct?)((float?)value)!);
				TestConvert<double?, UserStruct?>((double?)value, (UserStruct?)((double?)value)!);
				TestConvert<decimal?, UserStruct?>((decimal?)value, (UserStruct?)((decimal?)value)!);

				value = null;
				TestConvert<char?, UserStruct?>((char?)value, (UserStruct?)(char?)value);
				TestConvert<byte?, UserStruct?>((byte?)value, (UserStruct?)(byte?)value);
				TestConvert<sbyte?, UserStruct?>((sbyte?)value, (UserStruct?)(sbyte?)value);
				TestConvert<short?, UserStruct?>((short?)value, (UserStruct?)(short?)value);
				TestConvert<ushort?, UserStruct?>((ushort?)value, (UserStruct?)(ushort?)value);
				TestConvert<int?, UserStruct?>((int?)value, (UserStruct?)(int?)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint?), typeof(UserStruct?)));
				TestConvert<long?, UserStruct?>((long?)value, (UserStruct?)((long?)value)!);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(ulong?), typeof(UserStruct?)));
				TestConvert<float?, UserStruct?>((float?)value, (UserStruct?)((float?)value)!);
				TestConvert<double?, UserStruct?>((double?)value, (UserStruct?)((double?)value)!);
				TestConvert<decimal?, UserStruct?>((decimal?)value, (UserStruct?)((decimal?)value)!);
			}

			// 可空类型转为非可空类型。
			{
				UserStruct? value = new();
				TestConvert<UserStruct?, char>(value, (char)value);
				TestConvert<UserStruct?, byte>(value, (byte)value);
				TestConvert<UserStruct?, sbyte>(value, (sbyte)value);
				TestConvert<UserStruct?, short>(value, (short)value);
				TestConvert<UserStruct?, ushort>(value, (ushort)value);
				TestConvert<UserStruct?, int>(value, (int)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(uint)));
				TestConvert<UserStruct?, long>(value, (long)value);
				Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserStruct?), typeof(ulong)));
				TestConvert<UserStruct?, float>(value, (float)value);
				TestConvert<UserStruct?, double>(value, (double)value);
				TestConvert<UserStruct?, decimal>(value, (decimal)value);

				value = null;
				TestConvertException<UserStruct?, char, InvalidOperationException>(value, () => (char)value!);
				TestConvertException<UserStruct?, byte, InvalidOperationException>(value, () => (byte)value!);
			}

			// 可空类型的用户自定义类型转换。
			{
				UserNullableStruct value = new();
				TestConvert<UserNullableStruct, char>(value, (char)value!);
				TestConvert<UserNullableStruct, char?>(value, (char?)value!);
				TestConvert<UserNullableStruct, int>(value, (int)value!);
				TestConvert<UserNullableStruct, int?>(value, (int?)value!);
			}
			{
				UserNullableStruct? value = new();
				TestConvert<UserNullableStruct?, char>(value, (char)value!);
				TestConvert<UserNullableStruct?, char?>(value, (char?)value!);
				TestConvert<UserNullableStruct?, int>(value, (int)value!);
				TestConvert<UserNullableStruct?, int?>(value, (int?)value!);

				value = null;
				TestConvert<UserNullableStruct?, int?>(value, (int?)value);
				TestConvert<UserNullableStruct?, char?>(value, (char?)value!);
			}
			{
				int value = 103;
				TestConvert<int, UserNullableStruct>(value, (UserNullableStruct)value);
				TestConvert<int, UserNullableStruct?>(value, (UserNullableStruct?)value);
			}
			{
				int? value = 103;
				TestConvert<int?, UserNullableStruct>(value, (UserNullableStruct)value);
				TestConvert<int?, UserNullableStruct?>(value, (UserNullableStruct?)value);

				value = null;
				TestConvert<int?, UserNullableStruct?>(value, (UserNullableStruct?)value);
			}
			{
				char value = '\x0A';
				TestConvert<char, UserNullableStruct>(value, (UserNullableStruct)value);
				TestConvert<char, UserNullableStruct?>(value, (UserNullableStruct?)value);
			}
			{
				char? value = '\x0A';
				TestConvert<char?, UserNullableStruct>(value, (UserNullableStruct)value);
				TestConvert<char?, UserNullableStruct?>(value, (UserNullableStruct?)value);

				value = null;
				TestConvert<int?, UserNullableStruct?>(value, (UserNullableStruct?)value);
			}
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserNullableStruct), typeof(uint)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint), typeof(UserNullableStruct)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserNullableStruct), typeof(uint?)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint?), typeof(UserNullableStruct)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserNullableStruct?), typeof(uint)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint), typeof(UserNullableStruct?)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(UserNullableStruct?), typeof(uint?)));
			Assert.IsFalse(GenericConvert.CanChangeType(typeof(uint?), typeof(UserNullableStruct?)));

			// 值类型与引用类型的用户自定义类型转换。
			{
				UserNullableStruct value = new();
				TestConvert<UserNullableStruct, string>(value, (string)value!);
			}
			{
				UserNullableStruct? value = new();
				TestConvert<UserNullableStruct?, string>(value, (string)value!);

				value = null;
				TestConvert<UserNullableStruct?, string>(value, (string)value!);
			}
			{
				string value = "1943";
				TestConvert<string, UserNullableStruct>(value, (UserNullableStruct)value);
			}
			{
				string? value = "1943";
				TestConvert<string?, UserNullableStruct>(value, (UserNullableStruct)value);
				value = null;
				TestConvert<string?, UserNullableStruct?>(value, (UserNullableStruct?)value!);
			}
			// 其它测试用例
			{
				TestClass1 value = new();
				TestConvert<TestClass1, char>(value, (char)value);
				TestConvert<TestClass1, byte>(value, (byte)value);
				TestConvert<TestClass1, sbyte>(value, (sbyte)value);
				TestConvert<TestClass1, short>(value, (short)value);
				TestConvert<TestClass1, ushort>(value, (ushort)value);
				TestConvert<TestClass1, int>(value, (int)value);
				TestConvert<TestClass1, uint>(value, (uint)value);
				TestConvert<TestClass1, long>(value, (long)value);
				TestConvert<TestClass1, ulong>(value, (ulong)value);
				TestConvert<TestClass1, float>(value, (float)value);
				TestConvert<TestClass1, double>(value, (double)value);
				TestConvert<TestClass1, decimal>(value, (decimal)value);

				TestConvert<TestClass1, char?>(value, (char?)value);
				TestConvert<TestClass1, byte?>(value, (byte?)value);
				TestConvert<TestClass1, sbyte?>(value, (sbyte?)value);
				TestConvert<TestClass1, short?>(value, (short?)value);
				TestConvert<TestClass1, ushort?>(value, (ushort?)value);
				TestConvert<TestClass1, int?>(value, (int?)value);
				TestConvert<TestClass1, uint?>(value, (uint?)value);
				TestConvert<TestClass1, long?>(value, (long?)value);
				TestConvert<TestClass1, ulong?>(value, (ulong?)value);
				TestConvert<TestClass1, float?>(value, (float?)value);
				TestConvert<TestClass1, double?>(value, (double?)value);
				TestConvert<TestClass1, decimal?>(value, (decimal?)value);

				TestConvert<TestClass1, List<int>>(value, value);
				TestConvert<TestClass1, TestSubClass>(value, (TestSubClass)value);
				TestConvert<TestClass1, TestBaseClass>(value, (TestBaseClass)value);
			}
			{
				char value = (char)1;
				TestConvert<char, TestClass2>(value, value);
			}
			{
				sbyte value = 1;
				TestConvert<sbyte, TestClass2>(value, value);
			}
			{
				byte value = 1;
				TestConvert<byte, TestClass2>(value, value);
			}
			{
				short value = 1;
				TestConvert<short, TestClass2>(value, value);
			}
			{
				ushort value = 1;
				TestConvert<ushort, TestClass2>(value, value);
			}
			{
				int value = 1;
				TestConvert<int, TestClass2>(value, value);
			}
			{
				uint value = 1;
				TestConvert<uint, TestClass2>(value, (TestClass2)value);
			}
			{
				long value = 1;
				TestConvert<long, TestClass2>(value, (TestClass2)value);
			}
			{
				ulong value = 1;
				TestConvert<ulong, TestClass2>(value, (TestClass2)value);
			}
			{
				float value = 1;
				TestConvert<float, TestClass2>(value, (TestClass2)value);
			}
			{
				double value = 1;
				TestConvert<double, TestClass2>(value, (TestClass2)value);
			}
			{
				decimal value = 1;
				TestConvert<decimal, TestClass2>(value, (TestClass2)value);
			}
			{
				char? value = (char?)1;
				TestConvert<char?, TestClass2>(value, (TestClass2)value);
			}
			{
				sbyte? value = 1;
				TestConvert<sbyte?, TestClass2>(value, (TestClass2)value);
			}
			{
				byte? value = 1;
				TestConvert<byte?, TestClass2>(value, (TestClass2)value);
			}
			{
				short? value = 1;
				TestConvert<short?, TestClass2>(value, (TestClass2)value);
			}
			{
				ushort? value = 1;
				TestConvert<ushort?, TestClass2>(value, (TestClass2)value);
			}
			{
				int? value = 1;
				TestConvert<int?, TestClass2>(value, (TestClass2)value);
			}
			{
				uint? value = 1;
				TestConvert<uint?, TestClass2>(value, (TestClass2)value!);
			}
			{
				long? value = 1;
				TestConvert<long?, TestClass2>(value, (TestClass2)value!);
			}
			{
				ulong? value = 1;
				TestConvert<ulong?, TestClass2>(value, (TestClass2)value!);
			}
			{
				float? value = 1;
				TestConvert<float?, TestClass2>(value, (TestClass2)value!);
			}
			{
				double? value = 1;
				TestConvert<double?, TestClass2>(value, (TestClass2)value!);
			}
			{
				decimal? value = 1;
				TestConvert<decimal?, TestClass2>(value, (TestClass2)value!);
			}
			{
				List<int> value = new();
				TestConvert<List<int>, TestClass2>(value, (TestClass2)value);
			}
			{
				TestSubClass value = new();
				TestConvert<TestSubClass, TestClass2>(value, (TestClass2)value);
			}
			{
				TestBaseClass value = new();
				TestConvert<TestBaseClass, TestClass2>(value, (TestClass2)value);
			}
		}

		#endregion // 自定义类型转换

		/// <summary>
		/// 对类型转换提供者进行测试。
		/// </summary>
		[TestMethod]
		public void TestProvider()
		{
			TestConvert<string, int>("1234", 1234);
			TestConvert<string, TestEnum1>("1", TestEnum1.B);
			TestConvert<int, string>(1234, "1234");
			TestConvert<TestEnum1, string>(TestEnum1.B, "B");
		}

		/// <summary>
		/// 测试类型转换方法。
		/// </summary>
		private static void TestConvert<TInput, TOutput>(TInput value, TOutput expected)
		{
			Assert.IsTrue(GenericConvert.CanChangeType(typeof(TInput), typeof(TOutput)));
			Assert.AreEqual(expected, GenericConvert.GetConverter<TInput, TOutput>()!(value),
				"{0} => {1}", typeof(TInput), typeof(TOutput));
			Assert.AreEqual(expected, GenericConvert.GetConverter(typeof(TInput), typeof(TOutput))!(value),
				"{0} => {1}", typeof(TInput), typeof(TOutput));
		}

		/// <summary>
		/// 测试可能抛出异常的类型转换方法。
		/// </summary>
		private static void TestConvertException<TInput, TOutput, TException>(object? value, Func<object?> getExpected)
			where TException : Exception
		{
			try
			{
				TOutput expected = (TOutput)getExpected()!;
				Assert.AreEqual(expected, GenericConvert.GetConverter<TInput, TOutput>()!((TInput)value!),
					"{0} => {1}", typeof(TInput), typeof(TOutput));
				Assert.AreEqual(expected, GenericConvert.GetConverter(typeof(TInput), typeof(TOutput))!((TInput)value!),
					"{0} => {1}", typeof(TInput), typeof(TOutput));
			}
			catch (TException expectedException)
			{
				TException ex = Assert.ThrowsException<TException>(() =>
				{
					GenericConvert.GetConverter<TInput, TOutput>()!((TInput)value!);
				});
				Assert.AreEqual(expectedException.GetType(), ex.GetType());
				ex = Assert.ThrowsException<TException>(() =>
				{
					GenericConvert.GetConverter(typeof(TInput), typeof(TOutput))!(value);
				});
				Assert.AreEqual(expectedException.GetType(), ex.GetType());
			}
		}
	}
}

