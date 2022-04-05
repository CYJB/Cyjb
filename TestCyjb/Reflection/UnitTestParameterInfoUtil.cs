using System.Reflection;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Reflection
{
	/// <summary>
	/// <see cref="ParameterInfoUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestParameterInfoUtil
	{
#pragma warning disable IDE0060 // 删除未使用的参数
		private static void TestMethod(int a, string[] b, params int[] c) { }
#pragma warning restore IDE0060 // 删除未使用的参数

		/// <summary>
		/// 对 <see cref="ParameterInfoUtil.IsParamArray"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsParamArray()
		{
			MethodInfo method = GetType().GetMethod(nameof(TestMethod), BindingFlags.NonPublic | BindingFlags.Static)!;
			ParameterInfo[] parameters = method.GetParameters();
			Assert.IsFalse(parameters[0].IsParamArray());
			Assert.IsFalse(parameters[1].IsParamArray());
			Assert.IsTrue(parameters[2].IsParamArray());
		}
	}
}
