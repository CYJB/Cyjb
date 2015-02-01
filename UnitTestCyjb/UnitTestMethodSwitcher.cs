using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="MethodSwitcher"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestMethodSwitcher
	{
		/// <summary>
		/// 对 <see cref="MethodSwitcher.Create{TDelegate}(Delegate[])"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestCreateFromDelegates()
		{
			Func<object, string> switcher = MethodSwitcher.Create<Func<object, string>>(
				(Func<int, string>)TestClass.StaticMethod,
				(Func<string, string>)TestClass.StaticMethod,
				(Func<int[], string>)TestClass.StaticMethod,
				(Func<Array, string>)TestClass.StaticMethod,
				(Func<object, string>)TestClass.StaticMethod);
			var valInt = 10;
			Assert.AreEqual(TestClass.StaticMethod(valInt), switcher(valInt));
			var valLong = 10;
			Assert.AreEqual(TestClass.StaticMethod(valLong), switcher(valLong));
			var valStr = "10";
			Assert.AreEqual(TestClass.StaticMethod(valStr), switcher(valStr));
			var valIntArr = new int[2];
			Assert.AreEqual(TestClass.StaticMethod(valIntArr), switcher(valIntArr));
			var valLongArr = new long[2];
			Assert.AreEqual(TestClass.StaticMethod(valLongArr), switcher(valLongArr));
			string[] valStrArr = new string[3];
			Assert.AreEqual(TestClass.StaticMethod(valStrArr), switcher(valStrArr));
			object valObj = new object();
			Assert.AreEqual(TestClass.StaticMethod(valObj), switcher(valObj));
		}
		/// <summary>
		/// 对 <see cref="MethodSwitcher.Create{TDelegate}(Type)"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestCreateFromType()
		{
			Func<object, string> switcher = MethodSwitcher.Create<Func<object, string>>(typeof (TestClass));
			var valInt = 10;
			Assert.AreEqual(TestClass.StaticMethod(valInt), switcher(valInt));
			var valLong = 10;
			Assert.AreEqual(TestClass.StaticMethod(valLong), switcher(valLong));
			var valStr = "10";
			Assert.AreEqual(TestClass.StaticMethod(valStr), switcher(valStr));
			var valIntArr = new int[2];
			Assert.AreEqual(TestClass.StaticMethod(valIntArr), switcher(valIntArr));
			var valLongArr = new long[2];
			Assert.AreEqual(TestClass.StaticMethod(valLongArr), switcher(valLongArr));
			string[] valStrArr = new string[3];
			Assert.AreEqual(TestClass.StaticMethod(valStrArr), switcher(valStrArr));
			object valObj = new object();
			Assert.AreEqual(TestClass.StaticMethod(valObj), switcher(valObj));
		}
		/// <summary>
		/// 对 <see cref="MethodSwitcher.Create{TDelegate}(Object)"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestCreateFromObject()
		{
			TestClass obj = new TestClass();
			obj.Value = "XXX";
			Func<object, string> switcher = MethodSwitcher.Create<Func<object, string>>(obj, "Ins");
			var valInt = 10;
			Assert.AreEqual(obj.InstanceMethod(valInt), switcher(valInt));
			var valLong = 10;
			Assert.AreEqual(obj.InstanceMethod(valLong), switcher(valLong));
			var valStr = "10";
			Assert.AreEqual(obj.InstanceMethod(valStr), switcher(valStr));
			var valIntArr = new int[2];
			Assert.AreEqual(obj.InstanceMethod(valIntArr), switcher(valIntArr));
			var valLongArr = new long[2];
			Assert.AreEqual(obj.InstanceMethod(valLongArr), switcher(valLongArr));
			string[] valStrArr = new string[3];
			Assert.AreEqual(obj.InstanceMethod(valStrArr), switcher(valStrArr));
			object valObj = new object();
			Assert.AreEqual(obj.InstanceMethod(valObj), switcher(valObj));
		}
		private class TestClass
		{
			[Processor]
			public static string StaticMethod(int v) { return "Int32_" + v; }
			[Processor]
			public static string StaticMethod(string v) { return "String_" + v; }
			[Processor]
			public static string StaticMethod(int[] v) { return "Int32[]_" + v.Length; }
			[Processor]
			public static string StaticMethod(Array v) { return "Array_" + v.Length; }
			[Processor]
			public static string StaticMethod(object v) { return "Object_" + v; }
			public string Value;
			[Processor("Ins")]
			public string InstanceMethod(int v) { return "Int32_" + Value + "_" + v; }
			[Processor("Ins")]
			public string InstanceMethod(string v) { return "String_" + Value + "_" + v; }
			[Processor("Ins")]
			public string InstanceMethod(int[] v) { return "Int32[]_" + Value + "_" + v.Length; }
			[Processor("Ins")]
			public string InstanceMethod(Array v) { return "Array_" + Value + "_" + v.Length; }
			[Processor("Ins")]
			public string InstanceMethod(object v) { return "Object_" + Value + "_" + v; }
		}
	}
}
