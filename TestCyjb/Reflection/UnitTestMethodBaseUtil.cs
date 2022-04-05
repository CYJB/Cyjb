using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Cyjb;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Reflection
{
	/// <summary>
	/// <see cref="MethodBaseUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestMethodBaseUtil
	{
		private class TestClass
		{
			public string Value;
			[TestHint("Test1")]
			public TestClass() { Value = "123"; }
			[TestHint("Test2")]
			public TestClass(int a) { Value = a.ToString(); }
			[TestHint("Test3")]
			public TestClass(int[] b) { Value = (b?.Length ?? -1).ToString(); }
			[TestHint("Test4")]
			public TestClass(int a, string b) { Value = a.ToString() + b; }
			[TestHint("Test5")]
			public TestClass(int a, params int[] c) { Value = (a + c.Length).ToString(); }
			[TestHint("Test6")]
			public TestClass(short a, string b = "abc") { Value = a.ToString() + b; }
			[TestHint("Test7")]
			public TestClass(long a = 10, string b = "abc") { Value = a.ToString() + b; }
			[TestHint("Test8")]
			public TestClass(__arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				Value = text.ToString();
			}
			[TestHint("Test9")]
			public TestClass(string a, __arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(a);
				while (args.GetRemainingCount() > 0)
				{
					text.Append('_');
					text.Append(TypedReference.ToObject(args.GetNextArg()));
				}
				Value = text.ToString();
			}
			[TestHint("Test11")]
			public TestClass(string key, ref string value, out int value2)
			{
				string result = key + "_" + value;
				value = "xx";
				value2 = 103;
				Value = result;
			}

#pragma warning disable CA1822 // 将成员标记为 static
			public bool Test1() { return true; }
			public long Test2(int a) { return a; }
			public int Test3(int[] b) { return b?.Length ?? -1; }
			public string Test4(int a, string b) { return a.ToString() + b; }
			public int Test5(int a, params int[] c) { return a + c.Length; }
			public string Test6(int a, string b = "abc") { return a.ToString() + b; }
			public string Test7(int a = 10, string b = "abc") { return a.ToString() + b; }
			public string Test8(__arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				return text.ToString();
			}
			public string Test9(string a, __arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(a);
				while (args.GetRemainingCount() > 0)
				{
					text.Append('_');
					text.Append(TypedReference.ToObject(args.GetNextArg()));
				}
				return text.ToString();
			}
			public string Test10<T>(T key)
			{
				return "<" + typeof(T) + ">" + key;
			}
			public string Test11(string key, ref string value, out int value2)
			{
				string result = key + "_" + value;
				value = "xx";
				value2 = 101;
				return result;
			}
#pragma warning restore CA1822 // 将成员标记为 static

			public static bool STest1() { return true; }
			public static long STest2(int a) { return a; }
			public static int STest3(int[] b) { return b?.Length ?? -1; }
			public static string STest4(int a, string b) { return a.ToString() + b; }
			public static int STest5(int a, params int[] c) { return a + c.Length; }
			public static string STest6(int a, string b = "abc") { return a.ToString() + b; }
			public static string STest7(int a = 10, string b = "abc") { return a.ToString() + b; }
			public static string STest8(__arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				return text.ToString();
			}
			public static string STest9(string a, __arglist)
			{
				ArgIterator args = new(__arglist);
				StringBuilder text = new(a);
				while (args.GetRemainingCount() > 0)
				{
					text.Append('_');
					text.Append(TypedReference.ToObject(args.GetNextArg()));
				}
				return text.ToString();
			}
			public static string STest10<T>(T key)
			{
				return "<" + typeof(T) + ">" + key;
			}
			public static string STest11(string key, ref string value, out int value2)
			{
				string result = key + "_" + value;
				value = "xx";
				value2 = 102;
				return result;
			}
		}

		private delegate string TestDelegate1(TestClass instance, string key, ref string value, out int value2);
		private delegate string TestDelegate2(string key, ref string value, out int value2);
		private delegate TestClass TestDelegate3(string key, ref string value, out int value2);
		private delegate string TestDelegate4(ref string value, out int value2);
		private delegate TestClass TestDelegate5(ref string value, out int value2);

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.GetParameterTypes"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1")]
		[DataRow("Test2", typeof(int))]
		[DataRow("Test3", typeof(int[]))]
		[DataRow("Test4", typeof(int), typeof(string))]
		[DataRow("Test5", typeof(int), typeof(int[]))]
		[DataRow("Test6", typeof(int), typeof(string))]
		[DataRow("Test7", typeof(int), typeof(string))]
		[DataRow("Test8")]
		[DataRow("Test9", typeof(string))]
		public void TestGetParameterTypes(string methodName, params Type[] types)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			CollectionAssert.AreEqual(types, methodInfo.GetParameterTypes());
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type)"/> 创建实例方法委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(Func<TestClass, bool>), true)]
		[DataRow("Test1", typeof(Func<object, object>), true)]
		[DataRow("Test2", typeof(Func<TestClass, int, long>), 100, 100L)]
		[DataRow("Test2", typeof(Func<TestClass, short, int>), (short)10, 10)]
		[DataRow("Test2", typeof(Func<TestClass, object, object>), 100, 100L)]
		[DataRow("Test3", typeof(Func<TestClass, int[], int>), null, -1)]
		[DataRow("Test3", typeof(Func<TestClass, int[], int>), new int[] { 10, 20 }, 2)]
		[DataRow("Test4", typeof(Func<TestClass, long, string, string>), 2, "02", "202")]
		[DataRow("Test4", typeof(Func<TestClass, int?, object, object>), 20, "3", "203")]
		[DataRow("Test5", typeof(Func<TestClass, int, int>), 2, 2)]
		[DataRow("Test5", typeof(Func<TestClass, int, int, int>), 2, 0, 3)]
		[DataRow("Test5", typeof(Func<TestClass, int, int, long, uint, int>), 2, 0, 40, 100U, 5)]
		[DataRow("Test5", typeof(Func<TestClass, int, int, long, uint, int?, int>), 2, 0, 40, 100U, 230, 6)]
		[DataRow("Test6", typeof(Func<TestClass, int, string>), 2, "2abc")]
		[DataRow("Test6", typeof(Func<TestClass, int, string, string>), 2, "3", "23")]
		[DataRow("Test7", typeof(Func<TestClass, string>), "10abc")]
		[DataRow("Test8", typeof(Func<TestClass, string>), "")]
		[DataRow("Test8", typeof(Func<TestClass, int, string, string>), 10, "cd", "10_cd_")]
		[DataRow("Test9", typeof(Func<TestClass, string, string>), "cd", "cd")]
		[DataRow("Test9", typeof(Func<TestClass, string, int, long?, string, string>), "x", 10, 20L, "y", "x_10_20_y")]
		[DataRow("Test9", typeof(Func<TestClass, string, int, long?, string, string>), "x", 10, null, "s", "x_10__s")]
		[DataRow("Test10", typeof(Func<TestClass, string, string>), "cd", "<System.String>cd")]
		[DataRow("Test10", typeof(Func<TestClass, int?, string>), 20, "<System.Nullable`1[System.Int32]>20")]
		[DataRow("Test11", typeof(Func<TestClass, string, string, int, string>), "a", "b", 20, "a_b")]
		public void TestCreateOpenInstanceDelegate(string methodName, Type delegateType, params object?[] args)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			Delegate? dlg = methodInfo.PowerDelegate(delegateType);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			// 实例方法的第一个参数的实例。
			for (int i = args.Length - 1; i > 0; i--)
			{
				args[i] = args[i - 1];
			}
			args[0] = new TestClass();
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type)"/> 创建静态方法委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("STest1", typeof(Func<bool>), true)]
		[DataRow("STest1", typeof(Func<object>), true)]
		[DataRow("STest2", typeof(Func<int, long>), 100, 100L)]
		[DataRow("STest2", typeof(Func<short, int>), (short)10, 10)]
		[DataRow("STest2", typeof(Func<object, object>), 100, 100L)]
		[DataRow("STest3", typeof(Func<int[], int>), null, -1)]
		[DataRow("STest3", typeof(Func<int[], int>), new int[] { 10, 20 }, 2)]
		[DataRow("STest4", typeof(Func<long, string, string>), 2, "02", "202")]
		[DataRow("STest4", typeof(Func<int?, object, object>), 20, "3", "203")]
		[DataRow("STest5", typeof(Func<int, int>), 2, 2)]
		[DataRow("STest5", typeof(Func<int, int, int>), 2, 0, 3)]
		[DataRow("STest5", typeof(Func<int, int, long, uint, int>), 2, 0, 40, 100U, 5)]
		[DataRow("STest5", typeof(Func<int, int, long, uint, int?, int>), 2, 0, 40, 100U, 230, 6)]
		[DataRow("STest6", typeof(Func<int, string>), 2, "2abc")]
		[DataRow("STest6", typeof(Func<int, string, string>), 2, "3", "23")]
		[DataRow("STest7", typeof(Func<string>), "10abc")]
		[DataRow("STest8", typeof(Func<string>), "")]
		[DataRow("STest8", typeof(Func<int, string, string>), 10, "cd", "10_cd_")]
		[DataRow("STest9", typeof(Func<string, string>), "cd", "cd")]
		[DataRow("STest9", typeof(Func<string, int, long?, string, string>), "x", 10, 20L, "y", "x_10_20_y")]
		[DataRow("STest9", typeof(Func<string, int, long?, string, string>), "x", 10, null, "s", "x_10__s")]
		[DataRow("STest10", typeof(Func<string, string>), "cd", "<System.String>cd")]
		[DataRow("STest10", typeof(Func<int?, string>), 20, "<System.Nullable`1[System.Int32]>20")]
		[DataRow("STest11", typeof(Func<string, string, int, string>), "a", "b", 20, "a_b")]
		public void TestCreateOpenStaticDelegate(string methodName, Type delegateType, params object?[] args)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			Delegate? dlg = methodInfo.PowerDelegate(delegateType);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type, object?)"/> 创建实例方法委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(Func<bool>), true)]
		[DataRow("Test1", typeof(Func<object>), true)]
		[DataRow("Test2", typeof(Func<int, long>), 100, 100L)]
		[DataRow("Test2", typeof(Func<short, int>), (short)10, 10)]
		[DataRow("Test2", typeof(Func<object, object>), 100, 100L)]
		[DataRow("Test3", typeof(Func<int[], int>), null, -1)]
		[DataRow("Test3", typeof(Func<int[], int>), new int[] { 10, 20 }, 2)]
		[DataRow("Test4", typeof(Func<long, string, string>), 2, "02", "202")]
		[DataRow("Test4", typeof(Func<int?, object, object>), 20, "3", "203")]
		[DataRow("Test5", typeof(Func<int, int>), 2, 2)]
		[DataRow("Test5", typeof(Func<int, int, int>), 2, 0, 3)]
		[DataRow("Test5", typeof(Func<int, int, long, uint, int>), 2, 0, 40, 100U, 5)]
		[DataRow("Test5", typeof(Func<int, int, long, uint, int?, int>), 2, 0, 40, 100U, 230, 6)]
		[DataRow("Test6", typeof(Func<int, string>), 2, "2abc")]
		[DataRow("Test6", typeof(Func<int, string, string>), 2, "3", "23")]
		[DataRow("Test7", typeof(Func<string>), "10abc")]
		[DataRow("Test8", typeof(Func<string>), "")]
		[DataRow("Test8", typeof(Func<int, string, string>), 10, "cd", "10_cd_")]
		[DataRow("Test9", typeof(Func<string, string>), "cd", "cd")]
		[DataRow("Test9", typeof(Func<string, int, long?, string, string>), "x", 10, 20L, "y", "x_10_20_y")]
		[DataRow("Test9", typeof(Func<string, int, long?, string, string>), "x", 10, null, "s", "x_10__s")]
		[DataRow("Test10", typeof(Func<string, string>), "cd", "<System.String>cd")]
		[DataRow("Test10", typeof(Func<int?, string>), 20, "<System.Nullable`1[System.Int32]>20")]
		[DataRow("Test11", typeof(Func<string, string, int, string>), "a", "b", 20, "a_b")]
		public void TestCreateClosedInstanceDelegate(string methodName, Type delegateType, params object?[] args)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			Delegate? dlg = methodInfo.PowerDelegate(delegateType, new TestClass());
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type, object?)"/> 创建静态方法委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("STest2", typeof(Func<long>), 100, 100L)]
		[DataRow("STest2", typeof(Func<int>), (short)10, 10)]
		[DataRow("STest2", typeof(Func<int>), 10L, 10)]
		[DataRow("STest2", typeof(Func<object>), 100, 100L)]
		[DataRow("STest3", typeof(Func<int>), null, -1)]
		[DataRow("STest3", typeof(Func<int>), new int[] { 10, 20 }, 2)]
		[DataRow("STest4", typeof(Func<string, string>), 2L, "02", "202")]
		[DataRow("STest4", typeof(Func<object, object>), (char)20, "3", "203")]
		[DataRow("STest5", typeof(Func<int>), 2, 2)]
		[DataRow("STest5", typeof(Func<int, int>), 2, 0, 3)]
		[DataRow("STest5", typeof(Func<int, long, uint, int>), 2, 0, 40, 100U, 5)]
		[DataRow("STest5", typeof(Func<int, long, uint, int?, int>), 2, 0, 40, 100U, 230, 6)]
		[DataRow("STest6", typeof(Func<string>), 2, "2abc")]
		[DataRow("STest6", typeof(Func<string, string>), 2, "3", "23")]
		[DataRow("STest7", typeof(Func<string>), 20, "20abc")]
		[DataRow("STest8", typeof(Func<string>), "S", "S_")]
		[DataRow("STest8", typeof(Func<string, string>), 10, "cd", "10_cd_")]
		[DataRow("STest9", typeof(Func<string>), "cd", "cd")]
		[DataRow("STest9", typeof(Func<int, long?, string, string>), "x", 10, 20L, "y", "x_10_20_y")]
		[DataRow("STest9", typeof(Func<int, long?, string, string>), "x", 10, null, "s", "x_10__s")]
		[DataRow("STest10", typeof(Func<string>), "cd", "<System.String>cd")]
		[DataRow("STest10", typeof(Func<string>), null, "<System.Object>")]
		[DataRow("STest10", typeof(Func<string>), 20, "<System.Int32>20")]
		[DataRow("STest11", typeof(Func<string, int, string>), "a", "b", 20, "a_b")]
		public void TestClosedStaticDelegate(string methodName, Type delegateType, object? firstArgument, params object?[] args)
		{
			MethodInfo methodInfo = typeof(TestClass).GetMethod(methodName)!;
			Delegate? dlg = methodInfo.PowerDelegate(delegateType, firstArgument);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type)"/> 创建构造函数委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(Func<object>), "123")]
		[DataRow("Test2", typeof(Func<int, TestClass>), 100, "100")]
		[DataRow("Test2", typeof(Func<short, TestClass>), (short)10, "10")]
		[DataRow("Test2", typeof(Func<object, TestClass>), 100, "100")]
		[DataRow("Test3", typeof(Func<int[], TestClass>), null, "-1")]
		[DataRow("Test3", typeof(Func<int[], TestClass>), new int[] { 10, 20 }, "2")]
		[DataRow("Test4", typeof(Func<long, string, TestClass>), 2, "02", "202")]
		[DataRow("Test4", typeof(Func<int?, object, TestClass>), 20, "3", "203")]
		[DataRow("Test5", typeof(Func<int, TestClass>), 2, "2")]
		[DataRow("Test5", typeof(Func<int, int, TestClass>), 2, 0, "3")]
		[DataRow("Test5", typeof(Func<int, int, long, uint, TestClass>), 2, 0, 40, 100U, "5")]
		[DataRow("Test5", typeof(Func<int, int, long, uint, int?, TestClass>), 2, 0, 40, 100U, 230, "6")]
		[DataRow("Test6", typeof(Func<int, TestClass>), 2, "2abc")]
		[DataRow("Test6", typeof(Func<int, string, TestClass>), 2, "3", "23")]
		[DataRow("Test7", typeof(Func<TestClass>), "10abc")]
		// 目前构造函数不支持传入可变参数。
		[DataRow("Test8", typeof(Func<TestClass>), "")]
		[DataRow("Test9", typeof(Func<string, TestClass>), "cd", "cd")]
		[DataRow("Test11", typeof(Func<string, string, int, TestClass>), "a", "b", 20, "a_b")]
		public void TestCreateOpenConstructorDelegate(string hint, Type delegateType, params object?[] args)
		{
			ConstructorInfo constructorInfo = typeof(TestClass).GetConstructors().First(ctor =>
			{
				return ctor.GetCustomAttribute<TestHintAttribute>()?.Hint == hint;
			});
			Delegate? dlg = constructorInfo.PowerDelegate(delegateType);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, ((TestClass)dlg.DynamicInvoke(args)!).Value);
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate(MethodBase, Type, object?)"/> 创建构造函数委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test2", typeof(Func<TestClass>), 100, "100")]
		[DataRow("Test2", typeof(Func<TestClass>), (short)10, "10")]
		[DataRow("Test2", typeof(Func<TestClass>), 100, "100")]
		[DataRow("Test3", typeof(Func<TestClass>), null, "-1")]
		[DataRow("Test3", typeof(Func<TestClass>), new int[] { 10, 20 }, "2")]
		[DataRow("Test4", typeof(Func<string, TestClass>), 2, "02", "202")]
		[DataRow("Test4", typeof(Func<object, TestClass>), 20, "3", "203")]
		[DataRow("Test5", typeof(Func<TestClass>), 2, "2")]
		[DataRow("Test5", typeof(Func<int, TestClass>), 2, 0, "3")]
		[DataRow("Test5", typeof(Func<int, long, uint, TestClass>), 2, 0, 40, 100U, "5")]
		[DataRow("Test5", typeof(Func<int, long, uint, int?, TestClass>), 2, 0, 40, 100U, 230, "6")]
		[DataRow("Test6", typeof(Func<TestClass>), 2, "2abc")]
		[DataRow("Test6", typeof(Func<string, TestClass>), 2, "3", "23")]
		[DataRow("Test7", typeof(Func<TestClass>), 20, "20abc")]
		// 目前构造函数不支持传入可变参数。
		[DataRow("Test9", typeof(Func<TestClass>), "cd", "cd")]
		[DataRow("Test11", typeof(Func<string, int, TestClass>), "a", "b", 20, "a_b")]
		public void TestCreateClosedConstructorDelegate(string hint, Type delegateType, object? firstArgument, params object?[] args)
		{
			ConstructorInfo constructorInfo = typeof(TestClass).GetConstructors().First(ctor =>
			{
				return ctor.GetCustomAttribute<TestHintAttribute>()?.Hint == hint;
			});
			Delegate? dlg = constructorInfo.PowerDelegate(delegateType, firstArgument);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, ((TestClass)dlg.DynamicInvoke(args)!).Value);
		}

		/// <summary>
		/// 对 <see cref="MethodBaseUtil.PowerDelegate"/> 创建引用参数委托进行测试。
		/// </summary>
		[TestMethod]
		public void TestRef()
		{
			// 开放实例方法
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod("Test11")!;
				TestDelegate1? dlg = methodInfo.PowerDelegate<TestDelegate1>();
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg(new TestClass(), "abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(101, value2);
			}

			// 开放静态方法
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod("STest11")!;
				TestDelegate2? dlg = methodInfo.PowerDelegate<TestDelegate2>();
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(102, value2);
			}

			// 开放构造函数
			{
				ConstructorInfo ctor = typeof(TestClass).GetConstructor(
					new Type[] { typeof(string), typeof(string).MakeByRefType(), typeof(int).MakeByRefType() })!;
				TestDelegate3? dlg = ctor.PowerDelegate<TestDelegate3>();
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2).Value);
				Assert.AreEqual("xx", value);
				Assert.AreEqual(103, value2);
			}

			// 封闭实例方法
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod("Test11")!;
				TestDelegate2? dlg = methodInfo.PowerDelegate<TestDelegate2>(new TestClass());
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(101, value2);
			}

			// 封闭静态方法
			{
				MethodInfo methodInfo = typeof(TestClass).GetMethod("STest11")!;
				TestDelegate4? dlg5 = methodInfo.PowerDelegate<TestDelegate4>("abc");
				Assert.IsNotNull(dlg5);
				string value = "X";
				Assert.AreEqual("abc_X", dlg5(ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(102, value2);
			}

			// 封闭构造函数
			{
				ConstructorInfo ctor = typeof(TestClass).GetConstructor(
					new Type[] { typeof(string), typeof(string).MakeByRefType(), typeof(int).MakeByRefType() })!;
				TestDelegate5? dlg = ctor.PowerDelegate<TestDelegate5>("abc");
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg(ref value, out int value2).Value);
				Assert.AreEqual("xx", value);
				Assert.AreEqual(103, value2);
			}
		}
	}
}

