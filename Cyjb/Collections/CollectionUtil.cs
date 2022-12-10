namespace Cyjb.Collections;

/// <summary>
/// 提供对 <see cref="ICollection{T}"/> 的扩展方法。
/// </summary>
public static class CollectionUtil
{
	/// <summary>
	/// 返回一个空的只读集合。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	/// <returns>空的只读集合。</returns>
	public static IReadOnlyCollection<T> Empty<T>()
	{
		return EmptyList<T>.Instance;
	}

	/// <summary>
	/// 将指定集合的元素添加到当前集合中。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	/// <param name="collection">要添加到的集合。</param>
	/// <param name="other">要添加的元素集合。</param>
	/// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <c>null</c>。</exception>
	/// <exception cref="ArgumentNullException"><paramref name="other"/> 为 <c>null</c>。</exception>
	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> other)
	{
		ArgumentNullException.ThrowIfNull(collection);
		ArgumentNullException.ThrowIfNull(other);
		foreach (T item in other)
		{
			collection.Add(item);
		}
	}

	/// <summary>
	/// 返回当前集合的只读包装。
	/// </summary>
	/// <typeparam name="T">集合中元素的类型。</typeparam>
	/// <param name="collection">要被包装的集合。</param>
	/// <returns>当前集合的只读包装。</returns>
	public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
	{
		return new ReadOnlyCollectionWrap<T>(collection);
	}
}
