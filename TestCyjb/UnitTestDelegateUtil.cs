using System;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// <see cref="DelegateUtil"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestDelegateUtil
{
	private class TestClass
	{
#pragma warning disable CA1822 // 将成员标记为 static
		public bool Test1() { return true; }
		public long Test2(int a) { return a; }
#pragma warning restore CA1822 // 将成员标记为 static

		public static bool STest1() { return true; }
		public static long STest2(int a) { return a; }
		public static int STest3(TestClass intance) { return 0; }
	}

	/// <summary>
	/// 对 <see cref="DelegateUtil.Wrap"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestWrap()
	{
		TestClass instance = new();

		Func<TestClass, bool> test1 = (TestClass i) => i.Test1();
		Assert.AreEqual(test1(instance), test1.Wrap<Func<object, object>>()!(instance));
		Func<TestClass, int, long> test2 = (TestClass i, int a) => i.Test2(a);
		Assert.AreEqual(test2(instance, 10), test2.Wrap<Func<object, object, object>>()!(instance, 10));

		Func<bool> test3 = () => instance.Test1();
		Assert.AreEqual(test3(), test3.Wrap<Func<object>>()!());
		Func<int, long> test4 = (int a) => instance.Test2(a);
		Assert.AreEqual(test4(11), test4.Wrap<Func<object, object>>()!(11));

		Func<bool> test5 = TestClass.STest1;
		Assert.AreEqual(test5(), test5.Wrap<Func<object>>()!());
		Func<int, long> test6 = TestClass.STest2;
		Assert.AreEqual(test6(12), test6.Wrap<Func<object, object>>()!(12));
		Func<TestClass, int> test7 = TestClass.STest3;
		Assert.AreEqual(test7(instance), test7.Wrap<Func<object, short>>()!(instance));
	}
}
