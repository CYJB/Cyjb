using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Cyjb;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb;

/// <summary>
/// <see cref="StringView"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestStringView
{
	/// <summary>
	/// 对创建 <see cref="StringView"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestCtor()
	{
		string text = "abcdefg";
		Assert.AreEqual("abcdefg", new StringView(text).ToString());
		Assert.AreEqual("abcdefg", new StringView(text, 0).ToString());
		Assert.AreEqual("bcdefg", new StringView(text, 1).ToString());
		Assert.AreEqual("fg", new StringView(text, 5).ToString());
		Assert.AreEqual("", new StringView(text, 7).ToString());
		Assert.AreEqual("", new StringView(text, 0, 0).ToString());
		Assert.AreEqual("b", new StringView(text, 1, 1).ToString());
		Assert.AreEqual("ef", new StringView(text, 4, 2).ToString());
		Assert.AreEqual("", new StringView(text, 7, 0).ToString());

		Assert.AreEqual("abcdefg", text.AsView().ToString());
		Assert.AreEqual("bcdefg", text.AsView(1).ToString());
		Assert.AreEqual("b", text.AsView(1, 1).ToString());
	}

	/// <summary>
	/// 对 <see cref="StringView"/> 索引访问进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexer()
	{
		StringView view = "abcdefg".AsView(2, 2);
		Assert.AreEqual(2, view.Length);
		Assert.AreEqual('c', view[0]);
		Assert.AreEqual('d', view[1]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			char c = view[2];
		});
	}

	/// <summary>
	/// 对创建 <see cref="StringView.GetOrigin"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestGetOrigin()
	{
		string text = "abcdefg";
		text.AsView(1, 3).GetOrigin(out var originText, out var start, out var length);
		Assert.AreSame(text, originText);
		Assert.AreEqual(1, start);
		Assert.AreEqual(3, length);
	}

	/// <summary>
	/// 对 <see cref="StringView.AsSpan"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAsSpan()
	{
		string text = "abcdefg";
		StringView view = "0abcdefg3".AsView(1, 7);
		Assert.IsTrue(text.AsSpan().SequenceEqual(view.AsSpan()));
		Assert.IsTrue(text.AsSpan(0).SequenceEqual(view.AsSpan(0)));
		Assert.IsTrue(text.AsSpan(6).SequenceEqual(view.AsSpan(6)));
		Assert.IsTrue(text.AsSpan(7).SequenceEqual(view.AsSpan(7)));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.AsSpan(8);
		});
		Assert.IsTrue(text.AsSpan(0, 0).SequenceEqual(view.AsSpan(0, 0)));
		Assert.IsTrue(text.AsSpan(0, 4).SequenceEqual(view.AsSpan(0, 4)));
		Assert.IsTrue(text.AsSpan(2, 4).SequenceEqual(view.AsSpan(2, 4)));
		Assert.IsTrue(text.AsSpan(3, 4).SequenceEqual(view.AsSpan(3, 4)));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.AsSpan(3, 5);
		});
	}

	/// <summary>
	/// 对 <see cref="StringView.AsMemory"/> 方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestAsMemory()
	{
		string text = "abcdefg";
		StringView view = "0abcdefg3".AsView(1, 7);
		Assert.IsTrue(text.AsMemory().Span.SequenceEqual(view.AsMemory().Span));
		Assert.IsTrue(text.AsMemory(0).Span.SequenceEqual(view.AsMemory(0).Span));
		Assert.IsTrue(text.AsMemory(6).Span.SequenceEqual(view.AsMemory(6).Span));
		Assert.IsTrue(text.AsMemory(7).Span.SequenceEqual(view.AsMemory(7).Span));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.AsMemory(8);
		});
		Assert.IsTrue(text.AsMemory(0, 0).Span.SequenceEqual(view.AsMemory(0, 0).Span));
		Assert.IsTrue(text.AsMemory(0, 4).Span.SequenceEqual(view.AsMemory(0, 4).Span));
		Assert.IsTrue(text.AsMemory(2, 4).Span.SequenceEqual(view.AsMemory(2, 4).Span));
		Assert.IsTrue(text.AsMemory(3, 4).Span.SequenceEqual(view.AsMemory(3, 4).Span));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.AsMemory(3, 5);
		});
	}

	/// <summary>
	/// 对比较 <see cref="StringView"/> 进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow("abc", 0, 3, "abc", "abc")]
	[DataRow("xabcx", 1, 3, "abc", "abc")]
	[DataRow("abc", 0, 3, "abc", "abcd")]
	[DataRow("xabc", 1, 3, "abc", "accd")]
	[DataRow("abcx", 0, 3, "abc", "aacd")]
	[DataRow("abc", 0, 2, "ab", "ab")]
	[DataRow("abc", 0, 3, "abc", "ab")]
	[DataRow("abc", 0, 3, "abc", "ac")]
	public void TestComparison(string text, int start, int count, string test, string other)
	{
		StringView view = text.AsView(start, count);
		Assert.AreEqual(Math.Sign(test.CompareTo(other)), Math.Sign(view.CompareTo(other)));
		Assert.AreEqual(Math.Sign(test.CompareTo((object)other)), Math.Sign(view.CompareTo((object)other)));

		StringView otherView = other.AsView();
		Assert.AreEqual(Math.Sign(test.CompareTo(other)), Math.Sign(view.CompareTo(otherView)));
		Assert.AreEqual(Math.Sign(test.CompareTo((object)other)), Math.Sign(view.CompareTo((object)otherView)));
		Assert.AreEqual(Math.Sign(string.CompareOrdinal(test, other)), Math.Sign(StringView.CompareOrdinal(view, otherView)));

		StringView otherView2 = $"_{other}_".AsView(1, other.Length);
		Assert.AreEqual(Math.Sign(test.CompareTo(other)), Math.Sign(view.CompareTo(otherView2)));
		Assert.AreEqual(Math.Sign(test.CompareTo((object)other)), Math.Sign(view.CompareTo((object)otherView2)));
		Assert.AreEqual(Math.Sign(string.CompareOrdinal(test, other)), Math.Sign(StringView.CompareOrdinal(view, otherView2)));

		Assert.AreEqual(test == other, view == other);
		Assert.AreEqual(test == other, view == otherView);
		Assert.AreEqual(test == other, view == otherView2);
		Assert.AreEqual(test.GetHashCode(), view.GetHashCode());
	}

	/// <summary>
	/// 对 <see cref="StringView.Contains"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestContains()
	{
		StringView view = "abc".AsView();
		Assert.IsTrue(view.Contains('b'));
		Assert.IsFalse(view.Contains('B'));
		Assert.IsTrue(view.Contains('B', StringComparison.OrdinalIgnoreCase));
		Assert.IsTrue(view.Contains("abc"));
		Assert.IsFalse(view.Contains("Abc"));
		Assert.IsTrue(view.Contains("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(view.Contains('d'));

		view = "0abcd".AsView(1, 3);
		Assert.IsTrue(view.Contains('b'));
		Assert.IsFalse(view.Contains('B'));
		Assert.IsTrue(view.Contains('B', StringComparison.OrdinalIgnoreCase));
		Assert.IsTrue(view.Contains("abc"));
		Assert.IsFalse(view.Contains("Abc"));
		Assert.IsTrue(view.Contains("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(view.Contains('d'));
	}

	/// <summary>
	/// 对 <see cref="StringView.IndexOf"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOf()
	{
		StringView view = "abcaBc".AsView();
		Assert.AreEqual(0, view.IndexOf('a'));
		Assert.AreEqual(3, view.IndexOf('a', 1));
		Assert.AreEqual(-1, view.IndexOf('a', 1, 2));
		Assert.AreEqual(1, view.IndexOf('b'));
		Assert.AreEqual(-1, view.IndexOf('b', 2));
		Assert.AreEqual(1, view.IndexOf('b', 1, 3));
		Assert.AreEqual(-1, view.IndexOf('C'));
		Assert.AreEqual(-1, view.IndexOf('C', 3));
		Assert.AreEqual(-1, view.IndexOf('C', 3, 3));
		Assert.AreEqual(2, view.IndexOf('C', StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(0, view.IndexOf("abc"));
		Assert.AreEqual(-1, view.IndexOf("abc", 1));
		Assert.AreEqual(0, view.IndexOf("abc", 0, 3));
		Assert.AreEqual(-1, view.IndexOf("Abc"));
		Assert.AreEqual(-1, view.IndexOf("Abc", 4));
		Assert.AreEqual(-1, view.IndexOf("Abc", 4, 2));
		Assert.AreEqual(3, view.IndexOf("aBc"));
		Assert.AreEqual(3, view.IndexOf("aBc", 2));
		Assert.AreEqual(-1, view.IndexOf("aBc", 2, 3));
		Assert.AreEqual(0, view.IndexOf("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, view.IndexOf('d'));

		view = "0abcaBcd".AsView(1, 6);
		Assert.AreEqual(0, view.IndexOf('a'));
		Assert.AreEqual(3, view.IndexOf('a', 1));
		Assert.AreEqual(-1, view.IndexOf('a', 1, 2));
		Assert.AreEqual(1, view.IndexOf('b'));
		Assert.AreEqual(-1, view.IndexOf('b', 2));
		Assert.AreEqual(1, view.IndexOf('b', 1, 3));
		Assert.AreEqual(-1, view.IndexOf('C'));
		Assert.AreEqual(-1, view.IndexOf('C', 3));
		Assert.AreEqual(-1, view.IndexOf('C', 3, 3));
		Assert.AreEqual(2, view.IndexOf('C', StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(0, view.IndexOf("abc"));
		Assert.AreEqual(-1, view.IndexOf("abc", 1));
		Assert.AreEqual(0, view.IndexOf("abc", 0, 3));
		Assert.AreEqual(-1, view.IndexOf("Abc"));
		Assert.AreEqual(-1, view.IndexOf("Abc", 4));
		Assert.AreEqual(-1, view.IndexOf("Abc", 4, 2));
		Assert.AreEqual(3, view.IndexOf("aBc"));
		Assert.AreEqual(3, view.IndexOf("aBc", 2));
		Assert.AreEqual(-1, view.IndexOf("aBc", 2, 3));
		Assert.AreEqual(0, view.IndexOf("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, view.IndexOf('d'));
	}

	/// <summary>
	/// 对 <see cref="StringView.IndexOfAny"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestIndexOfAny()
	{
		StringView view = "abcaBc".AsView();
		Assert.AreEqual(0, view.IndexOfAny(new char[] { 'a', 'b' }));
		Assert.AreEqual(1, view.IndexOfAny(new char[] { 'a', 'b' }, 1));
		Assert.AreEqual(3, view.IndexOfAny(new char[] { 'a', 'b' }, 2));
		Assert.AreEqual(-1, view.IndexOfAny(new char[] { 'a', 'b' }, 2, 1));
		Assert.AreEqual(1, view.IndexOfAny(new char[] { 'b', 'c' }));
		Assert.AreEqual(-1, view.IndexOfAny(new char[] { 'C' }));

		view = "0abcaBcd".AsView(1, 6);
		Assert.AreEqual(0, view.IndexOfAny(new char[] { 'a', 'b' }));
		Assert.AreEqual(1, view.IndexOfAny(new char[] { 'a', 'b' }, 1));
		Assert.AreEqual(3, view.IndexOfAny(new char[] { 'a', 'b' }, 2));
		Assert.AreEqual(-1, view.IndexOfAny(new char[] { 'a', 'b' }, 2, 1));
		Assert.AreEqual(1, view.IndexOfAny(new char[] { 'b', 'c' }));
		Assert.AreEqual(-1, view.IndexOfAny(new char[] { 'C' }));
	}

	/// <summary>
	/// 对 <see cref="StringView.LastIndexOf"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOf()
	{
		StringView view = "abcaBc".AsView();
		Assert.AreEqual(3, view.LastIndexOf('a'));
		Assert.AreEqual(-1, view.LastIndexOf('a', 4));
		Assert.AreEqual(0, view.LastIndexOf('a', 0, 3));
		Assert.AreEqual(1, view.LastIndexOf('b'));
		Assert.AreEqual(1, view.LastIndexOf('b', 1));
		Assert.AreEqual(-1, view.LastIndexOf('b', 2, 3));
		Assert.AreEqual(-1, view.LastIndexOf('C'));
		Assert.AreEqual(5, view.LastIndexOf('C', StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(0, view.LastIndexOf("abc"));
		Assert.AreEqual(-1, view.LastIndexOf("abc", 1));
		Assert.AreEqual(-1, view.LastIndexOf("Abc"));
		Assert.AreEqual(3, view.LastIndexOf("aBc"));
		Assert.AreEqual(3, view.LastIndexOf("aBc", 3));
		Assert.AreEqual(-1, view.LastIndexOf("aBc", 3, 2));
		Assert.AreEqual(3, view.LastIndexOf("aBc", 3, 3));
		Assert.AreEqual(3, view.LastIndexOf("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, view.LastIndexOf('d'));

		view = "0abcaBcd".AsView(1, 6);
		Assert.AreEqual(3, view.LastIndexOf('a'));
		Assert.AreEqual(-1, view.LastIndexOf('a', 4));
		Assert.AreEqual(0, view.LastIndexOf('a', 0, 3));
		Assert.AreEqual(1, view.LastIndexOf('b'));
		Assert.AreEqual(1, view.LastIndexOf('b', 1));
		Assert.AreEqual(-1, view.LastIndexOf('b', 2, 3));
		Assert.AreEqual(-1, view.LastIndexOf('C'));
		Assert.AreEqual(5, view.LastIndexOf('C', StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(0, view.LastIndexOf("abc"));
		Assert.AreEqual(-1, view.LastIndexOf("abc", 1));
		Assert.AreEqual(-1, view.LastIndexOf("Abc"));
		Assert.AreEqual(3, view.LastIndexOf("aBc"));
		Assert.AreEqual(3, view.LastIndexOf("aBc", 3));
		Assert.AreEqual(-1, view.LastIndexOf("aBc", 3, 2));
		Assert.AreEqual(3, view.LastIndexOf("aBc", 3, 3));
		Assert.AreEqual(3, view.LastIndexOf("Abc", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(-1, view.LastIndexOf('d'));
	}

	/// <summary>
	/// 对 <see cref="StringView.LastIndexOfAny"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestLastIndexOfAny()
	{
		StringView view = "abcaBc".AsView();
		Assert.AreEqual(3, view.LastIndexOfAny(new char[] { 'a', 'b' }));
		Assert.AreEqual(-1, view.LastIndexOfAny(new char[] { 'a', 'b' }, 4));
		Assert.AreEqual(1, view.LastIndexOfAny(new char[] { 'a', 'b' }, 1, 2));
		Assert.AreEqual(5, view.LastIndexOfAny(new char[] { 'b', 'c' }));
		Assert.AreEqual(-1, view.LastIndexOfAny(new char[] { 'C' }));

		view = "0abcaBcd".AsView(1, 6);
		Assert.AreEqual(3, view.LastIndexOfAny(new char[] { 'a', 'b' }));
		Assert.AreEqual(-1, view.LastIndexOfAny(new char[] { 'a', 'b' }, 4));
		Assert.AreEqual(1, view.LastIndexOfAny(new char[] { 'a', 'b' }, 1, 2));
		Assert.AreEqual(5, view.LastIndexOfAny(new char[] { 'b', 'c' }));
		Assert.AreEqual(-1, view.LastIndexOfAny(new char[] { 'C' }));
	}

	/// <summary>
	/// 对 <see cref="StringView.StartsWith"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestStartsWith()
	{
		StringView view = "abcaBc".AsView();
		Assert.IsTrue(view.StartsWith('a'));
		Assert.IsFalse(view.StartsWith('0'));
		Assert.IsTrue(view.StartsWith("abc"));
		Assert.IsFalse(view.StartsWith("0abc"));
		Assert.IsFalse(view.StartsWith("aBc"));
		Assert.IsTrue(view.StartsWith("aBc", StringComparison.OrdinalIgnoreCase));

		view = "0abcaBcd".AsView(1, 6);
		Assert.IsTrue(view.StartsWith('a'));
		Assert.IsFalse(view.StartsWith('0'));
		Assert.IsTrue(view.StartsWith("abc"));
		Assert.IsFalse(view.StartsWith("0abc"));
		Assert.IsFalse(view.StartsWith("aBc"));
		Assert.IsTrue(view.StartsWith("aBc", StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// 对 <see cref="StringView.EndsWith"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestEndsWith()
	{
		StringView view = "abcaBc".AsView();
		Assert.IsTrue(view.EndsWith('c'));
		Assert.IsFalse(view.EndsWith('d'));
		Assert.IsTrue(view.EndsWith("aBc"));
		Assert.IsFalse(view.EndsWith("aBcd"));
		Assert.IsFalse(view.EndsWith("Abc"));
		Assert.IsTrue(view.EndsWith("Abc", StringComparison.OrdinalIgnoreCase));

		view = "0abcaBcd".AsView(1, 6);
		Assert.IsTrue(view.EndsWith('c'));
		Assert.IsFalse(view.EndsWith('d'));
		Assert.IsTrue(view.EndsWith("aBc"));
		Assert.IsFalse(view.EndsWith("aBcd"));
		Assert.IsFalse(view.EndsWith("Abc"));
		Assert.IsTrue(view.EndsWith("Abc", StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// 对 <see cref="StringView.Insert"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestInsert()
	{
		Assert.AreEqual("abc", "".AsView().Insert(0, "abc"));
		StringView view = "0abcd".AsView(1, 3);
		Assert.AreEqual("abc", view.Insert(0, ""));
		Assert.AreEqual("ababc", view.Insert(0, "ab"));
		Assert.AreEqual("aabbc", view.Insert(1, "ab"));
		Assert.AreEqual("ababc", view.Insert(2, "ab"));
		Assert.AreEqual("abcab", view.Insert(3, "ab"));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.Insert(4, "ab");
		});
	}

	/// <summary>
	/// 对 <see cref="StringView.Remove"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestRemove()
	{
		StringView view = "0abcd".AsView(1, 3);
		Assert.AreEqual("", view.Remove(0));
		Assert.AreEqual("a", view.Remove(1));
		Assert.AreEqual("ab", view.Remove(2));
		Assert.AreEqual("abc", view.Remove(3));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.Remove(4);
		});

		Assert.AreEqual("abc", view.Remove(0, 0));
		Assert.AreEqual("bc", view.Remove(0, 1));
		Assert.AreEqual("ac", view.Remove(1, 1));
		Assert.AreEqual("ab", view.Remove(2, 1));
		Assert.AreEqual("c", view.Remove(0, 2));
		Assert.AreEqual("a", view.Remove(1, 2));
		Assert.AreEqual("", view.Remove(0, 3));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.Remove(0, 4);
		});
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			view.Remove(2, 2);
		});
	}

	/// <summary>
	/// 对 <see cref="StringView.PadLeft"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestPadLeft()
	{
		StringView view = "0abcd".AsView(1, 3);
		Assert.AreEqual("abc", view.PadLeft(0));
		Assert.AreEqual("abc", view.PadLeft(3));
		Assert.AreEqual(" abc", view.PadLeft(4));
		Assert.AreEqual("  abc", view.PadLeft(5));

		Assert.AreEqual("abc", view.PadLeft(0, '0'));
		Assert.AreEqual("abc", view.PadLeft(3, '0'));
		Assert.AreEqual("0abc", view.PadLeft(4, '0'));
		Assert.AreEqual("00abc", view.PadLeft(5, '0'));
	}

	/// <summary>
	/// 对 <see cref="StringView.PadRight"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestPadRight()
	{
		StringView view = "0abcd".AsView(1, 3);
		Assert.AreEqual("abc", view.PadRight(0));
		Assert.AreEqual("abc", view.PadRight(3));
		Assert.AreEqual("abc ", view.PadRight(4));
		Assert.AreEqual("abc  ", view.PadRight(5));

		Assert.AreEqual("abc", view.PadRight(0, '0'));
		Assert.AreEqual("abc", view.PadRight(3, '0'));
		Assert.AreEqual("abc0", view.PadRight(4, '0'));
		Assert.AreEqual("abc00", view.PadRight(5, '0'));
	}

	/// <summary>
	/// 对 <see cref="StringView.Concat"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestConcat()
	{
		StringView view1 = "0abcd".AsView(1, 3);
		StringView view2 = "def".AsView();
		StringView view3 = "defgh".AsView(3);
		StringView view4 = "ijkl".AsView(0, 2);
		StringView view5 = "ijkl".AsView(2);
		Assert.AreEqual("abcdef", StringView.Concat(view1, view2));
		Assert.AreEqual("abcdefgh", StringView.Concat(view1, view2, view3));
		Assert.AreEqual("abcdefghij", StringView.Concat(view1, view2, view3, view4));

		StringView[] views1 = Array.Empty<StringView>();
		StringView[] views2 = new StringView[] { view1 };
		StringView[] views3 = new StringView[] { view1, view2, view3, view4, view5 };
		Assert.AreEqual("", StringView.Concat(views1));
		Assert.AreEqual("abc", StringView.Concat(views2));
		Assert.AreEqual("abcdefghijkl", StringView.Concat(view1, view2, view3, view4, view5));
		Assert.AreEqual("abcdefghijkl", StringView.Concat(views3));
		Assert.AreEqual("", StringView.Concat((IEnumerable<StringView>)views1));
		Assert.AreEqual("abc", StringView.Concat((IEnumerable<StringView>)views2));
		Assert.AreEqual("abcdefghijkl", StringView.Concat((IEnumerable<StringView>)views3));
	}

	/// <summary>
	/// 对 <see cref="StringView.Join"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestJoin()
	{
		StringView view1 = "0abcd".AsView(1, 3);
		StringView view2 = "def".AsView();
		StringView view3 = "defgh".AsView(3);
		StringView view4 = "ijkl".AsView(0, 2);
		StringView view5 = "ijkl".AsView(2);

		StringView[] views1 = Array.Empty<StringView>();
		StringView[] views2 = new StringView[] { view1 };
		StringView[] views3 = new StringView[] { view1, view2, view3, view4, view5 };

		Assert.AreEqual("", StringView.Join(',', views1));
		Assert.AreEqual("abc", StringView.Join(',', views2));
		Assert.AreEqual("abc,def,gh,ij,kl", StringView.Join(',', views3));

		Assert.AreEqual("", StringView.Join(',', (IEnumerable<StringView>)views1));
		Assert.AreEqual("abc", StringView.Join(',', (IEnumerable<StringView>)views2));
		Assert.AreEqual("abc,def,gh,ij,kl", StringView.Join(',', (IEnumerable<StringView>)views3));

		Assert.AreEqual("", StringView.Join("", views1));
		Assert.AreEqual("abc", StringView.Join("", views2));
		Assert.AreEqual("abcdefghijkl", StringView.Join("", views3));

		Assert.AreEqual("", StringView.Join(",", views1));
		Assert.AreEqual("abc", StringView.Join(",", views2));
		Assert.AreEqual("abc,def,gh,ij,kl", StringView.Join(",", views3));

		Assert.AreEqual("", StringView.Join(", ", views1));
		Assert.AreEqual("abc", StringView.Join(", ", views2));
		Assert.AreEqual("abc, def, gh, ij, kl", StringView.Join(", ", views3));

		Assert.AreEqual("", StringView.Join("", (IEnumerable<StringView>)views1));
		Assert.AreEqual("abc", StringView.Join("", (IEnumerable<StringView>)views2));
		Assert.AreEqual("abcdefghijkl", StringView.Join("", (IEnumerable<StringView>)views3));

		Assert.AreEqual("", StringView.Join(",", (IEnumerable<StringView>)views1));
		Assert.AreEqual("abc", StringView.Join(",", (IEnumerable<StringView>)views2));
		Assert.AreEqual("abc,def,gh,ij,kl", StringView.Join(",", (IEnumerable<StringView>)views3));

		Assert.AreEqual("", StringView.Join(", ", (IEnumerable<StringView>)views1));
		Assert.AreEqual("abc", StringView.Join(", ", (IEnumerable<StringView>)views2));
		Assert.AreEqual("abc, def, gh, ij, kl", StringView.Join(", ", (IEnumerable<StringView>)views3));
	}

	/// <summary>
	/// 对 <see cref="StringView.Replace"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestReplace()
	{
		StringView view = "0abcdef1abcdef2abcdef3".AsView(1, 20);
		Assert.AreEqual("abcdef1abcdef2abcdef", view.Replace('0', 'x'));
		Assert.AreEqual("xbcdef1xbcdef2xbcdef", view.Replace('a', 'x'));
		Assert.AreEqual("abxdef1abxdef2abxdef", view.Replace('c', 'x'));

		Assert.AreEqual("abcdef1abcdef2abcdef", view.Replace("0", "x"));
		Assert.AreEqual("abcdef1abcdef2abcdef", view.Replace("0a", "x1"));
		Assert.AreEqual("xcdef1xcdef2xcdef", view.Replace("ab", "x", StringComparison.Ordinal));
		Assert.AreEqual("ef1ef2ef", view.Replace("abcd", ""));
		Assert.AreEqual("abxxxdef1abxxxdef2abxxxdef", view.Replace("c", "xxx"));
		Assert.AreEqual("abxxxf1abxxxf2abxxxf", view.Replace("cde", "xxx", StringComparison.Ordinal));

		Assert.AreEqual("abcdef1abcdef2abcdef", view.Replace("0", "x", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("abcdef1abcdef2abcdef", view.Replace("0A", "x1", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("xcdef1xcdef2xcdef", view.Replace("aB", "x", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("ef1ef2ef", view.Replace("abCd", "", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("abxxxdef1abxxxdef2abxxxdef", view.Replace("C", "xxx", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("abxxxf1abxxxf2abxxxf", view.Replace("cDe", "xxx", StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// 对 <see cref="StringView.ReplaceLineEndings"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestReplaceLineEndings()
	{
		StringView view = "0a\n\r\n\n\rb\nc\u00853".AsView(1, 10);
		Assert.AreEqual("axxxxbxcx", view.ReplaceLineEndings("x"));
	}

	/// <summary>
	/// 对 <see cref="StringView.Split"/> 使用字符分割进行测试。
	/// </summary>
	[TestMethod]
	public void TestSplitChars()
	{
		StringSplitOptions[] optionArray = new[]
		{
			StringSplitOptions.None,
			StringSplitOptions.RemoveEmptyEntries,
			StringSplitOptions.TrimEntries,
			StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries,
		};
		StringViewComparer comparer = StringViewComparer.Instance;
		for (int c = 0; c < 4; c++)
		{
			foreach (StringSplitOptions options in optionArray)
			{
				CollectionAssert.AreEqual(
					"".Split(' ', c, options),
					"03".AsView(1, 0).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(' ', c, options),
					"0a3".AsView(1, 1).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					" a ".Split(' ', c, options),
					"0 a 3".AsView(1, 3).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"  a  ".Split(' ', c, options),
					"0  a  3".AsView(1, 5).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a b c".Split(' ', c, options),
					"0a b c3".AsView(1, 5).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a  b  c".Split(' ', c, options),
					"0a  b  c3".AsView(1, 7).Split(' ', c, options),
					comparer, "count {0} options {1}", c, options);

				char[] separators = Array.Empty<char>();
				CollectionAssert.AreEqual(
					"".Split(separators, c, options),
					"03".AsView(1, 0).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(separators, c, options),
					"0a3".AsView(1, 1).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				if (c == 2 && options == StringSplitOptions.TrimEntries)
				{
					// .NET 6 中，string.Split 会返回 "", "a "，与预期不符，并且在新版本中已经得到修复
					// https://github.com/dotnet/runtime/pull/81331
					// 这里与最新的逻辑保持一致。
					CollectionAssert.AreEqual(
						new string[] { "", "a" },
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "", "a" },
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "a", "b  c" },
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				else if (c == 3 && options == StringSplitOptions.TrimEntries)
				{
					// .NET 6 中，string.Split 会返回 "", "a "，与预期不符，并且在新版本中已经得到修复
					// https://github.com/dotnet/runtime/pull/81331
					// 这里与最新的逻辑保持一致。
					CollectionAssert.AreEqual(
						" a ".Split(separators, c, options),
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "", "", "a" },
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"a  b  c".Split(separators, c, options),
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				else
				{
					CollectionAssert.AreEqual(
						" a ".Split(separators, c, options),
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"  a  ".Split(separators, c, options),
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"a  b  c".Split(separators, c, options),
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				CollectionAssert.AreEqual(
					"a b c".Split(separators, c, options),
					"0a b c3".AsView(1, 5).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);

				separators = new char[] { ' ', 'a' };
				CollectionAssert.AreEqual(
					"".Split(separators, c, options),
					"03".AsView(1, 0).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(separators, c, options),
					"0a3".AsView(1, 1).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					" a ".Split(separators, c, options),
					"0 a 3".AsView(1, 3).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"  a  ".Split(separators, c, options),
					"0  a  3".AsView(1, 5).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"ca b c".Split(separators, c, options),
					"0ca b c3".AsView(1, 6).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"c a  b  c".Split(separators, c, options),
					"0c a  b  c3".AsView(1, 9).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
			}
		}
	}

	/// <summary>
	/// 对 <see cref="StringView.Split"/> 使用字符串分割进行测试。
	/// </summary>
	[TestMethod]
	public void TestSplitStrings()
	{
		StringSplitOptions[] optionArray = new[]
		{
			StringSplitOptions.None,
			StringSplitOptions.RemoveEmptyEntries,
			StringSplitOptions.TrimEntries,
			StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries,
		};
		StringViewComparer comparer = StringViewComparer.Instance;
		for (int c = 0; c < 4; c++)
		{
			foreach (StringSplitOptions options in optionArray)
			{
				CollectionAssert.AreEqual(
					"".Split(" ", c, options),
					"03".AsView(1, 0).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(" ", c, options),
					"0a3".AsView(1, 1).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					" a ".Split(" ", c, options),
					"0 a 3".AsView(1, 3).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"  a  ".Split(" ", c, options),
					"0  a  3".AsView(1, 5).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a b c".Split(" ", c, options),
					"0a b c3".AsView(1, 5).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a  b  c".Split(" ", c, options),
					"0a  b  c3".AsView(1, 7).Split(" ", c, options),
					comparer, "count {0} options {1}", c, options);

				string[] separators = Array.Empty<string>();
				CollectionAssert.AreEqual(
					"".Split(separators, c, options),
					"03".AsView(1, 0).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(separators, c, options),
					"0a3".AsView(1, 1).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				if (c == 2 && options == StringSplitOptions.TrimEntries)
				{
					// .NET 6 中，string.Split 会返回 "", "a "，与预期不符，并且在新版本中已经得到修复
					// https://github.com/dotnet/runtime/pull/81331
					// 这里与最新的逻辑保持一致。
					CollectionAssert.AreEqual(
						new string[] { "", "a" },
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "", "a" },
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "a", "b  c" },
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				else if (c == 3 && options == StringSplitOptions.TrimEntries)
				{
					// .NET 6 中，string.Split 会返回 "", "a "，与预期不符，并且在新版本中已经得到修复
					// https://github.com/dotnet/runtime/pull/81331
					// 这里与最新的逻辑保持一致。
					CollectionAssert.AreEqual(
						" a ".Split(separators, c, options),
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						new string[] { "", "", "a" },
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"a  b  c".Split(separators, c, options),
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				else
				{
					CollectionAssert.AreEqual(
						" a ".Split(separators, c, options),
						"0 a 3".AsView(1, 3).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"  a  ".Split(separators, c, options),
						"0  a  3".AsView(1, 5).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
					CollectionAssert.AreEqual(
						"a  b  c".Split(separators, c, options),
						"0a  b  c3".AsView(1, 7).Split(separators, c, options),
						comparer, "count {0} options {1}", c, options);
				}
				CollectionAssert.AreEqual(
					"a b c".Split(separators, c, options),
					"0a b c3".AsView(1, 5).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);

				separators = new string[] { " ", "a" };
				CollectionAssert.AreEqual(
					"".Split(separators, c, options),
					"03".AsView(1, 0).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"a".Split(separators, c, options),
					"0a3".AsView(1, 1).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					" a ".Split(separators, c, options),
					"0 a 3".AsView(1, 3).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"  a  ".Split(separators, c, options),
					"0  a  3".AsView(1, 5).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"ca b c".Split(separators, c, options),
					"0ca b c3".AsView(1, 6).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"c a  b  c".Split(separators, c, options),
					"0c a  b  c3".AsView(1, 9).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);

				separators = new string[] { " ", "abc" };
				CollectionAssert.AreEqual(
					"".Split(separators, c, options),
					"03".AsView(1, 0).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"abc".Split(separators, c, options),
					"0abc3".AsView(1, 3).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					" abc ".Split(separators, c, options),
					"0 abc 3".AsView(1, 5).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"  abc  ".Split(separators, c, options),
					"0  abc  3".AsView(1, 7).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"cabc b c".Split(separators, c, options),
					"0cabc b c3".AsView(1, 8).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
				CollectionAssert.AreEqual(
					"c abc  b  abc".Split(separators, c, options),
					"0c abc  b  abc3".AsView(1, 13).Split(separators, c, options),
					comparer, "count {0} options {1}", c, options);
			}
		}
	}

	/// <summary>
	/// 对 <see cref="StringView.Substring"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestSubstring()
	{
		Assert.AreEqual("abc", "0abc3".AsView(1, 3).Substring(0));
		Assert.AreEqual("bc", "0abc3".AsView(1, 3).Substring(1));
		Assert.AreEqual("c", "0abc3".AsView(1, 3).Substring(2));
		Assert.AreEqual("", "0abc3".AsView(1, 3).Substring(3));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			"0abc3".AsView(1, 3).Substring(4);
		});

		Assert.AreEqual("", "0abc3".AsView(1, 3).Substring(0, 0));
		Assert.AreEqual("a", "0abc3".AsView(1, 3).Substring(0, 1));
		Assert.AreEqual("bc", "0abc3".AsView(1, 3).Substring(1, 2));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
		{
			"0abc3".AsView(1, 3).Substring(1, 3);
		});
	}

	/// <summary>
	/// 对 <see cref="StringView.ToCharArray"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestToCharArray()
	{
		CollectionAssert.AreEqual("".ToCharArray(), "0abc3".AsView(1, 0).ToCharArray());
		CollectionAssert.AreEqual("abc".ToCharArray(), "0abc3".AsView(1, 3).ToCharArray());
	}

	/// <summary>
	/// 对 <see cref="StringView.TryConcat"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestTryConcat()
	{
		Assert.IsFalse("foo".AsView().TryConcat("bar", out var _));
		Assert.IsFalse("foo".AsView().TryConcat("foo", out var _));
		Assert.IsFalse("foobar".AsView(0, 3).TryConcat("foobar".AsView(2, 2), out var _));
		Assert.IsTrue("foo".AsView().TryConcat("", out var result));
		Assert.AreEqual("foo", result);
		Assert.IsTrue(StringView.Empty.TryConcat("bar", out result));
		Assert.AreEqual("bar", result);
		Assert.IsTrue("foobar".AsView(0, 3).TryConcat("foobar".AsView(3), out result));
		Assert.AreEqual("foobar", result);
		Assert.IsTrue("foobar".AsView(1, 2).TryConcat("foobar".AsView(3, 2), out result));
		Assert.AreEqual("ooba", result);
	}

	private static readonly CultureInfo[] TestCultures = new CultureInfo[] {
		CultureInfo.InvariantCulture,
		new("en-US"),
		new("ja-JP"),
		new("fr-FR"),
		new("tr-TR"),
	};

	/// <summary>
	/// 对 <see cref="StringView.ToLower"/> 进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow("")]
	[DataRow("A")]
	[DataRow("a")]
	[DataRow("ABC")]
	[DataRow("abc")]
	[DataRow("1")]
	[DataRow("123")]
	[DataRow("!")]
	[DataRow("HELLOWOR!LD123")]
	[DataRow("HelloWor!ld123")]
	[DataRow("Hello\n\0World\u0009!")]
	[DataRow("THIS IS A LONGER TEST CASE")]
	[DataRow("this Is A LONGER mIXEd casE test case")]
	[DataRow("THIS \t hAs \t SOMe \t tabs")]
	[DataRow("EMBEDDED\0NuLL\0Byte\0")]
	// LATIN CAPITAL LETTER O WITH ACUTE, which has a lower case variant.
	[DataRow("\u00D3")]
	// SNOWMAN, which does not have a lower case variant.
	[DataRow("\u2603")]
	// RAINBOW (outside the BMP and does not case)
	[DataRow("\U0001F308")]
	// Surrogate casing
	[DataRow("\U00010400")]
	// Unicode defines some codepoints which expand into multiple codepoints
	// when cased (see SpecialCasing.txt from UNIDATA for some examples). We have never done
	// these sorts of expansions, since it would cause string lengths to change when cased,
	// which is non-intuitive. In addition, there are some context sensitive mappings which
	// we also don't preform.
	// Greek Capital Letter Sigma (does not to case to U+03C2 with "final sigma" rule).
	[DataRow("\u03A3")]
	public void TestToLower(string text)
	{
		foreach (CultureInfo culture in TestCultures)
		{
			Assert.AreEqual(text.ToLower(culture), ("0" + text + "3").AsView(1, text.Length).ToLower(culture));
		}
	}

	/// <summary>
	/// 对 <see cref="StringView.ToUpper"/> 进行测试。
	/// </summary>
	[DataTestMethod]
	[DataRow("")]
	[DataRow("a")]
	[DataRow("abc")]
	[DataRow("A")]
	[DataRow("ABC")]
	[DataRow("1")]
	[DataRow("123")]
	[DataRow("!")]
	[DataRow("HelloWor!ld123")]
	[DataRow("HELLOWOR!LD123")]
	[DataRow("Hello\n\0World\u0009!")]
	[DataRow("this is a longer test case")]
	[DataRow("this Is A LONGER mIXEd casE test case")]
	[DataRow("this \t HaS \t somE \t TABS")]
	[DataRow("embedded\0NuLL\0Byte\0")]
	// LATIN SMALL LETTER O WITH ACUTE, mapped to LATIN CAPITAL LETTER O WITH ACUTE.
	[DataRow("\u00F3")]
	// SNOWMAN, which does not have an upper case variant.
	[DataRow("\u2603")]
	// RAINBOW (outside the BMP and does not case)
	[DataRow("\U0001F308")]
	// Surrogate casing
	[DataRow("\U00010428")]
	// Unicode defines some codepoints which expand into multiple codepoints
	// when cased (see SpecialCasing.txt from UNIDATA for some examples). We have never done
	// these sorts of expansions, since it would cause string lengths to change when cased,
	// which is non-intuitive. In addition, there are some context sensitive mappings which
	// we also don't preform.
	// es-zed does not case to SS when uppercased.
	[DataRow("\u00DF")]
	// Ligatures do not expand when cased.
	[DataRow("\uFB00")]
	// Precomposed character with no uppercase variant, we don't want to "decompose" this
	// as part of casing.
	[DataRow("\u0149")]
	[DataRow("\u03C3")]
	public void TestToUpper(string text)
	{
		foreach (CultureInfo culture in TestCultures)
		{
			Assert.AreEqual(text.ToUpper(culture), ("0" + text + "3").AsView(1, text.Length).ToUpper(culture));
		}
	}

	/// <summary>
	/// 对 <see cref="StringView.Trim"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestTrim()
	{
		Assert.AreEqual("abc", "0abc3".AsView(1, 3).Trim());
		Assert.AreEqual("abc", "0  abc  3".AsView(1, 7).Trim());
		Assert.AreEqual("abc", "   abc   ".AsView(1, 7).Trim());
		Assert.AreEqual("abc", "   abc ".AsView(1, 5).Trim());
		Assert.AreEqual("abc", " abc   ".AsView(1, 5).Trim());
		Assert.AreEqual("", "0     3".AsView(1, 5).Trim());
		Assert.AreEqual("", "03".AsView(1, 0).Trim());
		Assert.AreEqual("b", "0aba3".AsView(1, 3).Trim('a'));
		Assert.AreEqual("", "0aaa3".AsView(1, 3).Trim('a'));
		Assert.AreEqual("bab", "0bab3".AsView(1, 3).Trim('a'));
		Assert.AreEqual("", "0bab3".AsView(1, 3).Trim('a', 'b'));
	}

	/// <summary>
	/// 对 <see cref="StringView.TrimStart"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestTrimStart()
	{
		Assert.AreEqual("abc", "0abc3".AsView(1, 3).TrimStart());
		Assert.AreEqual("abc  ", "0  abc  3".AsView(1, 7).TrimStart());
		Assert.AreEqual("abc  ", "   abc   ".AsView(1, 7).TrimStart());
		Assert.AreEqual("abc", "   abc ".AsView(1, 5).TrimStart());
		Assert.AreEqual("abc  ", " abc   ".AsView(1, 5).TrimStart());
		Assert.AreEqual("", "0     3".AsView(1, 5).TrimStart());
		Assert.AreEqual("", "03".AsView(1, 0).TrimStart());
		Assert.AreEqual("ba", "0aba3".AsView(1, 3).TrimStart('a'));
		Assert.AreEqual("", "0aaa3".AsView(1, 3).TrimStart('a'));
		Assert.AreEqual("bab", "0bab3".AsView(1, 3).TrimStart('a'));
		Assert.AreEqual("", "0bab3".AsView(1, 3).TrimStart('a', 'b'));
	}

	/// <summary>
	/// 对 <see cref="StringView.TrimEnd"/> 进行测试。
	/// </summary>
	[TestMethod]
	public void TestTrimEnd()
	{
		Assert.AreEqual("abc", "0abc3".AsView(1, 3).TrimEnd());
		Assert.AreEqual("  abc", "0  abc  3".AsView(1, 7).TrimEnd());
		Assert.AreEqual("  abc", "   abc   ".AsView(1, 7).TrimEnd());
		Assert.AreEqual("  abc", "   abc ".AsView(1, 5).TrimEnd());
		Assert.AreEqual("abc", " abc   ".AsView(1, 5).TrimEnd());
		Assert.AreEqual("", "0     3".AsView(1, 5).TrimEnd());
		Assert.AreEqual("", "03".AsView(1, 0).TrimEnd());
		Assert.AreEqual("ab", "0aba3".AsView(1, 3).TrimEnd('a'));
		Assert.AreEqual("", "0aaa3".AsView(1, 3).TrimEnd('a'));
		Assert.AreEqual("bab", "0bab3".AsView(1, 3).TrimEnd('a'));
		Assert.AreEqual("", "0bab3".AsView(1, 3).TrimEnd('a', 'b'));
	}
}

class StringViewComparer : IComparer
{
	public static readonly StringViewComparer Instance = new();
	public int Compare(object? x, object? y)
	{
		return string.Compare(x?.ToString(), y?.ToString());
	}
}
