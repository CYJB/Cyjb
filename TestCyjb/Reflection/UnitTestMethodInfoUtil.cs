using System;
using System.Collections.Generic;
using System.Reflection;
using Cyjb.Collections;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Reflection
{
	/// <summary>
	/// <see cref="MethodInfoUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestMethodInfoUtil
	{
		private class TestClass
		{
#pragma warning disable IDE0060 // 删除未使用的参数
			public static void Test1() { }
			public static void Test2(int a) { }
			public static int Test3(int[] b) { return 0; }
			public static void Test4(int a, string b) { }
			public static void Test5(int a, params int[] c) { }
			public static int? Test6(int a, string b = "abc") { return 0; }
#pragma warning restore IDE0060 // 删除未使用的参数
		}

		/// <summary>
		/// 对 <see cref="MethodInfoUtil.GetParameterTypesWithReturn"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(void))]
		[DataRow("Test2", typeof(int), typeof(void))]
		[DataRow("Test3", typeof(int[]), typeof(int))]
		[DataRow("Test4", typeof(int), typeof(string), typeof(void))]
		[DataRow("Test5", typeof(int), typeof(int[]), typeof(void))]
		[DataRow("Test6", typeof(int), typeof(string), typeof(int?))]
		public void TestGetParameterTypesWithReturn(string methodName, params Type[] types)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			CollectionAssert.AreEqual(types, methodInfo.GetParameterTypesWithReturn());
		}

		private class TestClass2
		{
#pragma warning disable IDE0060 // 删除未使用的参数
			public static Type[] Test1<T>(T value) { return new Type[] { typeof(T) }; }
			public static Type[] Test2<T>(IList<T> list, params T[] args) { return new Type[] { typeof(T) }; }
			public static Type[] Test3<T>(T value, T value2) { return new Type[] { typeof(T) }; }
			public static Type[] Test4<T, T2>(T value, T2 value2, T2? value3 = default)
			{
				return new Type[] { typeof(T), typeof(T2) };
			}
			public static Type[] Test5<T>(T arg) { return new Type[] { typeof(T) }; }
			public static Type[] Test6<T>(ref T arg) { return new Type[] { typeof(T) }; }
			public static Type[] Test7<T>(T arg, T arg2) { return new Type[] { typeof(T) }; }
			public static Type[] Test8<T>(ref T arg, T arg2) { return new Type[] { typeof(T) }; }
			public static Type[] Test9<T>(IList<T> arg, T arg2) { return new Type[] { typeof(T) }; }
			public static Type[] Test10<T1, T2>(IList<IDictionary<T1, T2>> arg, IList<T1[]> arg2)
			{
				return new Type[] { typeof(T1), typeof(T2) };
			}
			public static Type[] Test11<T1, T2>(IEnumerable<IDictionary<T1, T2>> arg, IList<T1[]> arg2)
			{
				return new Type[] { typeof(T1), typeof(T2) };
			}
			public static Type[] Test12<T>(T a, params T[] b) { return new Type[] { typeof(T) }; }
			public static Type[] Test13<T1, T2>(Func<T1, T2> arg, T1 a, T2 b)
			{
				return new Type[] { typeof(T1), typeof(T2) };
			}
#pragma warning restore IDE0060 // 删除未使用的参数
		}
		private class TestBaseClass { }
		private class TestSubClass : TestBaseClass { }

		/// <summary>
		/// 对 <see cref="MethodInfoUtil.GenericArgumentsInferences"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGenericArgumentsInferences()
		{
			Type type = typeof(TestClass2);
			MethodInfo method = type.GetMethod("Test1")!;
			CollectionAssert.AreEqual(
				TestClass2.Test1(10),
				method.GenericArgumentsInferences(typeof(int))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test1((long?)10),
				method.GenericArgumentsInferences(typeof(long?))
			);

			method = type.GetMethod("Test2")!;
			CollectionAssert.AreEqual(
				TestClass2.Test2(Fake.Null<List<int>>()),
				method.GenericArgumentsInferences(typeof(List<int>))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test2(Fake.Null<List<int>>(), Fake.Null<int[]>()),
				method.GenericArgumentsInferences(typeof(List<int>), typeof(int[]))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test2(Fake.Null<List<int?>>(), 10, (int?)10, (short)10),
				method.GenericArgumentsInferences(typeof(IList<int?>), typeof(int), typeof(int?), typeof(short))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test2(Fake.Null<List<long?[]>>(), Fake.Null<long?[]>(), Fake.Null<long?[]>()),
				method.GenericArgumentsInferences(typeof(IList<long?[]>), typeof(long?[]), typeof(long?[]))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test2(Fake.Null<List<long?[]>>(), Fake.Null<long?[][]>()),
				method.GenericArgumentsInferences(typeof(IList<long?[]>), typeof(long?[][]))
			);

			method = type.GetMethod("Test3")!;
			CollectionAssert.AreEqual(
				TestClass2.Test3(Fake.Null<IList<int>>(), Fake.Null<int[]>()),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int[]))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test3(Fake.Null<IList<int>>(), new List<int>()),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(List<int>))
			);

			method = type.GetMethod("Test4")!;
			CollectionAssert.AreEqual(
				TestClass2.Test4(Fake.Null<IList<int>>(), 10),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test4(Fake.Null<IList<int>>(), 10, (int?)10),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int), typeof(int?))
			);
			CollectionAssert.AreEqual(
				TestClass2.Test4(Fake.Null<IList<int>>(), 10, 10),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int), typeof(int))
			);

			method = type.GetMethod("Test5")!;
			CollectionAssert.AreEqual(
				TestClass2.Test5(10),
				method.GenericArgumentsInferences(typeof(int)));
			CollectionAssert.AreEqual(
				TestClass2.Test5(10U),
				method.GenericArgumentsInferences(typeof(uint)));

			method = type.GetMethod("Test6")!;
			int intValue = 10;
			CollectionAssert.AreEqual(
				TestClass2.Test6(ref intValue),
				method.GenericArgumentsInferences(typeof(int)));
			uint uintValue = 10;
			CollectionAssert.AreEqual(
				TestClass2.Test6(ref uintValue),
				method.GenericArgumentsInferences(typeof(uint)));

			method = type.GetMethod("Test7")!;
			CollectionAssert.AreEqual(
				TestClass2.Test7(10, 20),
				method.GenericArgumentsInferences(typeof(int), typeof(int)));
			CollectionAssert.AreEqual(
				TestClass2.Test7("123", new object()),
				method.GenericArgumentsInferences(typeof(string), typeof(object)));
			CollectionAssert.AreEqual(
				TestClass2.Test7(Fake.Null<int[]>(), Fake.Null<IList<int>>()),
				method.GenericArgumentsInferences(typeof(int[]), typeof(IList<int>)));
			CollectionAssert.AreEqual(
				TestClass2.Test7(Fake.Null<IList<int>>(), Fake.Null<int[]>()),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int[])));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(uint), typeof(string)));

			method = type.GetMethod("Test8")!;
			IList<int> intListValue = new List<int>();
			CollectionAssert.AreEqual(
				TestClass2.Test8(ref intValue, 10),
				method.GenericArgumentsInferences(typeof(int), typeof(int)));
			CollectionAssert.AreEqual(
				TestClass2.Test8(ref intListValue, Fake.Null<int[]>()),
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int[])));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(int[]), typeof(IList<int>)));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(string), typeof(object)));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(uint), typeof(string)));

			method = type.GetMethod("Test9")!;
			CollectionAssert.AreEqual(
				TestClass2.Test9(Fake.Null<int[]>(), 10),
				method.GenericArgumentsInferences(typeof(int[]), typeof(int)));
			CollectionAssert.AreEqual(
				TestClass2.Test9(Fake.Null<long[]>(), 10),
				method.GenericArgumentsInferences(typeof(long[]), typeof(int)));

			method = type.GetMethod("Test10")!;
			CollectionAssert.AreEqual(
				TestClass2.Test10(new List<IDictionary<int, string>>(), Fake.Null<IList<int[]>>()),
				method.GenericArgumentsInferences(typeof(List<IDictionary<int, string>>), typeof(IList<int[]>)));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<long[]>)));

			method = type.GetMethod("Test11")!;
			CollectionAssert.AreEqual(
				TestClass2.Test11(new List<IDictionary<int, string>>(), Fake.Null<IList<int[]>>()),
				method.GenericArgumentsInferences(typeof(List<IDictionary<int, string>>), typeof(IList<int[]>)));
			CollectionAssert.AreEqual(
				TestClass2.Test11(new List<Dictionary<int, string>>(), Fake.Null<IList<int[]>>()),
				method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<int[]>)));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<long[]>)));

			method = type.GetMethod("Test12")!;
			CollectionAssert.AreEqual(
				TestClass2.Test12(10, 10, 20, 30),
				method.GenericArgumentsInferences(typeof(int), typeof(int), typeof(int), typeof(int)));
			CollectionAssert.AreEqual(
				TestClass2.Test12("", "", new object(), new object()),
				method.GenericArgumentsInferences(typeof(string), typeof(string), typeof(object), typeof(object)));
			CollectionAssert.AreEqual(
				TestClass2.Test12("", Fake.Null<string[]>()),
				method.GenericArgumentsInferences(typeof(string), typeof(string[])));
			CollectionAssert.AreEqual(
				TestClass2.Test12(Fake.Null<string[]>(), Fake.Null<string[]>()),
				method.GenericArgumentsInferences(typeof(string[]), typeof(string[])));
			CollectionAssert.AreEqual(
				TestClass2.Test12(Fake.Null<string[][]>(), Fake.Null<string[][]>()),
				method.GenericArgumentsInferences(typeof(string[][]), typeof(string[][])));
			CollectionAssert.AreEqual(
				TestClass2.Test12(Fake.Null<string[]>(), Fake.Null<string[][]>()),
				method.GenericArgumentsInferences(typeof(string[]), typeof(string[][])));
			CollectionAssert.AreEqual(
				TestClass2.Test12("", ""),
				method.GenericArgumentsInferences(typeof(string), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test12(""),
				method.GenericArgumentsInferences(typeof(string)));

			method = type.GetMethod("Test13")!;
			CollectionAssert.AreEqual(
				TestClass2.Test13((int a) => "", 10, ""),
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(int), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((TestBaseClass a) => "", new TestSubClass(), ""),
				method.GenericArgumentsInferences(typeof(Func<TestBaseClass, string>), typeof(TestSubClass), typeof(string)));
			Assert.IsNull(method.GenericArgumentsInferences(typeof(Func<short, string>), typeof(int), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((object a) => "", "", ""),
				method.GenericArgumentsInferences(typeof(Func<object, string>), typeof(string), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((int a) => "", (short)10, ""),
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(short), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((int a) => "", (short)10, new object()),
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(short), typeof(object)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((IList<int> a) => "", Fake.Null<int[]>(), new object()),
				method.GenericArgumentsInferences(typeof(Func<IList<int>, string>), typeof(int[]), typeof(object)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((IList<int> a) => new object(), Fake.Null<int[]>(), ""),
				method.GenericArgumentsInferences(typeof(Func<IList<int>, object>), typeof(int[]), typeof(string)));
			CollectionAssert.AreEqual(
				TestClass2.Test13((IList<int> a) => Fake.Null<int[]>(), Fake.Null<int[]>(), intListValue),
				method.GenericArgumentsInferences(typeof(Func<IList<int>, int[]>), typeof(int[]), typeof(IList<int>)));
		}
	}
}

