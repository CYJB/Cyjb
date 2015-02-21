using System;
using System.Collections.Generic;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.Reflection
{
	/// <summary>
	/// <see cref="TypeExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeExt
	{
		/// <summary>
		/// 对 <see cref="TypeExt.CloseDefinitionFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestCloseDefinitionFrom()
		{
			Assert.AreEqual(null, typeof(IEnumerable<int>).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.AreEqual(null, typeof(object).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.AreEqual(null, typeof(IEnumerable<int>).CloseDefinitionFrom(typeof(IEnumerable<int>)));

			Assert.AreEqual(typeof(IEnumerable<>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.AreEqual(typeof(IEnumerable<int>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(IEnumerable<int>)));
			Assert.AreEqual(typeof(IEnumerable<KeyValuePair<int, string>>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(Dictionary<int, string>)));
			Assert.AreEqual(typeof(IDictionary<int, string>), typeof(IDictionary<,>).CloseDefinitionFrom(typeof(Dictionary<int, string>)));

			Type closedType = typeof(TestBaseClass<,>).CloseDefinitionFrom(typeof(TestSubClass<>));
			Type[] genericArgs = closedType.GetGenericArguments();
			Assert.AreEqual(2, genericArgs.Length);
			Assert.AreEqual(typeof(int), genericArgs[0]);
			Assert.IsTrue(genericArgs[1].IsGenericParameter);

			Assert.AreEqual(typeof(TestBaseClass<int, string>), typeof(TestBaseClass<,>).CloseDefinitionFrom(typeof(TestSubClass<string>)));
		}
		private class TestBaseClass<T1, T2> { }
		private class TestSubClass<T> : TestBaseClass<int, T> { }
	}
}
