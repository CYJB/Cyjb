using System.Text;
using System;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;


/// <summary>
/// <see cref="ValueList{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestValueList
{
	/// <summary>
	/// 测试默认构造函数。
	/// </summary>
	[TestMethod]
	public void TestDefaultCtor()
	{
		using var list = default(ValueList<char>);
		Assert.AreEqual(0, list.Length);

		list.Add('a');
		Assert.AreEqual(1, list.Length);
		Assert.AreEqual("a", list.ToString());
	}

	/// <summary>
	/// 测试 Span 构造函数。
	/// </summary>
	[TestMethod]
	public void TestSpanCtor()
	{
		using var list = new ValueList<char>(new char[1]);
		Assert.AreEqual(0, list.Length);

		list.Add('a');
		Assert.AreEqual(1, list.Length);
		Assert.AreEqual("a", list.ToString());
	}

	/// <summary>
	/// 测试初始容量的构造函数。
	/// </summary>
	[TestMethod]
	public void TestInitialCapacityCtor()
	{
		using var list = new ValueList<char>(1);
		Assert.AreEqual(0, list.Length);

		list.Add('a');
		Assert.AreEqual(1, list.Length);
		Assert.AreEqual("a", list.ToString());
	}

	/// <summary>
	/// 测试索引。
	/// </summary>
	[TestMethod]
	public void TestIndexer()
	{
		const string Text1 = "foobar";
		using var list = new ValueList<char>();

		list.Add(Text1);

		Assert.AreEqual('b', list[3]);
		list[3] = 'c';
		Assert.AreEqual('c', list[3]);
	}

	/// <summary>
	/// 测试添加元素。
	/// </summary>
	[TestMethod]
	public void TestAdd()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();
		for (int i = 1; i <= 100; i++)
		{
			sb.Append((char)i);
			list.Add((char)i);
		}

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试添加重复元素。
	/// </summary>
	[TestMethod]
	public void TestAddRepeat()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();
		for (int i = 1; i <= 100; i++)
		{
			sb.Append((char)i, i);
			list.Add((char)i, i);
		}

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试添加多个元素。
	/// </summary>
	[TestMethod]
	public void TestAddMulti()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();
		for (int i = 1; i <= 100; i++)
		{
			string s = i.ToString();
			sb.Append(s);
			list.Add(s);
		}

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试添加大量元素。
	/// </summary>
	[DataTestMethod]
	[DataRow(0, 4 * 1024 * 1024)]
	[DataRow(1025, 4 * 1024 * 1024)]
	[DataRow(3 * 1024 * 1024, 6 * 1024 * 1024)]
	public void Add_String_Large_MatchesStringBuilder(int initialLength, int stringLength)
	{
		var sb = new StringBuilder(initialLength);
		using var list = new ValueList<char>(new char[initialLength]);

		string s = new('a', stringLength);
		sb.Append(s);
		list.Add(s);

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试添加 <see cref="Span{T}"/>。
	/// </summary>
	[TestMethod]
	public void TestAddSpan()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();

		for (int i = 1; i <= 1000; i++)
		{
			string s = i.ToString();

			sb.Append(s);

			Span<char> span = list.AddSpan(s.Length);
			Assert.AreEqual(sb.Length, list.Length);

			s.AsSpan().CopyTo(span);
		}

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试插入重复元素。
	/// </summary>
	[TestMethod]
	public void TestInsertRepeat()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();
		var rand = new Random(42);

		for (int i = 1; i <= 100; i++)
		{
			int index = rand.Next(sb.Length);
			sb.Insert(index, new string((char)i, 1), i);
			list.Insert(index, (char)i, i);
		}

		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试 <see cref="ValueList{T}.AsSpan"/>。
	/// </summary>
	[TestMethod]
	public void TestAsSpan()
	{
		var sb = new StringBuilder();
		using var list = new ValueList<char>();

		for (int i = 1; i <= 100; i++)
		{
			string s = i.ToString();
			sb.Append(s);
			list.Add(s);
		}

		var resultString = new string(list.AsSpan());
		Assert.AreEqual(sb.ToString(), resultString);
		Assert.AreEqual(sb.Length, list.Length);
		Assert.AreEqual(sb.ToString(), list.ToString());
	}

	/// <summary>
	/// 测试 <see cref="ValueList{T}.TryCopyTo"/>。
	/// </summary>
	[TestMethod]
	public void TestTryCopyTo()
	{
		using var list = new ValueList<char>();

		const string Text = "expected text";
		list.Add(Text);
		Assert.AreEqual(Text.Length, list.Length);

		Span<char> dst = new char[Text.Length - 1];
		Assert.IsFalse(list.TryCopyTo(dst, out int charsWritten));
		Assert.AreEqual(0, charsWritten);

		dst = new char[Text.Length + 1];
		Assert.IsTrue(list.TryCopyTo(dst, out charsWritten));
		Assert.AreEqual(Text.Length, charsWritten);
	}

	/// <summary>
	/// 测试在 <see cref="ValueList{T}.Dispose"/> 后重复使用。
	/// </summary>
	[TestMethod]
	public void TestDisposeThenReusable()
	{
		const string Text1 = "test";
		using var list = new ValueList<char>();

		list.Add(Text1);
		Assert.AreEqual(Text1.Length, list.Length);

		list.Dispose();

		Assert.AreEqual(0, list.Length);
		Assert.AreEqual(string.Empty, list.ToString());
		Assert.IsTrue(list.TryCopyTo(Span<char>.Empty, out _));
		list.Dispose();

		const string Text2 = "another test";
		list.Add(Text2);
		Assert.AreEqual(Text2.Length, list.Length);
		Assert.AreEqual(Text2, list.ToString());
	}

	/// <summary>
	/// 测试 <see cref="ValueList{T}.EnsureCapacity"/>。
	/// </summary>
	[TestMethod]
	public void TestEnsureCapacity()
	{
		using var list = new ValueList<char>(stackalloc char[32]);

		list.EnsureCapacity(65);

		Assert.IsTrue(list.Capacity >= 65);
	}

	/// <summary>
	/// 测试 <see cref="ValueList{T}.EnsureCapacity"/> 加倍缓存的场景。
	/// </summary>
	[TestMethod]
	public void TestEnsureCapacityDoubleBuffer()
	{
		using var list = new ValueList<char>(stackalloc char[32]);

		list.EnsureCapacity(33);

		Assert.IsTrue(list.Capacity >= 64);
	}

	/// <summary>
	/// 测试 <see cref="ValueList{T}.EnsureCapacity"/> 不修改容量的场景。
	/// </summary>
	[TestMethod]
	public void TestEnsureCapacityNoAlloc()
	{
		using var list = new ValueList<char>(stackalloc char[64]);

		list.EnsureCapacity(16);

		Assert.AreEqual(64, list.Capacity);
	}
}
