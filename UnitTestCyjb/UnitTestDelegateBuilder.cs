using System;
using System.Reflection;
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
		/// <summary>
		/// 测试通过 MethodInfo 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestMethodDelegateFromMethodInfo()
		{
			Type type = typeof(TestClass);
			MethodInfo method0 = type.GetMethod("TestStaticMethod", Type.EmptyTypes);
			MethodInfo method = type.GetMethod("TestStaticMethod", new Type[] { typeof(string) });
			MethodInfo method2 = type.GetMethod("TestStaticMethod", new Type[] { typeof(string), typeof(int) });
			MethodInfo method3 = type.GetMethod("TestStaticMethod2");
			MethodInfo method4 = type.GetMethod("TestStaticMethod3");
			// 开放的静态方法。
			Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<string>>()());
			Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<object>>()());
			Assert.AreEqual(null, method0.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual(null, method0.CreateDelegate<Func<int>>(false));
			Assert.AreEqual("Test1_StaticMethod", method.CreateDelegate<Func<string, string>>()("Test1"));
			Assert.AreEqual("Test2_StaticMethod", method.CreateDelegate<Func<string, string>>(null)("Test2"));
			Assert.AreEqual("Test2_StaticMethod", method.CreateDelegate<Func<string, string>>("NoUse")("Test2"));
			Assert.AreEqual(null, method.CreateDelegate<Func<int, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<string, int>>(false));
			Assert.AreEqual("Test3_StaticMethod", method.CreateDelegate<Func<object, string>>()("Test3"));
			Assert.AreEqual("Test4_StaticMethod", method.CreateDelegate<Func<object, object>>()("Test4"));
			Assert.AreEqual("Test5_10_StaticMethod", method2.CreateDelegate<Func<string, int, string>>()("Test5", 10));
			Assert.AreEqual("Test6_10_StaticMethod", method2.CreateDelegate<Func<string, short, string>>()("Test6", 10));
			Assert.AreEqual("Test7_10_StaticMethod", method2.CreateDelegate<Func<string, ulong, string>>()("Test7", 10UL));
			Assert.AreEqual("Test8_10_StaticMethod", method2.CreateDelegate<Func<object, ulong, string>>()("Test8", 10UL));
			Assert.AreEqual("Test9_10_StaticMethod", method2.CreateDelegate<Func<object, ulong, object>>()("Test9", 10UL));
			Assert.AreEqual(null, method2.CreateDelegate<Func<string, string, string>>(false));
			// 第一个参数封闭的静态方法。
			Assert.AreEqual("StaticMethod", method0.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("_StaticMethod", method.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("Test10_StaticMethod", method.CreateDelegate<Func<string>>("Test10")());
			Assert.AreEqual("Test11_StaticMethod", method.CreateDelegate<Func<object>>("Test11")());
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>("Test11", false));
			// 通用静态方法。
			Assert.AreEqual("StaticMethod", method0.CreateDelegate()("NoUse"));
			Assert.AreEqual("Test12_StaticMethod", method.CreateDelegate()("NoUse", "Test12"));
			AssertExt.ThrowsException(() => method0.CreateDelegate()("Instance", "more args"), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => method.CreateDelegate()("Instance"), typeof(TargetParameterCountException));
			// 开放的泛型静态方法。
			Assert.AreEqual("<System.String>Test13_StaticMethod", method3.CreateDelegate<Func<string, string>>()("Test13"));
			Assert.AreEqual("<System.Int32>14_StaticMethod", method3.CreateDelegate<Func<int, string>>()(14));
			Assert.AreEqual("<System.String>Test15_StaticMethod", method3.CreateDelegate<Func<string, string>>(null)("Test15"));
			Assert.AreEqual("<System.String>Test16_StaticMethod", method3.CreateDelegate<Func<string, string>>("NoUse")("Test16"));
			Assert.AreEqual(null, method3.CreateDelegate<Func<string, int>>(false));
			// 第一个参数封闭的泛型静态方法。
			Assert.AreEqual("<System.Object>_StaticMethod", method3.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("<System.String>Test17_StaticMethod", method3.CreateDelegate<Func<string>>("Test17")());
			Assert.AreEqual("<System.String>Test18_StaticMethod", method3.CreateDelegate<Func<object>>("Test18")());
			Assert.AreEqual("<System.Int32>19_StaticMethod", method3.CreateDelegate<Func<string>>(19)());
			Assert.AreEqual(null, method3.CreateDelegate<Func<int>>("Test11", false));
			// 特殊静态方法。
			Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<Func<string, string, int, string>>()("A", "B", 0));
			Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<Func<string, int, string>>("A")("B", 0));
			string value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod", method4.CreateDelegate<TestDelegate>()("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);
			// 实例方法。
			method0 = type.GetMethod("TestInstanceMethod", Type.EmptyTypes);
			method = type.GetMethod("TestInstanceMethod", new Type[] { typeof(string) });
			method2 = type.GetMethod("TestInstanceMethod", new Type[] { typeof(string), typeof(int) });
			method3 = type.GetMethod("TestInstanceMethod2");
			method4 = type.GetMethod("TestInstanceMethod3");
			TestClass tc = new TestClass();
			tc.Text = "TC";
			// 开放的实例方法。
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>()(tc));
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>(null)(tc));
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<TestClass, string>>("NoUse")(tc));
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<object, object>>()(tc));
			Assert.AreEqual(null, method0.CreateDelegate<Func<TestClass, string, string>>(false));
			Assert.AreEqual(null, method0.CreateDelegate<Func<TestClass, int>>(false));
			Assert.AreEqual("Test1_TC_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>()(tc, "Test1"));
			Assert.AreEqual("Test2_TC_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>(null)(tc, "Test2"));
			Assert.AreEqual("Test2_TC_InstanceMethod", method.CreateDelegate<Func<TestClass, string, string>>("NoUse")(tc, "Test2"));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, int, string>>(false));
			Assert.AreEqual(null, method.CreateDelegate<Func<TestClass, string, int>>(false));
			Assert.AreEqual("Test3_TC_InstanceMethod", method.CreateDelegate<Func<TestClass, object, string>>()(tc, "Test3"));
			Assert.AreEqual("Test4_TC_InstanceMethod", method.CreateDelegate<Func<TestClass, object, object>>()(tc, "Test4"));
			Assert.AreEqual("Test4_TC_InstanceMethod", method.CreateDelegate<Func<object, object, object>>()(tc, "Test4"));
			Assert.AreEqual("Test5_10_TC_InstanceMethod",
				method2.CreateDelegate<Func<TestClass, string, int, string>>()(tc, "Test5", 10));
			Assert.AreEqual("Test6_10_TC_InstanceMethod",
				method2.CreateDelegate<Func<TestClass, string, short, string>>()(tc, "Test6", 10));
			Assert.AreEqual("Test7_10_TC_InstanceMethod",
				method2.CreateDelegate<Func<TestClass, string, ulong, string>>()(tc, "Test7", 10UL));
			Assert.AreEqual("Test8_10_TC_InstanceMethod",
				method2.CreateDelegate<Func<TestClass, object, ulong, string>>()(tc, "Test8", 10UL));
			Assert.AreEqual("Test9_10_TC_InstanceMethod",
				method2.CreateDelegate<Func<TestClass, object, ulong, object>>()(tc, "Test9", 10UL));
			Assert.AreEqual(null, method2.CreateDelegate<Func<TestClass, string, string, string>>(false));
			// 第一个参数封闭的实例方法。
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate<Func<string>>(tc)());
			Assert.AreEqual("NullThis_InstanceMethod", method0.CreateDelegate<Func<string>>(null)());
			Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<string, string>>(null)("Test10"));
			Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<object, string>>(null)("Test10"));
			Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<string, object>>(null)("Test10"));
			Assert.AreEqual("Test10_NullThis_InstanceMethod", method.CreateDelegate<Func<object, object>>(null)("Test10"));
			Assert.AreEqual(null, method.CreateDelegate<Func<int, object>>(null));
			Assert.AreEqual("Test10_TC_InstanceMethod", method.CreateDelegate<Func<string, string>>(tc)("Test10"));
			Assert.AreEqual("Test11_TC_InstanceMethod", method.CreateDelegate<Func<string, object>>(tc)("Test11"));
			Assert.AreEqual("Test11_TC_InstanceMethod", method.CreateDelegate<Func<object, object>>(tc)("Test11"));
			Assert.AreEqual(null, method.CreateDelegate<Func<int>>(tc, false));
			// 通用实例方法。
			Assert.AreEqual("Test12_TC_InstanceMethod", method.CreateDelegate()(tc, "Test12"));
			Assert.AreEqual("TC_InstanceMethod", method0.CreateDelegate()(tc));
			// 开放的泛型实例方法。
			Assert.AreEqual("<System.String>Test13_TC_InstanceMethod",
				method3.CreateDelegate<Func<TestClass, string, string>>()(tc, "Test13"));
			Assert.AreEqual("<System.Int32>14_TC_InstanceMethod",
				method3.CreateDelegate<Func<TestClass, int, string>>()(tc, 14));
			Assert.AreEqual("<System.String>Test15_TC_InstanceMethod",
				method3.CreateDelegate<Func<TestClass, string, string>>(null)(tc, "Test15"));
			Assert.AreEqual("<System.String>Test16_TC_InstanceMethod",
				method3.CreateDelegate<Func<TestClass, string, string>>("NoUse")(tc, "Test16"));
			Assert.AreEqual("<System.Object>Test16_TC_InstanceMethod",
				method3.CreateDelegate<Func<object, object, string>>("NoUse")(tc, "Test16"));
			Assert.AreEqual(null, method3.CreateDelegate<Func<TestClass, string, int>>(false));
			// 第一个参数封闭的泛型实例方法。
			Assert.AreEqual("<System.String>Test17_NullThis_InstanceMethod",
				method3.CreateDelegate<Func<string, string>>(null)("Test17"));
			Assert.AreEqual("<System.Object>Test17_NullThis_InstanceMethod",
				method3.CreateDelegate<Func<object, string>>(null)("Test17"));
			Assert.AreEqual("<System.String>Test17_NullThis_InstanceMethod",
				method3.CreateDelegate<Func<string, object>>(null)("Test17"));
			Assert.AreEqual("<System.Object>Test17_NullThis_InstanceMethod",
				method3.CreateDelegate<Func<object, object>>(null)("Test17"));
			Assert.AreEqual("<System.Int32>17_NullThis_InstanceMethod",
				method3.CreateDelegate<Func<int, object>>(null)(17));
			Assert.AreEqual(null, method3.CreateDelegate<Func<int, int>>(null));
			Assert.AreEqual("<System.String>Test18_TC_InstanceMethod",
				method3.CreateDelegate<Func<string, string>>(tc)("Test18"));
			Assert.AreEqual("<System.String>Test19_TC_InstanceMethod",
				method3.CreateDelegate<Func<string, object>>(tc)("Test19"));
			Assert.AreEqual("<System.Object>Test20_TC_InstanceMethod",
				method3.CreateDelegate<Func<object, object>>(tc)("Test20"));
			Assert.AreEqual("<System.Int32>21_TC_InstanceMethod",
				method3.CreateDelegate<Func<int, object>>(tc)(21));
			// 特殊实例方法。
			Assert.AreEqual("A_B_InstanceMethod",
				method4.CreateDelegate<Func<TestClass, string, string, int, string>>()(tc, "A", "B", 0));
			Assert.AreEqual("B", tc.Text);
			tc.Text = "XXX";
			Assert.AreEqual("A_B_InstanceMethod", method4.CreateDelegate<Func<string, string, int, string>>(tc)("A", "B", 0));
			Assert.AreEqual("B", tc.Text);
			value = "B";
			value2 = 0;
			tc.Text = "XXX";
			Assert.AreEqual("A_B_InstanceMethod", method4.CreateDelegate<TestDelegate>(tc)("A", ref value, out value2));
			Assert.AreEqual("InstanceMethodRef", value);
			Assert.AreEqual("B", tc.Text);
			Assert.AreEqual(101, value2);
		}

		/// <summary>
		/// 测试通过 ConstructorInfo 构造方法委托。
		/// </summary>
		[TestMethod]
		public void TestCtorDelegateFromConstructorInfo()
		{
			Type type = typeof(TestClass);
			ConstructorInfo method0 = type.GetConstructor(Type.EmptyTypes);
			ConstructorInfo method = type.GetConstructor(new Type[] { typeof(string) });
			ConstructorInfo method2 = type.GetConstructor(new Type[] { typeof(string), typeof(int) });
			// 普通的构造函数。
			Assert.AreEqual("NoParam", method0.CreateDelegate<Func<TestClass>>()().Text);
			Assert.AreEqual("NoParam", ((TestClass)method0.CreateDelegate<Func<object>>()()).Text);
			Assert.AreEqual(null, method0.CreateDelegate<Func<string, object>>(false));
			Assert.AreEqual(null, method0.CreateDelegate<Func<int>>(false));
			Assert.AreEqual("Test1", method.CreateDelegate<Func<string, TestClass>>()("Test1").Text);
			Assert.AreEqual("Test2", method.CreateDelegate<Func<object, TestClass>>()("Test2").Text);
			Assert.AreEqual(null, method.CreateDelegate<Func<string, string>>(false));
			Assert.AreEqual("Test3_10", method2.CreateDelegate<Func<string, int, TestClass>>()("Test3", 10).Text);
			Assert.AreEqual("Test4_10", method2.CreateDelegate<Func<string, ulong, TestClass>>()("Test4", 10UL).Text);
			Assert.AreEqual("Test5_10", method2.CreateDelegate<Func<string, short, TestClass>>()("Test5", (short)10).Text);
			Assert.AreEqual(null, method2.CreateDelegate<Func<string, string, TestClass>>(false));
			// 通用构造函数。
			Assert.AreEqual("NoParam", ((TestClass)method0.CreateDelegate()()).Text);
			Assert.AreEqual("Test6", ((TestClass)method.CreateDelegate()("Test6")).Text);
			Assert.AreEqual("Test7_20", ((TestClass)method2.CreateDelegate()("Test7", 20)).Text);
			AssertExt.ThrowsException(() => method0.CreateDelegate()("more args"), typeof(TargetParameterCountException));
			AssertExt.ThrowsException(() => method.CreateDelegate()(), typeof(TargetParameterCountException));
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
				type.CreateDelegate<Func<string, string>>("TestStaticMethod2")("Test13"));
			Assert.AreEqual("<System.Int32>14_StaticMethod",
				type.CreateDelegate<Func<int, string>>("TestStaticMethod2")(14));
			Assert.AreEqual("<System.String>Test15_StaticMethod",
				type.CreateDelegate<Func<string, string>>("TestStaticMethod2", null)("Test15"));
			Assert.AreEqual(null, type.CreateDelegate<Func<string, int>>("TestInstanceMethod2", DefaultBinder, false));
			// 特殊静态方法。
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestStaticMethod3")("A", "B", 0));
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<Func<string, string, int, string>>("TestStaticMethod3", null)("A", "B", 0));
			string value = "B";
			int value2;
			Assert.AreEqual("A_B_StaticMethod",
				type.CreateDelegate<TestDelegate>("TestStaticMethod3")("A", ref value, out value2));
			Assert.AreEqual("StaticMethodRef", value);
			Assert.AreEqual(101, value2);
			// 实例方法。
			TestClass tc = new TestClass();
			tc.Text = "TC";
			// 开放的实例方法。
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<TestClass, string>>("TestInstanceMethod")(tc));
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<object, object>>("TestInstanceMethod")(tc));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, int>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual("Test1_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethod")(tc, "Test1"));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, string, int>>("TestInstanceMethod", DefaultBinder, false));
			Assert.AreEqual("Test3_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, string>>("TestInstanceMethod")(tc, "Test3"));
			Assert.AreEqual("Test4_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, object>>("TestInstanceMethod")(tc, "Test4"));
			Assert.AreEqual("Test4_TC_InstanceMethod",
				type.CreateDelegate<Func<object, object, object>>("TestInstanceMethod")(tc, "Test4"));
			Assert.AreEqual("Test5_10_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, int, string>>("TestInstanceMethod")(tc, "Test5", 10));
			Assert.AreEqual("Test6_10_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, short, string>>("TestInstanceMethod")(tc, "Test6", 10));
			Assert.AreEqual("Test7_10_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, ulong, string>>("TestInstanceMethod")(tc, "Test7", 10UL));
			Assert.AreEqual("Test8_10_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, ulong, string>>("TestInstanceMethod")(tc, "Test8", 10UL));
			Assert.AreEqual("Test9_10_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, object, ulong, object>>("TestInstanceMethod")(tc, "Test9", 10UL));
			Assert.AreEqual(null,
				type.CreateDelegate<Func<TestClass, string, string, string>>("TestInstanceMethod", DefaultBinder, false));
			// 第一个参数封闭的实例方法。
			Assert.AreEqual("TC_InstanceMethod", type.CreateDelegate<Func<string>>("TestInstanceMethod", tc)());
			Assert.AreEqual("Test10_TC_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethod", tc)("Test10"));
			Assert.AreEqual("Test11_TC_InstanceMethod",
				type.CreateDelegate<Func<string, object>>("TestInstanceMethod", tc)("Test11"));
			Assert.AreEqual("Test11_TC_InstanceMethod",
				type.CreateDelegate<Func<object, object>>("TestInstanceMethod", tc)("Test11"));
			Assert.AreEqual(null,
				type.CreateDelegate<Func<int>>("TestInstanceMethod", tc, DefaultBinder, false));
			// 开放的泛型实例方法。
			Assert.AreEqual("<System.String>Test13_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, string, string>>("TestInstanceMethod2")(tc, "Test13"));
			Assert.AreEqual("<System.Int32>14_TC_InstanceMethod",
				type.CreateDelegate<Func<TestClass, int, string>>("TestInstanceMethod2")(tc, 14));
			Assert.AreEqual(null, type.CreateDelegate<Func<TestClass, string, int>>("TestInstanceMethod2", DefaultBinder, false));
			// 第一个参数封闭的泛型实例方法。
			Assert.AreEqual("<System.String>Test18_TC_InstanceMethod",
				type.CreateDelegate<Func<string, string>>("TestInstanceMethod2", tc)("Test18"));
			Assert.AreEqual("<System.String>Test19_TC_InstanceMethod",
				type.CreateDelegate<Func<string, object>>("TestInstanceMethod2", tc)("Test19"));
			Assert.AreEqual("<System.Object>Test20_TC_InstanceMethod",
				type.CreateDelegate<Func<object, object>>("TestInstanceMethod2", tc)("Test20"));
			Assert.AreEqual("<System.Int32>21_TC_InstanceMethod",
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
		private class TestClass
		{

			#region 静态和实例方法

			public static string TestStaticMethod()
			{
				return "StaticMethod";
			}
			public static string TestStaticMethod(string key)
			{
				return key + "_StaticMethod";
			}
			public static string TestStaticMethod(string key, int value)
			{
				return key + "_" + value.ToString() + "_StaticMethod";
			}
			public static string TestStaticMethod2<T>(T key)
			{
				return "<" + typeof(T) + ">" + key + "_StaticMethod";
			}
			public static string TestStaticMethod3(string key, ref string value, out int value2)
			{
				string oldValue = value;
				value = "StaticMethodRef";
				value2 = 101;
				return key + "_" + oldValue + "_StaticMethod";
			}
			public string Text;
			public string TestInstanceMethod()
			{
				if (this == null)
				{
					return "NullThis_InstanceMethod";
				}
				return Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(string key)
			{
				if (this == null)
				{
					return key + "_NullThis_InstanceMethod";
				}
				return key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethod(string key, int value)
			{
				return key + "_" + value.ToString() + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethod2<T>(T key)
			{
				if (this == null)
				{
					return "<" + typeof(T) + ">" + key + "_NullThis_InstanceMethod";
				}
				return "<" + typeof(T) + ">" + key + "_" + Text + "_InstanceMethod";
			}
			public string TestInstanceMethod3(string key, ref string value, out int value2)
			{
				Text = value;
				value = "InstanceMethodRef";
				value2 = 101;
				return key + "_" + Text + "_InstanceMethod";
			}

			#endregion // 静态和实例方法

			#region 构造函数

			public TestClass()
			{
				this.Text = "NoParam";
			}
			public TestClass(string key)
			{
				this.Text = key;
			}
			public TestClass(string key, int value)
			{
				this.Text = key + "_" + value.ToString();
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
