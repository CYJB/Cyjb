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
	/// <see cref="ReadOnlyDictionaryBase{TKey, TValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestReadOnlyDictionaryBase
	{
		private class TestDictionary : ReadOnlyDictionaryBase<int, string>
		{
			private readonly Dictionary<int, string> items;
			public TestDictionary(Dictionary<int, string> items)
			{
				this.items = items;
			}
			protected override string GetItem(int key)
			{
				return items[key];
			}
			public override ICollection<int> Keys => items.Keys;
			public override ICollection<string> Values => items.Values;
			public override int Count => items.Count;
			public override bool ContainsKey(int key)
			{
				return items.ContainsKey(key);
			}
			public override IEnumerator<KeyValuePair<int, string>> GetEnumerator()
			{
				return items.GetEnumerator();
			}
			public override bool TryGetValue(int key, [MaybeNullWhen(false)] out string value)
			{
				return items.TryGetValue(key, out value);
			}
		}

		/// <summary>
		/// 对 <see cref="ReadOnlyDictionaryBase{TKey, TValue}"/> 类进行测试。
		/// </summary>
		[TestMethod]
		public void TestReadOnlyDictionaryBase()
		{
			TestDictionary dict = new(new Dictionary<int, string>()
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
			});
			Assert.AreEqual(3, dict.Count);
			Assert.IsTrue(dict.ContainsKey(1));
			Assert.IsTrue(dict.ContainsKey(2));
			Assert.IsTrue(dict.ContainsKey(3));
			Assert.AreEqual("1", dict[1]);
			Assert.AreEqual("2", dict[2]);
			Assert.AreEqual("3", dict[3]);
			Assert.ThrowsException<KeyNotFoundException>(() => dict[4]);
			CollectionAssert.AreEquivalent(new KeyValuePair<int, string>[] {
				new KeyValuePair<int, string>(1, "1") ,
				new KeyValuePair<int, string>(2, "2") ,
				new KeyValuePair<int, string>(3, "3") ,
			}, dict.ToArray());

			IDictionary dict2 = dict;
			Assert.IsTrue(dict2.Contains(1));
			Assert.IsTrue(dict2.Contains(2));
			Assert.IsTrue(dict2.Contains(3));
			Assert.AreEqual("1", dict2[1]);
			Assert.AreEqual("2", dict2[2]);
			Assert.AreEqual("3", dict2[3]);
			Assert.ThrowsException<KeyNotFoundException>(() => dict2[4]);
			Assert.ThrowsException<NotSupportedException>(() => dict2.Add(10, "10"));
			Assert.ThrowsException<NotSupportedException>(() => dict2.Remove(1));

			ICollection<KeyValuePair<int, string>> collection = dict;
			Assert.IsTrue(collection.Contains(new KeyValuePair<int, string>(1, "1")));
			Assert.IsFalse(collection.Contains(new KeyValuePair<int, string>(1, "2")));
			Assert.ThrowsException<NotSupportedException>(() => collection.Add(new KeyValuePair<int, string>(1, "2")));
			Assert.ThrowsException<NotSupportedException>(() => collection.Remove(new KeyValuePair<int, string>(1, "2")));
		}
	}
}
