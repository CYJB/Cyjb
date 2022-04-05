using Cyjb.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Cache
{
	/// <summary>
	/// <see cref="SimplyCache{TKey, TValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestSimplyCache
	{
		/// <summary>
		/// 对 <see cref="SimplyCache{TKey, TValue}"/> 的引用类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestClassType()
		{
			SimplyCache<int, string> cache = new();
			Assert.AreEqual(0, cache.Count);

			// 添加 1
			cache.Add(1, "1");
			Assert.AreEqual(1, cache.Count);

			Assert.IsTrue(cache.Contains(1));

			string? value;
			Assert.IsTrue(cache.TryGet(1, out value));
			Assert.AreEqual("1", value);

			Assert.IsFalse(cache.TryGet(2, out value));
			Assert.IsNull(value);

			// 添加 2
			Assert.AreEqual("2", cache.GetOrAdd(2, (int value) => value.ToString()));
			Assert.AreEqual(2, cache.Count);

			Assert.IsTrue(cache.Contains(2));

			Assert.IsTrue(cache.TryGet(2, out value));
			Assert.AreEqual("2", value);

			// 移除 1
			cache.Remove(1);
			Assert.AreEqual(1, cache.Count);
			Assert.IsFalse(cache.Contains(1));

			Assert.IsFalse(cache.TryGet(1, out value));
			Assert.IsNull(value);

			// 添加 3
			Assert.AreEqual("3", cache.GetOrAdd(3, "3", (int value, string arg) => arg));
			Assert.AreEqual(2, cache.Count);

			Assert.IsTrue(cache.Contains(3));

			Assert.IsTrue(cache.TryGet(3, out value));
			Assert.AreEqual("3", value);
		}

		/// <summary>
		/// 对 <see cref="SimplyCache{TKey, TValue}"/> 的值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestValueType()
		{
			SimplyCache<int, int> cache = new();
			Assert.AreEqual(0, cache.Count);

			// 添加 1
			cache.Add(1, 1);
			Assert.AreEqual(1, cache.Count);

			Assert.IsTrue(cache.Contains(1));

			int value;
			Assert.IsTrue(cache.TryGet(1, out value));
			Assert.AreEqual(1, value);

			Assert.IsFalse(cache.TryGet(2, out value));
			Assert.AreEqual(0, value);

			// 添加 2
			Assert.AreEqual(2, cache.GetOrAdd(2, (int value) => value));
			Assert.AreEqual(2, cache.Count);

			Assert.IsTrue(cache.Contains(2));

			Assert.IsTrue(cache.TryGet(2, out value));
			Assert.AreEqual(2, value);

			// 移除 1
			cache.Remove(1);
			Assert.AreEqual(1, cache.Count);
			Assert.IsFalse(cache.Contains(1));

			Assert.IsFalse(cache.TryGet(1, out value));
			Assert.AreEqual(0, value);

			// 添加 3
			Assert.AreEqual(3, cache.GetOrAdd(3, 3, (int value, int arg) => arg));
			Assert.AreEqual(2, cache.Count);

			Assert.IsTrue(cache.Contains(3));

			Assert.IsTrue(cache.TryGet(3, out value));
			Assert.AreEqual(3, value);
		}
	}
}
