using System;
using Cyjb.Conversions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Conversions
{
	/// <summary>
	/// <see cref="BaseConvert"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestBaseConvert
	{
		/// <summary>
		/// 对 <see cref="BaseConvert.ToSByte"/> 和 <see cref="BaseConvert.ToString(sbyte,int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("0", 13, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("60", 20, 120)]
		[DataRow("11121", 3, 124)]
		[DataRow("127", 10, 127)]
		[DataRow("9a", 13, 127)]
		[DataRow("52", 25, 127)]
		[DataRow("-128", 10, -128)]
		[DataRow("4c", 29, -128)]
		[DataRow("4m", 36, -90)]
		[DataRow("9b", 27, -2)]
		[DataRow("9c", 27, -1)]
		public void TestSByte(string? value, int fromBase, int expected)
		{
			Assert.AreEqual((sbyte)expected, BaseConvert.ToSByte(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString((sbyte)expected, fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToByte"/> 和 <see cref="BaseConvert.ToString(byte,int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("60", 20, 120)]
		[DataRow("11121", 3, 124)]
		[DataRow("9a", 13, 127)]
		[DataRow("127", 10, 127)]
		[DataRow("4c", 29, 128)]
		[DataRow("4m", 36, 166)]
		[DataRow("9b", 27, 254)]
		[DataRow("9c", 27, 255)]
		[DataRow("255", 10, 255)]
		[DataRow("f0", 17, 255)]
		public void TestByte(string? value, int fromBase, int expected)
		{
			Assert.AreEqual((byte)expected, BaseConvert.ToByte(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString((byte)expected, fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToInt16"/> 和 <see cref="BaseConvert.ToString(short,int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("11bb7", 13, 32767)]
		[DataRow("32767", 10, 32767)]
		[DataRow("U2V", 33, 32767)]
		[DataRow("-32768", 10, -32768)]
		[DataRow("19rr", 29, -32768)]
		[DataRow("y0e", 36, -21458)]
		[DataRow("38o5", 27, -2)]
		[DataRow("38o6", 27, -1)]
		public void TestInt16(string? value, int fromBase, int expected)
		{
			Assert.AreEqual((short)expected, BaseConvert.ToInt16(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString((short)expected, fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToUInt16"/> 和 <see cref="BaseConvert.ToString(ushort,int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("127", 10, 127)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("11bb7", 13, 32767)]
		[DataRow("19rr", 29, 32768)]
		[DataRow("y0e", 36, 44078)]
		[DataRow("38o5", 27, 65534)]
		[DataRow("38o6", 27, 65535)]
		[DataRow("65535", 10, 65535)]
		[DataRow("1r5U", 33, 65535)]
		public void TestUInt16(string? value, int fromBase, int expected)
		{
			Assert.AreEqual((ushort)expected, BaseConvert.ToUInt16(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString((ushort)expected, fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToInt32"/> 和 <see cref="BaseConvert.ToString(int,int)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("U2O", 33, 32760)]
		[DataRow("282ba4aaa", 13, 2147483647)]
		[DataRow("2147483647", 10, 2147483647)]
		[DataRow("8jmdnkm", 25, 2147483647)]
		[DataRow("3hk7988", 29, -2147483648)]
		[DataRow("-2147483648", 10, -2147483648)]
		[DataRow("1elf616", 36, -1235678902)]
		[DataRow("b28jpdk", 27, -2)]
		[DataRow("b28jpdl", 27, -1)]
		public void TestInt32(string? value, int fromBase, int expected)
		{
			Assert.AreEqual(expected, BaseConvert.ToInt32(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString(expected, fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToUInt32"/> 和 <see cref="BaseConvert.ToString(sbyte,uint)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("127", 10, 127)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("1r18", 33, 65381)]
		[DataRow("282ba4aaa", 13, 2147483647)]
		[DataRow("3hk7988", 29, 2147483648)]
		[DataRow("1elf616", 36, 3059288394)]
		[DataRow("b28jpdk", 27, 4294967294)]
		[DataRow("b28jpdl", 27, 4294967295)]
		[DataRow("4294967295", 10, 4294967295)]
		[DataRow("hek2mgk", 25, 4294967295)]
		public void TestUInt32(string? value, int fromBase, object expected)
		{
			Assert.AreEqual(Convert.ToUInt32(expected), BaseConvert.ToUInt32(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString(Convert.ToUInt32(expected), fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToInt64"/> 和 <see cref="BaseConvert.ToString(sbyte,long)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("U2O", 33, 32760)]
		[DataRow("3igoecjbmca687", 26, 9223372036854775807)]
		[DataRow("10B269549075433C37", 13, 9223372036854775807)]
		[DataRow("9223372036854775807", 10, 9223372036854775807)]
		[DataRow("q1se8f0m04isc", 29, -9223372036854775808)]
		[DataRow("-9223372036854775808", 10, -9223372036854775808)]
		[DataRow("26tvjyybszf7h", 36, -8071017880399937603)]
		[DataRow("4Eo8hfam6fllmn", 27, -2)]
		[DataRow("4Eo8hfam6fllmo", 27, -1)]
		public void TestInt64(string? value, int fromBase, object expected)
		{
			Assert.AreEqual(Convert.ToInt64(expected), BaseConvert.ToInt64(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString(Convert.ToInt64(expected), fromBase));
			}
		}

		/// <summary>
		/// 对 <see cref="BaseConvert.ToUInt64"/> 和 <see cref="BaseConvert.ToString(sbyte,ulong)"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(null, 10, 0)]
		[DataRow("0", 3, 0)]
		[DataRow("0", 10, 0)]
		[DataRow("1", 3, 1)]
		[DataRow("5d", 18, 103)]
		[DataRow("2V", 36, 103)]
		[DataRow("11121", 3, 124)]
		[DataRow("127", 10, 127)]
		[DataRow("f0", 17, 255)]
		[DataRow("1Gg0", 20, 14720)]
		[DataRow("1r18", 33, 65381)]
		[DataRow("10B269549075433C37", 13, 9223372036854775807)]
		[DataRow("q1se8f0m04isc", 29, 9223372036854775808)]
		[DataRow("26tvjyybszf7h", 36, 10375726193309614013)]
		[DataRow("4Eo8hfam6fllmn", 27, 18446744073709551614)]
		[DataRow("18446744073709551614", 10, 18446744073709551614)]
		[DataRow("7b7n2pcniokcge", 26, 18446744073709551614)]
		[DataRow("4Eo8hfam6fllmo", 27, 18446744073709551615)]
		public void TestUInt64(string? value, int fromBase, object expected)
		{
			Assert.AreEqual(Convert.ToUInt64(expected), BaseConvert.ToUInt64(value, fromBase));
			if (value != null)
			{
				Assert.AreEqual(value.ToUpper(), BaseConvert.ToString(Convert.ToUInt64(expected), fromBase));
			}
		}

		/// <summary>
		/// 对数值溢出进行测试。
		/// </summary>
		[TestMethod]
		public void TestOverflow()
		{
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToSByte("38o7", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToByte("38o7", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToInt16("38o7", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToUInt16("38o7", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToInt32("b28jpdm", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToUInt32("b28jpdm", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToInt64("4Eo8hfam6fllmp", 27));
			Assert.ThrowsException<OverflowException>(() => BaseConvert.ToUInt64("4Eo8hfam6fllmp", 27));
		}
	}
}

