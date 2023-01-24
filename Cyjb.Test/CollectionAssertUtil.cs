using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cyjb.Test.Collections;

/// <summary>
/// 提供泛型集合的断言。
/// </summary>
public static class CollectionAssertUtil
{

	#region AreEqual<T>

	/// <summary>
	/// 测试指定的集合是否以相同的顺序包含同样的元素。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="assert">Assert 实例。</param>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	public static void AreEqual<T>(this CollectionAssert assert, ICollection<T>? expected, ICollection<T>? actual)
	{
		AreEqual(assert, expected, actual, null, null, Array.Empty<object>());
	}

	/// <summary>
	/// 测试指定的集合是否以相同的顺序包含同样的元素。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="assert">Assert 实例。</param>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	/// <param name="comparer">元素的比较器。</param>
	/// <param name="message">当 <paramref name="actual"/> 与 <paramref name="expected"/> 不同时的消息。</param>
	/// <param name="parameters">消息的格式化参数。</param>
	[SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
	public static void AreEqual<T>(this CollectionAssert assert, ICollection<T>? expected, ICollection<T>? actual,
		IEqualityComparer<T>? comparer, string? message, params object?[] parameters)
	{
		comparer ??= EqualityComparer<T>.Default;
		if (!AreEqual(expected, actual, comparer, out string resultInfo))
		{
			if (message == null)
			{
				message = resultInfo;
			}
			else
			{
				message = string.Format(message, parameters);
				message = resultInfo + ". " + message;
			}
			Assert.Fail(message);
		}
	}

	/// <summary>
	/// 测试指定的集合是否以相同的顺序包含同样的元素。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	/// <param name="comparer">元素的比较器。</param>
	/// <param name="resultInfo">集合比较的结果信息。</param>
	/// <returns>如果集合相同，则为 <c>true</c>，否则为 <c>false</c>。</returns>
	private static bool AreEqual<T>(ICollection<T>? expected, ICollection<T>? actual,
		IEqualityComparer<T> comparer, out string resultInfo)
	{
		if (expected != actual)
		{
			if (expected == null || actual == null)
			{
				resultInfo = string.Empty;
				return false;
			}
			if (expected.Count != actual.Count)
			{
				resultInfo = Resources.NumberOfElementsDiff;
				return false;
			}
			IEnumerator<T> expectedEnum = expected.GetEnumerator();
			IEnumerator<T> actualEnum = actual.GetEnumerator();
			int index = 0;
			while (expectedEnum.MoveNext() && actualEnum.MoveNext())
			{
				if (!comparer.Equals(expectedEnum.Current, actualEnum.Current))
				{
					resultInfo = Resources.ElementsAtIndexDontMatch(index);
					return false;
				}
				index++;
			}
			resultInfo = Resources.BothCollectionsSameElements;
			return true;
		}
		resultInfo = Resources.BothCollectionsSameReference;
		return true;
	}

	#endregion // AreEqual<T>

	#region AreEquivalent<T>

	/// <summary>
	/// 测试指定的集合是否包含同样的元素（忽略顺序）。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="assert">Assert 实例。</param>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	public static void AreEquivalent<T>(this CollectionAssert assert, ICollection<T>? expected, ICollection<T>? actual)
	{
		AreEqual(assert, expected, actual, null, null, Array.Empty<object>());
	}

	/// <summary>
	/// 测试指定的集合是否包含同样的元素（忽略顺序）。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="assert">Assert 实例。</param>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	/// <param name="comparer">元素的比较器。</param>
	/// <param name="message">当 <paramref name="actual"/> 与 <paramref name="expected"/> 不同时的消息。</param>
	/// <param name="parameters">消息的格式化参数。</param>
	[SuppressMessage("Style", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
	public static void AreEquivalent<T>(this CollectionAssert assert, ICollection<T>? expected, ICollection<T>? actual,
		IEqualityComparer<T>? comparer, string? message, params object?[] parameters)
	{
		comparer ??= EqualityComparer<T>.Default;
		if (!AreEquivalent(expected, actual, comparer, out string resultInfo))
		{
			if (message == null)
			{
				message = resultInfo;
			}
			else
			{
				message = string.Format(message, parameters);
				message = resultInfo + ". " + message;
			}
			Assert.Fail(message);
		}
	}

#pragma warning disable CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。

	/// <summary>
	/// 测试指定的集合是否包含同样的元素（忽略顺序）。
	/// </summary>
	/// <typeparam name="T">集合中的元素类型。</typeparam>
	/// <param name="expected">预期的集合。</param>
	/// <param name="actual">实际的集合。</param>
	/// <param name="comparer">元素的比较器。</param>
	/// <param name="resultInfo">集合比较的结果信息。</param>
	/// <returns>如果集合相同，则为 <c>true</c>，否则为 <c>false</c>。</returns>
	private static bool AreEquivalent<T>(ICollection<T>? expected, ICollection<T>? actual,
		IEqualityComparer<T> comparer, out string resultInfo)
	{
		if (expected != actual)
		{
			if (expected == null || actual == null)
			{
				resultInfo = string.Empty;
				return false;
			}
			if (expected.Count != actual.Count)
			{
				resultInfo = Resources.NumberOfElementsDiff;
				return false;
			}
			Dictionary<T, int> expectedCounts = GetElementCounts(expected, comparer, out int expectedNullCount);
			Dictionary<T, int> actualCounts = GetElementCounts(actual, comparer, out int acutalNulLCount);
			if (acutalNulLCount != expectedNullCount)
			{
				resultInfo = Resources.CollectionsHasMismatchedElements(expectedNullCount, "null", acutalNulLCount);
				return false;
			}
			foreach (T key in expectedCounts.Keys)
			{
				expectedCounts.TryGetValue(key, out int expectedCount);
				actualCounts.TryGetValue(key, out int actualCount);
				if (expectedCount != actualCount)
				{
					resultInfo = Resources.CollectionsHasMismatchedElements(expectedCount, key, actualCount);
					return false;
				}
			}
			resultInfo = Resources.BothCollectionsSameElements;
			return true;
		}
		resultInfo = Resources.BothCollectionsSameReference;
		return true;
	}

	/// <summary>
	/// 统计集合中的元素个数。
	/// </summary>
	/// <typeparam name="T">集合中的元素个数。</typeparam>
	/// <param name="collection">要统计的集合。</param>
	/// <param name="comparer">元素的比较器。</param>
	/// <param name="nullCount"><c>null</c> 元素的个数。</param>
	/// <returns>集合中的元素统计结果。</returns>
	private static Dictionary<T, int> GetElementCounts<T>(ICollection<T> collection,
		IEqualityComparer<T> comparer, out int nullCount)
	{
		Dictionary<T, int> result = new(comparer);
		nullCount = 0;
		foreach (T item in collection)
		{
			if (item == null)
			{
				nullCount++;
				continue;
			}
			result.TryGetValue(item, out int value);
			result[item] = value + 1;
		}
		return result;
	}

#pragma warning restore CS8714 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 "notnull" 约束不匹配。

	#endregion // AreEquivalent<T>

}
