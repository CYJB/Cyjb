using System;
using System.Collections.Generic;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCyjb.Collections
{
	/// <summary>
	/// <see cref="Cyjb.Collections.CharSet"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestCharSet
	{
		private ISet<char> hashSet;
		private ISet<char> charSet;
		private ISet<char> lastHashSet;
		private ISet<char> lastCharSet;
		private ISet<char> hashSetIg;
		private ISet<char> charSetIg;
		private ISet<char> lastHashSetIg;
		private ISet<char> lastCharSetIg;
		[TestInitialize]
		public void InitializeSet()
		{
			lastHashSet = hashSet = new HashSet<char>();
			lastCharSet = charSet = new CharSet();
			lastHashSetIg = hashSetIg = new HashSet<char>(new CharComparer());
			lastCharSetIg = charSetIg = new CharSet(true);
		}
		/// <summary>
		/// 对 <see cref="Cyjb.Collections.CharSet"/> 的基本操作进行测试。
		/// </summary>
		[TestMethod]
		public void TestCharSetBasicOperation()
		{
			TestAdd('A');
			TestAdd('A');
			TestAdd('B');
			TestAdd('c');
			TestAdd('c');
			TestAdd('a');
			TestAdd('b');
			TestAdd('C');
			TestAdd('A');
			TestAdd('d');
			TestAdd('d');
			TestRemove('A');
			TestRemove('a');
			TestRemove('c');
			TestRemove('c');
			TestRemove('b');
			TestRemove('D');
			TestRemove('D');
			TestAdd('C');
			TestAdd('C');
		}
		/// <summary>
		/// 对 <see cref="Cyjb.Collections.CharSet"/> 的集合操作进行测试。
		/// </summary>
		[TestMethod]
		public void TestCharSetSetOperation()
		{
			TestSet((s, o) => s.UnionWith(o), "");
			TestSet((s, o) => s.UnionWith(o), "AABCCDEE");
			TestSet((s, o) => s.UnionWith(o), "abbccddf");
			TestSet((s, o) => s.UnionWith(o), "GGHIJKLLLMNNOP");
			TestSet((s, o) => s.UnionWith(o), "qrrrrrstuvwxyz");
			TestSet((s, o) => s.ExceptWith(o), "");
			TestSet((s, o) => s.ExceptWith(o), "AABBBB");
			TestSet((s, o) => s.ExceptWith(o), "abb");
			TestSet((s, o) => s.IntersectWith(o), "ABBBP");
			TestSet((s, o) => s.IntersectWith(o), "p");
			TestSet((s, o) => s.ExceptWith(o), "ddd");
			TestSet((s, o) => s.IntersectWith(o), "DDC");
			TestSet((s, o) => s.SymmetricExceptWith(o), "DDC");
			TestSet((s, o) => s.SymmetricExceptWith(o), "dc");
			TestSet((s, o) => s.SymmetricExceptWith(o), "Cc");
			TestSet((s, o) => s.ExceptWith(o), "Dd");
		}
		private void TestAdd(char ch)
		{
			hashSet.Add(ch);
			charSet.Add(ch);
			Assert.IsTrue(hashSet.SetEquals(charSet));
			Assert.AreEqual(hashSet.Count, charSet.Count);
			hashSetIg.Add(ch);
			charSetIg.Add(ch);
			HashSet<char> tempSet = new HashSet<char>(hashSetIg);
			Assert.IsTrue(tempSet.SetEquals(charSetIg));
			Assert.AreEqual(hashSetIg.Count, charSetIg.Count);
		}
		private void TestRemove(char ch)
		{
			Assert.AreEqual(hashSet.Remove(ch), charSet.Remove(ch));
			Assert.IsTrue(hashSet.SetEquals(charSet));
			Assert.AreEqual(hashSet.Count, charSet.Count);
			Assert.AreEqual(hashSetIg.Remove(ch), charSetIg.Remove(ch));
			HashSet<char> tempSet = new HashSet<char>(hashSetIg);
			Assert.IsTrue(tempSet.SetEquals(charSetIg));
			Assert.AreEqual(hashSetIg.Count, charSetIg.Count);
		}
		private void TestSet(Action<ISet<char>, IEnumerable<char>> action, string other)
		{
			action(hashSet, other);
			action(charSet, new CharSet(other));
			Assert.IsTrue(hashSet.SetEquals(charSet));
			Assert.AreEqual(hashSet.Count, charSet.Count);
			Assert.AreEqual(hashSet.IsProperSubsetOf(other), charSet.IsProperSubsetOf(other));
			Assert.AreEqual(hashSet.IsProperSupersetOf(other), charSet.IsProperSupersetOf(other));
			Assert.AreEqual(hashSet.IsSubsetOf(other), charSet.IsSubsetOf(other));
			Assert.AreEqual(hashSet.IsSupersetOf(other), charSet.IsSupersetOf(other));
			Assert.AreEqual(hashSet.Overlaps(other), charSet.Overlaps(other));
			Assert.AreEqual(hashSet.IsProperSubsetOf(lastHashSet), charSet.IsProperSubsetOf(lastCharSet));
			Assert.AreEqual(hashSet.IsProperSupersetOf(lastHashSet), charSet.IsProperSupersetOf(lastCharSet));
			Assert.AreEqual(hashSet.IsSubsetOf(lastHashSet), charSet.IsSubsetOf(lastCharSet));
			Assert.AreEqual(hashSet.IsSupersetOf(lastHashSet), charSet.IsSupersetOf(lastCharSet));
			Assert.AreEqual(hashSet.Overlaps(lastHashSet), charSet.Overlaps(lastCharSet));
			Assert.AreEqual(lastHashSet.IsProperSubsetOf(hashSet), lastCharSet.IsProperSubsetOf(charSet));
			Assert.AreEqual(lastHashSet.IsProperSupersetOf(hashSet), lastCharSet.IsProperSupersetOf(charSet));
			Assert.AreEqual(lastHashSet.IsSubsetOf(hashSet), lastCharSet.IsSubsetOf(charSet));
			Assert.AreEqual(lastHashSet.IsSupersetOf(hashSet), lastCharSet.IsSupersetOf(charSet));
			Assert.AreEqual(lastHashSet.Overlaps(hashSet), lastCharSet.Overlaps(charSet));
			lastHashSet = hashSet;
			lastCharSet = charSet;
			action(hashSetIg, other);
			action(charSetIg, new CharSet(other));
			Assert.IsTrue(hashSetIg.SetEquals(charSetIg));
			Assert.AreEqual(hashSetIg.Count, charSetIg.Count);
			Assert.AreEqual(hashSetIg.IsProperSubsetOf(other), charSetIg.IsProperSubsetOf(other));
			Assert.AreEqual(hashSetIg.IsProperSupersetOf(other), charSetIg.IsProperSupersetOf(other));
			Assert.AreEqual(hashSetIg.IsSubsetOf(other), charSetIg.IsSubsetOf(other));
			Assert.AreEqual(hashSetIg.IsSupersetOf(other), charSetIg.IsSupersetOf(other));
			Assert.AreEqual(hashSetIg.Overlaps(other), charSetIg.Overlaps(other));
			Assert.AreEqual(hashSetIg.IsProperSubsetOf(lastHashSetIg), charSetIg.IsProperSubsetOf(lastCharSetIg));
			Assert.AreEqual(hashSetIg.IsProperSupersetOf(lastHashSetIg), charSetIg.IsProperSupersetOf(lastCharSetIg));
			Assert.AreEqual(hashSetIg.IsSubsetOf(lastHashSetIg), charSetIg.IsSubsetOf(lastCharSetIg));
			Assert.AreEqual(hashSetIg.IsSupersetOf(lastHashSetIg), charSetIg.IsSupersetOf(lastCharSetIg));
			Assert.AreEqual(hashSetIg.Overlaps(lastHashSetIg), charSetIg.Overlaps(lastCharSetIg));
			Assert.AreEqual(lastHashSetIg.IsProperSubsetOf(hashSetIg), lastCharSetIg.IsProperSubsetOf(charSetIg));
			Assert.AreEqual(lastHashSetIg.IsProperSupersetOf(hashSetIg), lastCharSetIg.IsProperSupersetOf(charSetIg));
			Assert.AreEqual(lastHashSetIg.IsSubsetOf(hashSetIg), lastCharSetIg.IsSubsetOf(charSetIg));
			Assert.AreEqual(lastHashSetIg.IsSupersetOf(hashSetIg), lastCharSetIg.IsSupersetOf(charSetIg));
			Assert.AreEqual(lastHashSetIg.Overlaps(hashSetIg), lastCharSetIg.Overlaps(charSetIg));
			lastHashSetIg = hashSetIg;
			lastCharSetIg = charSetIg;
		}
		private class CharComparer : EqualityComparer<char>
		{
			public override bool Equals(char x, char y)
			{
				return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
			}
			public override int GetHashCode(char obj)
			{
				return char.ToUpperInvariant(obj).GetHashCode();
			}
		}
	}
}
