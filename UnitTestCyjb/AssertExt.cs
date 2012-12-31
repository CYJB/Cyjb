using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// 表示 Assert 类的相关扩展方法。
	/// </summary>
	internal static class AssertExt
	{
		/// <summary>
		/// 验证方法是否抛出了指定的异常。如果未抛出异常或不在期待的类型中，
		/// 则断言失败。
		/// </summary>
		/// <param name="action">要测试的方法。</param>
		/// <param name="expectedException">期待抛出的异常集合。</param>
		public static void ThrowsException(Action action, params Type[] expectedException)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				// 检查是否是期望的异常。
				for (int i = 0; i < expectedException.Length; i++)
				{
					if (expectedException[i].IsInstanceOfType(ex))
					{
						return;
					}
				}
				throw ex;
			}
			// 未发生异常。
			Assert.Fail("没有抛出期望的异常 {0}", string.Join(", ", expectedException.AsEnumerable<Type>()));
		}
	}
}
