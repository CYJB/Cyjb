using System;
using System.Reflection;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Reflection
{
	/// <summary>
	/// <see cref="PropertyInfoUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestPropertyInfoUtil
	{
		private class TestClass
		{
			public static string? STest1 { get; set; }
			public static int STest2 { get; set; }
			public static int? STest3 { get; set; }

			public string? Test1 { get; set; }
			public int Test2 { get; set; }
			public int? Test3 { get; set; }
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
				STest1 = "";
				STest2 = 0;
				STest3 = null;
			}
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.GetIndexParameterTypes"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1")]
		[DataRow("Item", typeof(int))]
		[DataRow("Item", typeof(int), typeof(string))]
		public void TestGetIndexParameterTypes(string propertyName, params Type[] types)
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty(propertyName, types)!;
			CollectionAssert.AreEqual(types, propertyInfo.GetIndexParameterTypes());
		}

		private class BaseClass1
		{
			public virtual int Test1 { get; set; }
			public virtual int Test2 { get; set; }
		}
		private class BaseClass2 : BaseClass1
		{
			public new int Test2 { get; set; }
			public virtual int Test3 { get; set; }
		}
		private class BaseClass3 : BaseClass2
		{
			public override int Test3 { get; set; }
			public virtual int Test4 { get; set; }
			public virtual int Test5 { get; set; }
		}
		private class SubClass : BaseClass3
		{
			public override int Test4 { get; set; }
			public new int Test5 { get; set; }
			public int Test6 { get; set; }
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.GetBaseDefinition"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("Test1", typeof(BaseClass1))]
		[DataRow("Test2", typeof(BaseClass2))]
		[DataRow("Test3", typeof(BaseClass2))]
		[DataRow("Test4", typeof(BaseClass3))]
		[DataRow("Test5", typeof(SubClass))]
		[DataRow("Test6", typeof(SubClass))]
		public void TestGetBaseDefinition(string propertyName, Type baseDefinitionType)
		{
			PropertyInfo propertyInfo = typeof(SubClass).GetProperty(propertyName)!;
			Assert.AreEqual(baseDefinitionType, propertyInfo.GetBaseDefinition().DeclaringType);
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.PowerDelegate(PropertyInfo, Type)"/> 创建实例字段委托进行测试。
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
		public void TestCreateOpenInstanceDelegate(string propertyName, Type valueType, object? value)
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty(propertyName)!;
			Delegate? setProperty = propertyInfo.PowerDelegate(typeof(Action<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = propertyInfo.PowerDelegate(typeof(Func<,>).MakeGenericType(typeof(TestClass), valueType));
			Assert.IsNotNull(getProperty);
			TestClass instance = new();
			setProperty.DynamicInvoke(instance, value);
			Assert.AreEqual(value, getProperty.DynamicInvoke(instance));
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.PowerDelegate(PropertyInfo, Type)"/> 创建静态字段委托进行测试。
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
		public void TestCreateOpenStaticDelegate(string propertyName, Type valueType, object? value)
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty(propertyName)!;
			Delegate? setProperty = propertyInfo.PowerDelegate(typeof(Action<>).MakeGenericType(valueType));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = propertyInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getProperty);
			TestClass.ResetStaticProperties();
			setProperty.DynamicInvoke(value);
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.PowerDelegate(PropertyInfo, Type, object?)"/> 创建实例字段委托进行测试。
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
		public void TestCreateClosedInstanceDelegate(string propertyName, Type valueType, object? value)
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty(propertyName)!;
			TestClass instance = new();
			Delegate? setProperty = propertyInfo.PowerDelegate(typeof(Action<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = propertyInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(value);
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.PowerDelegate(PropertyInfo, Type, object?)"/> 创建静态字段委托进行测试。
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
		public void TestClosedStaticDelegate(string propertyName, Type valueType, object? value)
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty(propertyName)!;
			Delegate? setProperty = propertyInfo.PowerDelegate(typeof(Action), value);
			Assert.IsNotNull(setProperty);
			// 封闭委托不支持获取属性
			Delegate? getProperty = propertyInfo.PowerDelegate(typeof(Func<>).MakeGenericType(valueType));
			Assert.IsNotNull(getProperty);
			TestClass.ResetStaticProperties();
			setProperty.DynamicInvoke();
			Assert.AreEqual(value, getProperty.DynamicInvoke());
		}

		/// <summary>
		/// 对 <see cref="PropertyInfoUtil.PowerDelegate"/> 创建索引参数委托进行测试。
		/// </summary>
		[TestMethod]
		public void TestItem()
		{
			PropertyInfo propertyInfo = typeof(TestClass).GetProperty("Item", new Type[] { typeof(int) })!;

			Delegate? setProperty = propertyInfo.PowerDelegate(typeof(Action<TestClass, int, string>));
			Assert.IsNotNull(setProperty);
			Delegate? getProperty = propertyInfo.PowerDelegate(typeof(Func<TestClass, int, string>));
			Assert.IsNotNull(getProperty);
			TestClass instance = new();
			setProperty.DynamicInvoke(instance, 1, "123");
			Assert.AreEqual("123", getProperty.DynamicInvoke(instance, 1));

			setProperty = propertyInfo.PowerDelegate(typeof(Action<int, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = propertyInfo.PowerDelegate(typeof(Func<int, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(2, "456");
			Assert.AreEqual("456", getProperty.DynamicInvoke(2));

			propertyInfo = typeof(TestClass).GetProperty("Item", new Type[] { typeof(int), typeof(int) })!;

			setProperty = propertyInfo.PowerDelegate(typeof(Action<TestClass, int, int, string>));
			Assert.IsNotNull(setProperty);
			getProperty = propertyInfo.PowerDelegate(typeof(Func<TestClass, int, int, string>));
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(instance, 1, 2, "789");
			Assert.AreEqual("789", getProperty.DynamicInvoke(instance, 1, 2));

			setProperty = propertyInfo.PowerDelegate(typeof(Action<int, int, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = propertyInfo.PowerDelegate(typeof(Func<int, int, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(3, 4, "10");
			Assert.AreEqual("10", getProperty.DynamicInvoke(3, 4));

			propertyInfo = typeof(TestClass).GetProperty("Item", new Type[] { typeof(int), typeof(string) })!;

			setProperty = propertyInfo.PowerDelegate(typeof(Action<TestClass, int, string, string>));
			Assert.IsNotNull(setProperty);
			getProperty = propertyInfo.PowerDelegate(typeof(Func<TestClass, int, string, string>));
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(instance, 1, "4", "11");
			Assert.AreEqual("11", getProperty.DynamicInvoke(instance, 1, "4"));

			setProperty = propertyInfo.PowerDelegate(typeof(Action<int, string, string>), instance);
			Assert.IsNotNull(setProperty);
			getProperty = propertyInfo.PowerDelegate(typeof(Func<int, string, string>), instance);
			Assert.IsNotNull(getProperty);
			setProperty.DynamicInvoke(3, "5", "12");
			Assert.AreEqual("12", getProperty.DynamicInvoke(3, "5"));
		}
	}
}

