using System;
using System.Collections.Generic;
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
		public static void AreEqual<T>(T[] expected, T[] actual)
		{
			if (expected == null)
			{
				if (actual != null)
				{
					Assert.Fail("实际的数组 {{{0}}} 不为 null", string.Join(", ", actual));
				}
			}
			else
			{
				if (actual == null)
				{
					Assert.Fail("实际的数组为 null");
				}
				else
				{
					if (expected.Length != actual.Length)
					{
						Assert.Fail("数组长度 {0} 不是期望的 {1}", actual.Length, expected.Length);
					}
					for (int i = 0; i < expected.Length; i++)
					{
						if (!EqualityComparer<T>.Default.Equals(expected[i], actual[i]))
						{
							Assert.Fail("期望得到 {{{0}}}，而实际得到的是 {{{1}}}",
								string.Join(", ", expected), string.Join(", ", actual));
						}
					}
				}
			}
		}
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
