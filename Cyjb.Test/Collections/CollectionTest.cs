using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cyjb.Test.Collections;

/// <summary>
/// 对 <see cref="ICollection{T}"/> 测试的类型。
/// </summary>
[Flags]
public enum CollectionTestType
{
	/// <summary>
	/// <see cref="ICollection{T}"/> 是排序的。
	/// </summary>
	Sorted = 1 << 1,
	/// <summary>
	/// <see cref="ICollection{T}"/> 是不按照添加顺序的。
	/// </summary>
	Unordered = 1 << 0,
	/// <summary>
	/// <see cref="ICollection{T}"/> 内的元素是不重复的。
	/// </summary>
	Unique = 1 << 2,
}

/// <summary>
/// 提供对 <see cref="ICollection{T}"/> 的通用测试能力。
/// </summary>
public class CollectionTest
{
	/// <summary>
	/// 对指定的 <see cref="ICollection{T}"/> 进行测试。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="collection">要测试的集合。</param>
	/// <param name="type">集合的类型。</param>
	/// <param name="values">用于测试的值。</param>
	public static void Test<T>(ICollection<T> collection, CollectionTestType type, params T[] values)
	{
		Assert.IsFalse(collection.IsReadOnly);
		Random rnd = Random.Shared;
		bool ordered = type.HasFlag(CollectionTestType.Sorted) || !type.HasFlag(CollectionTestType.Unordered);
		bool needSort = false;
		ICollection<T> expected;
		if (type.HasFlag(CollectionTestType.Unique))
		{
			if (type.HasFlag(CollectionTestType.Sorted))
			{
				expected = new SortedSet<T>();
			}
			else
			{
				expected = new HashSet<T>();
			}
		}
		else
		{
			expected = new List<T>();
			needSort = type.HasFlag(CollectionTestType.Sorted);
		}

		int index;
		for (int i = 0; i < 10; i++)
		{
			index = 0;
			while (true)
			{
				// 随机选择一种操作
				if (rnd.NextBoolean())
				{
					if (index < values.Length)
					{
						collection.Add(values[index]);
						expected.Add(values[index]);
						index++;
					}
					else
					{
						break;
					}
				}
				else
				{
					T item = values[rnd.Next(values.Length)];
					collection.Remove(item);
					expected.Remove(item);
				}
				if (needSort)
				{
					((List<T>)expected).Sort();
				}
				Test(expected, collection, ordered, values);
			}
			collection.Clear();
			expected.Clear();
			Test(expected, collection, ordered, values);
		}
	}

	/// <summary>
	/// 比较两个有集合是否一致。
	/// </summary>
	/// <typeparam name="T">集合的元素类型。</typeparam>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	/// <param name="ordered">集合是否是有序的。</param>
	/// <param name="values">集合所有可能的元素。</param>
	public static void Test<T>(ICollection<T> expected, ICollection<T> actual, bool ordered, T[] values)
	{
		if (ordered)
		{
			CollectionAssert.That.AreEqual(expected, actual);
		}
		else
		{
			CollectionAssert.That.AreEquivalent(expected, actual);
		}
		// 验证 contains 方法
		for (int j = 0; j < values.Length; j++)
		{
			Assert.AreEqual(expected.Contains(values[j]), actual.Contains(values[j]));
		}
		// 验证 CopyTo 方法。
		T[] array = new T[expected.Count];
		array.Fill(default(T));
		actual.CopyTo(array, 0);
		if (ordered)
		{
			CollectionAssert.That.AreEqual(expected, array);
		}
		else
		{
			CollectionAssert.That.AreEquivalent(expected, array);
		}
	}
}
