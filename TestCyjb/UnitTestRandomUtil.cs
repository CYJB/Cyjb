using System;
using System.Collections.Generic;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="RandomUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestRandomUtil
	{
		/// <summary>
		/// 对 <see cref="RandomUtil.NextBoolean"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestNextBoolean()
		{
			HashSet<bool> values = new();
			for (int i = 0; i < 100; i++)
			{
				values.Add(Random.Shared.NextBoolean());
			}
			HashSet<bool> expected = new() { false, true };
			Assert.IsTrue(expected.SetEquals(values));
		}
	}
}
