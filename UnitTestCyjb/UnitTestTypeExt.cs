using System;
using System.Collections;
using System.Collections.Generic;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.TypeExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsImplicitFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsImplicitFrom()
		{
			// object 可以从任何类型隐式类型转换。
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(string)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(TestClass)));
			// 子类隐式类型转换为父类。
			Assert.IsTrue(typeof(TestClass).IsImplicitFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsImplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Delegate).IsImplicitFrom(typeof(Func<int>)));
			Assert.IsTrue(typeof(Array).IsImplicitFrom(typeof(int[])));
			Assert.IsTrue(typeof(Array).IsImplicitFrom(typeof(object[])));
			Assert.IsTrue(typeof(IList<int>).IsImplicitFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(IList).IsImplicitFrom(typeof(List<int>)));
			// 用户自定义隐式类型转换运算符。
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsImplicitFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsImplicitFrom(typeof(TestClass13)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(TestClass8)));
			Assert.IsTrue(typeof(TestClass7).IsImplicitFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(int)));
			// CLS 基本类型间的隐式类型转换。
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsImplicitFrom(typeof(ushort)));
			// 用户自定义隐式类型转换运算符 + 基本类型隐式类型转换。
			Assert.IsTrue(typeof(long).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(long).IsImplicitFrom(typeof(TestClass6)));
			// Nullable<T> 的隐式类型转换。
			Assert.IsTrue(typeof(int?).IsImplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(long?).IsImplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct3)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct3?)));
			// 不允许从 object 隐式类型转换。
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(object)));
			// 不允许从父类隐式类型转换为子类。
			Assert.IsFalse(typeof(TestClass2).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(int[]).IsImplicitFrom(typeof(Array)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList<int>)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList)));
			Assert.IsFalse(typeof(Tristate).IsImplicitFrom(typeof(Enum)));
			Assert.IsFalse(typeof(Func<int>).IsImplicitFrom(typeof(Delegate)));
			// 不允许用户未定义的类型转换或显示类型转换。
			Assert.IsFalse(typeof(string).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(TestClass3)));
			// 不允许从窄类型隐式类型转换到宽类型。
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(long)));
			Assert.IsFalse(typeof(uint).IsImplicitFrom(typeof(short)));
			Assert.IsFalse(typeof(short).IsImplicitFrom(typeof(TestClass)));
			// 不允许的 Nullable<T> 隐式类型转换。
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(int?)));
			Assert.IsFalse(typeof(int?).IsImplicitFrom(typeof(long?)));
			Assert.IsFalse(typeof(TestStruct2).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2?).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct2?)));
			Assert.IsFalse(typeof(TestStruct2?).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct3?).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3?).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct3?)));
			// 不允许两次隐式类型转换。
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(TestClass3)));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsCastableFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsCastableFrom()
		{
			// object 可以与任何类型相互显示类型转换。
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(object)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(string)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(uint)));
			Assert.IsTrue(typeof(object).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(TestClass).IsCastableFrom(typeof(object)));
			// 可以沿着继承链任意显示类型转换。
			Assert.IsTrue(typeof(TestClass).IsCastableFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsCastableFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Delegate).IsCastableFrom(typeof(Func<int>)));
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(Enum)));
			Assert.IsTrue(typeof(Func<int>).IsCastableFrom(typeof(Delegate)));
			Assert.IsTrue(typeof(TestClass2).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(Array).IsCastableFrom(typeof(int[])));
			Assert.IsTrue(typeof(Array).IsCastableFrom(typeof(object[])));
			Assert.IsTrue(typeof(IList<int>).IsCastableFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(IList).IsCastableFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(int[]).IsCastableFrom(typeof(Array)));
			Assert.IsTrue(typeof(object[]).IsCastableFrom(typeof(Array)));
			Assert.IsTrue(typeof(List<int>).IsCastableFrom(typeof(IList<int>)));
			Assert.IsTrue(typeof(List<int>).IsCastableFrom(typeof(IList)));
			// 用户自定义显式类型转换运算符
			Assert.IsTrue(typeof(Enum).IsCastableFrom(typeof(TestClass13)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsCastableFrom(typeof(TestClass2)));
			// CLS 基本类型间的显式类型转换。
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsCastableFrom(typeof(ushort)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(long)));
			Assert.IsTrue(typeof(uint).IsCastableFrom(typeof(short)));
			// 用户自定义显式类型转换运算符 + 类型显式类型转换。
			Assert.IsTrue(typeof(long).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(short).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass3)));
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(TestClass13)));
			Assert.IsTrue(typeof(Enum).IsCastableFrom(typeof(TestClass14)));
			// 枚举的显示类型转换。
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(TypeCode)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(Tristate)));
			// Nullable<T> 的显式类型转换。
			Assert.IsTrue(typeof(int?).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(long?).IsCastableFrom(typeof(int?)));
			Assert.IsTrue(typeof(int?).IsCastableFrom(typeof(long?)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(int?)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct3)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct3?)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(TestClass8)));
			Assert.IsTrue(typeof(TestClass7).IsCastableFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(TestClass10).IsCastableFrom(typeof(TestClass9)));
			Assert.IsTrue(typeof(TestClass11).IsCastableFrom(typeof(TestClass9)));
			Assert.IsTrue(typeof(TestClass9).IsCastableFrom(typeof(TestClass10)));
			Assert.IsTrue(typeof(TestClass9).IsCastableFrom(typeof(TestClass11)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass12)));
			Assert.IsTrue(typeof(TestClass12).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(long).IsCastableFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct3?)));
			// 不允许两次隐式类型转换。
			Assert.IsFalse(typeof(TestClass).IsCastableFrom(typeof(TestClass3)));
			// 不允许用户未定义的类型转换。
			Assert.IsFalse(typeof(string).IsCastableFrom(typeof(TestClass)));
			// 不允许枚举的两次类型转换。
			Assert.IsFalse(typeof(int).IsCastableFrom(typeof(TestClass13)));
			Assert.IsFalse(typeof(int).IsCastableFrom(typeof(TestClass14)));
			// 不允许的 Nullable<T> 类型转换。
			Assert.IsFalse(typeof(TestStruct2).IsCastableFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2).IsCastableFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsCastableFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2?).IsCastableFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsCastableFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsCastableFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsCastableFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct3?).IsCastableFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3?).IsCastableFrom(typeof(TestStruct?)));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsCastableFromOpenGenericIsAssignableFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestOpenGenericIsAssignableFrom()
		{
			Type[] genericArguments;
			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(IEnumerable<>), out genericArguments));
			Assert.AreEqual(1, genericArguments.Length);
			Assert.IsTrue(genericArguments[0].IsGenericParameter);
			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(IEnumerable<int>), out genericArguments));
			Assert.AreEqual(1, genericArguments.Length);
			Assert.AreEqual(typeof(int), genericArguments[0]);
			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(Dictionary<int, string>), out genericArguments));
			Assert.AreEqual(1, genericArguments.Length);
			Assert.AreEqual(typeof(KeyValuePair<int, string>), genericArguments[0]);
			Assert.IsTrue(typeof(IDictionary<,>).OpenGenericIsAssignableFrom(typeof(Dictionary<int, string>), out genericArguments));
			Assert.AreEqual(2, genericArguments.Length);
			Assert.AreEqual(typeof(int), genericArguments[0]);
			Assert.AreEqual(typeof(string), genericArguments[1]);
			Assert.IsTrue(typeof(TestClass4<,>).OpenGenericIsAssignableFrom(typeof(TestClass5<>), out genericArguments));
			Assert.AreEqual(2, genericArguments.Length);
			Assert.AreEqual(typeof(int), genericArguments[0]);
			Assert.IsTrue(genericArguments[1].IsGenericParameter);
			Assert.IsTrue(typeof(TestClass4<,>).OpenGenericIsAssignableFrom(typeof(TestClass5<string>), out genericArguments));
			Assert.AreEqual(2, genericArguments.Length);
			Assert.AreEqual(typeof(int), genericArguments[0]);
			Assert.AreEqual(typeof(string), genericArguments[1]);
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
		}
		private class TestClass7 { }
		private class TestClass8 : TestClass7 { }
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
		}
		private struct TestStruct2
		{
			public static implicit operator TestStruct(TestStruct2 tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct3 { }
	}
}
