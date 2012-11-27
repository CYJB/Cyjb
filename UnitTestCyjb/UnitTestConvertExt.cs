using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.ConvertExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestConvertExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToSByte"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToSByte()
		{
			Assert.AreEqual(0, ConvertExt.ToSByte("0", 13));
			Assert.AreEqual(1, ConvertExt.ToSByte("1", 3));
			Assert.AreEqual(120, ConvertExt.ToSByte("60", 20));
			Assert.AreEqual(127, ConvertExt.ToSByte("9a", 13));
			Assert.AreEqual(-1, ConvertExt.ToSByte("9c", 27));
			Assert.AreEqual(-2, ConvertExt.ToSByte("9b", 27));
			Assert.AreEqual(-90, ConvertExt.ToSByte("4m", 36));
			Assert.AreEqual(-128, ConvertExt.ToSByte("4c", 29));
			bool hasException = false;
			try
			{
				ConvertExt.ToSByte("38o7", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToInt16"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt16()
		{
			Assert.AreEqual(0, ConvertExt.ToInt16("0", 13));
			Assert.AreEqual(1, ConvertExt.ToInt16("1", 3));
			Assert.AreEqual(14720, ConvertExt.ToInt16("1Gg0", 20));
			Assert.AreEqual(32767, ConvertExt.ToInt16("11bb7", 13));
			Assert.AreEqual(-1, ConvertExt.ToInt16("38o6", 27));
			Assert.AreEqual(-2, ConvertExt.ToInt16("38o5", 27));
			Assert.AreEqual(-21458, ConvertExt.ToInt16("y0e", 36));
			Assert.AreEqual(-32768, ConvertExt.ToInt16("19rr", 29));
			bool hasException = false;
			try
			{
				ConvertExt.ToInt16("38o7", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToInt32"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt32()
		{
			Assert.AreEqual(0, ConvertExt.ToInt32("0", 13));
			Assert.AreEqual(1, ConvertExt.ToInt32("1", 3));
			Assert.AreEqual(14720, ConvertExt.ToInt32("1Gg0", 20));
			Assert.AreEqual(2147483647, ConvertExt.ToInt32("282ba4aaa", 13));
			Assert.AreEqual(-1, ConvertExt.ToInt32("b28jpdl", 27));
			Assert.AreEqual(-2, ConvertExt.ToInt32("b28jpdk", 27));
			Assert.AreEqual(-1235678902, ConvertExt.ToInt32("1elf616", 36));
			Assert.AreEqual(-2147483648, ConvertExt.ToInt32("3hk7988", 29));
			bool hasException = false;
			try
			{
				ConvertExt.ToInt32("b28jpdm", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToInt64"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToInt64()
		{
			Assert.AreEqual(0L, ConvertExt.ToInt64("0", 13));
			Assert.AreEqual(1L, ConvertExt.ToInt64("1", 3));
			Assert.AreEqual(14720L, ConvertExt.ToInt64("1Gg0", 20));
			Assert.AreEqual(9223372036854775807L, ConvertExt.ToInt64("10B269549075433C37", 13));
			Assert.AreEqual(-1L, ConvertExt.ToInt64("4Eo8hfam6fllmo", 27));
			Assert.AreEqual(-2L, ConvertExt.ToInt64("4Eo8hfam6fllmn", 27));
			Assert.AreEqual(-8071017880399937603L, ConvertExt.ToInt64("26tvjyybszf7h", 36));
			Assert.AreEqual(-9223372036854775808L, ConvertExt.ToInt64("q1se8f0m04isc", 29));
			bool hasException = false;
			try
			{
				ConvertExt.ToInt64("4Eo8hfam6fllmp", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToByte"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToByte()
		{
			Assert.AreEqual(0, ConvertExt.ToByte("0", 13));
			Assert.AreEqual(1, ConvertExt.ToByte("1", 3));
			Assert.AreEqual(120, ConvertExt.ToByte("60", 20));
			Assert.AreEqual(127, ConvertExt.ToByte("9a", 13));
			Assert.AreEqual(128, ConvertExt.ToByte("4c", 29));
			Assert.AreEqual(166, ConvertExt.ToByte("4m", 36));
			Assert.AreEqual(254, ConvertExt.ToByte("9b", 27));
			Assert.AreEqual(255, ConvertExt.ToByte("9c", 27));
			bool hasException = false;
			try
			{
				ConvertExt.ToByte("38o7", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToUInt16"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt16()
		{
			Assert.AreEqual(0, ConvertExt.ToUInt16("0", 13));
			Assert.AreEqual(1, ConvertExt.ToUInt16("1", 3));
			Assert.AreEqual(14720, ConvertExt.ToUInt16("1Gg0", 20));
			Assert.AreEqual(32767, ConvertExt.ToUInt16("11bb7", 13));
			Assert.AreEqual(32768, ConvertExt.ToUInt16("19rr", 29));
			Assert.AreEqual(44078, ConvertExt.ToUInt16("y0e", 36));
			Assert.AreEqual(65534, ConvertExt.ToUInt16("38o5", 27));
			Assert.AreEqual(65535, ConvertExt.ToUInt16("38o6", 27));
			bool hasException = false;
			try
			{
				ConvertExt.ToUInt16("38o7", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToUInt32"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt32()
		{
			Assert.AreEqual(0U, ConvertExt.ToUInt32("0", 13));
			Assert.AreEqual(1U, ConvertExt.ToUInt32("1", 3));
			Assert.AreEqual(14720U, ConvertExt.ToUInt32("1Gg0", 20));
			Assert.AreEqual(2147483647U, ConvertExt.ToUInt32("282ba4aaa", 13));
			Assert.AreEqual(2147483648U, ConvertExt.ToUInt32("3hk7988", 29));
			Assert.AreEqual(3059288394U, ConvertExt.ToUInt32("1elf616", 36));
			Assert.AreEqual(4294967294U, ConvertExt.ToUInt32("b28jpdk", 27));
			Assert.AreEqual(4294967295U, ConvertExt.ToUInt32("b28jpdl", 27));
			bool hasException = false;
			try
			{
				ConvertExt.ToUInt32("b28jpdm", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToUInt64"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToUInt64()
		{
			Assert.AreEqual(0UL, ConvertExt.ToUInt64("0", 13));
			Assert.AreEqual(1UL, ConvertExt.ToUInt64("1", 3));
			Assert.AreEqual(14720UL, ConvertExt.ToUInt64("1Gg0", 20));
			Assert.AreEqual(9223372036854775807UL, ConvertExt.ToUInt64("10B269549075433C37", 13));
			Assert.AreEqual(9223372036854775808UL, ConvertExt.ToUInt64("q1se8f0m04isc", 29));
			Assert.AreEqual(10375726193309614013UL, ConvertExt.ToUInt64("26tvjyybszf7h", 36));
			Assert.AreEqual(18446744073709551614UL, ConvertExt.ToUInt64("4Eo8hfam6fllmn", 27));
			Assert.AreEqual(18446744073709551615UL, ConvertExt.ToUInt64("4Eo8hfam6fllmo", 27));
			bool hasException = false;
			try
			{
				ConvertExt.ToUInt64("4Eo8hfam6fllmp", 27);
			}
			catch (OverflowException)
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "没有正确引发 OverflowException。");
		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ToString"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestToString()
		{
			// 对转换 SByte 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString((sbyte)0, 13));
			Assert.AreEqual("1", ConvertExt.ToString((sbyte)1, 3));
			Assert.AreEqual("60", ConvertExt.ToString((sbyte)120, 20));
			Assert.AreEqual("9A", ConvertExt.ToString((sbyte)127, 13));
			Assert.AreEqual("9C", ConvertExt.ToString((sbyte)-1, 27));
			Assert.AreEqual("-2", ConvertExt.ToString((sbyte)-2, 10));
			Assert.AreEqual("4M", ConvertExt.ToString((sbyte)-90, 36));
			Assert.AreEqual("4C", ConvertExt.ToString((sbyte)-128, 29));

			// 对转换 Int16 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString((short)0, 13));
			Assert.AreEqual("1", ConvertExt.ToString((short)1, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString((short)14720, 20));
			Assert.AreEqual("11BB7", ConvertExt.ToString((short)32767, 13));
			Assert.AreEqual("38O6", ConvertExt.ToString((short)-1, 27));
			Assert.AreEqual("-2", ConvertExt.ToString((short)-2, 10));
			Assert.AreEqual("Y0E", ConvertExt.ToString((short)-21458, 36));
			Assert.AreEqual("19RR", ConvertExt.ToString((short)-32768, 29));

			// 对转换 Int32 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString(0, 13));
			Assert.AreEqual("1", ConvertExt.ToString(1, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString(14720, 20));
			Assert.AreEqual("282BA4AAA", ConvertExt.ToString(2147483647, 13));
			Assert.AreEqual("B28JPDL", ConvertExt.ToString(-1, 27));
			Assert.AreEqual("-2", ConvertExt.ToString(-2, 10));
			Assert.AreEqual("1ELF616", ConvertExt.ToString(-1235678902, 36));
			Assert.AreEqual("3HK7988", ConvertExt.ToString(-2147483648, 29));

			// 对转换 Int64 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString(0L, 13));
			Assert.AreEqual("1", ConvertExt.ToString(1L, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString(14720L, 20));
			Assert.AreEqual("10B269549075433C37", ConvertExt.ToString(9223372036854775807L, 13));
			Assert.AreEqual("4EO8HFAM6FLLMO", ConvertExt.ToString(-1L, 27));
			Assert.AreEqual("-2", ConvertExt.ToString(-2L, 10));
			Assert.AreEqual("26TVJYYBSZF7H", ConvertExt.ToString(-8071017880399937603L, 36));
			Assert.AreEqual("Q1SE8F0M04ISC", ConvertExt.ToString(-9223372036854775808L, 29));

			// 对转换 Byte 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString((byte)0, 13));
			Assert.AreEqual("1", ConvertExt.ToString((byte)1, 3));
			Assert.AreEqual("60", ConvertExt.ToString((byte)120, 20));
			Assert.AreEqual("9A", ConvertExt.ToString((byte)127, 13));
			Assert.AreEqual("4C", ConvertExt.ToString((byte)128, 29));
			Assert.AreEqual("4M", ConvertExt.ToString((byte)166, 36));
			Assert.AreEqual("254", ConvertExt.ToString((byte)254, 10));
			Assert.AreEqual("9C", ConvertExt.ToString((byte)255, 27));

			// 对转换 UInt16 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString((ushort)0, 13));
			Assert.AreEqual("1", ConvertExt.ToString((ushort)1, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString((ushort)14720, 20));
			Assert.AreEqual("11BB7", ConvertExt.ToString((ushort)32767, 13));
			Assert.AreEqual("19RR", ConvertExt.ToString((ushort)32768, 29));
			Assert.AreEqual("Y0E", ConvertExt.ToString((ushort)44078, 36));
			Assert.AreEqual("65534", ConvertExt.ToString((ushort)65534, 10));
			Assert.AreEqual("38O6", ConvertExt.ToString((ushort)65535, 27));

			// 对转换 UInt32 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString(0U, 13));
			Assert.AreEqual("1", ConvertExt.ToString(1U, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString(14720U, 20));
			Assert.AreEqual("282BA4AAA", ConvertExt.ToString(2147483647U, 13));
			Assert.AreEqual("3HK7988", ConvertExt.ToString(2147483648U, 29));
			Assert.AreEqual("1ELF616", ConvertExt.ToString(3059288394U, 36));
			Assert.AreEqual("4294967294", ConvertExt.ToString(4294967294U, 10));
			Assert.AreEqual("B28JPDL", ConvertExt.ToString(4294967295U, 27));

			// 对转换 UInt64 进行测试。
			Assert.AreEqual("0", ConvertExt.ToString(0U, 13));
			Assert.AreEqual("1", ConvertExt.ToString(1U, 3));
			Assert.AreEqual("1GG0", ConvertExt.ToString(14720U, 20));
			Assert.AreEqual("10B269549075433C37", ConvertExt.ToString(9223372036854775807UL, 13));
			Assert.AreEqual("Q1SE8F0M04ISC", ConvertExt.ToString(9223372036854775808UL, 29));
			Assert.AreEqual("26TVJYYBSZF7H", ConvertExt.ToString(10375726193309614013UL, 36));
			Assert.AreEqual("18446744073709551614", ConvertExt.ToString(18446744073709551614UL, 10));
			Assert.AreEqual("4EO8HFAM6FLLMO", ConvertExt.ToString(18446744073709551615UL, 27));
		}
	}
}
