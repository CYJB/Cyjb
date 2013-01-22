using System;
using System.Reflection;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.PowerBinder"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestPowerBinder
	{
		/// <summary>
		/// 测试调用字段。
		/// </summary>
		[TestMethod]
		public void TestInvokeField()
		{
			Type type = typeof(TestClass);
			Type subType = typeof(TestSubClass);
			TestSubClass targetSub = new TestSubClass();
			TestClass target = targetSub;
			// 测试父类。
			// DefaultBinder
			Assert.AreEqual("TestClass", type.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual((short)10, type.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual(100L, type.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual("TestClass4", type.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			// CastBinder
			Assert.AreEqual("TestClass", type.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual((short)10, type.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual(100L, type.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual("TestClass4", type.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			// 测试子类。
			// DefaultBinder
			Assert.AreEqual("TestSubClass", subType.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual(11, subType.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual(101, subType.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			Assert.AreEqual(102, subType.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.DefaultBinder,
				target, new object[0]));
			// CastBinder
			Assert.AreEqual("TestSubClass", subType.InvokeMember("TestField", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual(11, subType.InvokeMember("TestField2", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual(101, subType.InvokeMember("TestField3", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			Assert.AreEqual(102, subType.InvokeMember("TestField4", BindingFlags.GetField, PowerBinder.CastBinder,
				target, new object[0]));
			// 测试设置父类字段。
			// DefaultBinder
			type.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { "TestClass0" });
			Assert.AreEqual("TestClass0", target.TestField);
			type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { (short)20 });
			Assert.AreEqual((short)20, target.TestField2);
			AssertExt.ThrowsException(() =>
				type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 20 }), typeof(ArgumentException), typeof(InvalidCastException));
			type.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 200 });
			Assert.AreEqual(200L, target.TestField3);
			type.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { "TestClass44" });
			Assert.AreEqual("TestClass44", target.TestField4);
			// CastBinder
			type.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { "TestClass0" });
			Assert.AreEqual("TestClass0", target.TestField);
			type.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 20 });
			Assert.AreEqual((short)20, target.TestField2);
			type.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 200 });
			Assert.AreEqual(200L, target.TestField3);
			type.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { "TestClass44" });
			Assert.AreEqual("TestClass44", target.TestField4);
			// 测试设置子类字段。
			// DefaultBinder
			subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { "TestSubClass0" });
			Assert.AreEqual("TestSubClass0", targetSub.TestField);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 21 });
			Assert.AreEqual((short)21, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 201 });
			Assert.AreEqual(201, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 202 });
			Assert.AreEqual(202, targetSub.TestField4);
			// CastBinder
			subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { "TestSubClass0" });
			Assert.AreEqual("TestSubClass0", targetSub.TestField);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 21 });
			Assert.AreEqual((short)21, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 201 });
			Assert.AreEqual(201, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 202 });
			Assert.AreEqual(202, targetSub.TestField4);
			// 测试设置子类和父类字段。
			// DefaultBinder
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { (short)22 });
			Assert.AreEqual((short)22, target.TestField2);
			AssertExt.ThrowsException(() =>
				subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 23L }),
				typeof(MissingFieldException), typeof(ArgumentException), typeof(InvalidCastException));
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { 202L });
			Assert.AreEqual(202L, target.TestField3);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { (short)203 });
			Assert.AreEqual(203, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { "TestClass00" });
			Assert.AreEqual("TestClass00", target.TestField4);
			// CastBinder
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { (short)22 });
			Assert.AreEqual((short)22, target.TestField2);
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 23L });
			Assert.AreEqual(23, targetSub.TestField2);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 202L });
			Assert.AreEqual(202L, target.TestField3);
			subType.InvokeMember("TestField3", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { (short)203 });
			Assert.AreEqual(203, targetSub.TestField3);
			subType.InvokeMember("TestField4", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { "TestClass00" });
			Assert.AreEqual("TestClass00", target.TestField4);
			// 测试强制类型转换。
			// DefaultBinder
			AssertExt.ThrowsException(() =>
				subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.DefaultBinder, target,
				new object[1] { Tristate.True }),
				typeof(MissingFieldException), typeof(ArgumentException), typeof(InvalidCastException));
			// CastBinder
			subType.InvokeMember("TestField2", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { Tristate.True });
			Assert.AreEqual((int)Tristate.True, targetSub.TestField2);
			AssertExt.ThrowsException(() =>
				subType.InvokeMember("TestField", BindingFlags.SetField, PowerBinder.CastBinder, target,
				new object[1] { 22 }), typeof(MissingFieldException));
		}
		/// <summary>
		/// 测试调用方法。
		/// </summary>
		[TestMethod]
		public void TestInvokeMethod()
		{
			Type type = typeof(TestSubClass);
			BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod;
			BindingFlags bindingOptFlags = bindingFlags | BindingFlags.OptionalParamBinding;
			// 测试完整的调用。
			Assert.AreEqual("()", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[0]));
			Assert.AreEqual("(int 10)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10 }));
			Assert.AreEqual("(int 10)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.CastBinder, null,
				new object[] { 10 }));
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10, 20 }));
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.CastBinder, null,
				new object[] { 10, 20 }));
			Assert.AreEqual("(int 30, string , bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, null, true }));
			// 测试完整的调用与类型转换。
			Type[] invalidCastExceptions = new Type[] { 
				typeof(ArgumentException), typeof(InvalidCastException), typeof(MissingMethodException) };
			Assert.AreEqual("(int 10)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10 }));
			Assert.AreEqual("(int 10)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.CastBinder, null,
				new object[] { (short)10 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10L }), invalidCastExceptions);
			Assert.AreEqual("(int 10)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.CastBinder, null,
				new object[] { 10L }));
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10, (byte)20 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10UL, 20L }), invalidCastExceptions);
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10, 20L }), invalidCastExceptions);
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod", bindingFlags, PowerBinder.CastBinder, null,
				new object[] { 10UL, 20L }));
			// 测试命名参数。
			Assert.AreEqual("(int 30, string str, bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true }));
			Assert.AreEqual("(int 30, string str, bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { "str", 30, true }, null, null, new string[] { "value2", "value1", "value3" }));
			Assert.AreEqual("(int 30, string str, bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { "str", 30, true }, null, null, new string[] { "value2" }));
			Assert.AreEqual("(int 30, string str, bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { true, "str", 30 }, null, null, new string[] { "value3", "value2", "value1" }));
			Assert.AreEqual("(int 30, string str, bool True)",
				type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { true, "str", 30 }, null, null, new string[] { "value3", "value2" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10 }, null, null, new string[] { "values" }), typeof(MissingMethodException));
			// 测试默认参数和 params 参数。
			Assert.AreEqual("(int 30, string str, bool True, int[] 1,2,3)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true, 1, 2, 3 }));
			Assert.AreEqual("(int 30, string str, bool True, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true }));
			Assert.AreEqual("(int 30, string str, bool False, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str" }));
			Assert.AreEqual("(int 30, string text, bool False, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30 }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[0]), typeof(MissingMethodException));
			// 测试命名参数、默认参数和 params 参数。
			Assert.AreEqual("(int 30, string str, bool True, int[] 1,2,3)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true, 1, 2, 3 }, null, null,
				new string[] { "value1", "value2", "value3", "value4" }));
			Assert.AreEqual("(int 30, string str, bool True, int[] 1,2,3)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true, 1, 2, 3 }, null, null,
				new string[] { "value1", "value2", "value3" }));
			Assert.AreEqual("(int 30, string str, bool True, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", true }, null, null,
				new string[] { "value1", "value2", "value3" }));
			Assert.AreEqual("(int 30, string str, bool True, int[] 1,2,3)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", 1, true, 2, 3 }, null, null,
				new string[] { "value1", "value2", "value4" }));
			Assert.AreEqual("(int 30, string str, bool False, int[] 1)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30, "str", new int[] { 1 } }, null, null,
				new string[] { "value1", "value2", "value4" }));
			Assert.AreEqual("(int 30, string text, bool True, int[] 1)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { new int[] { 1 }, true, 30 }, null, null,
				new string[] { "value4", "value3", "value1" }));
			Assert.AreEqual("(int 30, string text, bool False, int[] 1)",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { new int[] { 1 }, 30 }, null, null,
				new string[] { "value4", "value1" }));
			Assert.AreEqual("(int 30, string text, bool True, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { true, 30 }, null, null,
				new string[] { "value3", "value1" }));
			Assert.AreEqual("(int 30, string text, bool False, int[] )",
				type.InvokeMember("TestMethod2", bindingOptFlags, PowerBinder.DefaultBinder, null,
				new object[] { 30 }, null, null,
				new string[] { "value1" }));
			// 测试泛型方法。
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10, 20 }));
			Assert.AreEqual("(int 10, int 20)", type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10, 20 }));
			Assert.AreEqual("<System.Int16>(10, 20)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10, (short)20 }));
			Assert.AreEqual("<System.Int64>(10, 20)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10, (long)20 }));
			Assert.AreEqual("(int 10, string str)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10, "str" }));
			Assert.AreEqual("(int 10, string str)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (short)10, "str" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { (long)10, "str" }), typeof(MissingMethodException));
			Assert.AreEqual("<System.String>(text, str)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { "text", "str" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { "str", 10 }), typeof(MissingMethodException));
			Assert.AreEqual("<System.String, System.Int32>(text, str, 10)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { "text", "str", 10 }));
			Assert.AreEqual("<System.Int32, System.Int32>(10, 20, 30)",
				type.InvokeMember("TestMethod3", bindingFlags, PowerBinder.DefaultBinder, null,
				new object[] { 10, 20, 30 }));
			// 测试选择方法。
			BindingFlags bindingInsFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;
			BindingFlags bindingInsOptFlags = bindingInsFlags | BindingFlags.OptionalParamBinding;
			TestSubClass subClass = new TestSubClass();
			Assert.AreEqual("sub(int 10, int 20)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, 20 }));
			Assert.AreEqual("(bool True, string str)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { true, "str" }));
			Assert.AreEqual("(int 10, string str)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str" }));
			AssertExt.ThrowsException(() => type.InvokeMember("TestMethod4", bindingInsOptFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str" }), typeof(AmbiguousMatchException));
			Assert.AreEqual("(int 10, string str, bool False)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str", false }));
			Assert.AreEqual("(int 10, string str, int[] 1)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str", 1 }));
			Assert.AreEqual("(int 10, string str, int[] 1)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str", new int[] { 1 } }));
			Assert.AreEqual("(int 10, string str, int[] 1,2)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.CastBinder, subClass,
				new object[] { 10, "str", 1, 2 }));
			Assert.AreEqual("<System.Boolean>(True, True)",
				type.InvokeMember("TestMethod4", bindingInsFlags, PowerBinder.DefaultBinder, subClass,
				new object[] { true, true }));
		}
		private class TestClass
		{
			public string TestField = "TestClass";
			public short TestField2 = 10;
			public long TestField3 = 100;
			public string TestField4 = "TestClass4";
			public string TestProperty { get; set; }
			public short TestProperty2 { get; set; }
			public long TestProperty3 { get; set; }
			public string TestProperty4 { get; set; }
			public object TestMethod4(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public object TestMethod4(bool value1, string value2)
			{
				return string.Concat("(bool ", value1, ", string ", value2, ")");
			}
		}
		private class TestSubClass : TestClass
		{
			public new string TestField = "TestSubClass";
			public new int TestField2 = 11;
			public new int TestField3 = 101;
			public new int TestField4 = 102;
			public new string TestProperty { get; set; }
			public new int TestProperty2 { get; set; }
			public new int TestProperty3 { get; set; }
			public new int TestProperty4 { get; set; }
			public static object TestMethod()
			{
				return "()";
			}
			public static object TestMethod(int value)
			{
				return string.Concat("(int ", value, ")");
			}
			public static object TestMethod(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public static object TestMethod(int value1, string value2, bool value3)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3, ")");
			}
			public static object TestMethod2(int value1, string value2 = "text", bool value3 = false, params int[] value4)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3,
					", int[] ", string.Join(",", value4), ")");
			}
			public static object TestMethod3(int value1, int value2)
			{
				return string.Concat("(int ", value1, ", int ", value2, ")");
			}
			public static object TestMethod3(int value1, string value2)
			{
				return string.Concat("(int ", value1, ", string ", value2, ")");
			}
			public static object TestMethod3<T>(T value1, T value2)
			{
				return string.Concat("<", typeof(T), ">(", value1, ", ", value2, ")");
			}
			public static object TestMethod3<T1, T2>(T1 value1, T1 value2, T2 value3)
			{
				return string.Concat("<", typeof(T1), ", ", typeof(T2), ">(", value1, ", ", value2, ", ", value3, ")");
			}
			public new object TestMethod4(int value1, int value2)
			{
				return string.Concat("sub(int ", value1, ", int ", value2, ")");
			}
			public object TestMethod4(int value1, string value2)
			{
				return string.Concat("(int ", value1, ", string ", value2, ")");
			}
			public object TestMethod4(int value1, string value2, bool value3 = true)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", bool ", value3, ")");
			}
			public object TestMethod4(int value1, string value2, params int[] value3)
			{
				return string.Concat("(int ", value1, ", string ", value2, ", int[] ", string.Join(",", value3), ")");
			}
			public object TestMethod4<T>(T value1, T value2)
			{
				return string.Concat("<", typeof(T), ">(", value1, ", ", value2, ")");
			}
		}
	}
}
