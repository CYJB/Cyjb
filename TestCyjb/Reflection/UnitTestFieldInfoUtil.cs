using System;
using System.Reflection;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Reflection
{
	/// <summary>
	/// <see cref="FieldInfoUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestFieldInfoUtil
	{
		private class TestClass
		{
			public string? Test1 = "";
			public int Test2 = 0;
			public int? Test3 = null;

			public static string? STest1 = "";
			public static int STest2 = 0;
			public static int? STest3 = null;

			public static void ResetStaticFields()
			{
				STest1 = "";
				STest2 = 0;
				STest3 = null;
			}
		}

		private delegate string TestDelegate1(TestClass instance, string key, ref string value, out int value2);
		private delegate string TestDelegate2(string key, ref string value, out int value2);
		private delegate TestClass TestDelegate3(string key, ref string value, out int value2);
		private delegate string TestDelegate4(ref string value, out int value2);
		private delegate TestClass TestDelegate5(ref string value, out int value2);

		/// <summary>
		/// 对 <see cref="FieldInfoUtil.PowerDelegate(FieldInfo, Type)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(string), "123")]
		[DataRow("Test1", typeof(object), "456")]
		[DataRow("Test2", typeof(int), 100)]
		[DataRow("Test2", typeof(long), 100L)]
		[DataRow("Test2", typeof(char), (char)100)]
		[DataRow("Test3", typeof(int), 100)]
		[DataRow("Test3", typeof(long), 100L)]
		[DataRow("Test3", typeof(char), (char)100)]
		public void TestCreateOpenInstanceDelegate(string fieldName, Type valueType, object? value)
		{
			FieldInfo fieldInfo = typeof(TestClass).GetField(fieldName)!;
			Delegate? setField = fieldInfo.PowerDelegate(typeof(Action<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(setField);
			Delegate? getField = fieldInfo.PowerDelegate(typeof(Func<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(getField);
			TestClass instance = new();
			setField.DynamicInvoke(instance, value);
			Assert.AreEqual(value, getField.DynamicInvoke(instance));

			// 额外的入参会被忽略
			setField = fieldInfo.PowerDelegate(typeof(Action<,,>).MakeGenericType(typeof(TestClass), valueType, typeof(object)));
			Assert.IsNotNull(setField);
			getField = fieldInfo.PowerDelegate(typeof(Func<,,>).MakeGenericType(typeof(TestClass), typeof(object), valueType));
			Assert.IsNotNull(getField);
			instance = new();
			setField.DynamicInvoke(instance, value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(instance, new object()));
		}

		/// <summary>
		/// 对 <see cref="FieldInfoUtil.PowerDelegate(FieldInfo, Type)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("STest1", typeof(string), "123")]
		[DataRow("STest1", typeof(object), "456")]
		[DataRow("STest2", typeof(int), 100)]
		[DataRow("STest2", typeof(long), 100L)]
		[DataRow("STest2", typeof(char), (char)100)]
		[DataRow("STest3", typeof(int), 100)]
		[DataRow("STest3", typeof(long), 100L)]
		[DataRow("STest3", typeof(char), (char)100)]
		public void TestCreateOpenStaticDelegate(string fieldName, Type valueType, object? value)
		{
			FieldInfo fieldInfo = typeof(TestClass).GetField(fieldName)!;
			Delegate? setField = fieldInfo.PowerDelegate(typeof(Action<>).MakeGenericType(valueType));
			Assert.IsNotNull(setField);
			Delegate? getField = fieldInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(value);
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			setField = fieldInfo.PowerDelegate(typeof(Action<,>).MakeGenericType(valueType, typeof(object)));
			Assert.IsNotNull(setField);
			getField = fieldInfo.PowerDelegate(typeof(Func<,>).MakeGenericType(typeof(object), valueType));
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}

		/// <summary>
		/// 对 <see cref="FieldInfoUtil.PowerDelegate(FieldInfo, Type, object?)"/> 创建实例字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(string), "123")]
		[DataRow("Test1", typeof(object), "456")]
		[DataRow("Test2", typeof(int), 100)]
		[DataRow("Test2", typeof(long), 100L)]
		[DataRow("Test2", typeof(char), (char)100)]
		[DataRow("Test3", typeof(int), 100)]
		[DataRow("Test3", typeof(long), 100L)]
		[DataRow("Test3", typeof(char), (char)100)]
		public void TestCreateClosedInstanceDelegate(string fieldName, Type valueType, object? value)
		{
			FieldInfo fieldInfo = typeof(TestClass).GetField(fieldName)!;
			TestClass instance = new();
			Delegate? setField = fieldInfo.PowerDelegate(typeof(Action<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(setField);
			Delegate? getField = fieldInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(getField);
			setField.DynamicInvoke(value);
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			instance = new();
			setField = fieldInfo.PowerDelegate(typeof(Action<,>).MakeGenericType(valueType, typeof(object)), instance);
			Assert.IsNotNull(setField);
			getField = fieldInfo.PowerDelegate(typeof(Func<,>).MakeGenericType(typeof(object), valueType), instance);
			Assert.IsNotNull(getField);
			setField.DynamicInvoke(value, new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}

		/// <summary>
		/// 对 <see cref="FieldInfoUtil.PowerDelegate(FieldInfo, Type, object?)"/> 创建静态字段委托进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("STest1", typeof(string), "123")]
		[DataRow("STest1", typeof(object), "456")]
		[DataRow("STest2", typeof(int), 100)]
		[DataRow("STest2", typeof(long), 100L)]
		[DataRow("STest2", typeof(char), (char)100)]
		[DataRow("STest3", typeof(int), 100)]
		[DataRow("STest3", typeof(long), 100L)]
		[DataRow("STest3", typeof(char), (char)100)]
		public void TestClosedStaticDelegate(string fieldName, Type valueType, object? value)
		{
			FieldInfo fieldInfo = typeof(TestClass).GetField(fieldName)!;
			Delegate? setField = fieldInfo.PowerDelegate(typeof(Action), value);
			Assert.IsNotNull(setField);
			Delegate? getField = fieldInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType), value);
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke();
			Assert.AreEqual(value, getField.DynamicInvoke());

			// 额外的入参会被忽略
			setField = fieldInfo.PowerDelegate(typeof(Action<object>), value);
			Assert.IsNotNull(setField);
			getField = fieldInfo.PowerDelegate(typeof(Func<,>).MakeGenericType(typeof(object), valueType), value);
			Assert.IsNotNull(getField);
			TestClass.ResetStaticFields();
			setField.DynamicInvoke(new object());
			Assert.AreEqual(value, getField.DynamicInvoke(new object()));
		}
	}
}

