using System;
using System.Collections.Generic;
using System.Reflection;
using Cyjb.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.Reflection
{
	/// <summary>
	/// <c>TypeBounds</c> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeBounds
	{
		/// <summary>
		/// 对 <c>TypeBounds</c> 的相关方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestTypeBounds()
		{
			// 精确推断。
			// 泛型参数。
			TestTypeBounds("TestExact1", new[] { typeof(string) }, new[] { typeof(string) });
			TestTypeBounds("TestExact1", new[] { typeof(string), typeof(string) }, new[] { typeof(string) });
			TestTypeBounds("TestExact1", new[] { typeof(string), typeof(object) }, null);
			// 数组。
			TestTypeBounds("TestExact2", new[] { typeof(string[,]) }, new[] { typeof(string) });
			TestTypeBounds("TestExact2", new[] { typeof(string[,]), typeof(string[,]) }, new[] { typeof(string) });
			TestTypeBounds("TestExact2", new[] { typeof(string[,]), typeof(string[]) }, null);
			TestTypeBounds("TestExact2", new[] { typeof(string[,]), typeof(object[,]) }, null);
			// 可空类型。
			TestTypeBounds("TestExact3", new[] { typeof(int?) }, new[] { typeof(int) });
			TestTypeBounds("TestExact3", new[] { typeof(int?), typeof(int?) }, new[] { typeof(int) });
			TestTypeBounds("TestExact3", new[] { typeof(int?), typeof(long?) }, null);
			// 泛型类型。
			TestTypeBounds("TestExact4", new[] { typeof(IEnumerable<string>) }, new[] { typeof(string) });
			TestTypeBounds("TestExact4", new[] { typeof(IEnumerable<string>), typeof(IEnumerable<string>) },
				new[] { typeof(string) });
			TestTypeBounds("TestExact4", new[] { typeof(IEnumerable<string>), typeof(IEnumerable<object>) },
				null);
			TestTypeBounds("TestExact4", new[] { typeof(IEnumerable<string>), typeof(ICollection<string>) },
				null);

			// 下限推断。
			// 泛型参数。
			TestTypeBounds("TestLowerBound1", new[] { typeof(string) }, new[] { typeof(string) });
			TestTypeBounds("TestLowerBound1", new[] { typeof(string), typeof(string) }, new[] { typeof(string) });
			TestTypeBounds("TestLowerBound1", new[] { typeof(List<int>), typeof(IList<int>) }, new[] { typeof(IList<int>) });
			TestTypeBounds("TestLowerBound1", new[] { typeof(int[]), typeof(IList<int>) }, new[] { typeof(IList<int>) });
			TestTypeBounds("TestLowerBound1", new[] { typeof(string), typeof(int) }, null);
			TestTypeBounds("TestLowerBound1", new[] { typeof(string), typeof(object) }, new[] { typeof(object) });
			TestTypeBounds("TestLowerBound1", new[] { typeof(int), typeof(long) }, new[] { typeof(long) });
			TestTypeBounds("TestLowerBound2", new[] { typeof(int), typeof(long), typeof(double), typeof(float) },
				new[] { typeof(double) });
			TestTypeBounds("TestLowerBound2", new[] { typeof(int), typeof(long), typeof(double), typeof(float), 
				typeof(decimal) }, null);
			// 可空类型。
			TestTypeBounds("TestLowerBound3", new[] { typeof(int?) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound3", new[] { typeof(int?), typeof(long?) }, new[] { typeof(long) });
			// 数组。
			TestTypeBounds("TestLowerBound4", new[] { typeof(string), typeof(string[]) }, new[] { typeof(string) });
			TestTypeBounds("TestLowerBound5", new[] { typeof(int[,]) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound5", new[] { typeof(int[,]), typeof(int[,]) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound5", new[] { typeof(int[,]), typeof(int[]) }, null);
			TestTypeBounds("TestLowerBound5", new[] { typeof(string[,]), typeof(object[,]) }, new[] { typeof(object) });
			TestTypeBounds("TestLowerBound6", new[] { typeof(int[]) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound6", new[] { typeof(IEnumerable<int>) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound6", new[] { typeof(IEnumerable<int>), typeof(int[]) }, new[] { typeof(int) });
			TestTypeBounds("TestLowerBound6", new[] { typeof(IEnumerable<int>), typeof(long[]) }, null);
			TestTypeBounds("TestLowerBound6", new[] { typeof(IEnumerable<string>), typeof(object[]) },
				new[] { typeof(object) });
			// 泛型类型。
			TestTypeBounds("TestLowerBound6", new[] { typeof(ICollection<int>), typeof(IEnumerable<int>) },
				new[] { typeof(int) });
			TestTypeBounds("TestLowerBound6", new[] { typeof(ICollection<int>), typeof(IList<long>) }, null);
			TestTypeBounds("TestLowerBound6", new[] { typeof(ICollection<string>), typeof(IEnumerable<object>) },
				new[] { typeof(object) });
			TestTypeBounds("TestLowerBound7", new[] { typeof(IComparer<int>), typeof(IComparer<long>) }, null);
			TestTypeBounds("TestLowerBound7", new[] { typeof(IComparer<string>), typeof(IComparer<object>) },
				new[] { typeof(string) });

			// 上限推断。
			// 泛型参数。
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(string) }, new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(string), typeof(string) }, new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(List<int>), typeof(IList<int>) },
				new[] { typeof(List<int>) });
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(int[]), typeof(IList<int>) },
				new[] { typeof(int[]) });
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(string), typeof(int) }, null);
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(string), typeof(object) }, new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound1", new[] { typeof(int), typeof(long) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound2", new[] { typeof(int), typeof(long), typeof(double), typeof(float) },
				new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound2", new[] { typeof(int), typeof(long), typeof(double), typeof(float), 
				typeof(decimal) }, new[] { typeof(int) });
			// 可空类型。
			TestTypeUpperBounds("TestUpperBound3", new[] { typeof(int?) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound3", new[] { typeof(int?), typeof(long?) }, new[] { typeof(int) });
			// 数组。
			TestTypeUpperBounds("TestUpperBound4", new[] { typeof(string), typeof(string[]) }, new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound5", new[] { typeof(int[,]) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound5", new[] { typeof(int[,]), typeof(int[,]) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound5", new[] { typeof(int[,]), typeof(int[]) }, null);
			TestTypeUpperBounds("TestUpperBound5", new[] { typeof(string[,]), typeof(object[,]) }, new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(int[]) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(IEnumerable<int>) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(IEnumerable<int>), typeof(int[]) }, new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(IEnumerable<int>), typeof(long[]) }, null);
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(IEnumerable<string>), typeof(object[]) },
				new[] { typeof(string) });
			// 泛型类型。
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(ICollection<int>), typeof(IEnumerable<int>) },
				new[] { typeof(int) });
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(ICollection<int>), typeof(IList<long>) }, null);
			TestTypeUpperBounds("TestUpperBound6", new[] { typeof(ICollection<string>), typeof(IEnumerable<object>) },
				new[] { typeof(string) });
			TestTypeUpperBounds("TestUpperBound7", new[] { typeof(IComparer<int>), typeof(IComparer<long>) }, null);
			TestTypeUpperBounds("TestUpperBound7", new[] { typeof(IComparer<string>), typeof(IComparer<object>) },
				new[] { typeof(object) });
		}
		private static void TestTypeBounds(string methodName, Type[] types, Type[] expectedArgs)
		{
			MethodInfo method = typeof(UnitTestTypeBounds).GetMethod(methodName,
				BindingFlags.Static | BindingFlags.NonPublic);
			PrivateObject typeBounds = new PrivateObject("Cyjb", "Cyjb.Reflection.TypeBounds",
				new object[] { method.GetGenericArguments() });
			Type[] paramTypes = method.GetParameterTypes();
			int last = paramTypes.Length - 1;
			for (int i = 0; i < types.Length; i++)
			{
				if (!(bool)typeBounds.Invoke("TypeInferences", paramTypes[i > last ? last : i], types[i]))
				{
					Assert.AreEqual(expectedArgs, null);
					return;
				}
			}
			AssertExt.AreEqual(expectedArgs, (Type[])typeBounds.Invoke("FixTypeArguments"));
		}
		private static void TestTypeUpperBounds(string methodName, Type[] types, Type[] expectedArgs)
		{
			MethodInfo method = typeof(UnitTestTypeBounds).GetMethod(methodName,
				BindingFlags.Static | BindingFlags.NonPublic);
			PrivateObject typeBounds = new PrivateObject("Cyjb", "Cyjb.Reflection.TypeBounds",
				new object[] { method.GetGenericArguments() });
			Type[] paramTypes = method.GetParameterTypes();
			int last = paramTypes.Length - 1;
			for (int i = 0; i < types.Length; i++)
			{
				if (!(bool)typeBounds.Invoke("TypeInferences", paramTypes[i > last ? last : i], types[i], true))
				{
					Assert.AreEqual(expectedArgs, null);
					return;
				}
			}
			AssertExt.AreEqual(expectedArgs, (Type[])typeBounds.Invoke("FixTypeArguments"));
		}
		// 精确推断
		private static void TestExact1<T>(ref T arg1, ref T arg2) { }
		private static void TestExact2<T>(ref T[,] arg1, ref T[,] arg2) { }
		private static void TestExact3<T>(ref T? arg1, ref T? arg2) where T : struct { }
		private static void TestExact4<T>(ref IEnumerable<T> arg1, ref IEnumerable<T> arg2) { }
		// 下限推断
		private static void TestLowerBound1<T>(T arg1, T arg2) { }
		private static void TestLowerBound2<T>(T arg1, T arg2, T arg3, T arg4, T arg5) { }
		private static void TestLowerBound3<T>(T? arg1, T? arg2) where T : struct { }
		private static void TestLowerBound4<T>(T arg1, params T[] arg2) { }
		private static void TestLowerBound5<T>(T[,] arg1, T[,] arg2) { }
		private static void TestLowerBound6<T>(IEnumerable<T> arg1, IEnumerable<T> arg2) { }
		private static void TestLowerBound7<T>(IComparer<T> arg1, IComparer<T> arg2) { }
		// 上限推断
		private static void TestUpperBound1<T>(T arg1, T arg2) { }
		private static void TestUpperBound2<T>(T arg1, T arg2, T arg3, T arg4, T arg5) { }
		private static void TestUpperBound3<T>(T? arg1, T? arg2) where T : struct { }
		private static void TestUpperBound4<T>(T arg1, params T[] arg2) { }
		private static void TestUpperBound5<T>(T[,] arg1, T[,] arg2) { }
		private static void TestUpperBound6<T>(T[] arg1, T[] arg2) { }
		private static void TestUpperBound7<T>(IComparer<T> arg1, IComparer<T> arg2) { }
	}
}
