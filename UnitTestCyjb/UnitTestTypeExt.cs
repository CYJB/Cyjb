using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb
{
	/// <summary>
	/// <see cref="Cyjb.TypeExt"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeExt
	{
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsImplicitFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsImplicitFrom()
		{
			TestIsImplicitFromHelper(TypeExt.IsImplicitFrom);
			// 不允许的情况。
			// 6.1.4 可以为 null 的隐式转换
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(int?)));
			Assert.IsFalse(typeof(short?).IsImplicitFrom(typeof(int)));
			Assert.IsFalse(typeof(int?).IsImplicitFrom(typeof(long?)));
			// 6.1.6 隐式引用转换
			Assert.IsFalse(typeof(string).IsImplicitFrom(typeof(object)));
			Assert.IsFalse(typeof(TestClass).IsImplicitFrom(typeof(object)));
			Assert.IsFalse(typeof(TestClass2).IsImplicitFrom(typeof(TestClass)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList<int>)));
			Assert.IsFalse(typeof(ISet<int>).IsImplicitFrom(typeof(List<int>)));
			Assert.IsFalse(typeof(IList).IsImplicitFrom(typeof(ICollection)));
			Assert.IsFalse(typeof(ISet<int>).IsImplicitFrom(typeof(List<int>)));
			Assert.IsFalse(typeof(TestClass6[]).IsImplicitFrom(typeof(TestClass7[])));
			Assert.IsFalse(typeof(TestClass2[]).IsImplicitFrom(typeof(TestClass[])));
			Assert.IsFalse(typeof(long[]).IsImplicitFrom(typeof(int[])));
			Assert.IsFalse(typeof(int[]).IsImplicitFrom(typeof(Array)));
			// 6.1.11 用户定义的隐式转换
			Assert.IsFalse(typeof(TestStruct2).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2?).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct2?)));
			Assert.IsFalse(typeof(TestStruct3).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct3?).IsImplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3?).IsImplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct3?)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct5)));
			Assert.IsFalse(typeof(TestStruct).IsImplicitFrom(typeof(TestStruct5?)));
			Assert.IsFalse(typeof(TestStruct?).IsImplicitFrom(typeof(TestStruct5?)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(TestClass12)));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsImplicitFrom"/> 方法进行测试的辅助方法。
		/// </summary>
		public void TestIsImplicitFromHelper(Func<Type, Type, bool> func)
		{
			// 转换的定义来自 CSharp Language Specification v5.0 的第 6.1 节。

			#region 6.1.1 标识转换

			Assert.IsTrue(func(typeof(object), typeof(object)));
			Assert.IsTrue(func(typeof(bool), typeof(bool)));
			Assert.IsTrue(func(typeof(char), typeof(char)));
			Assert.IsTrue(func(typeof(sbyte), typeof(sbyte)));
			Assert.IsTrue(func(typeof(short), typeof(short)));
			Assert.IsTrue(func(typeof(ushort), typeof(ushort)));
			Assert.IsTrue(func(typeof(int), typeof(int)));
			Assert.IsTrue(func(typeof(uint), typeof(uint)));
			Assert.IsTrue(func(typeof(long), typeof(long)));
			Assert.IsTrue(func(typeof(ulong), typeof(ulong)));
			Assert.IsTrue(func(typeof(float), typeof(float)));
			Assert.IsTrue(func(typeof(double), typeof(double)));
			Assert.IsTrue(func(typeof(decimal), typeof(decimal)));
			Assert.IsTrue(func(typeof(TestClass), typeof(TestClass)));
			// 这里还有一点是 dynamic 和 object 是等效的，但由于在运行时
			// dynamic 和 object 没有区别（参见规范 4.7 节），
			// 因此在判断类型转换时完全不用考虑它。

			#endregion // 6.1.1 标识转换

			#region 6.1.2 隐式数值转换

			Assert.IsTrue(func(typeof(short), typeof(sbyte)));
			Assert.IsTrue(func(typeof(int), typeof(sbyte)));
			Assert.IsTrue(func(typeof(long), typeof(sbyte)));
			Assert.IsTrue(func(typeof(float), typeof(sbyte)));
			Assert.IsTrue(func(typeof(double), typeof(sbyte)));
			Assert.IsTrue(func(typeof(decimal), typeof(sbyte)));
			Assert.IsTrue(func(typeof(short), typeof(byte)));
			Assert.IsTrue(func(typeof(ushort), typeof(byte)));
			Assert.IsTrue(func(typeof(int), typeof(byte)));
			Assert.IsTrue(func(typeof(uint), typeof(byte)));
			Assert.IsTrue(func(typeof(long), typeof(byte)));
			Assert.IsTrue(func(typeof(ulong), typeof(byte)));
			Assert.IsTrue(func(typeof(float), typeof(byte)));
			Assert.IsTrue(func(typeof(double), typeof(byte)));
			Assert.IsTrue(func(typeof(decimal), typeof(byte)));
			Assert.IsTrue(func(typeof(int), typeof(short)));
			Assert.IsTrue(func(typeof(long), typeof(short)));
			Assert.IsTrue(func(typeof(float), typeof(short)));
			Assert.IsTrue(func(typeof(double), typeof(short)));
			Assert.IsTrue(func(typeof(decimal), typeof(short)));
			Assert.IsTrue(func(typeof(int), typeof(ushort)));
			Assert.IsTrue(func(typeof(uint), typeof(ushort)));
			Assert.IsTrue(func(typeof(long), typeof(ushort)));
			Assert.IsTrue(func(typeof(ulong), typeof(ushort)));
			Assert.IsTrue(func(typeof(float), typeof(ushort)));
			Assert.IsTrue(func(typeof(double), typeof(ushort)));
			Assert.IsTrue(func(typeof(decimal), typeof(ushort)));
			Assert.IsTrue(func(typeof(long), typeof(int)));
			Assert.IsTrue(func(typeof(float), typeof(int)));
			Assert.IsTrue(func(typeof(double), typeof(int)));
			Assert.IsTrue(func(typeof(decimal), typeof(int)));
			Assert.IsTrue(func(typeof(long), typeof(uint)));
			Assert.IsTrue(func(typeof(ulong), typeof(uint)));
			Assert.IsTrue(func(typeof(float), typeof(uint)));
			Assert.IsTrue(func(typeof(double), typeof(uint)));
			Assert.IsTrue(func(typeof(decimal), typeof(uint)));
			Assert.IsTrue(func(typeof(float), typeof(long)));
			Assert.IsTrue(func(typeof(double), typeof(long)));
			Assert.IsTrue(func(typeof(decimal), typeof(long)));
			Assert.IsTrue(func(typeof(float), typeof(ulong)));
			Assert.IsTrue(func(typeof(double), typeof(ulong)));
			Assert.IsTrue(func(typeof(decimal), typeof(ulong)));
			Assert.IsTrue(func(typeof(ushort), typeof(char)));
			Assert.IsTrue(func(typeof(int), typeof(char)));
			Assert.IsTrue(func(typeof(uint), typeof(char)));
			Assert.IsTrue(func(typeof(long), typeof(char)));
			Assert.IsTrue(func(typeof(ulong), typeof(char)));
			Assert.IsTrue(func(typeof(float), typeof(char)));
			Assert.IsTrue(func(typeof(double), typeof(char)));
			Assert.IsTrue(func(typeof(decimal), typeof(char)));
			Assert.IsTrue(func(typeof(double), typeof(float)));

			#endregion // 6.1.2 隐式数值转换

			// 6.1.3 隐式枚举转换，针对的是十进制数字文本 0，不在考虑范围内。

			#region 6.1.4 可以为 null 的隐式转换

			// 从 S 到 T? 的隐式转换。
			Assert.IsTrue(func(typeof(int?), typeof(int)));
			Assert.IsTrue(func(typeof(long?), typeof(int)));
			// 从 S? 到 T? 的隐式转换。
			Assert.IsTrue(func(typeof(int?), typeof(int?)));
			Assert.IsTrue(func(typeof(long?), typeof(int?)));

			#endregion // 6.1.4 可以为 null 的隐式转换

			// 6.1.5 null 文本转换，不在考虑范围内。

			#region 6.1.6 隐式引用转换

			// 6.1.6.1 从任何 reference-type 到 object （和 dynamic）。
			Assert.IsTrue(func(typeof(object), typeof(string)));
			Assert.IsTrue(func(typeof(object), typeof(TestClass)));
			// 6.1.6.2 从任何 class-type S 到任何 class-type T（前提是 S 是从 T 派生的）。
			Assert.IsTrue(func(typeof(TestClass), typeof(TestClass2)));
			// 6.1.6.3 从任何 class-type S 到任何 interface-type T（前提是 S 实现了 T）。
			Assert.IsTrue(func(typeof(IList<int>), typeof(List<int>)));
			// 6.1.6.4 从任何 interface-type S 到任何 interface-type T（前提是 S 是从 T 派生的）。
			Assert.IsTrue(func(typeof(IList), typeof(List<int>)));
			Assert.IsTrue(func(typeof(ICollection), typeof(IList)));
			// 6.1.6.5 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			// o S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			// o SE 和 TE 都是 reference-type。
			// o 存在从 SE 到 TE 的隐式引用转换。
			Assert.IsTrue(func(typeof(object[]), typeof(string[])));
			Assert.IsTrue(func(typeof(object[]), typeof(TestClass[])));
			Assert.IsTrue(func(typeof(TestClass[]), typeof(TestClass2[])));
			Assert.IsTrue(func(typeof(IList<int>[]), typeof(List<int>[])));
			Assert.IsTrue(func(typeof(IList[]), typeof(List<int>[])));
			Assert.IsTrue(func(typeof(ICollection[]), typeof(IList[])));
			Assert.IsTrue(func(typeof(Array[]), typeof(int[][])));
			Assert.IsTrue(func(typeof(Array[]), typeof(object[][])));
			Assert.IsTrue(func(typeof(Array[]), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList[]), typeof(int[][])));
			Assert.IsTrue(func(typeof(IList[]), typeof(object[][])));
			Assert.IsTrue(func(typeof(IList[]), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<int>[]), typeof(int[][])));
			Assert.IsTrue(func(typeof(IList<object>[]), typeof(object[][])));
			Assert.IsTrue(func(typeof(IList<TestClass>[]), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(Delegate[]), typeof(Action[])));
			Assert.IsTrue(func(typeof(Delegate[]), typeof(Func<int>[])));
			Assert.IsTrue(func(typeof(ISerializable[]), typeof(Action[])));
			Assert.IsTrue(func(typeof(ISerializable[]), typeof(Func<int>[])));
			Assert.IsTrue(func(typeof(IEnumerable<object>[]), typeof(IEnumerable<string>[])));
			Assert.IsTrue(func(typeof(IEnumerable<object>[]), typeof(IList<string>[])));
			Assert.IsTrue(func(typeof(IEnumerable<object>[]), typeof(List<string>[])));
			Assert.IsTrue(func(typeof(IEqualityComparer<string>[]), typeof(IEqualityComparer<object>[])));
			Assert.IsTrue(func(typeof(IEqualityComparer<string>[]), typeof(EqualityComparer<object>[])));
			// 6.1.6.6 从任何 array-type 到 System.Array 及其实现的接口。
			Assert.IsTrue(func(typeof(Array), typeof(int[])));
			Assert.IsTrue(func(typeof(Array), typeof(object[])));
			Assert.IsTrue(func(typeof(Array), typeof(TestClass[])));
			Assert.IsTrue(func(typeof(IList), typeof(int[])));
			Assert.IsTrue(func(typeof(IList), typeof(object[])));
			Assert.IsTrue(func(typeof(IList), typeof(TestClass[])));
			// 6.1.6.7 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口
			// （前提是存在从 S 到 T 的隐式标识或引用转换）。
			Assert.IsTrue(func(typeof(IList<int>), typeof(int[])));
			Assert.IsTrue(func(typeof(IList<object>), typeof(object[])));
			Assert.IsTrue(func(typeof(IList<TestClass>), typeof(TestClass[])));
			Assert.IsTrue(func(typeof(IList<object>), typeof(string[])));
			Assert.IsTrue(func(typeof(IList<object>), typeof(TestClass[])));
			Assert.IsTrue(func(typeof(IList<TestClass>), typeof(TestClass2[])));
			Assert.IsTrue(func(typeof(IList<IList<int>>), typeof(List<int>[])));
			Assert.IsTrue(func(typeof(IList<IList>), typeof(List<int>[])));
			Assert.IsTrue(func(typeof(IList<ICollection>), typeof(IList[])));
			Assert.IsTrue(func(typeof(IList<object[]>), typeof(string[][])));
			Assert.IsTrue(func(typeof(IList<object[]>), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<TestClass[]>), typeof(TestClass2[][])));
			Assert.IsTrue(func(typeof(IList<IList<int>[]>), typeof(List<int>[][])));
			Assert.IsTrue(func(typeof(IList<IList<int>[]>), typeof(List<int>[][])));
			Assert.IsTrue(func(typeof(IList<IList[]>), typeof(List<int>[][])));
			Assert.IsTrue(func(typeof(IList<ICollection[]>), typeof(IList[][])));
			Assert.IsTrue(func(typeof(IList<Array[]>), typeof(int[][][])));
			Assert.IsTrue(func(typeof(IList<Array[]>), typeof(object[][][])));
			Assert.IsTrue(func(typeof(IList<Array[]>), typeof(TestClass[][][])));
			Assert.IsTrue(func(typeof(IList<IList[]>), typeof(int[][][])));
			Assert.IsTrue(func(typeof(IList<IList[]>), typeof(object[][][])));
			Assert.IsTrue(func(typeof(IList<IList[]>), typeof(TestClass[][][])));
			Assert.IsTrue(func(typeof(IList<IList<int>[]>), typeof(int[][][])));
			Assert.IsTrue(func(typeof(IList<IList<object>[]>), typeof(object[][][])));
			Assert.IsTrue(func(typeof(IList<IList<TestClass>[]>), typeof(TestClass[][][])));
			Assert.IsTrue(func(typeof(IList<Delegate[]>), typeof(Action[][])));
			Assert.IsTrue(func(typeof(IList<Delegate[]>), typeof(Func<int>[][])));
			Assert.IsTrue(func(typeof(IList<ISerializable[]>), typeof(Action[][])));
			Assert.IsTrue(func(typeof(IList<ISerializable[]>), typeof(Func<int>[][])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>[]>), typeof(IEnumerable<string>[][])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>[]>), typeof(IList<string>[][])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>[]>), typeof(List<string>[][])));
			Assert.IsTrue(func(typeof(IList<IEqualityComparer<string>[]>), typeof(IEqualityComparer<object>[][])));
			Assert.IsTrue(func(typeof(IList<IEqualityComparer<string>[]>), typeof(EqualityComparer<object>[][])));
			Assert.IsTrue(func(typeof(IList<Array>), typeof(int[][])));
			Assert.IsTrue(func(typeof(IList<Array>), typeof(object[][])));
			Assert.IsTrue(func(typeof(IList<Array>), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<IList>), typeof(int[][])));
			Assert.IsTrue(func(typeof(IList<IList>), typeof(object[][])));
			Assert.IsTrue(func(typeof(IList<IList>), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<IList<int>>), typeof(int[][])));
			Assert.IsTrue(func(typeof(IList<IList<object>>), typeof(object[][])));
			Assert.IsTrue(func(typeof(IList<IList<TestClass>>), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<IList<object>>), typeof(string[][])));
			Assert.IsTrue(func(typeof(IList<IList<object>>), typeof(TestClass[][])));
			Assert.IsTrue(func(typeof(IList<Delegate>), typeof(Action[])));
			Assert.IsTrue(func(typeof(IList<Delegate>), typeof(Func<int>[])));
			Assert.IsTrue(func(typeof(IList<ISerializable>), typeof(Action[])));
			Assert.IsTrue(func(typeof(IList<ISerializable>), typeof(Func<int>[])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>>), typeof(IEnumerable<string>[])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>>), typeof(IList<string>[])));
			Assert.IsTrue(func(typeof(IList<IEnumerable<object>>), typeof(List<string>[])));
			Assert.IsTrue(func(typeof(IList<IEqualityComparer<string>>), typeof(IEqualityComparer<object>[])));
			Assert.IsTrue(func(typeof(IList<IEqualityComparer<string>>), typeof(EqualityComparer<object>[])));
			// 6.1.6.8 从任何 delegate-type 到 System.Delegate 及其实现的接口。
			Assert.IsTrue(func(typeof(Delegate), typeof(Action)));
			Assert.IsTrue(func(typeof(Delegate), typeof(Func<int>)));
			Assert.IsTrue(func(typeof(ISerializable), typeof(Action)));
			Assert.IsTrue(func(typeof(ISerializable), typeof(Func<int>)));
			// 6.1.6.9 从 null 文本到任何 reference-type，不在考虑范围内。
			// 6.1.6.10 从任何 reference-type 到 reference-type T
			// （前提是它具有到 reference-type T0 的隐式标识或引用转换，且 T0 具有到 T 的标识转换）。
			// 在这里标识转换不在考虑范围内，进一步内容可以参考 Stack Overflow 上的问题
			// http://stackoverflow.com/questions/3736789/question-regarding-implicit-conversions-in-the-c-sharp-language-specification。
			// 因此此条规则也可以不考虑。
			// 6.1.6.11 从任何 reference-type 到接口或委托类型 T
			// （前提是它具有到接口或委托类型 T0 的隐式标识或引用转换，且 T0 可变化转换为T）。
			// 这里的变化转换在规范的 13.1.3.2 节，就是泛型的协变和逆变。
			// 协变。
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(IEnumerable<string>)));
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(IList<string>)));
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(List<string>)));
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(IEnumerable<TestClass>)));
			Assert.IsTrue(func(typeof(IEnumerable<TestClass>), typeof(IEnumerable<TestClass2>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList>), typeof(IEnumerable<List<int>>)));
			Assert.IsTrue(func(typeof(IEnumerable<ICollection>), typeof(IEnumerable<IList>)));
			Assert.IsTrue(func(typeof(IEnumerable<object[]>), typeof(IEnumerable<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<TestClass[]>), typeof(IEnumerable<TestClass2[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<int>[]>), typeof(IEnumerable<List<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList[]>), typeof(IEnumerable<List<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<ICollection[]>), typeof(IEnumerable<IList[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array[]>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array[]>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array[]>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList[]>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList[]>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList[]>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<int>[]>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object>[]>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<TestClass>[]>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Delegate[]>), typeof(IEnumerable<Action[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Delegate[]>), typeof(IEnumerable<Func<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<ISerializable[]>), typeof(IEnumerable<Action[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<ISerializable[]>), typeof(IEnumerable<Func<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>[]>), typeof(IEnumerable<IEnumerable<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>[]>), typeof(IEnumerable<IList<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>[]>), typeof(IEnumerable<List<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEqualityComparer<string>[]>), typeof(IEnumerable<IEqualityComparer<object>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEqualityComparer<string>[]>), typeof(IEnumerable<EqualityComparer<object>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array>), typeof(IEnumerable<int[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array>), typeof(IEnumerable<object[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Array>), typeof(IEnumerable<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList>), typeof(IEnumerable<int[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList>), typeof(IEnumerable<object[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList>), typeof(IEnumerable<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<int>>), typeof(IEnumerable<int[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object>>), typeof(IEnumerable<object[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<TestClass>>), typeof(IEnumerable<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object>>), typeof(IEnumerable<string[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object>>), typeof(IEnumerable<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<TestClass>>), typeof(IEnumerable<TestClass2[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<int>>>), typeof(IEnumerable<List<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList>>), typeof(IEnumerable<List<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ICollection>>), typeof(IEnumerable<IList[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object[]>>), typeof(IEnumerable<string[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<object[]>>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<TestClass[]>>), typeof(IEnumerable<TestClass2[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<int>[]>>), typeof(IEnumerable<List<int>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<int>[]>>), typeof(IEnumerable<List<int>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList[]>>), typeof(IEnumerable<List<int>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ICollection[]>>), typeof(IEnumerable<IList[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array[]>>), typeof(IEnumerable<int[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array[]>>), typeof(IEnumerable<object[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array[]>>), typeof(IEnumerable<TestClass[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList[]>>), typeof(IEnumerable<int[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList[]>>), typeof(IEnumerable<object[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList[]>>), typeof(IEnumerable<TestClass[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<int>[]>>), typeof(IEnumerable<int[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<object>[]>>), typeof(IEnumerable<object[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<TestClass>[]>>), typeof(IEnumerable<TestClass[][][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Delegate[]>>), typeof(IEnumerable<Action[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Delegate[]>>), typeof(IEnumerable<Func<int>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ISerializable[]>>), typeof(IEnumerable<Action[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ISerializable[]>>), typeof(IEnumerable<Func<int>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>[]>>), typeof(IEnumerable<IEnumerable<string>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>[]>>), typeof(IEnumerable<IList<string>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>[]>>), typeof(IEnumerable<List<string>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEqualityComparer<string>[]>>), typeof(IEnumerable<IEqualityComparer<object>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEqualityComparer<string>[]>>), typeof(IEnumerable<EqualityComparer<object>[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array>>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array>>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Array>>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList>>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList>>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList>>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<int>>>), typeof(IEnumerable<int[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<object>>>), typeof(IEnumerable<object[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<TestClass>>>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<object>>>), typeof(IEnumerable<string[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IList<object>>>), typeof(IEnumerable<TestClass[][]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Delegate>>), typeof(IEnumerable<Action[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<Delegate>>), typeof(IEnumerable<Func<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ISerializable>>), typeof(IEnumerable<Action[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<ISerializable>>), typeof(IEnumerable<Func<int>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>>>), typeof(IEnumerable<IEnumerable<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>>>), typeof(IEnumerable<IList<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEnumerable<object>>>), typeof(IEnumerable<List<string>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEqualityComparer<string>>>), typeof(IEnumerable<IEqualityComparer<object>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<IList<IEqualityComparer<string>>>), typeof(IEnumerable<EqualityComparer<object>[]>)));
			Assert.IsTrue(func(typeof(IEnumerable<Delegate>), typeof(IEnumerable<Action>)));
			Assert.IsTrue(func(typeof(IEnumerable<Delegate>), typeof(IEnumerable<Func<int>>)));
			Assert.IsTrue(func(typeof(IEnumerable<ISerializable>), typeof(IEnumerable<Action>)));
			Assert.IsTrue(func(typeof(IEnumerable<ISerializable>), typeof(IEnumerable<Func<int>>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>>), typeof(IEnumerable<IEnumerable<string>>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>>), typeof(IEnumerable<IList<string>>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEnumerable<object>>), typeof(IEnumerable<List<string>>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEqualityComparer<string>>), typeof(IEnumerable<IEqualityComparer<object>>)));
			Assert.IsTrue(func(typeof(IEnumerable<IEqualityComparer<string>>), typeof(IEnumerable<EqualityComparer<object>>)));
			// 逆变。
			Assert.IsTrue(func(typeof(IEqualityComparer<string>), typeof(IEqualityComparer<object>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<string>), typeof(EqualityComparer<object>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass>), typeof(IEqualityComparer<object>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass2>), typeof(IEqualityComparer<TestClass>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>>), typeof(IEqualityComparer<IList>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList>), typeof(IEqualityComparer<ICollection>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[]>), typeof(IEqualityComparer<object[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass2[]>), typeof(IEqualityComparer<TestClass[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[]>), typeof(IEqualityComparer<IList<int>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[]>), typeof(IEqualityComparer<IList[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList[]>), typeof(IEqualityComparer<ICollection[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<Array[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<Array[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<Array[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<IList[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<IList[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<IList<int>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<IList<object>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<TestClass>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[]>), typeof(IEqualityComparer<Delegate[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[]>), typeof(IEqualityComparer<Delegate[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[]>), typeof(IEqualityComparer<ISerializable[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[]>), typeof(IEqualityComparer<ISerializable[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEnumerable<string>[]>), typeof(IEqualityComparer<IEnumerable<object>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList<string>[]>), typeof(IEqualityComparer<IEnumerable<object>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<string>[]>), typeof(IEqualityComparer<IEnumerable<object>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEqualityComparer<object>[]>), typeof(IEqualityComparer<IEqualityComparer<string>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<EqualityComparer<object>[]>), typeof(IEqualityComparer<IEqualityComparer<string>[]>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[]>), typeof(IEqualityComparer<Array>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[]>), typeof(IEqualityComparer<Array>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[]>), typeof(IEqualityComparer<Array>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[]>), typeof(IEqualityComparer<IList>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[]>), typeof(IEqualityComparer<IList>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[]>), typeof(IEqualityComparer<IList>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[]>), typeof(IEqualityComparer<IList<int>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[]>), typeof(IEqualityComparer<IList<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[]>), typeof(IEqualityComparer<IList<TestClass>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<string[]>), typeof(IEqualityComparer<IList<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[]>), typeof(IEqualityComparer<IList<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass2[]>), typeof(IEqualityComparer<IList<TestClass>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[]>), typeof(IEqualityComparer<IList<IList<int>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[]>), typeof(IEqualityComparer<IList<IList>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList[]>), typeof(IEqualityComparer<IList<ICollection>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<string[][]>), typeof(IEqualityComparer<IList<object[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<object[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass2[][]>), typeof(IEqualityComparer<IList<TestClass[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[][]>), typeof(IEqualityComparer<IList<IList<int>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[][]>), typeof(IEqualityComparer<IList<IList<int>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<int>[][]>), typeof(IEqualityComparer<IList<IList[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList[][]>), typeof(IEqualityComparer<IList<ICollection[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][][]>), typeof(IEqualityComparer<IList<Array[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][][]>), typeof(IEqualityComparer<IList<Array[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][][]>), typeof(IEqualityComparer<IList<Array[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][][]>), typeof(IEqualityComparer<IList<IList[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][][]>), typeof(IEqualityComparer<IList<IList[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][][]>), typeof(IEqualityComparer<IList<IList[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][][]>), typeof(IEqualityComparer<IList<IList<int>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][][]>), typeof(IEqualityComparer<IList<IList<object>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][][]>), typeof(IEqualityComparer<IList<IList<TestClass>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[][]>), typeof(IEqualityComparer<IList<Delegate[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[][]>), typeof(IEqualityComparer<IList<Delegate[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[][]>), typeof(IEqualityComparer<IList<ISerializable[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[][]>), typeof(IEqualityComparer<IList<ISerializable[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEnumerable<string>[][]>), typeof(IEqualityComparer<IList<IEnumerable<object>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList<string>[][]>), typeof(IEqualityComparer<IList<IEnumerable<object>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<string>[][]>), typeof(IEqualityComparer<IList<IEnumerable<object>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEqualityComparer<object>[][]>), typeof(IEqualityComparer<IList<IEqualityComparer<string>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<EqualityComparer<object>[][]>), typeof(IEqualityComparer<IList<IEqualityComparer<string>[]>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<IList<Array>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<IList<Array>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<Array>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<IList<IList>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<IList<IList>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<IList>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<int[][]>), typeof(IEqualityComparer<IList<IList<int>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<object[][]>), typeof(IEqualityComparer<IList<IList<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<IList<TestClass>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<string[][]>), typeof(IEqualityComparer<IList<IList<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<TestClass[][]>), typeof(IEqualityComparer<IList<IList<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[]>), typeof(IEqualityComparer<IList<Delegate>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[]>), typeof(IEqualityComparer<IList<Delegate>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action[]>), typeof(IEqualityComparer<IList<ISerializable>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>[]>), typeof(IEqualityComparer<IList<ISerializable>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEnumerable<string>[]>), typeof(IEqualityComparer<IList<IEnumerable<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList<string>[]>), typeof(IEqualityComparer<IList<IEnumerable<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<string>[]>), typeof(IEqualityComparer<IList<IEnumerable<object>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEqualityComparer<object>[]>), typeof(IEqualityComparer<IList<IEqualityComparer<string>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<EqualityComparer<object>[]>), typeof(IEqualityComparer<IList<IEqualityComparer<string>>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action>), typeof(IEqualityComparer<Delegate>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>>), typeof(IEqualityComparer<Delegate>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Action>), typeof(IEqualityComparer<ISerializable>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<Func<int>>), typeof(IEqualityComparer<ISerializable>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEnumerable<string>>), typeof(IEqualityComparer<IEnumerable<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IList<string>>), typeof(IEqualityComparer<IEnumerable<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<List<string>>), typeof(IEqualityComparer<IEnumerable<object>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<IEqualityComparer<object>>), typeof(IEqualityComparer<IEqualityComparer<string>>)));
			Assert.IsTrue(func(typeof(IEqualityComparer<EqualityComparer<object>>), typeof(IEqualityComparer<IEqualityComparer<string>>)));
			// 6.1.6.12 涉及已知为引用类型的类型参数的隐式转换。这个转换是针对类型参数 T 的，因此同样不做考虑。

			#endregion // 6.1.6 隐式引用转换

			#region 6.1.7 装箱转换

			// 从 non-nullable-value-type 到 object。
			Assert.IsTrue(func(typeof(object), typeof(uint)));
			Assert.IsTrue(func(typeof(object), typeof(TestStruct)));
			// 从 non-nullable-value-type 到 System.ValueType。
			Assert.IsTrue(func(typeof(ValueType), typeof(int)));
			Assert.IsTrue(func(typeof(ValueType), typeof(TestStruct2)));
			// 从 non-nullable-value-type 到其实现的接口。
			Assert.IsTrue(func(typeof(IComparable<int>), typeof(int)));
			Assert.IsTrue(func(typeof(IEnumerable<string>), typeof(TestStruct4)));
			// 从 enum-type 转换为 System.Enum 类型。
			Assert.IsTrue(func(typeof(Enum), typeof(Tristate)));
			// 从 nullable-type 到引用类型的装箱转换，如果存在从对应的 non-nullable-value-type 到该引用类型的装箱转换。
			Assert.IsTrue(func(typeof(object), typeof(uint?)));
			Assert.IsTrue(func(typeof(object), typeof(TestStruct?)));
			Assert.IsTrue(func(typeof(ValueType), typeof(int?)));
			Assert.IsTrue(func(typeof(ValueType), typeof(TestStruct2?)));
			Assert.IsTrue(func(typeof(IComparable<int>), typeof(int?)));
			Assert.IsTrue(func(typeof(IEnumerable<string>), typeof(TestStruct4?)));
			Assert.IsTrue(func(typeof(Enum), typeof(Tristate?)));
			// 如果值类型具有到接口或委托类型 I0 的装箱转换，且 I0 变化转换为接口类型 I，则值类型具有到 I 的装箱转换。
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(TestStruct4)));
			Assert.IsTrue(func(typeof(IEnumerable<object>), typeof(TestStruct4?)));

			#endregion // 6.1.7 装箱转换

			// 6.1.8 隐式动态转换、6.1.9 隐式常量表达式转换、6.1.10 涉及类型形参的隐式转换，不在考虑范围内。

			#region 6.1.11 用户定义的隐式转换

			Assert.IsTrue(func(typeof(TestStruct), typeof(TestStruct2)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct2)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct2?)));
			Assert.IsTrue(func(typeof(TestStruct), typeof(TestStruct3)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct3)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct3?)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct5)));
			Assert.IsTrue(func(typeof(TestStruct), typeof(TestStruct6)));
			Assert.IsTrue(func(typeof(TestStruct), typeof(TestStruct6?)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct6)));
			Assert.IsTrue(func(typeof(TestStruct?), typeof(TestStruct6?)));
			Assert.IsTrue(func(typeof(int), typeof(TestClass)));
			Assert.IsTrue(func(typeof(int), typeof(TestClass2)));
			Assert.IsTrue(func(typeof(long), typeof(TestClass2)));
			Assert.IsTrue(func(typeof(bool), typeof(TestClass2)));
			Assert.IsTrue(func(typeof(Enum), typeof(TestClass13)));
			Assert.IsTrue(func(typeof(int?), typeof(TestClass12)));
			Assert.IsTrue(func(typeof(TestClass6), typeof(TestClass8)));
			Assert.IsTrue(func(typeof(TestClass7), typeof(TestClass6)));
			Assert.IsTrue(func(typeof(TestClass6), typeof(int)));
			Assert.IsTrue(func(typeof(long), typeof(TestClass)));
			Assert.IsTrue(func(typeof(decimal), typeof(TestClass)));
			Assert.IsTrue(func(typeof(decimal), typeof(TestClass2)));
			Assert.IsTrue(func(typeof(TestClass6), typeof(short)));
			Assert.IsTrue(func(typeof(long), typeof(TestClass6)));

			#endregion // 6.1.11 用户定义的隐式转换

		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsExplicitFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsExplicitFrom()
		{
			// 转换的定义来自 CSharp Language Specification v5.0 的第 6.2 节。

			// 测试所有隐式转换。
			TestIsImplicitFromHelper(TypeExt.IsExplicitFrom);

			#region 6.2.1 显式数值转换。

			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(float).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(float).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(double).IsExplicitFrom(typeof(decimal)));

			#endregion // 6.2.1 显式数值转换。

			#region 6.2.2 显式枚举转换

			Assert.IsTrue(typeof(sbyte).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(byte).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(ulong).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(char).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(float).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(double).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(Tristate)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(float)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(double)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(decimal)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(TypeCode)));

			#endregion // 6.2.2 显式枚举转换

			#region 6.2.3 可以为 null 的显式转换

			Assert.IsTrue(typeof(int?).IsExplicitFrom(typeof(long?)));
			Assert.IsTrue(typeof(int?).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(long?).IsExplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(int?).IsExplicitFrom(typeof(long?)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(TestStruct).IsExplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct?).IsExplicitFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct?).IsExplicitFrom(typeof(TestStruct2)));
			Assert.IsTrue(typeof(TestStruct).IsExplicitFrom(typeof(TestStruct3)));
			Assert.IsTrue(typeof(TestStruct?).IsExplicitFrom(typeof(TestStruct3?)));
			Assert.IsTrue(typeof(TestStruct).IsExplicitFrom(typeof(TestStruct2?)));
			Assert.IsTrue(typeof(TestStruct).IsExplicitFrom(typeof(TestStruct3?)));

			#endregion // 6.2.3 可以为 null 的显式转换

			#region 6.2.4 显式引用转换

			// 6.2.4.1 从 object 和 dynamic 到任何其他 reference-type。
			Assert.IsTrue(typeof(string).IsExplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(TestClass).IsExplicitFrom(typeof(object)));
			// 6.2.4.2 从任何 class-type S 到任何 class-type T（前提是 S 为 T 的基类）。
			Assert.IsTrue(typeof(TestClass2).IsExplicitFrom(typeof(TestClass)));
			// 未实现 - 6.2.4.3 从任何 class-type S 到任何 interface-type T（前提是 S 未密封并且 S 未实现 T）。
			// Assert.IsTrue(typeof(ISet<int>).IsExplicitFrom(typeof(List<int>)));
			// 未实现 - 6.2.4.4 从任何 interface-type S 到任何 class-type T（前提是 T 未密封或 T 实现 S）。
			// Assert.IsTrue(typeof(List<int>).IsExplicitFrom(typeof(ISet<int>)));
			// 未实现 - 6.2.4.5 从任何 interface-type S 到任何 interface-type T（前提是 S 不是从 T 派生的）。
			// Assert.IsTrue(typeof(IList<int>).IsExplicitFrom(typeof(IList)));
			// 未实现 - 6.2.4.6 从元素类型为 SE 的 array-type S 到元素类型为 TE 的 array-type T（前提是以下所列条件均成立）：
			// o S 和 T 只有元素类型不同。换言之，S 和 T 具有相同的维数。
			// o SE 和 TE 都是 reference-type。
			// o 存在从 SE 到 TE 的显式引用转换。
			// Assert.IsTrue(typeof(string[]).IsExplicitFrom(typeof(object[])));
			// Assert.IsTrue(typeof(TestClass[]).IsExplicitFrom(typeof(object[])));
			// Assert.IsTrue(typeof(TestClass2[]).IsExplicitFrom(typeof(TestClass[])));
			// Assert.IsTrue(typeof(ISet<int>[]).IsExplicitFrom(typeof(List<int>[])));
			// Assert.IsTrue(typeof(List<int>[]).IsExplicitFrom(typeof(ISet<int>[])));
			// Assert.IsTrue(typeof(IList<int>[]).IsExplicitFrom(typeof(IList[])));
			// Assert.IsTrue(typeof(int[][]).IsExplicitFrom(typeof(Array[])));
			// Assert.IsTrue(typeof(int[][]).IsExplicitFrom(typeof(IList[])));
			// Assert.IsTrue(typeof(object[][]).IsExplicitFrom(typeof(Array[])));
			// Assert.IsTrue(typeof(IList<TestClass>[]).IsExplicitFrom(typeof(object[][])));
			// Assert.IsTrue(typeof(ICollection<string>[]).IsExplicitFrom(typeof(object[][])));
			// Assert.IsTrue(typeof(int[][]).IsExplicitFrom(typeof(IList<char>[])));
			// Assert.IsTrue(typeof(object[][]).IsExplicitFrom(typeof(ICollection<string>[])));
			// Assert.IsTrue(typeof(Func<int>[]).IsExplicitFrom(typeof(Delegate[])));
			// Assert.IsTrue(typeof(Func<int>[]).IsExplicitFrom(typeof(IDisposable[])));
			// 6.2.4.7 从 System.Array 及其实现的接口到任何 array-type。
			Assert.IsTrue(typeof(int[]).IsExplicitFrom(typeof(Array)));
			Assert.IsTrue(typeof(int[]).IsExplicitFrom(typeof(IList)));
			Assert.IsTrue(typeof(object[]).IsExplicitFrom(typeof(Array)));
			// 未实现 - 6.2.4.8 从一维数组类型 S[] 到 System.Collections.Generic.IList<T> 及其基接口
			// （前提是存在从 S 到 T 的显式标识或引用转换）。
			// Assert.IsTrue(typeof(IList<char>).IsExplicitFrom(typeof(int[])));
			// Assert.IsTrue(typeof(ICollection<string>).IsExplicitFrom(typeof(object[])));
			// 未实现 - 6.2.4.9 从 System.Collections.Generic.IList<S> 及其基接口到一维数组类型 T[]
			// （前提是存在从 S 到 T 的显式标识或引用转换）。
			// Assert.IsTrue(typeof(int[]).IsExplicitFrom(typeof(IList<char>)));
			// Assert.IsTrue(typeof(object[]).IsExplicitFrom(typeof(ICollection<string>)));
			// 6.2.4.10 从 System.Delegate 及其实现的接口到任何 delegate-type。
			Assert.IsTrue(typeof(Func<int>).IsExplicitFrom(typeof(Delegate)));
			Assert.IsTrue(typeof(Func<int>).IsExplicitFrom(typeof(ISerializable)));
			// 未实现 - 6.2.4.11 从引用类型到引用类型 T（前提是它具有到引用类型 T0 的显式引用转换，且 T0 具有到 T 的标识转换）。
			// 参考隐式类型转换部分的 6.1.6.10，此条规则不考虑。
			// 未实现 - 6.2.4.12 从引用类型到接口或委托类型 T（前提是它具有到接口或委托类型 T0 的显式引用转换，
			// 且 T0 可变化转换为 T，或 T 可变化转换为 T0）。
			// 这个不是很明白，有些测试不通。
			// 这条不甚了解，先不考虑。
			// 未实现 - 6.2.4.13 从 D<S1…Sn> 到 D<T1…Tn>，其中 D<X1…Xn> 是泛型委托类型，
			// D<S1…Sn> 与 D<T1…Tn> 不兼容或不相同，并且，对于 D 的每个类型形参 Xi，存在以下情况：
			// o 如果 Xi 是固定的，则 Si 与 Ti 相同。
			// o 如果 Xi 是协变的，且存在从 Si 到 Ti 的隐式或显式标识或引用转换。
			// o 如果 Xi 是逆变的，则 Si 与 Ti 相同或同为引用类型。
			// Assert.IsTrue(typeof(TestDelegate<string>).IsExplicitFrom(typeof(TestDelegate<string>)));
			// Assert.IsTrue(typeof(TestDelegate<TestClass>).IsExplicitFrom(typeof(TestDelegate<TestClass>)));
			// Assert.IsTrue(typeof(Func<IList>).IsExplicitFrom(typeof(Func<string>)));
			// Assert.IsTrue(typeof(Action<IList>).IsExplicitFrom(typeof(Action<ICollection>)));
			// Assert.IsTrue(typeof(Action<string>).IsExplicitFrom(typeof(Action<object>)));
			// Assert.IsTrue(typeof(Func<string>).IsExplicitFrom(typeof(Func<object>)));
			// Assert.IsTrue(typeof(Func<IList>).IsExplicitFrom(typeof(Func<string>)));
			// Assert.IsFalse(typeof(TestDelegate<IList>).IsExplicitFrom(typeof(TestDelegate<string>)));
			// Assert.IsFalse(typeof(TestDelegate<IList>).IsExplicitFrom(typeof(TestDelegate<ICollection>)));
			// Assert.IsFalse(typeof(TestDelegate<string>).IsExplicitFrom(typeof(TestDelegate<object>)));
			// Assert.IsFalse(typeof(TestDelegate<string>).IsExplicitFrom(typeof(TestDelegate<object>)));
			// Assert.IsFalse(typeof(TestDelegate<IList>).IsExplicitFrom(typeof(TestDelegate<string>)));
			// Assert.IsFalse(typeof(Action<string>).IsExplicitFrom(typeof(Action<int>)));
			// Assert.IsFalse(typeof(Func<string>).IsExplicitFrom(typeof(Func<int>)));
			// 6.2.4.14 涉及已知为引用类型的类型形参的显式转换，这个不必考虑。

			#endregion // 6.2.4 显式引用转换

			#region 6.2.5 拆箱转换

			// 从类型 object、dynamic 和 System.ValueType 到任何 non-nullable-value-type 的取消装箱转换。
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(ValueType)));
			// 从任何 interface-type 到实现 interface-type 的任何 non-nullable-value-type 的取消装箱转换。
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(IConvertible)));
			Assert.IsTrue(typeof(TestStruct4).IsExplicitFrom(typeof(IEnumerable<string>)));
			// 类型 System.Enum 可以取消装箱为任何 enum-type。
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(Enum)));
			// 从引用类型到 nullable-type 的取消装箱转换，
			// 条件是存在从该引用类型到 nullable-type 的基础 non-nullable-value-typee 的取消装箱转换。
			Assert.IsTrue(typeof(int?).IsExplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(long?).IsExplicitFrom(typeof(ValueType)));
			Assert.IsTrue(typeof(int?).IsExplicitFrom(typeof(IConvertible)));
			Assert.IsTrue(typeof(TestStruct4?).IsExplicitFrom(typeof(IEnumerable<string>)));
			Assert.IsTrue(typeof(TestStruct4?).IsExplicitFrom(typeof(IEnumerable<object>)));
			Assert.IsTrue(typeof(Tristate?).IsExplicitFrom(typeof(Enum)));
			// 如果值类型 S 具有来自接口或委托类型 I0 的取消装箱转换，
			// 且 I0 可变化转换为 I 或 I 可变化转换为 I0，则它具有来自 I 的取消装箱转换。
			Assert.IsTrue(typeof(TestStruct4).IsExplicitFrom(typeof(IEnumerable<object>)));

			#endregion // 6.2.5 拆箱转换

			#region 6.2.8 用户定义的显式转换

			Assert.IsTrue(typeof(TestClass6).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(TestClass6).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(TestClass6).IsExplicitFrom(typeof(TestClass8)));
			Assert.IsTrue(typeof(TestClass7).IsExplicitFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(TestClass10).IsExplicitFrom(typeof(TestClass9)));
			Assert.IsTrue(typeof(TestClass11).IsExplicitFrom(typeof(TestClass9)));
			Assert.IsTrue(typeof(TestClass9).IsExplicitFrom(typeof(TestClass10)));
			Assert.IsTrue(typeof(TestClass9).IsExplicitFrom(typeof(TestClass11)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(TestClass12)));
			Assert.IsTrue(typeof(TestClass12).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(TestClass6)));
			Assert.IsTrue(typeof(Enum).IsExplicitFrom(typeof(TestClass13)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(bool).IsExplicitFrom(typeof(TestClass2)));
			Assert.IsTrue(typeof(long).IsExplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(TestClass)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(TestClass3)));
			Assert.IsTrue(typeof(Tristate).IsExplicitFrom(typeof(TestClass13)));
			Assert.IsTrue(typeof(Enum).IsExplicitFrom(typeof(TestClass14)));

			#endregion // 6.2.8 用户定义的显式转换

			// 不允许两次隐式类型转换。
			Assert.IsFalse(typeof(TestClass).IsExplicitFrom(typeof(TestClass3)));
			// 不允许用户未定义的类型转换。
			Assert.IsFalse(typeof(string).IsExplicitFrom(typeof(TestClass)));
			// 不允许枚举的两次类型转换。
			Assert.IsFalse(typeof(int).IsExplicitFrom(typeof(TestClass13)));
			Assert.IsFalse(typeof(int).IsExplicitFrom(typeof(TestClass14)));
			// 不允许的 Nullable<T> 类型转换。
			Assert.IsFalse(typeof(TestStruct2).IsExplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2).IsExplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsExplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct2?).IsExplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct2?).IsExplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsExplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3).IsExplicitFrom(typeof(TestStruct?)));
			Assert.IsFalse(typeof(TestStruct3?).IsExplicitFrom(typeof(TestStruct)));
			Assert.IsFalse(typeof(TestStruct3?).IsExplicitFrom(typeof(TestStruct?)));
		}
		/// <summary>
		/// 对 <see cref="Cyjb.TypeExt.IsExplicitFromOpenGenericIsAssignableFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestOpenGenericIsAssignableFrom()
		{
			Type closedType;
			Assert.IsFalse(typeof(IEnumerable<int>).OpenGenericIsAssignableFrom(typeof(IEnumerable<>), out closedType));
			Assert.IsFalse(typeof(object).OpenGenericIsAssignableFrom(typeof(IEnumerable<>), out closedType));
			Assert.IsFalse(typeof(IEnumerable<int>).OpenGenericIsAssignableFrom(typeof(IEnumerable<int>), out closedType));

			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(IEnumerable<>), out closedType));
			Assert.AreEqual(typeof(IEnumerable<>), closedType);

			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(IEnumerable<int>), out closedType));
			Assert.AreEqual(typeof(IEnumerable<int>), closedType);

			Assert.IsTrue(typeof(IEnumerable<>).OpenGenericIsAssignableFrom(typeof(Dictionary<int, string>), out closedType));
			Assert.AreEqual(typeof(IEnumerable<KeyValuePair<int, string>>), closedType);

			Assert.IsTrue(typeof(IDictionary<,>).OpenGenericIsAssignableFrom(typeof(Dictionary<int, string>), out closedType));
			Assert.AreEqual(typeof(IDictionary<int, string>), closedType);

			Assert.IsTrue(typeof(TestClass4<,>).OpenGenericIsAssignableFrom(typeof(TestClass5<>), out closedType));
			Type[] genericArgs = closedType.GetGenericArguments();
			Assert.AreEqual(2, genericArgs.Length);
			Assert.AreEqual(typeof(int), genericArgs[0]);
			Assert.IsTrue(genericArgs[1].IsGenericParameter);

			Assert.IsTrue(typeof(TestClass4<,>).OpenGenericIsAssignableFrom(typeof(TestClass5<string>), out closedType));
			Assert.AreEqual(typeof(TestClass4<int, string>), closedType);
		}

		#region 测试辅助类

		private class TestClass
		{
			public static implicit operator int(TestClass tc)
			{
				return 0;
			}
		}
		private class TestClass2 : TestClass
		{
			public static implicit operator bool(TestClass2 tc)
			{
				return false;
			}
		}
		private class TestClass3
		{
			public static explicit operator int(TestClass3 tc)
			{
				return 0;
			}
		}
		private class TestClass4<T1, T2> { }
		private class TestClass5<T> : TestClass4<int, T> { }
		private class TestClass6
		{
			public static implicit operator TestClass6(int t)
			{
				return new TestClass6();
			}
			public static implicit operator int(TestClass6 t)
			{
				return 1;
			}
			public static implicit operator TestClass6(TestClass7 t)
			{
				return new TestClass6();
			}
			public static implicit operator TestClass8(TestClass6 t)
			{
				return new TestClass8();
			}
		}
		private class TestClass7 { }
		private class TestClass8 : TestClass7 { }
		private class TestClass9
		{
			public static explicit operator TestClass10(TestClass9 t)
			{
				return new TestClass10();
			}
			public static explicit operator TestClass9(TestClass11 t)
			{
				return new TestClass9();
			}
		}
		private class TestClass10 { }
		private class TestClass11 : TestClass10 { }
		private class TestClass12
		{
			public static implicit operator Nullable<int>(TestClass12 t)
			{
				return 12;
			}
			public static implicit operator TestClass12(Nullable<int> t)
			{
				return new TestClass12();
			}
		}
		private class TestClass13
		{
			public static implicit operator Enum(TestClass13 t)
			{
				return Tristate.False;
			}
		}
		private class TestClass14 : IEnumerable<TestClass10>
		{
			public static implicit operator Tristate(TestClass14 t)
			{
				return Tristate.False;
			}
			public IEnumerator<TestClass10> GetEnumerator() { return null; }
			IEnumerator IEnumerable.GetEnumerator() { return null; }
		}
		private struct TestStruct
		{
			public static implicit operator TestStruct(TestStruct3 tc)
			{
				return new TestStruct();
			}
			public static implicit operator TestStruct(TestStruct6? tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct2
		{
			public static implicit operator TestStruct(TestStruct2 tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct3 { }
		private struct TestStruct4 : IEnumerable<string>
		{
			public IEnumerator<string> GetEnumerator() { return null; }
			IEnumerator IEnumerable.GetEnumerator() { return null; }
		}
		private struct TestStruct5
		{
			public static implicit operator TestStruct?(TestStruct5 tc)
			{
				return new TestStruct();
			}
		}
		private struct TestStruct6 { }
		private delegate void TestDelegate<T>(T a);

		#endregion // 测试辅助类

	}
}
