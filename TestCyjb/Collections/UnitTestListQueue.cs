using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;

/// <summary>
/// <see cref="ListQueue{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestListQueue
{
	/// <summary>
	/// 对 <see cref="ListQueue{T}"/> 的堆栈方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestQueue()
	{
		ListQueue<int> queue = new();
		Assert.AreEqual(0, queue.Count);

		queue.Enqueue(1);
		Assert.AreEqual(1, queue.Count);
		Assert.AreEqual(1, queue.Peek());
		Assert.IsTrue(queue.TryPeek(out int value));
		Assert.AreEqual(1, value);
		Assert.AreEqual(1, queue[0]);
		Assert.AreEqual(1, queue[^1]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => queue[1]);

		queue.Enqueue(2);
		Assert.AreEqual(2, queue.Count);
		Assert.AreEqual(1, queue.Peek());
		Assert.IsTrue(queue.TryPeek(out value));
		Assert.AreEqual(1, value);
		Assert.AreEqual(1, queue[0]);
		Assert.AreEqual(2, queue[1]);
		Assert.AreEqual(2, queue[^1]);
		Assert.AreEqual(1, queue[^2]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => queue[2]);

		queue.Enqueue(3);
		queue.Enqueue(4);
		queue.Enqueue(5);
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, queue.ToArray());
		CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, queue.Select(v => v).ToArray());
		int[] array = new int[10];
		queue.CopyTo(array, 2);
		CollectionAssert.AreEqual(new int[] { 0, 0, 1, 2, 3, 4, 5, 0, 0, 0 }, array);

		IEnumerator<int> enumerator = queue.GetEnumerator();
		enumerator.MoveNext();
		queue.Enqueue(6);
		Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
		Assert.AreEqual(1, queue[0]);
		Assert.AreEqual(2, queue[1]);
		Assert.AreEqual(6, queue[^1]);
		Assert.AreEqual(5, queue[^2]);

		Assert.AreEqual(6, queue.Count);
		Assert.AreEqual(1, queue.Dequeue());
		Assert.IsTrue(queue.TryDequeue(out value));
		Assert.AreEqual(2, value);

		queue.Clear();
		Assert.AreEqual(0, queue.Count);
		Assert.IsFalse(queue.TryDequeue(out value));
		Assert.ThrowsException<InvalidOperationException>(() => queue.Dequeue());
	}
}
