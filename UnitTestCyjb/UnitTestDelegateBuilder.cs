using System;
using System.Reflection;
using System.Text;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.DelegateBuilder"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestDelegateBuilder
	{

		#region 测试通用委托

		/// <summary>
		/// 测试构造 MethodInvoker 委托。
		/// </summary>
		[TestMethod]
		public void TestMethodInvoker()
		{
			Type type = typeof(TestClass);

			// 静态方法
			// 无参数
			MethodInvoker invoker = type.GetMethod("TestStaticMethod", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("StaticMethod", invoker("NoUse"));
			Assert.AreEqual("StaticMethod", invoker(null));
			// 字符串参数
			invoker = type.GetMethod("TestStaticMethod", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_StaticMethod", invoker("NoUse", "Test"));
			Assert.AreEqual("Test_StaticMethod", invoker(null, "Test"));
			AssertExt.ThrowsException(() => invoker("NoUse", null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => invoker("NoUse"), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => invoker("NoUse", "Test", "more args"), typeof(TargetParameterCountException));
			// 整数参数
			invoker = type.GetMethod("TestStaticMethod", new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10_StaticMethod", invoker("NoUse", 10));
			Assert.AreEqual("10_StaticMethod", invoker(null, 10));
			// 可变参数
			invoker = type.GetMethod("TestStaticMethodVarargs", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("StaticMethod", invoker(null));
			invoker = type.GetMethod("TestStaticMethodVarargs", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_StaticMethod", invoker(null, "Test"));
			// 引用参数
			invoker = type.GetMethod("TestStaticMethodRef").CreateDelegate();
			Assert.AreEqual("A_B_StaticMethod", invoker(null, "A", "B", 0));

			// 实例方法
			TestClass instance = new TestClass { Text = "Instance" };
			// 无参数
			invoker = type.GetMethod("TestInstanceMethod", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("Instance_InstanceMethod", invoker(instance));
			AssertExt.ThrowsException(() => invoker(null), typeof(ArgumentNullException));
			// 字符串参数
			invoker = type.GetMethod("TestInstanceMethod", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_Instance_InstanceMethod", invoker(instance, "Test"));
			AssertExt.ThrowsException(() => invoker(instance, null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => invoker(instance), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => invoker(instance, "Test", "more args"), typeof(TargetParameterCountException));
			// 整数参数
			invoker = type.GetMethod("TestInstanceMethod", new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10_Instance_InstanceMethod", invoker(instance, 10));
			// 可变参数
			invoker = type.GetMethod("TestInstanceMethodVarargs", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("Instance_InstanceMethod", invoker(instance));
			invoker = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test_Instance_InstanceMethod", invoker(instance, "Test"));
			// 引用参数
			invoker = type.GetMethod("TestInstanceMethodRef").CreateDelegate();
			Assert.AreEqual("A_B_InstanceMethod", invoker(instance, "A", "B", 0));

			// ToString
			invoker = typeof(object).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker(10));
			Assert.AreEqual("10", invoker("10"));
			invoker = typeof(string).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker("10"));
			invoker = typeof(int).GetMethod("ToString", Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("10", invoker(10));
		}
		/// <summary>
		/// 测试构造 InstanceCreator 委托。
		/// </summary>
		[TestMethod]
		public void TestInstanceCreator()
		{
			Type type = typeof(TestClass);

			// 无参数
			InstanceCreator creator = type.GetConstructor(Type.EmptyTypes).CreateDelegate();
			Assert.AreEqual("NoParam", ((TestClass)creator()).Text);
			// 字符串参数
			creator = type.GetConstructor(new[] { typeof(string) }).CreateDelegate();
			Assert.AreEqual("Test", ((TestClass)creator("Test")).Text);
			AssertExt.ThrowsException(() => creator(null), typeof(ArgumentNullException));
			AssertExt.ThrowsException(() => creator(), typeof(TargetParameterCountException));
			// 整数参数
			creator = type.GetConstructor(new[] { typeof(int) }).CreateDelegate();
			Assert.AreEqual("10", ((TestClass)creator(10)).Text);

			// 默认构造函数
			Assert.AreEqual("NoParam", ((TestClass)typeof(TestClass).CreateInstanceCreator()()).Text);
			Assert.AreEqual(0, typeof(int).CreateInstanceCreator()());
			Assert.AreEqual(typeof(TestStruct), typeof(TestStruct).CreateInstanceCreator()().GetType());
		}

		#endregion // 测试通用委托

		#region 测试开放方法委托

		/// <summary>
		/// 测试构造开放方法委托。
		/// </summary>
		[TestMethod]
		public void TestOpenMethodDelegate()
		{
			Type type = typeof(TestClass);

			// 静态方法
			// 无参数
			MethodBase method = type.GetMethod("TestStaticMethod", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>()());
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, method.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(false));
			// 字符串参数
			MethodBase methodStr = type.GetMethod("TestStaticMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, string>>()("Test"));
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<object, object>>()("Test"));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<int, string>>(false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, int>>(false));
			// 整数参数
			MethodBase methodInt = type.GetMethod("TestStaticMethod", new[] { typeof(int) });
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<int, string>>()(10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<long, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<short, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, string>>()(10));
			Assert.AreEqual("10_StaticMethod", methodInt.CreateDelegate<Func<object, object>>()(10));
			// 可选参数
			MethodBase methodOptional = type.GetMethod("TestStaticMethodOptional");
			Assert.AreEqual("Test_10_StaticMethod", methodOptional.CreateDelegate<Func<string, int, string>>()("Test", 10));
			Assert.AreEqual("Test_0_StaticMethod", methodOptional.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("defaultKey_0_StaticMethod", methodOptional.CreateDelegate<Func<string>>()());
			// 可变参数
			method = type.GetMethod("TestStaticMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("StaticMethod", method.CreateDelegate<Func<string>>()());
			Assert.AreEqual("Test_StaticMethod", method.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test_Test2_StaticMethod", method.CreateDelegate<Func<string, string, string>>()("Test", "Test2"));
			methodStr = type.GetMethod("TestStaticMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_StaticMethod", methodStr.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("Test2_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string>>()(
				"Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_StaticMethod", methodStr.CreateDelegate<Func<string, string, string, string>>()(
				"Test", "Test2", "Test3"));
			// 泛型方法
			MethodBase methodGeneric = type.GetMethod("TestStaticMethodGeneric");
			Assert.AreEqual("<System.String>Test_StaticMethod", methodGeneric.CreateDelegate<Func<string, string>>()("Test"));
			Assert.AreEqual("<System.Int32>10_StaticMethod", methodGeneric.CreateDelegate<Func<int, string>>()(10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<string, int>>(false));
			// 引用参数
			MethodBase methodRef = type.GetMethod("TestStaticMethodRef");
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<Func<string, string, int, string>>()("A", "B", 0));
			string value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod", methodRef.CreateDelegate<TestDelegate>()("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);

			// 实例方法
			TestClass instance = new TestClass { Text = "Instance" };
			// 无参数
			method = type.GetMethod("TestInstanceMethod", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>()(instance));
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<object, object>>()(instance));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, string, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, int>>(false));
			// 字符串参数
			methodStr = type.GetMethod("TestInstanceMethod", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, int, string>>(false));
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<TestClass, string, int>>(false));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, object, object>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<object, object, object>>()(
				instance, "Test"));
			// 整数参数
			methodInt = type.GetMethod("TestInstanceMethod", new[] { typeof(int) });
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, int, string>>()(instance, 10));
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<TestClass, string, string>>(false));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, long, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, short, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, string>>()(
				instance, 10));
			Assert.AreEqual("10_Instance_InstanceMethod", methodInt.CreateDelegate<Func<TestClass, object, object>>()(
				instance, 10));
			// 可选参数
			methodOptional = type.GetMethod("TestInstanceMethodOptional");
			Assert.AreEqual("Test_10_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, int, string>>()(instance, "Test", 10));
			Assert.AreEqual("Test_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string, string>>()(instance, "Test"));
			Assert.AreEqual("defaultKey_0_Instance_InstanceMethod",
				methodOptional.CreateDelegate<Func<TestClass, string>>()(instance));
			// 可变参数
			method = type.GetMethod("TestInstanceMethodVarargs", Type.EmptyTypes);
			Assert.AreEqual("Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string>>()(instance));
			Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test_Test2_Instance_InstanceMethod",
				method.CreateDelegate<Func<TestClass, string, string, string>>()(instance, "Test", "Test2"));
			methodStr = type.GetMethod("TestInstanceMethodVarargs", new[] { typeof(string) });
			Assert.AreEqual("Test_Instance_InstanceMethod", methodStr.CreateDelegate<Func<TestClass, string, string>>()(
				instance, "Test"));
			Assert.AreEqual("Test2_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string>>()(instance, "Test", "Test2"));
			Assert.AreEqual("Test2_Test3_Test_Instance_InstanceMethod",
				methodStr.CreateDelegate<Func<TestClass, string, string, string, string>>()(instance, "Test", "Test2", "Test3"));
			// 泛型方法
			methodGeneric = type.GetMethod("TestInstanceMethodGeneric");
			Assert.AreEqual("<System.String>Test_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, string, string>>()(instance, "Test"));
			Assert.AreEqual("<System.Int32>10_Instance_InstanceMethod",
				methodGeneric.CreateDelegate<Func<TestClass, int, string>>()(instance, 10));
			Assert.AreEqual(null, methodGeneric.CreateDelegate<Func<TestClass, string, int>>(false));
			// 引用参数
			methodRef = type.GetMethod("TestInstanceMethodRef");
			Assert.AreEqual("A_B_InstanceMethod",
				methodRef.CreateDelegate<Func<TestClass, string, string, int, string>>()(instance, "A", "B", 0));
			Assert.AreEqual("B", instance.Text);
			value = "X";
			Assert.AreEqual("A_X_InstanceMethod", methodRef.CreateDelegate<TestInstanceDelegate>()(
				instance, "A", ref value, out value2));
			Assert.AreEqual("X", instance.Text);
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual(101, value2);
		}
		/// <summary>
		/// 测试构造开放构造函数委托。
		/// </summary>
		[TestMethod]
		public void TestOpenConstructorDelegate()
		{
			Type type = typeof(TestClass);

			// 无参数
			MethodBase method = type.GetConstructor(Type.EmptyTypes);
			Assert.AreEqual("NoParam", method.CreateDelegate<Func<TestClass>>()().Text);
			Assert.AreEqual("NoParam", ((TestClass)method.CreateDelegate<Func<object>>()()).Text);
			Assert.AreEqual(null, method.CreateDelegate<Func<string, object>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(false));
			// 字符串参数
			MethodBase methodStr = type.GetConstructor(new[] { typeof(string) });
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<string, TestClass>>()("Test").Text);
			Assert.AreEqual("Test", methodStr.CreateDelegate<Func<object, TestClass>>()("Test").Text);
			Assert.AreEqual(null, methodStr.CreateDelegate<Func<string, string>>(false));
			// 整数
			MethodBase methodInt = type.GetConstructor(new[] { typeof(int) });
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<int, TestClass>>()(10).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<ulong, TestClass>>()(10UL).Text);
			Assert.AreEqual("10", methodInt.CreateDelegate<Func<short, TestClass>>()(10).Text);
			Assert.AreEqual(null, methodInt.CreateDelegate<Func<string, TestClass>>(false));
		}

		#endregion // 测试开放方法委托

		/// <summary>
		/// 测试通过 MethodInfo 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestMethodDelegateFromMethodInfo()
		{
			//Type type = typeof(TestClass);
			//MethodInfo method0 = type.GetMethod("TestStaticMethod", Type.EmptyTypes);
			//MethodInfo method = type.GetMethod("TestStaticMethod", new Type[] { typeof(string) });
			//MethodInfo method2 = type.GetMethod("TestStaticMethod", new Type[] { typeof(string), typeof(int) });
			//MethodInfo method3 = type.GetMethod("TestStaticMethodGeneric");
			//MethodInfo method4 = type.GetMethod("TestStaticMethodRef");
			//// 开放的静态方法。
			//Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<string>>()());
			//Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<object>>()());
			//Assert.AreEqual(null, method0.CreateDelegate<Func<string, string>>(false));
			//Assert.AreEqual(null, method0.CreateDelegate<Func<int>>(false));
			//Assert.AreEqual("Test1_StaticMethod", method.CreateDelegate<Func<string, string>>()("Test1"));
			//Assert.AreEqual("Test2_StaticMethod", method.CreateDelegate<Func<string, string>>(null)("Test2"));
			//Assert.AreEqual("Test2_StaticMethod", method.CreateDelegate<Func<string, string>>("NoUse")("Test2"));
			//Assert.AreEqual(null, method.CreateDelegate<Func<int, string>>(false));
			//Assert.AreEqual(null, method.CreateDelegate<Func<string, int>>(false));
			//Assert.AreEqual("Test3_StaticMethod", method.CreateDelegate<Func<object, string>>()("Test3"));
			//Assert.AreEqual("Test4_StaticMethod", method.CreateDelegate<Func<object, object>>()("Test4"));
			//Assert.AreEqual("Test5_10_StaticMethod", method2.CreateDelegate<Func<string, int, string>>()("Test5", 10));
			//Assert.AreEqual("Test6_10_StaticMethod", method2.CreateDelegate<Func<string, short, string>>()("Test6", 10));
			//Assert.AreEqual("Test7_10_StaticMethod", method2.CreateDelegate<Func<string, ulong, string>>()("Test7", 10UL));
			//Assert.AreEqual("Test8_10_StaticMethod", method2.CreateDelegate<Func<object, ulong, string>>()("Test8", 10UL));
			//Assert.AreEqual("Test9_10_StaticMethod", method2.CreateDelegate<Func<object, ulong, object>>()("Test9", 10UL));
			//Assert.AreEqual(null, method2.CreateDelegate<Func<string, string, string>>(false));
			//// 第一个参数封闭的静态方法。
			//Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<string>>(null)());
			//Assert.AreEqual("_StaticMethod", method.CreateDelegate<Func<string>>(null)());
			//Assert.AreEqual("Test10_StaticMethod", method.CreateDelegate<Func<string>>("Test10")());
			//Assert.AreEqual("Test11_StaticMethod", method.CreateDelegate<Func<object>>("Test11")());
			//Assert.AreEqual(null, method.CreateDelegate<Func<int>>("Test11", false));
			//// 开放的泛型静态方法。
			//Assert.AreEqual("<System.String>Test13_StaticMethod", method3.CreateDelegate<Func<string, string>>()("Test13"));
			//Assert.AreEqual("<System.Int32>14_StaticMethod", method3.CreateDelegate<Func<int, string>>()(14));
			//Assert.AreEqual("<System.String>Test15_StaticMethod", method3.CreateDelegate<Func<string, string>>(null)("Test15"));
			//Assert.AreEqual("<System.String>Test16_StaticMethod", method3.CreateDelegate<Func<string, string>>("NoUse")("Test16"));
			//Assert.AreEqual(null, method3.CreateDelegate<Func<string, int>>(false));
			//// 第一个参数封闭的泛型静态方法。
			//Assert.AreEqual("<System.Object>_StaticMethod", method3.CreateDelegate<Func<string>>(null)());
			//Assert.AreEqual("<System.String>Test17_StaticMethod", method3.CreateDelegate<Func<string>>("Test17")());
			//Assert.AreEqual("<System.String>Test18_StaticMethod", method3.CreateDelegate<Func<object>>("Test18")());
			//Assert.AreEqual("<System.Int32>19_StaticMethod", method3.CreateDelegate<Func<string>>(19)());
			//Assert.AreEqual(null, method3.CreateDelegate<Func<int>>("Test11", false));
			//// 特殊静态方法。
			//Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<Func<string, string, int, string>>()("A", "B", 0));
			//Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<Func<string, int, string>>("A")("B", 0));
			//string value = "B";
			//int value2;
			//Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<TestDelegate>()("A", ref value, out value2));
			//Assert.AreEqual("StaticMethodRef", value);
			//Assert.AreEqual(101, value2);
			//// 实例方法。
			//method0 = type.GetMethod("TestInstanceMethod", Type.EmptyTypes);
			//method = type.GetMethod("TestInstanceMethod", new Type[] { typeof(string) });
			//method2 = type.GetMethod("TestInstanceMethod", new Type[] { typeof(string), typeof(int) });
			//method3 = type.GetMethod("TestInstanceMethod2");
			//method4 = type.GetMethod("TestInstanceMethod3");
			//TestClass tc = new TestClass();
			//tc.Text = "TC";
			//// 开放的实例方法。
			//Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>()(tc));
			//Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>(null)(tc));
			//Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>("NoUse")(tc));
			//Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<object, object>>()(tc));
			//Assert.AreEqual(null, method0.CreateDelegate<Func<TestClass, string, string>>(false));
			//Assert.AreEqual(null, method0.CreateDelegate<Func<TestClass, int>>(false));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>()(tc, "Test1"));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>(null)(tc, "Test2"));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>("NoUse")(tc, "Test2"));
			//Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, int, string>>(false));
			//Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, string, int>>(false));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, object, string>>()(tc, "Test3"));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<TestClass, object, object>>()(tc, "Test4"));
			//Assert.AreEqual("Test_Instance_InstanceMethod", method.CreateDelegate<Func<object, object, object>>()(tc, "Test4"));
			//Assert.AreEqual("Test5_1_Instance_InstanceMethod",
			//	method2.CreateDelegate<Func<TestClass, string, int, string>>()(tc, "Test5", 10));
			//Assert.AreEqual("Test6_1_Instance_InstanceMethod",
			//	method2.CreateDelegate<Func<TestClass, string, short, string>>()(tc, "Test6", 10));
			//Assert.AreEqual("Test7_1_Instance_InstanceMethod",
			//	method2.CreateDelegate<Func<TestClass, string, ulong, string>>()(tc, "Test7", 10UL));
			//Assert.AreEqual("Test8_1_Instance_InstanceMethod",
			//	method2.CreateDelegate<Func<TestClass, object, ulong, string>>()(tc, "Test8", 10UL));
			//Assert.AreEqual("Test9_1_Instance_InstanceMethod",
			//	method2.CreateDelegate<Func<TestClass, object, ulong, object>>()(tc, "Test9", 10UL));
			//Assert.AreEqual(null, method2.CreateDelegate<Func<TestClass, string, string, string>>(false));
			//// 第一个参数封闭的实例方法。
			//Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<string>>(tc)());
			//Assert.AreEqual("NullThis_InstanceMethod", method0.CreateDelegate<Func<string>>(null)());
			//Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<string, string>>(null)("Test10"));
			//Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<object, string>>(null)("Test10"));
			//Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<string, object>>(null)("Test10"));
			//Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<object, object>>(null)("Test10"));
			//Assert.AreEqual(null, method.CreateDelegate<Func<int, object>>(null));
			//Assert.AreEqual("Test1_Instance_InstanceMethod", method.CreateDelegate<Func<string, string>>(tc)("Test10"));
			//Assert.AreEqual("Test1_Instance_InstanceMethod", method.CreateDelegate<Func<string, object>>(tc)("Test11"));
			//Assert.AreEqual("Test1_Instance_InstanceMethod", method.CreateDelegate<Func<object, object>>(tc)("Test11"));
			//Assert.AreEqual(null, method.CreateDelegate<Func<int>>(tc, false));
			//// 开放的泛型实例方法。
			//Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<TestClass, string, string>>()(tc, "Test13"));
			//Assert.AreEqual("<System.Int32>1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<TestClass, int, string>>()(tc, 14));
			//Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<TestClass, string, string>>(null)(tc, "Test15"));
			//Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<TestClass, string, string>>("NoUse")(tc, "Test16"));
			//Assert.AreEqual("<System.Object>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<object, object, string>>("NoUse")(tc, "Test16"));
			//Assert.AreEqual(null, method3.CreateDelegate<Func<TestClass, string, int>>(false));
			//// 第一个参数封闭的泛型实例方法。
			//Assert.AreEqual("<System.String>Test17_NullThis_InstanceMethod",
			//	method3.CreateDelegate<Func<string, string>>(null)("Test17"));
			//Assert.AreEqual("<System.Object>Test17_NullThis_InstanceMethod",
			//	method3.CreateDelegate<Func<object, string>>(null)("Test17"));
			//Assert.AreEqual("<System.String>Test17_NullThis_InstanceMethod",
			//	method3.CreateDelegate<Func<string, object>>(null)("Test17"));
			//Assert.AreEqual("<System.Object>Test17_NullThis_InstanceMethod",
			//	method3.CreateDelegate<Func<object, object>>(null)("Test17"));
			//Assert.AreEqual("<System.Int32>17_NullThis_InstanceMethod",
			//	method3.CreateDelegate<Func<int, object>>(null)(17));
			//Assert.AreEqual(null, method3.CreateDelegate<Func<int, int>>(null));
			//Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<string, string>>(tc)("Test18"));
			//Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<string, object>>(tc)("Test19"));
			//Assert.AreEqual("<System.Object>Test2_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<object, object>>(tc)("Test20"));
			//Assert.AreEqual("<System.Int32>2_Instance_InstanceMethod",
			//	method3.CreateDelegate<Func<int, object>>(tc)(21));
			//// 特殊实例方法。
			//Assert.AreEqual("A_B_InstanceMethod",
			//	method4.CreateDelegate<Func<TestClass, string, string, int, string>>()(tc, "A", "B", 0));
			//Assert.AreEqual("B", tc.Text);
			//tc.Text = "XXX";
			//Assert.AreEqual("A_B_InstanceMethod", method4.CreateDelegate<Func<string, string, int, string>>(tc)("A", "B", 0));
			//Assert.AreEqual("B", tc.Text);
			//value = "B";
			//value2 = 0;
			//tc.Text = "XXX";
			//Assert.AreEqual("A_B_InstanceMethod", method4.CreateDelegate<TestDelegate>(tc)("A", ref value, out value2));
			//Assert.AreEqual("InstanceMethodRef", value);
			//Assert.AreEqual("B", tc.Text);
			//Assert.AreEqual(101, value2);
		}


		/// <summary>
		/// 测试通过 PropertyInfo 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestPropertyDelegateFromPropertyInfo()
		{
			Type type = typeof(TestClass);
			PropertyInfo property = type.GetProperty("TestStaticProperty");
			// 开放的静态属性。
			property.CreateDelegate<Action<string>>()("Test1");
			Assert.AreEqual("Test1", TestClass.TestStaticProperty);
			Assert.AreEqual("Test1", property.CreateDelegate<Func<string>>()());
			Assert.AreEqual("Test1", property.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, property.CreateDelegate<Func<int>>(false));
			property.CreateDelegate<Action<object>>()("Test2");
			Assert.AreEqual("Test2", TestClass.TestStaticProperty);
			// 第一个参数封闭的静态属性。
			property.CreateDelegate<Action>("Test3")();
			Assert.AreEqual("Test3", TestClass.TestStaticProperty);
			// 实例属性。
			property = type.GetProperty("TestInstanceProperty");
			PropertyInfo property2 = type.GetProperty("Item", new Type[] { typeof(int) });
			PropertyInfo property3 = type.GetProperty("Item", new Type[] { typeof(int), typeof(int) });
			TestClass tc = new TestClass();
			// 开放的实例属性。
			property.CreateDelegate<Action<TestClass, string>>()(tc, "Test1");
			Assert.AreEqual("Test1", tc.TestInstanceProperty);
			Assert.AreEqual("Test1", property.CreateDelegate<Func<TestClass, string>>()(tc));
			Assert.AreEqual("Test1", property.CreateDelegate<Func<object, object>>()(tc));
			Assert.AreEqual(null, property.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, property.CreateDelegate<Func<TestClass, int>>(false));
			property.CreateDelegate<Action<object, object>>("NoUse")(tc, "Test2");
			Assert.AreEqual("Test2", tc.TestInstanceProperty);
			property2.CreateDelegate<Action<TestClass, int, string>>()(tc, 0, "Test3");
			property2.CreateDelegate<Action<object, object, object>>(null)(tc, 1, "Test4");
			property3.CreateDelegate<Action<TestClass, short, ulong, string>>("NoUse")(tc, 0, 2, "Test5");
			property3.CreateDelegate<Action<object, object, short, object>>()(tc, 1, 2, "Test6");
			Assert.AreEqual("Test3", tc[0]);
			Assert.AreEqual("Test4", tc[1]);
			Assert.AreEqual("Test5", tc[2]);
			Assert.AreEqual("Test6", tc[3]);
			Assert.AreEqual("Test3", property3.CreateDelegate<Func<TestClass, int, short, string>>()(tc, 0, 0));
			Assert.AreEqual("Test4", property3.CreateDelegate<Func<object, object, short, object>>()(tc, 2, -1));
			Assert.AreEqual("Test5", property2.CreateDelegate<Func<TestClass, int, string>>()(tc, 2));
			Assert.AreEqual("Test6", property2.CreateDelegate<Func<object, object, object>>()(tc, 3));
			// 第一个参数封闭的实例属性。
			property.CreateDelegate<Action<string>>(tc)("Test7");
			Assert.AreEqual("Test7", tc.TestInstanceProperty);
			Assert.AreEqual("Test7", property.CreateDelegate<Func<string>>(tc)());
			Assert.AreEqual("Test7", property.CreateDelegate<Func<object>>(tc)());
			Assert.AreEqual(null, property.CreateDelegate<Func<int>>(tc, false));
			property.CreateDelegate<Action<object>>(tc)("Test8");
			Assert.AreEqual("Test8", tc.TestInstanceProperty);
			property2.CreateDelegate<Action<int, string>>(tc)(4, "Test9");
			property2.CreateDelegate<Action<object, object>>(tc)(5, "Test10");
			property3.CreateDelegate<Action<short, ulong, string>>(tc)(3, 3, "Test11");
			property3.CreateDelegate<Action<object, short, object>>(tc)(10, -3, "Test12");
			Assert.AreEqual("Test9", tc[4]);
			Assert.AreEqual("Test10", tc[5]);
			Assert.AreEqual("Test11", tc[6]);
			Assert.AreEqual("Test12", tc[7]);
			Assert.AreEqual("Test9", property3.CreateDelegate<Func<TestClass, int, short, string>>()(tc, 4, 0));
			Assert.AreEqual("Test10", property3.CreateDelegate<Func<object, object, short, object>>()(tc, -1, 6));
			Assert.AreEqual("Test11", property2.CreateDelegate<Func<TestClass, int, string>>()(tc, 6));
			Assert.AreEqual("Test12", property2.CreateDelegate<Func<object, object, object>>()(tc, 7));
		}

		/// <summary>
		/// 测试通过 FieldInfo 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestFieldDelegateFromFieldInfo()
		{
			Type type = typeof(TestClass);
			FieldInfo field = type.GetField("TestStaticField");
			// 开放的静态字段。
			field.CreateDelegate<Action<string>>()("Test1");
			Assert.AreEqual("Test1", TestClass.TestStaticField);
			Assert.AreEqual("Test1", field.CreateDelegate<Func<string>>()());
			Assert.AreEqual("Test1", field.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, field.CreateDelegate<Func<int>>(false));
			field.CreateDelegate<Action<object>>()("Test2");
			Assert.AreEqual("Test2", TestClass.TestStaticField);
			// 第一个参数封闭的静态字段。
			field.CreateDelegate<Action>("Test3")();
			Assert.AreEqual("Test3", TestClass.TestStaticField);
			// 实例字段。
			field = type.GetField("TestInstanceField");
			TestClass tc = new TestClass();
			// 开放的实例字段。
			field.CreateDelegate<Action<TestClass, string>>()(tc, "Test1");
			Assert.AreEqual("Test1", tc.TestInstanceField);
			Assert.AreEqual("Test1", field.CreateDelegate<Func<TestClass, string>>()(tc));
			Assert.AreEqual("Test1", field.CreateDelegate<Func<object, object>>()(tc));
			Assert.AreEqual(null, field.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, field.CreateDelegate<Func<TestClass, int>>(false));
			field.CreateDelegate<Action<object, object>>("NoUse")(tc, "Test2");
			Assert.AreEqual("Test2", tc.TestInstanceField);
			// 第一个参数封闭的实例字段。
			field.CreateDelegate<Action<string>>(tc)("Test7");
			Assert.AreEqual("Test7", tc.TestInstanceField);
			Assert.AreEqual("Test7", field.CreateDelegate<Func<string>>(tc)());
			Assert.AreEqual("Test7", field.CreateDelegate<Func<object>>(tc)());
			Assert.AreEqual(null, field.CreateDelegate<Func<int>>(tc, false));
			field.CreateDelegate<Action<object>>(tc)("Test8");
			Assert.AreEqual("Test8", tc.TestInstanceField);
		}

		/// <summary>
		/// 测试通过 Type 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestMethodDelegateFromType()
		{
			BindingFlags DefaultBinder = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
			Type type = typeof(TestClass);
			// 开放的静态方法。
			Assert.AreEqual("StaticMethod", type.CreateDelegate<Func<string>>("TestStaticMethod")());
			Assert.AreEqual("StaticMethod", type.CreateDelegate<Func<object>>("TestStaticMethod")());
			Assert.AreEqual(null, type.CreateDelegate<Func<int>>("TestStaticMethod", DefaultBinder, false));
			Assert.AreEqual("Test1_StaticMethod", type.CreateDelegate<Func<string, string>>("TestStaticMethod")("Test1"));
			Assert.AreEqual("Test2_StaticMethod", type.CreateDelegate<Func<string, string>>("TestStaticMethod")("Test2"));
			Assert.AreEqual("Test2_StaticMethod", type.CreateDelegate<Func<string, string>>("TestStaticMethod", null)("Test2"));
			Assert.AreEqual(null, type.CreateDelegate<Func<int, string>>("TestStaticMethod", DefaultBinder, false));
			Assert.AreEqual(null, type.CreateDelegate<Func<string, int>>("TestStaticMethod", DefaultBinder, false));
			Assert.AreEqual("Test3_StaticMethod", type.CreateDelegate<Func<object, string>>("TestStaticMethod")("Test3"));
			Assert.AreEqual("Test4_StaticMethod", type.CreateDelegate<Func<object, object>>("TestStaticMethod")("Test4"));
			Assert.AreEqual("Test5_10_StaticMethod",
				type.CreateDelegate<Func<string, int, string>>("TestStaticMethod")("Test5", 10));
			Assert.AreEqual("Test6_10_StaticMethod",
				type.CreateDelegate<Func<string, short, string>>("TestStaticMethod")("Test6", 10));
			Assert.AreEqual("Test7_10_StaticMethod",
				type.CreateDelegate<Func<string, ulong, string>>("TestStaticMethod")("Test7", 10UL));
			Assert.AreEqual("Test8_10_StaticMethod",
				type.CreateDelegate<Func<object, ulong, string>>("TestStaticMethod")("Test8", 10UL));
			Assert.AreEqual("Test9_10_StaticMethod",
				type.CreateDelegate<Func<object, ulong, object>>("TestStaticMethod")("Test9", 10UL));
			Assert.AreEqual(null, type.CreateDelegate<Func<string, string, string>>("TestStaticMethod", DefaultBinder, false));
			// 开放的泛型静态方法。
			Assert.AreEqual("<System.String>Test13_StaticMethod",
				type.CreateDelegate<Func<string, string>>("TestStaticMethodGeneric")("Test13"));
			Assert.AreEqual("<System.Int32>14_StaticMethod",
				type.CreateDelegate<Func<int, string>>("TestStaticMethodGeneric")(14));
			Assert.AreEqual("<System.String>Test15_StaticMethod",
				type.CreateDelegate<Func<string, string>>("TestStaticMethodGeneric", null)("Test15"));
			Assert.AreEqual(null, type.CreateDelegate<Func<string, int>>("TestInstanceMethod2", DefaultBinder, false));
			// 特殊静态方法。
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestStaticMethodRef")("A", "B", 0));
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestStaticMethodRef", null)("A", "B", 0));
			string value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<TestDelegate>("TestStaticMethodRef")("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);
			// 实例方法。
			TestClass tc = new TestClass();
			tc.Text = "TC";
			// 开放的实例方法。
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<TestClass, string>>("TestInstanceMethod")(tc));
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<object, object>>("TestInstanceMethod")(tc));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, int>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethod")(tc, "Test1"));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, string, int>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, string>>("TestInstanceMethod")(tc, "Test3"));
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, object>>("TestInstanceMethod")(tc, "Test4"));
			Assert.AreEqual("Test_Instance_InstanceMethod",
				type.CreateDelegate<Func<object, object, object>>("TestInstanceMethod")(tc, "Test4"));
			Assert.AreEqual("Test5_1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, int, string>>("TestInstanceMethod")(tc, "Test5", 10));
			Assert.AreEqual("Test6_1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, short, string>>("TestInstanceMethod")(tc, "Test6", 10));
			Assert.AreEqual("Test7_1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, ulong, string>>("TestInstanceMethod")(tc, "Test7", 10UL));
			Assert.AreEqual("Test8_1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, ulong, string>>("TestInstanceMethod")(tc, "Test8", 10UL));
			Assert.AreEqual("Test9_1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, ulong, object>>("TestInstanceMethod")(tc, "Test9", 10UL));
			Assert.AreEqual(null,
				type.CreateDelegate<Func<TestClass, string, string, string>>("TestInstanceMethod", DefaultBinder, false));
			// 第一个参数封闭的实例方法。
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<string>>("TestInstanceMethod", tc)());
			Assert.AreEqual("Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethod", tc)("Test10"));
			Assert.AreEqual("Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, object>>("TestInstanceMethod", tc)("Test11"));
			Assert.AreEqual("Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<object, object>>("TestInstanceMethod", tc)("Test11"));
			Assert.AreEqual(null,
				type.CreateDelegate<Func<int>>("TestInstanceMethod", tc, DefaultBinder, false));
			// 开放的泛型实例方法。
			Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethod2")(tc, "Test13"));
			Assert.AreEqual("<System.Int32>1_Instance_InstanceMethod",
				type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethod2")(tc, 14));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, string, int>>("TestInstanceMethod2", DefaultBinder, false));
			// 第一个参数封闭的泛型实例方法。
			Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethod2", tc)("Test18"));
			Assert.AreEqual("<System.String>Test1_Instance_InstanceMethod",
				type.CreateDelegate<Func<string, object>>("TestInstanceMethod2", tc)("Test19"));
			Assert.AreEqual("<System.Object>Test2_Instance_InstanceMethod",
				type.CreateDelegate<Func<object, object>>("TestInstanceMethod2", tc)("Test20"));
			Assert.AreEqual("<System.Int32>2_Instance_InstanceMethod",
				type.CreateDelegate<Func<int, object>>("TestInstanceMethod2", tc)(21));
			// 特殊实例方法。
			Assert.AreEqual("A_B_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string, int, string>>("TestInstanceMethod3")(tc, "A", "B", 0));
			Assert.AreEqual("B", tc.Text);
			tc.Text = "XXX";
			Assert.AreEqual("A_B_InstanceMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestInstanceMethod3", tc)("A", "B", 0));
			Assert.AreEqual("B", tc.Text);
			value = "B";
			value2 = 0;
			tc.Text = "XXX";
			Assert.AreEqual("A_B_InstanceMethod",
				type.CreateDelegate<TestDelegate>("TestInstanceMethod3", tc)("A", ref value, out value2));
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual("B", tc.Text);
			Assert.AreEqual(101, value2);
		}

		private delegate string TestDelegate(string key, ref string value, out int value2);
		private delegate string TestInstanceDelegate(TestClass instance, string key, ref string value, out int value2);
		private class TestStruct { }
		private class TestClass
		{

			#region 静态方法

			public static string TestStaticMethod()
			{
				return "StaticMethod";
			}
			public static string TestStaticMethod(string key)
			{
				return key + "_StaticMethod";
			}
			public static string TestStaticMethod(int key)
			{
				return key + "_StaticMethod";
			}
			public static string TestStaticMethodOptional(string key = "defaultKey", int value = 0)
			{
				return key + "_" + value + "_StaticMethod";
			}
			public static string TestStaticMethodVarargs(__arglist)
			{
				ArgIterator args = new ArgIterator(__arglist);
				StringBuilder text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append("StaticMethod");
				return text.ToString();
			}
			public static string TestStaticMethodVarargs(string key, __arglist)
			{
				ArgIterator args = new ArgIterator(__arglist);
				StringBuilder text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(key);
				text.Append("_StaticMethod");
				return text.ToString();
			}
			public static string TestStaticMethodGeneric<T>(T key)
			{
				return "<" + typeof(T) + ">" + key + "_StaticMethod";
			}
			public static string TestStaticMethodRef(string key, ref string value, out int value2)
			{
				string oldValue = value;
				value = "StaticMethodRef";
				value2 = 101;
				return key + "_" + oldValue + "_StaticMethod";
			}

			#endregion // 静态方法

			#region 实例方法

			public string Text;
			public string TestInstanceMethod()
			{
				return Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(string key)
			{
				return key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(int key)
			{
				return key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodOptional(string key = "defaultKey", int value = 0)
			{
				return key + "_" + value + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodVarargs(__arglist)
			{
				ArgIterator args = new ArgIterator(__arglist);
				StringBuilder text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(Text);
				text.Append("_InstanceMethod");
				return text.ToString();
			}
			public string TestInstanceMethodVarargs(string key, __arglist)
			{
				ArgIterator args = new ArgIterator(__arglist);
				StringBuilder text = new StringBuilder(16);
				while (args.GetRemainingCount() > 0)
				{
					text.Append(TypedReference.ToObject(args.GetNextArg()));
					text.Append('_');
				}
				text.Append(key);
				text.Append('_');
				text.Append(Text);
				text.Append("_InstanceMethod");
				return text.ToString();
			}
			public string TestInstanceMethodGeneric<T>(T key)
			{
				return "<" + typeof(T) + ">" + key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethodRef(string key, ref string value, out int value2)
			{
				Text = value;
				value = "InstanceMethodRef";
				value2 = 101;
				return key + "_" + Text + "_InstanceMethod";
			}

			#endregion // 实例方法

			#region 构造函数

			public TestClass()
			{
				this.Text = "NoParam";
			}
			public TestClass(string key)
			{
				this.Text = key;
			}
			public TestClass(int key)
			{
				this.Text = key.ToString();
			}

			#endregion // 构造函数

			#region 静态和实例属性

			public static string TestStaticProperty { get; set; }
			public string TestInstanceProperty { get; set; }
			private string[] items = new string[10];
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

			#endregion // 静态和实例属性

			#region 静态和实例字段

			public static string TestStaticField = null;
			public string TestInstanceField = null;

			#endregion // 静态和实例字段

		}
	}
}
