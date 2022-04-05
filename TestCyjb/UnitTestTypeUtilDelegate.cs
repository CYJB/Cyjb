using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="TypeUtil"/> 类创建委托的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeUtilDelegate
	{
		private class TestClass
		{
			public string Value;
			public TestClass() { Value = "123"; }
			public TestClass(int a) { Value = a.ToString(); }
			public TestClass(int[] b) { Value = (b?.Length ?? -1).ToString(); }
			public TestClass(int a, string b) { Value = a.ToString() + b; }
			public TestClass(int a, params int[] c) { Value = (a + c.Length).ToString(); }
			public TestClass(short a, string b = "abc") { Value = a.ToString() + b; }
			public TestClass(long a = 10, string b = "abc") { Value = a.ToString() + b; }
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
			public static string? SPTest1 { get; set; }
			public static int SPTest2 { get; set; }
			public static int? SPTest3 { get; set; }

			public string? PTest1 { get; set; }
			public int PTest2 { get; set; }
			public int? PTest3 { get; set; }
			private readonly string[] items = new string[10];
			public string this[int index]
			{
				get { return items[index]; }
				set { items[index] = value; }
			}
			public string this[int index, int index2]
			{
				get { return items[index + index2]; }
				set { items[index + index2] = value; }
			}
			public string this[int index, string index2]
			{
				get { return items[index + int.Parse(index2)]; }
				set { items[index + int.Parse(index2)] = value; }
			}
			public static void ResetStaticProperties()
			{
				SPTest1 = "";
				SPTest2 = 0;
				SPTest3 = null;
			}

			public string? FTest1 = "";
			public int FTest2 = 0;
			public int? FTest3 = null;

			public static string? SFTest1 = "";
			public static int SFTest2 = 0;
			public static int? SFTest3 = null;

			public static void ResetStaticFields()
			{
				SFTest1 = "";
				SFTest2 = 0;
				SFTest3 = null;
			}
		}

		private delegate string TestDelegate1(TestClass instance, string key, ref string value, out int value2);
		private delegate string TestDelegate2(string key, ref string value, out int value2);
		private delegate TestClass TestDelegate3(string key, ref string value, out int value2);
		private delegate string TestDelegate4(ref string value, out int value2);
		private delegate TestClass TestDelegate5(ref string value, out int value2);

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建实例方法委托进行测试。
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
		public void TestCreateOpenInstanceMethodDelegate(string methodName, Type delegateType, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType);
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
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建静态方法委托进行测试。
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
		public void TestCreateOpenStaticMethodDelegate(string methodName, Type delegateType, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建实例方法委托进行测试。
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
		public void TestCreateClosedInstancMethodeDelegate(string methodName, Type delegateType, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType, new TestClass());
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建静态方法委托进行测试。
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
		public void TestClosedStaticMethodDelegate(string methodName, Type delegateType, object? firstArgument, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType, firstArgument);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, dlg.DynamicInvoke(args));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建构造函数委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(".ctor", typeof(Func<int, TestClass>), 100, "100")]
		[DataRow(".ctor", typeof(Func<short, TestClass>), (short)10, "10abc")]
		[DataRow(".ctor", typeof(Func<object, TestClass>), new int[0], "0")]
		[DataRow(".ctor", typeof(Func<int[], TestClass>), null, "-1")]
		[DataRow(".ctor", typeof(Func<int[], TestClass>), new int[] { 10, 20 }, "2")]
		[DataRow(".ctor", typeof(Func<long, string, TestClass>), 2, "02", "202")]
		[DataRow(".ctor", typeof(Func<int?, object, TestClass>), 20, "3", "203")]
		[DataRow(".ctor", typeof(Func<int, TestClass>), 2, "2")]
		[DataRow(".ctor", typeof(Func<int, int, TestClass>), 2, 0, "3")]
		[DataRow(".ctor", typeof(Func<int, int, long, uint, TestClass>), 2, 0, 40, 100U, "5")]
		[DataRow(".ctor", typeof(Func<int, int, long, uint, int?, TestClass>), 2, 0, 40, 100U, 230, "6")]
		[DataRow(".ctor", typeof(Func<int, TestClass>), 2, "2")]
		[DataRow(".ctor", typeof(Func<int, string, TestClass>), 2, "3", "23")]
		[DataRow(".ctor", typeof(Func<string, string, int, TestClass>), "a", "b", 20, "a_b")]
		public void TestCreateOpenConstructorDelegate(string methodName, Type delegateType, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, ((TestClass)dlg.DynamicInvoke(args)!).Value);
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建构造函数委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow(".ctor", typeof(Func<TestClass>), 100, "100")]
		[DataRow(".ctor", typeof(Func<TestClass>), (short)10, "10abc")]
		[DataRow(".ctor", typeof(Func<TestClass>), null, "-1")]
		[DataRow(".ctor", typeof(Func<TestClass>), new int[] { 10, 20 }, "2")]
		[DataRow(".ctor", typeof(Func<string, TestClass>), 2, "02", "202")]
		[DataRow(".ctor", typeof(Func<TestClass>), 2, "2")]
		[DataRow(".ctor", typeof(Func<int, TestClass>), 2, 0, "3")]
		[DataRow(".ctor", typeof(Func<int, long, uint, TestClass>), 2, 0, 40, 100U, "5")]
		[DataRow(".ctor", typeof(Func<int, long, uint, int?, TestClass>), 2, 0, 40, 100U, 230, "6")]
		[DataRow(".ctor", typeof(Func<string, TestClass>), 2, "3", "23")]
		[DataRow(".ctor", typeof(Func<TestClass>), 20, "20")]
		[DataRow(".ctor", typeof(Func<string, int, TestClass>), "a", "b", 20, "a_b")]
		public void TestCreateClosedConstructorDelegate(string methodName, Type delegateType, object? firstArgument, params object?[] args)
		{
			Delegate? dlg = typeof(TestClass).PowerDelegate(methodName, delegateType, firstArgument);
			Assert.IsNotNull(dlg);
			// 最后一个参数作为预期值。
			object? expected = args[^1];
			args = args.Resize(args.Length - 1);
			Assert.AreEqual(expected, ((TestClass)dlg.DynamicInvoke(args)!).Value);
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate"/> 创建引用参数委托进行测试。
		/// </summary>
		[TestMethod]
		public void TestRefMethod()
		{
			// 开放实例方法
			{
				TestDelegate1? dlg = typeof(TestClass).PowerDelegate<TestDelegate1>("Test11");
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg(new TestClass(), "abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(101, value2);
			}

			// 开放静态方法
			{
				TestDelegate2? dlg = typeof(TestClass).PowerDelegate<TestDelegate2>("STest11");
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(102, value2);
			}

			// 开放构造函数
			{
				TestDelegate3? dlg = typeof(TestClass).PowerDelegate<TestDelegate3>(".ctor", BindingFlags.CreateInstance);
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2).Value);
				Assert.AreEqual("xx", value);
				Assert.AreEqual(103, value2);
			}

			// 封闭实例方法
			{
				TestDelegate2? dlg = typeof(TestClass).PowerDelegate<TestDelegate2>("Test11", new TestClass());
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg("abc", ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(101, value2);
			}

			// 封闭静态方法
			{
				TestDelegate4? dlg = typeof(TestClass).PowerDelegate<TestDelegate4>("STest11", "abc");
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg(ref value, out int value2));
				Assert.AreEqual("xx", value);
				Assert.AreEqual(102, value2);
			}

			// 封闭构造函数
			{
				TestDelegate5? dlg = typeof(TestClass).PowerDelegate<TestDelegate5>(".ctor", "abc", BindingFlags.CreateInstance);
				Assert.IsNotNull(dlg);
				string value = "X";
				Assert.AreEqual("abc_X", dlg(ref value, out int value2).Value);
				Assert.AreEqual("xx", value);
				Assert.AreEqual(103, value2);
			}
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("PTest1", typeof(string), "123")]
		[DataRow("PTest1", typeof(object), "456")]
		[DataRow("PTest2", typeof(int), 100)]
		[DataRow("PTest2", typeof(long), 100L)]
		[DataRow("PTest2", typeof(char), (char)100)]
		[DataRow("PTest3", typeof(int), 100)]
		[DataRow("PTest3", typeof(long), 100L)]
		[DataRow("PTest3", typeof(char), (char)100)]
		public void TestCreateOpenInstancePropertyDelegate(string propertyName, Type valueType, object? value)
		{
			Delegate? setProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Action<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Func<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(getProperty);
			TestClass instance = new();
			setProperty.DynamicInvoke(instance, value);
			Assert.AreEqual(value, getProperty.DynamicInvoke(instance));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("SPTest1", typeof(string), "123")]
		[DataRow("SPTest1", typeof(object), "456")]
		[DataRow("SPTest2", typeof(int), 100)]
		[DataRow("SPTest2", typeof(long), 100L)]
		[DataRow("SPTest2", typeof(char), (char)100)]
		[DataRow("SPTest3", typeof(int), 100)]
		[DataRow("SPTest3", typeof(long), 100L)]
		[DataRow("SPTest3", typeof(char), (char)100)]
		public void TestCreateOpenStaticPropertyDelegate(string propertyName, Type valueType, object? value)
		{
			Delegate? setProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Action<>).MakeGenericType(valueType));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getProperty);
			TestClass.ResetStaticProperties();
			setProperty.DynamicInvoke(value);
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("PTest1", typeof(string), "123")]
		[DataRow("PTest1", typeof(object), "456")]
		[DataRow("PTest2", typeof(int), 100)]
		[DataRow("PTest2", typeof(long), 100L)]
		[DataRow("PTest2", typeof(char), (char)100)]
		[DataRow("PTest3", typeof(int), 100)]
		[DataRow("PTest3", typeof(long), 100L)]
		[DataRow("PTest3", typeof(char), (char)100)]
		public void TestCreateClosedInstancePropertyDelegate(string propertyName, Type valueType, object? value)
		{
			TestClass instance = new();
			Delegate? setProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Action<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Func<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(value);
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("SPTest1", typeof(string), "123")]
		[DataRow("SPTest1", typeof(object), "456")]
		[DataRow("SPTest2", typeof(int), 100)]
		[DataRow("SPTest2", typeof(long), 100L)]
		[DataRow("SPTest2", typeof(char), (char)100)]
		[DataRow("SPTest3", typeof(int), 100)]
		[DataRow("SPTest3", typeof(long), 100L)]
		[DataRow("SPTest3", typeof(char), (char)100)]
		public void TestClosedStaticPropertyDelegate(string propertyName, Type valueType, object? value)
		{
			Delegate? setProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Action), value);
			Assert.IsNotNull(setProperty);
			// 封闭委托不支持获取属性
			Delegate? getProperty = typeof(TestClass).PowerDelegate(propertyName, typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getProperty);
			TestClass.ResetStaticProperties();
			setProperty.DynamicInvoke();
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate"/> 创建索引参数委托进行测试。
		/// </summary>
		[TestMethod]
		public void TestItemProperty()
		{
			Delegate? setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<TestClass, int, string>));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<TestClass, int, string>));
			Assert.IsNotNull(getProperty);
			TestClass instance = new();
			setProperty.DynamicInvoke(instance, 1, "123");
			Assert.AreEqual("123", getProperty.DynamicInvoke(instance, 1));

			setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<int, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<int, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(2, "456");
			Assert.AreEqual("456", getProperty.DynamicInvoke(2));

			setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<TestClass, int, int, string>));
			Assert.IsNotNull(setProperty);
			getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<TestClass, int, int, string>));
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(instance, 1, 2, "789");
			Assert.AreEqual("789", getProperty.DynamicInvoke(instance, 1, 2));

			setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<int, int, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<int, int, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(3, 4, "10");
			Assert.AreEqual("10", getProperty.DynamicInvoke(3, 4));

			setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<TestClass, int, string, string>));
			Assert.IsNotNull(setProperty);
			getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<TestClass, int, string, string>));
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(instance, 1, "4", "11");
			Assert.AreEqual("11", getProperty.DynamicInvoke(instance, 1, "4"));

			setProperty = typeof(TestClass).PowerDelegate("Item", typeof(Action<int, string, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = typeof(TestClass).PowerDelegate("Item", typeof(Func<int, string, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(3, "5", "12");
			Assert.AreEqual("12", getProperty.DynamicInvoke(3, "5"));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("FTest1", typeof(string), "123")]
		[DataRow("FTest1", typeof(object), "456")]
		[DataRow("FTest2", typeof(int), 100)]
		[DataRow("FTest2", typeof(long), 100L)]
		[DataRow("FTest2", typeof(char), (char)100)]
		[DataRow("FTest3", typeof(int), 100)]
		[DataRow("FTest3", typeof(long), 100L)]
		[DataRow("FTest3", typeof(char), (char)100)]
		public void TestCreateOpenInstanceFieldDelegate(string fieldName, Type valueType, object? value)
		{
			Delegate? setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(setField);
			Delegate? getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(getField);
			TestClass instance = new();
			setField.DynamicInvoke(instance, value);
			Assert.AreEqual(value, getField.DynamicInvoke(instance));

			// 额外的入参会被忽略
			setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<,,>).MakeGenericType(typeof(TestClass), valueType, typeof(object)));
			Assert.IsNotNull(setField);
			getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<,,>).MakeGenericType(typeof(TestClass), typeof(object), valueType));
			Assert.IsNotNull(getField);
			instance = new();
			setField.DynamicInvoke(instance, value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(instance, new object()));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("SFTest1", typeof(string), "123")]
		[DataRow("SFTest1", typeof(object), "456")]
		[DataRow("SFTest2", typeof(int), 100)]
		[DataRow("SFTest2", typeof(long), 100L)]
		[DataRow("SFTest2", typeof(char), (char)100)]
		[DataRow("SFTest3", typeof(int), 100)]
		[DataRow("SFTest3", typeof(long), 100L)]
		[DataRow("SFTest3", typeof(char), (char)100)]
		public void TestCreateOpenStaticFieldDelegate(string fieldName, Type valueType, object? value)
		{
			Delegate? setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<>).MakeGenericType(valueType));
			Assert.IsNotNull(setField);
			Delegate? getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(value);
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<,>).MakeGenericType(valueType, typeof(object)));
			Assert.IsNotNull(setField);
			getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<,>).MakeGenericType(typeof(object), valueType));
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("FTest1", typeof(string), "123")]
		[DataRow("FTest1", typeof(object), "456")]
		[DataRow("FTest2", typeof(int), 100)]
		[DataRow("FTest2", typeof(long), 100L)]
		[DataRow("FTest2", typeof(char), (char)100)]
		[DataRow("FTest3", typeof(int), 100)]
		[DataRow("FTest3", typeof(long), 100L)]
		[DataRow("FTest3", typeof(char), (char)100)]
		public void TestCreateClosedInstanceFieldDelegate(string fieldName, Type valueType, object? value)
		{
			TestClass instance = new();
			Delegate? setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(setField);
			Delegate? getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(getField);
			setField.DynamicInvoke(value);
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			instance = new();
			setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<,>).MakeGenericType(valueType, typeof(object)), instance);
			Assert.IsNotNull(setField);
			getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<,>).MakeGenericType(typeof(object), valueType), instance);
			Assert.IsNotNull(getField);
			setField.DynamicInvoke(value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.PowerDelegate(Type, string, Type, object?)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("SFTest1", typeof(string), "123")]
		[DataRow("SFTest1", typeof(object), "456")]
		[DataRow("SFTest2", typeof(int), 100)]
		[DataRow("SFTest2", typeof(long), 100L)]
		[DataRow("SFTest2", typeof(char), (char)100)]
		[DataRow("SFTest3", typeof(int), 100)]
		[DataRow("SFTest3", typeof(long), 100L)]
		[DataRow("SFTest3", typeof(char), (char)100)]
		public void TestClosedStaticFieldDelegate(string fieldName, Type valueType, object? value)
		{
			Delegate? setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action), value);
			Assert.IsNotNull(setField);
			Delegate? getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<>).MakeGenericType(valueType), value);
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke();
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			setField = typeof(TestClass).PowerDelegate(fieldName, typeof(Action<object>), value);
			Assert.IsNotNull(setField);
			getField = typeof(TestClass).PowerDelegate(fieldName, typeof(Func<,>).MakeGenericType(typeof(object), valueType), value);
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}
	}
}

