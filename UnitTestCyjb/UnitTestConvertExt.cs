using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		#region 测试进制转换

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
			AssertExt.ThrowsException(() => ConvertExt.ToSByte("38o7", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToInt16("38o7", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToInt32("b28jpdm", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToInt64("4Eo8hfam6fllmp", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToByte("38o7", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToUInt16("38o7", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToUInt32("b28jpdm", 27), typeof(OverflowException));
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
			AssertExt.ThrowsException(() => ConvertExt.ToUInt64("4Eo8hfam6fllmp", 27), typeof(OverflowException));
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

		#endregion // 测试进制转换

		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ImplicitChangeType"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestImplicitChangeType()
		{
			MethodInfo method = typeof(ConvertExt).GetMethod("ImplicitChangeType", BindingFlags.NonPublic | BindingFlags.Static);
			TestImplicitChangeTypeHelper((value, type) => method.Invoke(null, new object[] { value, type, null }));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsImplicitFrom"/> 方法进行测试的辅助方法。
		/// </summary>
		public void TestImplicitChangeTypeHelper(Func<object, Type, object> func)
		{
			object value = new object();

			#region 6.1.1 标识转换

			Assert.AreEqual(value, func(value, typeof(object)));
			value = true; Assert.AreEqual(value, func(value, typeof(bool)));
			value = 'A'; Assert.AreEqual(value, func(value, typeof(char)));
			value = (sbyte)10; Assert.AreEqual(value, func(value, typeof(sbyte)));
			value = (short)11; Assert.AreEqual(value, func(value, typeof(short)));
			value = (ushort)12; Assert.AreEqual(value, func(value, typeof(ushort)));
			value = (int)13; Assert.AreEqual(value, func(value, typeof(int)));
			value = (uint)14; Assert.AreEqual(value, func(value, typeof(uint)));
			value = (long)15; Assert.AreEqual(value, func(value, typeof(long)));
			value = (ulong)16; Assert.AreEqual(value, func(value, typeof(ulong)));
			value = (float)17; Assert.AreEqual(value, func(value, typeof(float)));
			value = (double)18; Assert.AreEqual(value, func(value, typeof(double)));
			value = (decimal)19; Assert.AreEqual(value, func(value, typeof(decimal)));
			value = new TestClass(); Assert.AreEqual(value, func(value, typeof(TestClass)));
			// 这里还有一点是 dynamic 和 object 是等效的，但由于在运行时
			// dynamic 和 object 没有区别（参见规范 4.7 节），
			// 因此在判断类型转换时完全不用考虑它。

			#endregion // 6.1.1 标识转换

			#region 6.1.2 隐式数值转换

			Assert.AreEqual((short)10, func((sbyte)10, typeof(short)));
			Assert.AreEqual((int)11, func((sbyte)11, typeof(int)));
			Assert.AreEqual((long)12, func((sbyte)12, typeof(long)));
			Assert.AreEqual((float)13, func((sbyte)13, typeof(float)));
			Assert.AreEqual((double)14, func((sbyte)14, typeof(double)));
			Assert.AreEqual((decimal)15, func((sbyte)15, typeof(decimal)));
			Assert.AreEqual((short)10, func((byte)10, typeof(short)));
			Assert.AreEqual((ushort)11, func((byte)11, typeof(ushort)));
			Assert.AreEqual((int)12, func((byte)12, typeof(int)));
			Assert.AreEqual((uint)13, func((byte)13, typeof(uint)));
			Assert.AreEqual((long)14, func((byte)14, typeof(long)));
			Assert.AreEqual((ulong)15, func((byte)15, typeof(ulong)));
			Assert.AreEqual((float)16, func((byte)16, typeof(float)));
			Assert.AreEqual((double)17, func((byte)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((byte)18, typeof(decimal)));
			Assert.AreEqual((int)12, func((short)12, typeof(int)));
			Assert.AreEqual((long)14, func((short)14, typeof(long)));
			Assert.AreEqual((float)16, func((short)16, typeof(float)));
			Assert.AreEqual((double)17, func((short)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((short)18, typeof(decimal)));
			Assert.AreEqual((int)12, func((ushort)12, typeof(int)));
			Assert.AreEqual((uint)13, func((ushort)13, typeof(uint)));
			Assert.AreEqual((long)14, func((ushort)14, typeof(long)));
			Assert.AreEqual((ulong)15, func((ushort)15, typeof(ulong)));
			Assert.AreEqual((float)16, func((ushort)16, typeof(float)));
			Assert.AreEqual((double)17, func((ushort)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((ushort)18, typeof(decimal)));
			Assert.AreEqual((long)14, func((int)14, typeof(long)));
			Assert.AreEqual((float)16, func((int)16, typeof(float)));
			Assert.AreEqual((double)17, func((int)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((int)18, typeof(decimal)));
			Assert.AreEqual((long)14, func((uint)14, typeof(long)));
			Assert.AreEqual((ulong)15, func((uint)15, typeof(ulong)));
			Assert.AreEqual((float)16, func((uint)16, typeof(float)));
			Assert.AreEqual((double)17, func((uint)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((uint)18, typeof(decimal)));
			Assert.AreEqual((float)16, func((long)16, typeof(float)));
			Assert.AreEqual((double)17, func((long)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((long)18, typeof(decimal)));
			Assert.AreEqual((float)16, func((ulong)16, typeof(float)));
			Assert.AreEqual((double)17, func((ulong)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((ulong)18, typeof(decimal)));
			Assert.AreEqual((ushort)11, func((char)11, typeof(ushort)));
			Assert.AreEqual((int)12, func((char)12, typeof(int)));
			Assert.AreEqual((uint)13, func((char)13, typeof(uint)));
			Assert.AreEqual((long)14, func((char)14, typeof(long)));
			Assert.AreEqual((ulong)15, func((char)15, typeof(ulong)));
			Assert.AreEqual((float)16, func((char)16, typeof(float)));
			Assert.AreEqual((double)17, func((char)17, typeof(double)));
			Assert.AreEqual((decimal)18, func((char)18, typeof(decimal)));
			Assert.AreEqual((double)17, func((float)17, typeof(double)));

			#endregion // 6.1.2 隐式数值转换

			// 6.1.3 隐式枚举转换，针对的是十进制数字文本 0，不在考虑范围内。

			#region 6.1.4 可以为 null 的隐式转换

			// 从 S 到 T? 的隐式转换。
			Assert.AreEqual((int?)10, func((int)10, typeof(int?)));
			Assert.AreEqual((long?)10, func((int)10, typeof(long?)));
			// 从 S? 到 T? 的隐式转换。
			Assert.AreEqual((int?)10, func((int?)10, typeof(int?)));
			Assert.AreEqual((long?)10, func((int?)10, typeof(long?)));
			Assert.AreEqual((long?)null, func((int?)null, typeof(long?)));

			#endregion // 6.1.4 可以为 null 的隐式转换

			// 6.1.5 null 文本转换，不在考虑范围内。

			#region 6.1.6 隐式引用转换

			// 6.1.6.1 从任何 reference-type 到 object （和 dynamic）。
			value = "abc"; Assert.AreEqual((object)value, func(value, typeof(object)));
			value = new TestClass(); Assert.AreEqual((object)value, func(value, typeof(object)));
			// 6.1.6.2 从任何 class-type S 到任何 class-type T（前提是 S 是从 T 派生的）。
			value = new TestClass2(); Assert.AreEqual((TestClass)value, func(value, typeof(TestClass)));
			// 6.1.6.3 从任何 class-type S 到任何 interface-type T（前提是 S 实现了 T）。
			value = new List<int>(); Assert.AreEqual((IList<int>)value, func(value, typeof(IList<int>)));
			// 6.1.6.4 从任何 interface-type S 到任何 interface-type T（前提是 S 是从 T 派生的）。
			value = new List<int>(); Assert.AreEqual((IList)value, func(value, typeof(IList)));
			// 6.1.6.5 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			// o S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			// o SE 和 TE 都是 reference-type。
			// o 存在从 SE 到 TE 的隐式引用转换。
			value = new string[0]; Assert.AreEqual((object[])value, func(value, typeof(object[])));
			value = new TestClass2[0]; Assert.AreEqual((TestClass[])value, func(value, typeof(TestClass[])));
			value = new List<int>[0]; Assert.AreEqual((IList[])value, func(value, typeof(IList[])));
			value = new List<string>[0]; Assert.AreEqual(value, func(value, typeof(IEnumerable<object>[])));
			// 6.1.6.6 从任何 array-type 到 System.Array 及其实现的接口。
			value = new int[0]; Assert.AreEqual((Array)value, func(value, typeof(Array)));
			value = new object[0]; Assert.AreEqual((Array)value, func(value, typeof(Array)));
			// 6.1.6.7 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口
			// （前提是存在从 S 到 T 的隐式标识或引用转换）。
			value = new int[0]; Assert.AreEqual((IList<int>)value, func(value, typeof(IList<int>)));
			value = new TestClass[0][][];
			Assert.AreEqual((IList<IList<TestClass>[]>)value, func(value, typeof(IList<IList<TestClass>[]>)));
			// 6.1.6.8 从任何 delegate-type 到 System.Delegate 及其实现的接口。
			value = new Func<int>(TestMethod); Assert.AreEqual((Delegate)value, func((Delegate)value, typeof(Delegate)));
			// 6.1.6.9 从 null 文本到任何 reference-type，不在考虑范围内。
			// 6.1.6.10 从任何 reference-type 到 reference-type T
			// （前提是它具有到 reference-type T0 的隐式标识或引用转换，且 T0 具有到 T 的标识转换）。
			// 此条规则也可以不考虑。
			// 6.1.6.11 从任何 reference-type 到接口或委托类型 T
			// （前提是它具有到接口或委托类型 T0 的隐式标识或引用转换，且 T0 可变化转换为T）。
			// 这里的变化转换在规范的 13.1.3.2 节，就是泛型的协变和逆变。
			// 协变。
			value = Enumerable.Empty<string>();
			Assert.AreEqual((IEnumerable<object>)value, func(value, typeof(IEnumerable<object>)));
			value = Enumerable.Empty<IEqualityComparer<object>[]>();
			Assert.AreEqual((IEnumerable<IEqualityComparer<string>[]>)value, func(value, typeof(IEnumerable<IEqualityComparer<string>[]>)));
			value = Enumerable.Empty<object[][]>();
			Assert.AreEqual((IEnumerable<IList<IList<object>>>)value, func(value, typeof(IEnumerable<IList<IList<object>>>)));
			// 逆变。
			value = EqualityComparer<object>.Default;
			Assert.AreEqual((IEqualityComparer<string>)value, func(value, typeof(IEqualityComparer<string>)));
			value = EqualityComparer<IEnumerable<object>[]>.Default;
			Assert.AreEqual((IEqualityComparer<IEnumerable<string>[]>)value, func(value, typeof(IEqualityComparer<IEnumerable<string>[]>)));
			value = EqualityComparer<IList<TestClass[]>>.Default;
			Assert.AreEqual((IEqualityComparer<TestClass2[][]>)value, func(value, typeof(IEqualityComparer<TestClass2[][]>)));
			// 6.1.6.12 涉及已知为引用类型的类型参数的隐式转换。这个转换是针对类型参数 T 的，因此同样不做考虑。

			#endregion // 6.1.6 隐式引用转换

			#region 6.1.7 装箱转换

			// 从 non-nullable-value-type 到 object。
			value = (uint)10; Assert.AreEqual((object)value, func(value, typeof(object)));
			value = new TestStruct(); Assert.AreEqual((object)value, func(value, typeof(object)));
			// 从 non-nullable-value-type 到 System.ValueType。
			value = 10; Assert.AreEqual((ValueType)value, func(value, typeof(ValueType)));
			value = new TestStruct2(); Assert.AreEqual((ValueType)value, func(value, typeof(ValueType)));
			// 从 non-nullable-value-type 到其实现的接口。
			value = 10; Assert.AreEqual((IComparable<int>)value, func(value, typeof(IComparable<int>)));
			value = new TestStruct4(); Assert.AreEqual((IEnumerable<string>)value, func(value, typeof(IEnumerable<string>)));
			// 从 enum-type 转换为 System.Enum 类型。
			value = Tristate.True; Assert.AreEqual((Enum)value, func(value, typeof(Enum)));
			// 从 nullable-type 到引用类型的装箱转换，如果存在从对应的 non-nullable-value-type 到该引用类型的装箱转换。
			value = (uint?)10; Assert.AreEqual((object)value, func(value, typeof(object)));
			value = (int?)11; Assert.AreEqual((ValueType)value, func(value, typeof(ValueType)));
			value = (int?)null; Assert.AreEqual((ValueType)value, func(value, typeof(ValueType)));
			value = (int?)12; Assert.AreEqual((IComparable<int>)value, func(value, typeof(IComparable<int>)));
			value = (Tristate?)Tristate.True; Assert.AreEqual((Enum)value, func(value, typeof(Enum)));
			// 如果值类型具有到接口或委托类型 I0 的装箱转换，且 I0 变化转换为接口类型 I，则值类型具有到 I 的装箱转换。
			value = new TestStruct4(); Assert.AreEqual((IEnumerable<object>)value, func(value, typeof(IEnumerable<object>)));
			value = (TestStruct4?)new TestStruct4(); Assert.AreEqual((IEnumerable<object>)value, func(value, typeof(IEnumerable<object>)));
			value = (TestStruct4?)null; Assert.AreEqual((IEnumerable<object>)value, func(value, typeof(IEnumerable<object>)));

			#endregion // 6.1.7 装箱转换

			// 6.1.8 隐式动态转换、6.1.9 隐式常量表达式转换、6.1.10 涉及类型形参的隐式转换，不在考虑范围内。

			#region 6.1.11 用户定义的隐式转换

			value = new TestStruct2();
			object expectedValue = (TestStruct)new TestStruct2();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct)));
			value = new TestStruct2();
			expectedValue = (TestStruct?)new TestStruct2();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct2?)new TestStruct2();
			expectedValue = (TestStruct?)(TestStruct2?)new TestStruct2();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct2?)null;
			Assert.AreEqual((TestStruct?)value, func(value, typeof(TestStruct?)));
			value = new TestStruct3();
			expectedValue = (TestStruct)new TestStruct3();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct)));
			value = new TestStruct3();
			expectedValue = (TestStruct?)new TestStruct3();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct3?)new TestStruct3();
			expectedValue = (TestStruct?)(TestStruct3?)new TestStruct3();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct3?)null; Assert.AreEqual(value, func(value, typeof(TestStruct?)));
			value = new TestStruct5();
			expectedValue = (TestStruct?)new TestStruct5();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = new TestStruct6();
			expectedValue = (TestStruct)new TestStruct6();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct)));
			value = (TestStruct6?)new TestStruct6();
			expectedValue = (TestStruct)(TestStruct6?)new TestStruct6();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct)));
			value = new TestStruct6();
			expectedValue = (TestStruct?)new TestStruct6();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct6?)new TestStruct6();
			expectedValue = (TestStruct?)(TestStruct6?)new TestStruct6();
			Assert.AreEqual(expectedValue, func(value, typeof(TestStruct?)));
			value = (TestStruct6?)null; Assert.AreEqual(value, func(value, typeof(TestStruct?)));
			value = new TestClass();
			expectedValue = (int)new TestClass();
			Assert.AreEqual(expectedValue, func(value, typeof(int)));
			value = new TestClass2();
			expectedValue = (int)new TestClass2();
			Assert.AreEqual(expectedValue, func(value, typeof(int)));
			value = new TestClass2();
			expectedValue = (long)new TestClass2();
			Assert.AreEqual(expectedValue, func(value, typeof(long)));
			value = new TestClass2();
			expectedValue = (bool)new TestClass2();
			Assert.AreEqual(expectedValue, func(value, typeof(bool)));
			value = new TestClass13();
			expectedValue = (Enum)new TestClass13();
			Assert.AreEqual(expectedValue, func(value, typeof(Enum)));
			value = new TestClass12();
			expectedValue = (int?)new TestClass12();
			Assert.AreEqual(expectedValue, func(value, typeof(int?)));
			value = new TestClass8();
			expectedValue = (TestClass6)new TestClass8();
			Assert.AreEqual(expectedValue, func(value, typeof(TestClass6)));
			value = new TestClass6();
			expectedValue = (TestClass7)new TestClass6();
			Assert.AreEqual(expectedValue, func(value, typeof(TestClass7)));
			value = 10;
			expectedValue = (TestClass6)10;
			Assert.AreEqual(expectedValue, func(value, typeof(TestClass6)));
			value = new TestClass();
			expectedValue = (long)new TestClass();
			Assert.AreEqual(expectedValue, func(value, typeof(long)));
			value = new TestClass();
			expectedValue = (decimal)new TestClass();
			Assert.AreEqual(expectedValue, func(value, typeof(decimal)));
			value = new TestClass2();
			expectedValue = (decimal)new TestClass2();
			Assert.AreEqual(expectedValue, func(value, typeof(decimal)));
			value = (short)10;
			expectedValue = (TestClass6)(short)10;
			Assert.AreEqual(expectedValue, func(value, typeof(TestClass6)));
			value = new TestClass6();
			expectedValue = (long)new TestClass6();
			Assert.AreEqual(expectedValue, func(value, typeof(long)));

			#endregion // 6.1.11 用户定义的隐式转换

		}
		/// <summary>
		/// 对 <see cref="Cyjb.ConvertExt.ChangeType"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestChangeType()
		{
			TestImplicitChangeTypeHelper(ConvertExt.ChangeType);

			#region 6.2.1 显式数值转换

			Assert.AreEqual((byte)10, ConvertExt.ChangeType((sbyte)10, typeof(byte)));
			Assert.AreEqual((ushort)11, ConvertExt.ChangeType((sbyte)11, typeof(ushort)));
			Assert.AreEqual((uint)12, ConvertExt.ChangeType((sbyte)12, typeof(uint)));
			Assert.AreEqual((ulong)13, ConvertExt.ChangeType((sbyte)13, typeof(ulong)));
			Assert.AreEqual((char)14, ConvertExt.ChangeType((sbyte)14, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((byte)10, typeof(sbyte)));
			Assert.AreEqual((char)11, ConvertExt.ChangeType((byte)11, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((short)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((short)11, typeof(byte)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((short)13, typeof(ushort)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((short)15, typeof(uint)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((short)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((short)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((ushort)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((ushort)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((ushort)12, typeof(short)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((ushort)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((int)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((int)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((int)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((int)13, typeof(ushort)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((int)15, typeof(uint)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((int)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((int)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((uint)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((uint)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((uint)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((uint)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((uint)14, typeof(int)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((uint)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((long)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((long)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((long)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((long)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((long)14, typeof(int)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((long)15, typeof(uint)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((long)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((long)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((ulong)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((ulong)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((ulong)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((ulong)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((ulong)14, typeof(int)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((ulong)15, typeof(uint)));
			Assert.AreEqual((long)16, ConvertExt.ChangeType((ulong)16, typeof(long)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((ulong)18, typeof(char)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((char)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((char)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((char)12, typeof(short)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((float)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((float)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((float)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((float)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((float)14, typeof(int)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((float)15, typeof(uint)));
			Assert.AreEqual((long)16, ConvertExt.ChangeType((float)16, typeof(long)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((float)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((float)18, typeof(char)));
			Assert.AreEqual((decimal)21, ConvertExt.ChangeType((float)21, typeof(decimal)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((double)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((double)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((double)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((double)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((double)14, typeof(int)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((double)15, typeof(uint)));
			Assert.AreEqual((long)16, ConvertExt.ChangeType((double)16, typeof(long)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((double)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((double)18, typeof(char)));
			Assert.AreEqual((float)19, ConvertExt.ChangeType((double)19, typeof(float)));
			Assert.AreEqual((decimal)21, ConvertExt.ChangeType((double)21, typeof(decimal)));
			Assert.AreEqual((sbyte)10, ConvertExt.ChangeType((decimal)10, typeof(sbyte)));
			Assert.AreEqual((byte)11, ConvertExt.ChangeType((decimal)11, typeof(byte)));
			Assert.AreEqual((short)12, ConvertExt.ChangeType((decimal)12, typeof(short)));
			Assert.AreEqual((ushort)13, ConvertExt.ChangeType((decimal)13, typeof(ushort)));
			Assert.AreEqual((int)14, ConvertExt.ChangeType((decimal)14, typeof(int)));
			Assert.AreEqual((uint)15, ConvertExt.ChangeType((decimal)15, typeof(uint)));
			Assert.AreEqual((long)16, ConvertExt.ChangeType((decimal)16, typeof(long)));
			Assert.AreEqual((ulong)17, ConvertExt.ChangeType((decimal)17, typeof(ulong)));
			Assert.AreEqual((char)18, ConvertExt.ChangeType((decimal)18, typeof(char)));
			Assert.AreEqual((float)19, ConvertExt.ChangeType((decimal)19, typeof(float)));
			Assert.AreEqual((double)20, ConvertExt.ChangeType((decimal)20, typeof(double)));

			#endregion // 6.2.1 显式数值转换

			#region 6.2.2 显式枚举转换

			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((sbyte)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((byte)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((short)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((ushort)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((int)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((uint)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((long)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((ulong)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((char)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((float)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((double)1, typeof(Tristate)));
			Assert.AreEqual(Tristate.True, ConvertExt.ChangeType((decimal)1, typeof(Tristate)));
			Assert.AreEqual((sbyte)1, ConvertExt.ChangeType(Tristate.True, typeof(sbyte)));
			Assert.AreEqual((byte)1, ConvertExt.ChangeType(Tristate.True, typeof(byte)));
			Assert.AreEqual((short)1, ConvertExt.ChangeType(Tristate.True, typeof(short)));
			Assert.AreEqual((ushort)1, ConvertExt.ChangeType(Tristate.True, typeof(ushort)));
			Assert.AreEqual((int)1, ConvertExt.ChangeType(Tristate.True, typeof(int)));
			Assert.AreEqual((uint)1, ConvertExt.ChangeType(Tristate.True, typeof(uint)));
			Assert.AreEqual((long)1, ConvertExt.ChangeType(Tristate.True, typeof(long)));
			Assert.AreEqual((ulong)1, ConvertExt.ChangeType(Tristate.True, typeof(ulong)));
			Assert.AreEqual((char)1, ConvertExt.ChangeType(Tristate.True, typeof(char)));
			Assert.AreEqual((float)1, ConvertExt.ChangeType(Tristate.True, typeof(float)));
			Assert.AreEqual((double)1, ConvertExt.ChangeType(Tristate.True, typeof(double)));
			Assert.AreEqual((decimal)1, ConvertExt.ChangeType(Tristate.True, typeof(decimal)));
			Assert.AreEqual((BindingFlags)1, ConvertExt.ChangeType(Tristate.True, typeof(BindingFlags)));
			Assert.AreEqual((Tristate)1, ConvertExt.ChangeType((BindingFlags)1, typeof(Tristate)));

			#endregion // 6.2.2 显式枚举转换

			#region 6.2.3 可以为 null 的显式转换

			// 从 S 到 T? 的显式转换。
			Assert.AreEqual((int?)10, ConvertExt.ChangeType((long)10, typeof(int?)));
			// 从 S? 到 T? 的显式转换。
			Assert.AreEqual((int?)10, ConvertExt.ChangeType((long?)10, typeof(int?)));
			Assert.AreEqual((int?)null, ConvertExt.ChangeType((long?)null, typeof(int?)));
			// 从 S? 到 T 的显式转换。
			Assert.AreEqual(10, ConvertExt.ChangeType((long?)10, typeof(int)));

			#endregion // 6.2.3 可以为 null 的显式转换

		}

		#region 测试辅助类

		private static int TestMethod() { return 0; }
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
		private class TestClass4<T1, T2> { }
		private class TestClass5<T> : TestClass4<int, T> { }
		private class TestClass6
		{
			public static implicit operator TestClass6(int t)
			{
				return new TestClass6();
			}
			public static implicit operator int(TestClass6 t)
			{
				return 1;
			}
			public static implicit operator TestClass6(TestClass7 t)
			{
				return new TestClass6();
			}
			public static implicit operator TestClass8(TestClass6 t)
			{
				return new TestClass8();
			}
			public override bool Equals(object obj)
			{
				if (obj == null) { return false; }
				return obj.GetType() == typeof(TestClass6);
			}
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
		private class TestClass7 { }
		private class TestClass8 : TestClass7
		{
			public override bool Equals(object obj)
			{
				if (obj == null) { return false; }
				return obj.GetType() == typeof(TestClass8);
			}
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
		private class TestClass9
		{
			public static explicit operator TestClass10(TestClass9 t)
			{
				return new TestClass10();
			}
			public static explicit operator TestClass9(TestClass11 t)
			{
				return new TestClass9();
			}
		}
		private class TestClass10 { }
		private class TestClass11 : TestClass10 { }
		private class TestClass12
		{
			public static implicit operator Nullable<int>(TestClass12 t)
			{
				return 12;
			}
			public static implicit operator TestClass12(Nullable<int> t)
			{
				return new TestClass12();
			}
		}
		private class TestClass13
		{
			public static implicit operator Enum(TestClass13 t)
			{
				return Tristate.False;
			}
		}
		private class TestClass14
		{
			public static implicit operator Tristate(TestClass14 t)
			{
				return Tristate.False;
			}
		}
		private struct TestStruct
		{
			public static implicit operator TestStruct(TestStruct3 tc)
			{
				return new TestStruct();
			}
			public static implicit operator TestStruct(TestStruct6? tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct2
		{
			public static implicit operator TestStruct(TestStruct2 tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct3 { }
		private struct TestStruct4 : IEnumerable<string>
		{
			public IEnumerator<string> GetEnumerator() { return null; }
			IEnumerator IEnumerable.GetEnumerator() { return null; }
		}
		private struct TestStruct5
		{
			public static implicit operator TestStruct?(TestStruct5 tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct6 { }
		private delegate void TestDelegate<T>(T a);

		#endregion // 测试辅助类

	}
}
