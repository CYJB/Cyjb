using System;
using System.Collections.Generic;
using System.Reflection;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.Reflection
{
	/// <summary>
	/// <see cref="MethodExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestMethodExt
	{
		/// <summary>
		/// 对 <c>MethodExt.GenericArgumentsInferences</c> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGenericArgumentsInferences()
		{
			Type type = typeof(TestClass);
			MethodInfo method = type.GetMethod("TestMethod");
			AssertExt.AreEqual(new[] { typeof(int) }, method.GenericArgumentsInferences(typeof(int)));
			AssertExt.AreEqual(new[] { typeof(uint) }, method.GenericArgumentsInferences(typeof(uint)));
			method = type.GetMethod("TestMethod2");
			AssertExt.AreEqual(new[] { typeof(int) }, method.GenericArgumentsInferences(typeof(int)));
			AssertExt.AreEqual(new[] { typeof(uint) }, method.GenericArgumentsInferences(typeof(uint)));
			method = type.GetMethod("TestMethod3");
			AssertExt.AreEqual(new[] { typeof(int) },
				method.GenericArgumentsInferences(typeof(int), typeof(int)));
			AssertExt.AreEqual(new[] { typeof(object) },
				method.GenericArgumentsInferences(typeof(string), typeof(object)));
			AssertExt.AreEqual(new[] { typeof(IList<int>) },
				method.GenericArgumentsInferences(typeof(int[]), typeof(IList<int>)));
			AssertExt.AreEqual(new[] { typeof(IList<int>) },
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int[])));
			AssertExt.AreEqual(null, method.GenericArgumentsInferences(typeof(uint), typeof(string)));
			method = type.GetMethod("TestMethod4");
			AssertExt.AreEqual(new[] { typeof(int) },
				method.GenericArgumentsInferences(typeof(int), typeof(int)));
			AssertExt.AreEqual(new[] { typeof(IList<int>) },
				method.GenericArgumentsInferences(typeof(IList<int>), typeof(int[])));
			AssertExt.AreEqual(null, method.GenericArgumentsInferences(typeof(int[]), typeof(IList<int>)));
			AssertExt.AreEqual(null, method.GenericArgumentsInferences(typeof(string), typeof(object)));
			AssertExt.AreEqual(null, method.GenericArgumentsInferences(typeof(uint), typeof(string)));
			method = type.GetMethod("TestMethod5");
			AssertExt.AreEqual(new[] { typeof(int) }, method.GenericArgumentsInferences(typeof(int[]), typeof(int)));
			AssertExt.AreEqual(new[] { typeof(long) }, method.GenericArgumentsInferences(typeof(long[]), typeof(int)));
			method = type.GetMethod("TestMethod6");
			AssertExt.AreEqual(new[] { typeof(int), typeof(string) },
				method.GenericArgumentsInferences(typeof(List<IDictionary<int, string>>), typeof(IList<int[]>)));
			AssertExt.AreEqual(null,
				method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<long[]>)));
			method = type.GetMethod("TestMethod7");
			AssertExt.AreEqual(new[] { typeof(int), typeof(string) },
				method.GenericArgumentsInferences(typeof(List<IDictionary<int, string>>), typeof(IList<int[]>)));
			AssertExt.AreEqual(new[] { typeof(int), typeof(string) },
				method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<int[]>)));
			AssertExt.AreEqual(null,
				method.GenericArgumentsInferences(typeof(List<Dictionary<int, string>>), typeof(IList<long[]>)));
			method = type.GetMethod("TestMethod8");
			AssertExt.AreEqual(new[] { typeof(int) },
				method.GenericArgumentsInferences(typeof(int), typeof(int), typeof(int), typeof(int)));
			AssertExt.AreEqual(new[] { typeof(object) },
				method.GenericArgumentsInferences(typeof(string), typeof(string), typeof(object), typeof(object)));
			AssertExt.AreEqual(new[] { typeof(string) },
				method.GenericArgumentsInferences(typeof(string), typeof(string[])));
			AssertExt.AreEqual(new[] { typeof(string[]) },
				method.GenericArgumentsInferences(typeof(string[]), typeof(string[])));
			AssertExt.AreEqual(new[] { typeof(string[][]) },
				method.GenericArgumentsInferences(typeof(string[][]), typeof(string[][])));
			AssertExt.AreEqual(new[] { typeof(string[]) },
				method.GenericArgumentsInferences(typeof(string[]), typeof(string[][])));
			AssertExt.AreEqual(new[] { typeof(string) },
				method.GenericArgumentsInferences(typeof(string), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(string) }, method.GenericArgumentsInferences(typeof(string)));
			method = type.GetMethod("TestMethod9");
			AssertExt.AreEqual(new[] { typeof(int), typeof(string) },
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(int), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(TestClass2), typeof(string) },
				method.GenericArgumentsInferences(typeof(Func<TestClass2, string>), typeof(TestClass3), typeof(string)));
			AssertExt.AreEqual(null,
				method.GenericArgumentsInferences(typeof(Func<short, string>), typeof(int), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(object), typeof(string) },
				method.GenericArgumentsInferences(typeof(Func<object, string>), typeof(string), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(int), typeof(string) },
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(short), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(int), typeof(object) },
				method.GenericArgumentsInferences(typeof(Func<int, string>), typeof(short), typeof(object)));
			AssertExt.AreEqual(new[] { typeof(IList<int>), typeof(object) },
				method.GenericArgumentsInferences(typeof(Func<IList<int>, string>), typeof(int[]), typeof(object)));
			AssertExt.AreEqual(new[] { typeof(IList<int>), typeof(object) },
				method.GenericArgumentsInferences(typeof(Func<IList<int>, object>), typeof(int[]), typeof(string)));
			AssertExt.AreEqual(new[] { typeof(IList<int>), typeof(IList<int>) },
				method.GenericArgumentsInferences(typeof(Func<IList<int>, int[]>), typeof(int[]), typeof(IList<int>)));
		}
		private class TestClass
		{
			public static void TestMethod<T>(T arg) { }
			public static void TestMethod2<T>(ref T arg) { }
			public static void TestMethod3<T>(T arg, T arg2) { }
			public static void TestMethod4<T>(ref T arg, T arg2) { }
			public static void TestMethod5<T>(IList<T> arg, T arg2) { }
			public static void TestMethod6<T1, T2>(IList<IDictionary<T1, T2>> arg, IList<T1[]> arg2) { }
			public static void TestMethod7<T1, T2>(IEnumerable<IDictionary<T1, T2>> arg, IList<T1[]> arg2) { }
			public static void TestMethod8<T>(T a, params T[] b) { }
			public static void TestMethod9<T1, T2>(Func<T1, T2> arg, T1 a, T2 b) { }
		}
		private class TestClass2 { }
		private class TestClass3 : TestClass2 { }
	}
}
