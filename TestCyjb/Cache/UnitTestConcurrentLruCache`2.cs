using Cyjb.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Cache
{
	/// <summary>
	/// <see cref="ConcurrentLruCache{TKey, TValue}"/> 类的单元测试。
	/// </summary>
	[TestClass]
	public class UnitTestConcurrentLruCache
	{
		/// <summary>
		/// 对 <see cref="ConcurrentLruCache{TKey, TValue}"/> 的引用类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestClassType()
		{
			ConcurrentLruCache<int, string> cache = new(3);
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

			// 添加 4
			Assert.AreEqual("4", cache.GetOrAdd(4, (int value) => value.ToString()));
			Assert.AreEqual(3, cache.Count);

			Assert.IsTrue(cache.Contains(4));

			Assert.IsTrue(cache.TryGet(4, out value));
			Assert.AreEqual("4", value);

			// 添加 5
			Assert.AreEqual("5", cache.GetOrAdd(5, (int value) => value.ToString()));
			Assert.AreEqual(3, cache.Count);

			Assert.IsTrue(cache.Contains(5));

			Assert.IsTrue(cache.TryGet(5, out value));
			Assert.AreEqual("5", value);

			// 2 被淘汰
			Assert.IsFalse(cache.Contains(2));
		}

		/// <summary>
		/// 对 <see cref="ConcurrentLruCache{TKey, TValue}"/> 的值类型进行测试。
		/// </summary>
		[TestMethod]
		public void TestValueType()
		{
			ConcurrentLruCache<int, int> cache = new(3);
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

			// 添加 4
			Assert.AreEqual(4, cache.GetOrAdd(4, (int value) => value));
			Assert.AreEqual(3, cache.Count);

			Assert.IsTrue(cache.Contains(4));

			Assert.IsTrue(cache.TryGet(4, out value));
			Assert.AreEqual(4, value);

			// 添加 5
			Assert.AreEqual(5, cache.GetOrAdd(5, (int value) => value));
			Assert.AreEqual(3, cache.Count);

			Assert.IsTrue(cache.Contains(5));

			Assert.IsTrue(cache.TryGet(5, out value));
			Assert.AreEqual(5, value);

			// 2 被淘汰
			Assert.IsFalse(cache.Contains(2));
		}
	}
}
