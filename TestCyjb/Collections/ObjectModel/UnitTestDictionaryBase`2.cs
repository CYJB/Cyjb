using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cyjb.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections.ObjectModel
{
	/// <summary>
	/// <see cref="DictionaryBase{TKey, TValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestDictionaryBase
	{
		private class TestDictionary : DictionaryBase<int, string>
		{
			private readonly Dictionary<int, string> items = new();
			public override string this[int key] { get => items[key]; set => items[key] = value; }
			public override ICollection<int> Keys => items.Keys;
			public override ICollection<string> Values => items.Values;
			public override int Count => items.Count;
			public override void Add(int key, string value)
			{
				items.Add(key, value);
			}
			public override void Clear()
			{
				items.Clear();
			}
			public override bool ContainsKey(int key)
			{
				return items.ContainsKey(key);
			}
			public override IEnumerator<KeyValuePair<int, string>> GetEnumerator()
			{
				return items.GetEnumerator();
			}
			public override bool Remove(int key)
			{
				return items.Remove(key);
			}
			public override bool TryGetValue(int key, [MaybeNullWhen(false)] out string value)
			{
				return items.TryGetValue(key, out value);
			}
		}

		/// <summary>
		/// 对 <see cref="DictionaryBase{TKey, TValue}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestDictionaryBase()
		{
			TestDictionary dict = new();
			Assert.AreEqual(0, dict.Count);
			Assert.IsFalse(dict.ContainsKey(1));

			dict.Add(1, "1");
			Assert.AreEqual(1, dict.Count);
			Assert.AreEqual("1", dict[1]);
			Assert.IsTrue(dict.ContainsKey(1));
			CollectionAssert.AreEquivalent(new KeyValuePair<int, string>[] {
				new KeyValuePair<int, string>(1, "1") ,
			}, dict.ToArray());

			dict.Add(2, "2");
			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual("1", dict[1]);
			Assert.AreEqual("2", dict[2]);
			Assert.IsTrue(dict.ContainsKey(1));
			Assert.IsTrue(dict.ContainsKey(2));
			CollectionAssert.AreEquivalent(new KeyValuePair<int, string>[] {
				new KeyValuePair<int, string>(1, "1") ,
				new KeyValuePair<int, string>(2, "2") ,
			}, dict.ToArray());

			IDictionary dict2 = dict;
			Assert.IsTrue(dict2.Contains(1));
			Assert.IsTrue(dict2.Contains(2));
			Assert.IsFalse(dict2.Contains(3));
			Assert.AreEqual("1", dict2[1]);
			Assert.AreEqual("2", dict2[2]);
			Assert.ThrowsException<KeyNotFoundException>(() => dict2[3]);

			ICollection<KeyValuePair<int, string>> collection = dict;
			Assert.IsTrue(collection.Contains(new KeyValuePair<int, string>(1, "1")));
			Assert.IsFalse(collection.Contains(new KeyValuePair<int, string>(1, "2")));
			collection.Add(new KeyValuePair<int, string>(3, "3"));
			Assert.AreEqual(3, dict.Count);
			Assert.AreEqual("3", dict[3]);
			Assert.IsFalse(collection.Remove(new KeyValuePair<int, string>(1, "2")));
			Assert.IsTrue(collection.Remove(new KeyValuePair<int, string>(1, "1")));

			Assert.AreEqual(2, dict.Count);
			Assert.IsFalse(dict.ContainsKey(1));
			Assert.IsTrue(dict.ContainsKey(2));
			Assert.IsTrue(dict.ContainsKey(3));
			CollectionAssert.AreEquivalent(new KeyValuePair<int, string>[] {
				new KeyValuePair<int, string>(2, "2") ,
				new KeyValuePair<int, string>(3, "3") ,
			}, dict.ToArray());

			dict.Clear();
			Assert.AreEqual(0, dict.Count);
			Assert.IsFalse(dict.ContainsKey(1));
			Assert.IsFalse(dict.ContainsKey(2));
			CollectionAssert.AreEqual(Array.Empty<KeyValuePair<int, string>>(), dict.ToArray());
		}
	}
}
