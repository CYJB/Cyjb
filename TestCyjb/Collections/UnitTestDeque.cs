using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;

/// <summary>
/// <see cref="Deque{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestDeque
{
	/// <summary>
	/// 对 <see cref="Deque{T}"/> 的堆栈方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestDeque()
	{
		Deque<int> queue = new();
		Assert.AreEqual(0, queue.Count);

		queue.PushFront(1);
		Assert.AreEqual(1, queue.Count);
		Assert.AreEqual(1, queue.PeekFront());
		Assert.AreEqual(1, queue.PeekBack());
		Assert.IsTrue(queue.TryPeekFront(out int value));
		Assert.AreEqual(1, value);
		Assert.IsTrue(queue.TryPeekBack(out value));
		Assert.AreEqual(1, value);
		Assert.AreEqual(1, queue[0]);
		Assert.AreEqual(1, queue[^1]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => queue[1]);

		queue.PushBack(2);
		Assert.AreEqual(2, queue.Count);
		Assert.AreEqual(1, queue.PeekFront());
		Assert.AreEqual(2, queue.PeekBack());
		Assert.IsTrue(queue.TryPeekFront(out value));
		Assert.AreEqual(1, value);
		Assert.IsTrue(queue.TryPeekBack(out value));
		Assert.AreEqual(2, value);
		Assert.AreEqual(1, queue[0]);
		Assert.AreEqual(2, queue[1]);
		Assert.AreEqual(2, queue[^1]);
		Assert.AreEqual(1, queue[^2]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => queue[2]);

		queue.PushBack(3);
		queue.PushBack(4);
		queue.PushFront(5);
		CollectionAssert.AreEqual(new int[] { 5, 1, 2, 3, 4 }, queue.ToArray());
		CollectionAssert.AreEqual(new int[] { 5, 1, 2, 3, 4 }, queue.Select(v => v).ToArray());
		int[] array = new int[10];
		queue.CopyTo(array, 2);
		CollectionAssert.AreEqual(new int[] { 0, 0, 5, 1, 2, 3, 4, 0, 0, 0 }, array);

		IEnumerator<int> enumerator = queue.GetEnumerator();
		enumerator.MoveNext();
		queue.PushBack(6);
		Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
		Assert.AreEqual(5, queue[0]);
		Assert.AreEqual(1, queue[1]);
		Assert.AreEqual(6, queue[^1]);
		Assert.AreEqual(4, queue[^2]);

		Assert.AreEqual(6, queue.Count);
		Assert.AreEqual(5, queue.PopFront());
		Assert.IsTrue(queue.TryPopFront(out value));
		Assert.AreEqual(1, value);

		queue.Clear();
		Assert.AreEqual(0, queue.Count);
		Assert.IsFalse(queue.TryPopFront(out value));
		Assert.ThrowsException<InvalidOperationException>(() => queue.PopBack());
	}
}
