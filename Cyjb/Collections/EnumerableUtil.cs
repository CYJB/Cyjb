namespace Cyjb.Collections
{
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
	}
}
