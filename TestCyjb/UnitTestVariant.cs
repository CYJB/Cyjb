using System;
using System.Collections.Generic;
using Cyjb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// Variant 系列类的单元测试。
/// </summary>
[TestClass]
public class UnitTestVariant
{
	/// <summary>
	/// 对 <see cref="Variant{T1, T2}"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestVariant_2()
	{
		Variant<string, int> value = new(10);
		Assert.IsTrue(value.TryGetValue(out int ivalue));
		Assert.AreEqual(10, ivalue);
		Assert.IsFalse(value.TryGetValue(out string? svalue));
		Assert.AreEqual(10, (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);

		value = "10";
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsTrue(value.TryGetValue(out svalue));
		Assert.AreEqual("10", svalue);
		Assert.AreEqual("10", (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);

		value.SetValue(20);
		Assert.IsTrue(value.TryGetValue(out ivalue));
		Assert.AreEqual(20, ivalue);
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.AreEqual(20, (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
	}

	/// <summary>
	/// 对 <see cref="Variant{T1, T2, T3}"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestVariant_3()
	{
		Variant<string, int, double> value = new(10);
		Assert.IsTrue(value.TryGetValue(out int ivalue));
		Assert.AreEqual(10, ivalue);
		Assert.IsFalse(value.TryGetValue(out string? svalue));
		Assert.IsFalse(value.TryGetValue(out double dvalue));
		Assert.AreEqual(10, (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);

		value = "10";
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsTrue(value.TryGetValue(out svalue));
		Assert.AreEqual("10", svalue);
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.AreEqual("10", (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);

		value.SetValue(20.2);
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsTrue(value.TryGetValue(out dvalue));
		Assert.AreEqual(20.2, dvalue);
		Assert.AreEqual(20.2, (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
	}

	/// <summary>
	/// 对 <see cref="Variant{T1, T2, T3, T4}"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestVariant_4()
	{
		Variant<string, double, long, int> value = new(10);
		Assert.IsFalse(value.TryGetValue(out string? svalue));
		Assert.IsFalse(value.TryGetValue(out double dvalue));
		Assert.IsFalse(value.TryGetValue(out long lvalue));
		Assert.IsTrue(value.TryGetValue(out int ivalue));
		Assert.AreEqual(10, ivalue);
		Assert.AreEqual(10, (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);

		value = "10";
		Assert.IsTrue(value.TryGetValue(out svalue));
		Assert.AreEqual("10", svalue);
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.IsFalse(value.TryGetValue(out lvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.AreEqual("10", (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);

		value.SetValue(20.2);
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsTrue(value.TryGetValue(out dvalue));
		Assert.AreEqual(20.2, dvalue);
		Assert.IsFalse(value.TryGetValue(out lvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.AreEqual(20.2, (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);

		value = (long)20;
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsTrue(value.TryGetValue(out lvalue));
		Assert.AreEqual(20, lvalue);
		Assert.AreEqual(20, (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
	}

	/// <summary>
	/// 对 <see cref="Variant{T1, T2, T3, T4, T5}"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestVariant_5()
	{
		Variant<string, double, long, int, List<int>> value = new(10);
		Assert.IsFalse(value.TryGetValue(out string? svalue));
		Assert.IsFalse(value.TryGetValue(out double dvalue));
		Assert.IsFalse(value.TryGetValue(out long lvalue));
		Assert.IsTrue(value.TryGetValue(out int ivalue));
		Assert.AreEqual(10, ivalue);
		Assert.IsFalse(value.TryGetValue(out List<int>? livalue));
		Assert.AreEqual(10, (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (List<int>)value);

		value = "10";
		Assert.IsTrue(value.TryGetValue(out svalue));
		Assert.AreEqual("10", svalue);
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.IsFalse(value.TryGetValue(out lvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsFalse(value.TryGetValue(out livalue));
		Assert.AreEqual("10", (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (List<int>)value);

		value.SetValue(20.2);
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsTrue(value.TryGetValue(out dvalue));
		Assert.AreEqual(20.2, dvalue);
		Assert.IsFalse(value.TryGetValue(out lvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsFalse(value.TryGetValue(out livalue));
		Assert.AreEqual(20.2, (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (List<int>)value);

		value = (long)20;
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsTrue(value.TryGetValue(out lvalue));
		Assert.AreEqual(20, lvalue);
		Assert.IsFalse(value.TryGetValue(out livalue));
		Assert.AreEqual(20, (long)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (List<int>)value);

		List<int> list = new();
		value = list;
		Assert.IsFalse(value.TryGetValue(out svalue));
		Assert.IsFalse(value.TryGetValue(out dvalue));
		Assert.IsFalse(value.TryGetValue(out ivalue));
		Assert.IsFalse(value.TryGetValue(out lvalue));
		Assert.IsTrue(value.TryGetValue(out livalue));
		Assert.AreEqual(list, livalue);
		Assert.AreEqual(list, (List<int>)value);
		Assert.ThrowsException<InvalidCastException>(() => (string)value);
		Assert.ThrowsException<InvalidCastException>(() => (double)value);
		Assert.ThrowsException<InvalidCastException>(() => (int)value);
		Assert.ThrowsException<InvalidCastException>(() => (long)value);
	}
}

