using System;
using System.Collections.Generic;
using System.Linq;
using Cyjb.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCyjb.Collections;

/// <summary>
/// <see cref="ListStack{T}"/> 类的单元测试。
/// </summary>
[TestClass]
public class UnitTestListStack
{
	/// <summary>
	/// 对 <see cref="ListStack{T}"/> 的堆栈方法进行测试。
	/// </summary>
	[TestMethod]
	public void TestStack()
	{
		ListStack<int> stack = new();
		Assert.AreEqual(0, stack.Count);

		stack.Push(1);
		Assert.AreEqual(1, stack.Count);
		Assert.AreEqual(1, stack.Peek());
		Assert.IsTrue(stack.TryPeek(out int value));
		Assert.AreEqual(1, value);
		Assert.AreEqual(1, stack[0]);
		Assert.AreEqual(1, stack[^1]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => stack[1]);

		stack.Push(2);
		Assert.AreEqual(2, stack.Count);
		Assert.AreEqual(2, stack.Peek());
		Assert.IsTrue(stack.TryPeek(out value));
		Assert.AreEqual(2, value);
		Assert.AreEqual(2, stack[0]);
		Assert.AreEqual(1, stack[1]);
		Assert.AreEqual(1, stack[^1]);
		Assert.AreEqual(2, stack[^2]);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => stack[2]);

		stack.Push(3);
		stack.Push(4);
		stack.Push(5);
		CollectionAssert.AreEqual(new int[] { 5, 4, 3, 2, 1 }, stack.ToArray());
		CollectionAssert.AreEqual(new int[] { 5, 4, 3, 2, 1 }, stack.Select(v => v).ToArray());
		int[] array = new int[10];
		stack.CopyTo(array, 2);
		CollectionAssert.AreEqual(new int[] { 0, 0, 5, 4, 3, 2, 1, 0, 0, 0 }, array);

		IEnumerator<int> enumerator = stack.GetEnumerator();
		enumerator.MoveNext();
		stack.Push(6);
		Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
		Assert.AreEqual(6, stack[0]);
		Assert.AreEqual(5, stack[1]);
		Assert.AreEqual(1, stack[^1]);
		Assert.AreEqual(2, stack[^2]);

		Assert.AreEqual(6, stack.Count);
		Assert.AreEqual(6, stack.Pop());
		Assert.IsTrue(stack.TryPop(out value));
		Assert.AreEqual(5, value);

		stack.Clear();
		Assert.AreEqual(0, stack.Count);
		Assert.IsFalse(stack.TryPop(out value));
		Assert.ThrowsException<InvalidOperationException>(() => stack.Pop());
	}
}
