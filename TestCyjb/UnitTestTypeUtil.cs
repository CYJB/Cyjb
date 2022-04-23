using System;
using System.Collections;
using System.Collections.Generic;
using Cyjb;
using Cyjb.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb
{
	/// <summary>
	/// <see cref="TypeUtil"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestTypeUtil
	{
		/// <summary>
		/// 对 <see cref="TypeUtil.IsNumeric"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsNumeric()
		{
			Assert.IsTrue(typeof(char).IsNumeric());
			Assert.IsTrue(typeof(sbyte).IsNumeric());
			Assert.IsTrue(typeof(byte).IsNumeric());
			Assert.IsTrue(typeof(short).IsNumeric());
			Assert.IsTrue(typeof(ushort).IsNumeric());
			Assert.IsTrue(typeof(int).IsNumeric());
			Assert.IsTrue(typeof(uint).IsNumeric());
			Assert.IsTrue(typeof(long).IsNumeric());
			Assert.IsTrue(typeof(ulong).IsNumeric());
			Assert.IsTrue(typeof(float).IsNumeric());
			Assert.IsTrue(typeof(double).IsNumeric());
			Assert.IsTrue(typeof(decimal).IsNumeric());

			Assert.IsFalse(typeof(bool).IsNumeric());
			Assert.IsFalse(typeof(DateTime).IsNumeric());
			Assert.IsFalse(typeof(DBNull).IsNumeric());
			Assert.IsFalse(Fake.Null<Type>().IsNumeric());
			Assert.IsFalse(typeof(object).IsNumeric());
			Assert.IsFalse(typeof(string).IsNumeric());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsUnsigned"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsUnsigned()
		{
			Assert.IsTrue(typeof(char).IsUnsigned());
			Assert.IsTrue(typeof(byte).IsUnsigned());
			Assert.IsTrue(typeof(ushort).IsUnsigned());
			Assert.IsTrue(typeof(uint).IsUnsigned());
			Assert.IsTrue(typeof(ulong).IsUnsigned());

			Assert.IsFalse(typeof(sbyte).IsUnsigned());
			Assert.IsFalse(typeof(short).IsUnsigned());
			Assert.IsFalse(typeof(int).IsUnsigned());
			Assert.IsFalse(typeof(long).IsUnsigned());
			Assert.IsFalse(typeof(float).IsUnsigned());
			Assert.IsFalse(typeof(double).IsUnsigned());
			Assert.IsFalse(typeof(decimal).IsUnsigned());

			Assert.IsFalse(typeof(bool).IsUnsigned());
			Assert.IsFalse(typeof(DateTime).IsUnsigned());
			Assert.IsFalse(typeof(DBNull).IsUnsigned());
			Assert.IsFalse(Fake.Null<Type>().IsUnsigned());
			Assert.IsFalse(typeof(object).IsUnsigned());
			Assert.IsFalse(typeof(string).IsUnsigned());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsSigned"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsSigned()
		{
			Assert.IsTrue(typeof(sbyte).IsSigned());
			Assert.IsTrue(typeof(short).IsSigned());
			Assert.IsTrue(typeof(int).IsSigned());
			Assert.IsTrue(typeof(long).IsSigned());

			Assert.IsFalse(typeof(char).IsSigned());
			Assert.IsFalse(typeof(byte).IsSigned());
			Assert.IsFalse(typeof(ushort).IsSigned());
			Assert.IsFalse(typeof(uint).IsSigned());
			Assert.IsFalse(typeof(ulong).IsSigned());
			Assert.IsFalse(typeof(float).IsSigned());
			Assert.IsFalse(typeof(double).IsSigned());
			Assert.IsFalse(typeof(decimal).IsSigned());

			Assert.IsFalse(typeof(bool).IsSigned());
			Assert.IsFalse(typeof(DateTime).IsSigned());
			Assert.IsFalse(typeof(DBNull).IsSigned());
			Assert.IsFalse(Fake.Null<Type>().IsSigned());
			Assert.IsFalse(typeof(object).IsSigned());
			Assert.IsFalse(typeof(string).IsSigned());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsNullable"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsNullable()
		{
			Assert.IsTrue(typeof(int?).IsNullable());
			Assert.IsFalse(typeof(int).IsNullable());
			Assert.IsFalse(typeof(object).IsNullable());
			Assert.IsFalse(Fake.Null<Type>().IsNullable());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.GetNonNullableType"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGetNonNullableType()
		{
			Assert.AreEqual(typeof(int), typeof(int?).GetNonNullableType());
			Assert.AreEqual(typeof(int), typeof(int).GetNonNullableType());
			Assert.AreEqual(typeof(object), typeof(object).GetNonNullableType());
			Assert.ThrowsException<ArgumentNullException>(() => Fake.Null<Type>().GetNonNullableType());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsDelegate"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsDelegate()
		{
			Assert.IsFalse(typeof(int).IsDelegate());
			Assert.IsFalse(Fake.Null<Type>().IsDelegate());
			Assert.IsFalse(typeof(Delegate).IsDelegate());
			Assert.IsTrue(typeof(Action).IsDelegate());
			Assert.IsTrue(typeof(Func<int>).IsDelegate());
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsImplicitFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsImplicitFrom()
		{
			Assert.IsTrue(typeof(short).IsImplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(short).IsImplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(short).IsImplicitFrom(typeof(short)));
			Assert.IsFalse(typeof(short).IsImplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(ushort).IsImplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(ushort).IsImplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(int).IsImplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(uint).IsImplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(uint).IsImplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(decimal).IsImplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(decimal).IsImplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(long?).IsImplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(object).IsImplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(IList<int>).IsImplicitFrom(typeof(List<int>)));
			Assert.IsFalse(typeof(int).IsImplicitFrom(typeof(object)));
			Assert.IsFalse(typeof(List<int>).IsImplicitFrom(typeof(IList<int>)));

			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(byte)));
			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(short)));
			Assert.IsFalse(typeof(short).IsConvertFrom(typeof(int)));
			Assert.IsTrue(typeof(ushort).IsConvertFrom(typeof(char)));
			Assert.IsTrue(typeof(ushort).IsConvertFrom(typeof(byte)));
			Assert.IsTrue(typeof(int).IsConvertFrom(typeof(char)));
			Assert.IsTrue(typeof(int).IsConvertFrom(typeof(ushort)));
			Assert.IsTrue(typeof(uint).IsConvertFrom(typeof(char)));
			Assert.IsTrue(typeof(uint).IsConvertFrom(typeof(ushort)));
			Assert.IsTrue(typeof(decimal).IsConvertFrom(typeof(long)));
			Assert.IsTrue(typeof(decimal).IsConvertFrom(typeof(ulong)));
			Assert.IsTrue(typeof(long?).IsConvertFrom(typeof(int?)));
			Assert.IsTrue(typeof(object).IsConvertFrom(typeof(uint)));
			Assert.IsTrue(typeof(IList<int>).IsConvertFrom(typeof(List<int>)));
			Assert.IsFalse(typeof(int).IsConvertFrom(typeof(object)));
			Assert.IsFalse(typeof(List<int>).IsConvertFrom(typeof(IList<int>)));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.IsExplicitFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestIsExplicitFrom()
		{
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(sbyte)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(short)));
			Assert.IsTrue(typeof(short).IsExplicitFrom(typeof(int)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(ushort).IsExplicitFrom(typeof(byte)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(char)));
			Assert.IsTrue(typeof(uint).IsExplicitFrom(typeof(ushort)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(long)));
			Assert.IsTrue(typeof(decimal).IsExplicitFrom(typeof(ulong)));
			Assert.IsTrue(typeof(long?).IsExplicitFrom(typeof(int?)));
			Assert.IsTrue(typeof(object).IsExplicitFrom(typeof(uint)));
			Assert.IsTrue(typeof(IList<int>).IsExplicitFrom(typeof(List<int>)));
			Assert.IsTrue(typeof(int).IsExplicitFrom(typeof(object)));
			Assert.IsTrue(typeof(List<int>).IsExplicitFrom(typeof(IList<int>)));

			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(sbyte), true));
			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(byte), true));
			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(short), true));
			Assert.IsTrue(typeof(short).IsConvertFrom(typeof(int), true));
			Assert.IsTrue(typeof(ushort).IsConvertFrom(typeof(char), true));
			Assert.IsTrue(typeof(ushort).IsConvertFrom(typeof(byte), true));
			Assert.IsTrue(typeof(int).IsConvertFrom(typeof(char), true));
			Assert.IsTrue(typeof(int).IsConvertFrom(typeof(ushort), true));
			Assert.IsTrue(typeof(uint).IsConvertFrom(typeof(char), true));
			Assert.IsTrue(typeof(uint).IsConvertFrom(typeof(ushort), true));
			Assert.IsTrue(typeof(decimal).IsConvertFrom(typeof(long), true));
			Assert.IsTrue(typeof(decimal).IsConvertFrom(typeof(ulong), true));
			Assert.IsTrue(typeof(long?).IsConvertFrom(typeof(int?), true));
			Assert.IsTrue(typeof(object).IsConvertFrom(typeof(uint), true));
			Assert.IsTrue(typeof(IList<int>).IsConvertFrom(typeof(List<int>), true));
			Assert.IsTrue(typeof(int).IsConvertFrom(typeof(object), true));
			Assert.IsTrue(typeof(List<int>).IsConvertFrom(typeof(IList<int>), true));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.GetEncompassingType"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGetEncompassingType()
		{
			Type[] types = new Type[] { typeof(short), typeof(sbyte) };
			Assert.AreEqual(typeof(short), TypeUtil.GetEncompassingType(types));

			types = new Type[] { typeof(char), typeof(short), typeof(int), typeof(int?) };
			Assert.AreEqual(typeof(int?), TypeUtil.GetEncompassingType(types));

			types = new Type[] { typeof(IList<int>), typeof(ICollection<int>), typeof(List<int>), typeof(LinkedList<int>) };
			Assert.AreEqual(typeof(ICollection<int>), TypeUtil.GetEncompassingType(types));

			types = new Type[] { typeof(void), typeof(void) };
			Assert.AreEqual(typeof(void), TypeUtil.GetEncompassingType(types));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.GetEncompassedType"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestGetEncompassedType()
		{
			Type[] types = new Type[] { typeof(short), typeof(sbyte) };
			Assert.AreEqual(typeof(sbyte), TypeUtil.GetEncompassedType(types));

			types = new Type[] { typeof(char), typeof(ushort), typeof(int), typeof(int?) };
			Assert.AreEqual(typeof(char), TypeUtil.GetEncompassedType(types));

			types = new Type[] { typeof(IList<int>), typeof(ICollection<int>), typeof(List<int>), typeof(LinkedList<int>) };
			Assert.IsNull(TypeUtil.GetEncompassedType(types));

			types = new Type[] { typeof(void), typeof(void) };
			Assert.AreEqual(typeof(void), TypeUtil.GetEncompassedType(types));
		}

		private class TestList<T> : List<T>
		{
		}

		private class TestClass : IEnumerable<int>, IEnumerable<string>
		{
			IEnumerator<int> IEnumerable<int>.GetEnumerator()
			{
				throw new NotImplementedException();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}

		private class TestBaseClass<T1, T2> { }
		private class TestSubClass<T> : TestBaseClass<int, T> { }

		/// <summary>
		/// 对 <see cref="TypeUtil.CloseDefinitionFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestCloseDefinitionFrom()
		{
			Assert.IsNull(typeof(int).CloseDefinitionFrom(typeof(IEquatable<int>)));
			Assert.IsNull(typeof(string).CloseDefinitionFrom(typeof(object)));
			Assert.IsNull(typeof(object).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.IsNull(typeof(IEnumerable).CloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(IEnumerable<int>).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.IsNull(typeof(IEnumerable<int>).CloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(IEnumerable<int>).CloseDefinitionFrom(typeof(IEnumerable<int>)));
			Assert.IsNull(typeof(List<int>).CloseDefinitionFrom(typeof(TestList<int>)));

			Assert.IsNull(typeof(TestList<>).CloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(List<>).CloseDefinitionFrom(typeof(object)));
			Assert.AreEqual(typeof(List<int>), typeof(List<>).CloseDefinitionFrom(typeof(TestList<int>)));

			Assert.AreEqual(typeof(IEnumerable<>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(IEnumerable<>)));
			Assert.AreEqual(typeof(IEnumerable<int>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(IEnumerable<int>)));
			Assert.AreEqual(typeof(IEnumerable<int>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(List<int>)));
			Assert.AreEqual(typeof(IEnumerable<KeyValuePair<int, string>>), typeof(IEnumerable<>).CloseDefinitionFrom(typeof(Dictionary<int, string>)));
			Assert.AreEqual(typeof(ICollection<int>), typeof(ICollection<>).CloseDefinitionFrom(typeof(List<int>)));
			Assert.AreEqual(typeof(IDictionary<int, string>), typeof(IDictionary<,>).CloseDefinitionFrom(typeof(Dictionary<int, string>)));
			Assert.AreEqual(typeof(TestBaseClass<int, string>), typeof(TestBaseClass<,>).CloseDefinitionFrom(typeof(TestSubClass<string>)));

			Type? type = typeof(IEnumerable<>).CloseDefinitionFrom(typeof(TestClass));
			Assert.IsTrue(type == typeof(IEnumerable<int>) || type == typeof(IEnumerable<string>));

			type = typeof(TestBaseClass<,>).CloseDefinitionFrom(typeof(TestSubClass<>));
			Type[] genericArgs = type.GetGenericArguments();
			Assert.AreEqual(2, genericArgs.Length);
			Assert.AreEqual(typeof(int), genericArgs[0]);
			Assert.IsTrue(genericArgs[1].IsGenericParameter);
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.UniqueCloseDefinitionFrom"/> 方法进行测试。
		/// </summary>
		[TestMethod]
		public void TestUniqueCloseDefinitionFrom()
		{
			Assert.IsNull(typeof(int).UniqueCloseDefinitionFrom(typeof(IEquatable<int>)));
			Assert.IsNull(typeof(string).UniqueCloseDefinitionFrom(typeof(object)));
			Assert.IsNull(typeof(IEnumerable).UniqueCloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(IEnumerable<int>).UniqueCloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(List<int>).UniqueCloseDefinitionFrom(typeof(TestList<int>)));

			Assert.IsNull(typeof(TestList<>).UniqueCloseDefinitionFrom(typeof(List<int>)));
			Assert.IsNull(typeof(List<>).UniqueCloseDefinitionFrom(typeof(object)));
			Assert.AreEqual(typeof(List<int>), typeof(List<>).UniqueCloseDefinitionFrom(typeof(TestList<int>)));

			Assert.AreEqual(typeof(IEnumerable<int>), typeof(IEnumerable<>).UniqueCloseDefinitionFrom(typeof(List<int>)));
			Assert.AreEqual(typeof(ICollection<int>), typeof(ICollection<>).UniqueCloseDefinitionFrom(typeof(List<int>)));

			Assert.IsNull(typeof(IEnumerable<>).UniqueCloseDefinitionFrom(typeof(TestClass)));
		}

		/// <summary>
		/// 对 <see cref="TypeUtil.FullName"/> 方法进行测试。
		/// </summary>
		[DataTestMethod]
		[DataRow("System.Int32", typeof(int))]
		[DataRow("System.Collections.IEnumerable", typeof(IEnumerable))]
		[DataRow("System.Collections.Generic.IEnumerable`1", typeof(IEnumerable<>))]
		[DataRow("System.Collections.Generic.IEnumerable`1[System.Int32]", typeof(IEnumerable<int>))]
		[DataRow("System.Collections.Generic.Dictionary`2[System.String,System.Nullable`1[System.Int32]]",
			typeof(Dictionary<string, int?>))]
		public void TestFullName(string fullName, Type type)
		{
			Assert.AreEqual(fullName, type.FullName());
		}
	}
}

