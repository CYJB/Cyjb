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
			// Trues
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(string)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(TestClass).IsImplicitFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsImplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(long).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsImplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsImplicitFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(Enum).IsImplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Delegate).IsImplicitFrom(typeof(Func<int>)));
			Assert.IsTrue(typeof(int?).IsImplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(long?).IsImplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(Array).IsImplicitFrom(typeof(int[])));
			Assert.IsTrue(typeof(Array).IsImplicitFrom(typeof(object[])));
			Assert.IsTrue(typeof(IList<int>).IsImplicitFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(IList).IsImplicitFrom(typeof(List<int>)));
			// Nullable<T>
			Assert.IsTrue(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct3)));
			Assert.IsTrue(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct3?)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(TestClass6).IsImplicitFrom(typeof(TestClass8)));
			Assert.IsTrue(typeof(TestClass7).IsImplicitFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(long).IsImplicitFrom(typeof(TestClass6)));
			// Falses
			Assert.IsFalse(typeof(int?).IsImplicitFrom(typeof(long?)));
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(object)));
			Assert.IsFalse(typeof(TestClass2).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(long)));
			Assert.IsFalse(typeof(uint).IsImplicitFrom(typeof(short)));
			Assert.IsFalse(typeof(short).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(string).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(Tristate).IsImplicitFrom(typeof(Enum)));
			Assert.IsFalse(typeof(Func<int>).IsImplicitFrom(typeof(Delegate)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(int?)));
			Assert.IsFalse(typeof(int[]).IsImplicitFrom(typeof(Array)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList<int>)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList)));
			// Nullable<T>
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
			Assert.IsTrue(typeof(long?).IsCastableFrom(typeof(int?)));
			Assert.IsTrue(typeof(int?).IsCastableFrom(typeof(long?)));
			Assert.IsTrue(typeof(TestClass).IsCastableFrom(typeof(object)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(long)));
			Assert.IsTrue(typeof(uint).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(short).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(TestClass3)));
			Assert.IsTrue(typeof(Tristate).IsCastableFrom(typeof(Enum)));
			Assert.IsTrue(typeof(Func<int>).IsCastableFrom(typeof(Delegate)));
			Assert.IsTrue(typeof(int).IsCastableFrom(typeof(int?)));
			Assert.IsTrue(typeof(TestClass2).IsCastableFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(Array).IsCastableFrom(typeof(int[])));
			Assert.IsTrue(typeof(Array).IsCastableFrom(typeof(object[])));
			Assert.IsTrue(typeof(IList<int>).IsCastableFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(IList).IsCastableFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(int[]).IsCastableFrom(typeof(Array)));
			Assert.IsTrue(typeof(object[]).IsCastableFrom(typeof(Array)));
			Assert.IsTrue(typeof(List<int>).IsCastableFrom(typeof(IList<int>)));
			Assert.IsTrue(typeof(List<int>).IsCastableFrom(typeof(IList)));
			// Nullable<T>
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct3)));
			Assert.IsTrue(typeof(TestStruct?).IsCastableFrom(typeof(TestStruct3?)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(short)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(int)));
			Assert.IsTrue(typeof(TestClass6).IsCastableFrom(typeof(TestClass8)));
			Assert.IsTrue(typeof(TestClass7).IsCastableFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(long).IsCastableFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct).IsCastableFrom(typeof(TestStruct3?)));
			// Falses
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(TestClass3)));
			Assert.IsFalse(typeof(string).IsCastableFrom(typeof(TestClass)));
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
