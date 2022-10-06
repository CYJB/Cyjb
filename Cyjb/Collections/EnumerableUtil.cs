namespace Cyjb.Collections;

/// <summary>
/// 提供对 <see cref="IEnumerable{T}"/> 接口的扩展方法。
/// </summary>
public static class EnumerableUtil
{
	/// <summary>
	/// 判断序列中是否不包含重复元素。
	/// </summary>
	/// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
	/// <param name="source">要判断是否不包含重复元素的序列。</param>
	/// <param name="comparer">用于比较值的 <see cref="IEqualityComparer{T}"/>。</param>
	/// <returns>如果序列中不包含重复元素，则为 <c>true</c>；否则为 <c>false</c>。</returns>
	/// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <c>null</c>。</exception>
	public static bool IsDistinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source);
		HashSet<TSource> set = new(comparer);
		return source.All(set.Add);
	}

	/// <summary>
	/// 将指定的序列转换为数组，并返回其元素个数。
	/// </summary>
	/// <typeparam name="T"><paramref name="source"/> 中的元素类型。</typeparam>
	/// <param name="source">要转换为数组的序列。</param>
	/// <param name="count">序列中包含的元素个数。</param>
	/// <returns>转换后的数组。</returns>
	internal static T[] ToArray<T>(IEnumerable<T> source, out int count)
	{
		if (source is ICollection<T> collection)
		{
			int cnt = collection.Count;
			if (cnt > 0)
			{
				T[] items = new T[cnt];
				collection.CopyTo(items, 0);
				count = cnt;
				return items;
			}
		}
		else
		{
			using IEnumerator<T> enumerator = source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				T[] items = new T[] { enumerator.Current, default!, default!, default! };
				count = 1;
				while (enumerator.MoveNext())
				{
					if (count == items.Length)
					{
						Array.Resize(ref items, count * 2);
					}
					items[count++] = enumerator.Current;
				}
				return items;
			}
		}
		count = 0;
		return Array.Empty<T>();
	}

}
